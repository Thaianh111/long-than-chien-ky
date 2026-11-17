/**
 * Trùm Thủy Tinh — máy trạng thái và 6 chiêu. Thiết kế: design/gdd/boss-thuy-tinh.md
 * Số liệu: src/data/boss-thuy-tinh.json.
 */
import { BT, T } from '../core/config';
import type { Game } from '../core/game';
import type { Box } from '../core/types';
import { sparks } from '../core/fx';
import { hurtPlayer, slashBox } from '../entities/player';
import { respawn } from '../core/flow';
import { createBoss, type BossMove, type BossShot } from './state';

type Weights = Record<BossMove, number>;

export function say(g: Game, lines: string[], dur: number, who?: string): void { g.talk = { lines, t: 0, dur, who }; }

/** Đưa trùm và đấu trường về như lúc chưa vào trận (người chơi chết thì trùm hồi đầy máu). */
export function bossReset(g: Game): void {
  const A = g.level.arena, G = A.gate;
  if (g.boss.gateShut) g.map.rect(G.x, G.y, G.w, G.h, 0);
  g.boss = createBoss(A); g.bproj.length = 0; g.arenaWater = 0; g.arenaHaz = 0; g.talk = null;
}

function startShift(g: Game): void {
  const B = g.boss; B.st = 'shift'; B.t = 0; B.phase = 1; g.bproj.length = 0; g.shake = 10; g.flashT = 0.06;
}

/** Trừ máu trùm. Trả về false nếu trùm đang bất tử (vào trận, chuyển pha, chết, vừa trúng). */
export function hurtBoss(g: Game, d: number): boolean {
  const B = g.boss;
  if (B.st === 'intro' || B.st === 'shift' || B.st === 'dead' || B.st === 'wait' || B.inv > 0) return false;
  B.hp = Math.max(0, B.hp - d); B.inv = BT.INV; B.flash = 0.1; B.lostSince += d;
  sparks(g, B.x, B.y - 70, 10, '#8ff0ff'); g.sfx.hit(d > 1); g.hits++;
  if (B.hp <= 0) { B.st = 'dead'; B.t = 0; g.bproj.length = 0; say(g, BT.LINE_OUT, 4); g.kills++; }
  else if (B.phase === 0 && B.hp <= BT.P2 && B.st !== 'atk') startShift(g);
  return true;
}

function push(g: Game, o: Partial<BossShot> & Pick<BossShot, 'k' | 'x' | 'y'>): void {
  g.bproj.push({ t: 0, life: 6, dmg: 2, vx: 0, vy: 0, r: 8, h: 0, ...o });
}

/**
 * Chọn chiêu tiếp theo theo trọng số (bảng §3.4 trong thiết kế): đòn đầu luôn là Sóng Thần,
 * không lặp một chiêu 3 lần liền, không khiên 2 lần liền, Đại Hồng Thủy chỉ ở pha 2.
 */
export function chooseMove(g: Game): BossMove {
  const B = g.boss, P = g.P;
  if (!B.first) { B.first = true; B.last = ['wave']; return 'wave'; }
  const w: Weights = { ...(BT.WEIGHT[B.phase] as Weights) }, d = Math.abs(B.x - (P.x + P.w / 2));
  if (d < 120) { w.wave += 2; w.bubbles -= 1; }
  if (d > 300) w.dash += 3;
  if (P.wall) { w.spears += 2; w.dash -= 2; }
  if (B.phase === 1 && B.floodCd <= 0 && B.lostSince >= 6) w.flood += 6;
  if (B.last[0] && B.last[0] === B.last[1]) w[B.last[0]] = 0;
  if (B.last[0] === 'shield') w.shield = 0;
  const keys = Object.keys(w) as BossMove[];
  let sum = 0; for (const k of keys) { w[k] = Math.max(0, w[k]); sum += w[k]; }
  let r = g.rng() * sum, pick: BossMove = 'wave';
  for (const k of keys) { if (w[k] <= 0) continue; pick = k; r -= w[k]; if (r <= 0) break; }
  B.last = [pick, B.last[0]].filter(Boolean) as BossMove[];
  return pick;
}

/**
 * Vị trí n giáo nước: chia đấu trường thành n + 2 ô, bỏ trống 2 ô KHÔNG kề nhau,
 * nên luôn có 2 khe rộng ≥ 64px để người chơi đứng (test: tests/unit/boss).
 */
export function spearSpots(g: Game, n: number): number[] {
  const A = g.level.arena, L = A.x0 + 16, R = A.x1 - 16, slots = n + 2, gap = (R - L) / slots;
  const a = Math.floor(g.rng() * slots);
  let b = a; while (Math.abs(b - a) < 2) b = Math.floor(g.rng() * slots);
  const out: number[] = [];
  for (let i = 0; i < slots; i++) if (i !== a && i !== b) out.push(L + gap * (i + 0.5) + (g.rng() - 0.5) * 24);
  return out;
}

export function startMove(g: Game, m: BossMove): void {
  const B = g.boss, P = g.P;
  B.st = 'atk'; B.move = m; B.t = 0; B.stage = 'W'; B.face = P.x + P.w / 2 < B.x ? -1 : 1; B.n = 0;
  if (m === 'spears') B.spots = spearSpots(g, BT.SPEAR.N[B.phase]);
  if (m === 'flood') B.fly = false;
}

function stageTime(g: Game, m: BossMove): { W: number; A: number; R: number } {
  const p = g.boss.phase;
  switch (m) {
    case 'wave': return { W: BT.WAVE.W, A: p ? BT.WAVE.GAP2 + 0.05 : 0.05, R: BT.WAVE.R };
    case 'dash': return { W: BT.DASH.W[p], A: 9, R: BT.DASH.R };
    case 'bubbles': return { W: BT.BUB.W, A: BT.BUB.N[p] * BT.BUB.GAP[p], R: BT.BUB.R };
    case 'shield': return { W: BT.SHIELD.W, A: BT.SHIELD.HOLD[p], R: 0.5 };
    case 'spears': return { W: BT.SPEAR.W[p], A: 0.5, R: BT.SPEAR.R };
    case 'flood': return { W: BT.FLOOD.W, A: BT.FLOOD.UP + BT.FLOOD.HOLD + BT.FLOOD.DOWN, R: BT.FLOOD.R };
  }
}

function spawnWave(g: Game): void {
  const B = g.boss;
  push(g, { k: 'wave', x: B.x + B.face * 40, y: g.level.arena.floorY, vx: B.face * BT.WAVE.SPD, w: 32, h: B.phase ? BT.WAVE.H2 : BT.WAVE.H, dmg: BT.WAVE.DMG });
  g.shake = 5; g.sfx.land();
}

function atkStep(g: Game, dt: number, pcx: number): void {
  const B = g.boss, P = g.P, A = g.level.arena, fl = A.floorY, m = B.move!, p = B.phase, S = stageTime(g, m);
  if (B.stage === 'W' && B.t >= S.W) {
    B.stage = 'A'; B.t = 0; B.n = 0;
    if (m === 'wave') { spawnWave(g); B.n = 1; }
    if (m === 'spears') for (const x of B.spots) push(g, { k: 'spear', x, y: fl - 360, vy: BT.SPEAR.SPD, w: 16, h: 48, dmg: BT.SPEAR.DMG });
  } else if (B.stage === 'A') {
    if (m === 'wave' && p && B.n === 1 && B.t > BT.WAVE.GAP2) { B.n = 2; spawnWave(g); }
    if (m === 'dash') {
      B.x += B.face * BT.DASH.SPD[p] * dt;
      if (B.x < A.x0 + 34 || B.x > A.x1 - 34) {
        B.x = Math.max(A.x0 + 34, Math.min(A.x1 - 34, B.x)); B.stage = 'R'; B.t = 0; g.shake = 8; g.sfx.land(); sparks(g, B.x + B.face * 40, fl - 40, 14, '#e8f6f8');
      }
      if (Math.abs(pcx - B.x) < 60 && P.y + P.h > fl - 60 && P.y < fl - 20) hurtPlayer(g, BT.DASH.DMG, B.x);
    }
    if (m === 'bubbles' && B.n < BT.BUB.N[p] && B.t >= B.n * BT.BUB.GAP[p]) {
      const vx = BT.BUB.VX[B.n] * Math.sign(pcx - B.x || B.face);
      push(g, { k: 'bub', x: B.x + B.face * 30, y: B.y - 80, vx, vy: -BT.BUB.VY, r: BT.BUB.SIZE, dmg: BT.BUB.DMG, refl: true }); B.n++; g.sfx.shoot(0);
    }
    if (m === 'shield' && B.t >= (B.n + 1) * BT.SHIELD.DROP[p] && B.t < S.A) {
      B.n++; push(g, { k: 'drop', x: B.x + B.face * 20, y: fl - 76, vx: B.face * 300, r: 6, dmg: BT.SHIELD.DMG });
    }
    if (m === 'flood') {
      const t = B.t, F = BT.FLOOD, base = BT.WATER_P2;
      B.fly = true; B.y += ((fl - 185) - B.y) * Math.min(1, dt * 4);
      g.arenaHaz = t < F.UP ? base + (F.PEAK - base) * t / F.UP : t < F.UP + F.HOLD ? F.PEAK : Math.max(base, F.PEAK - (F.PEAK - base) * (t - F.UP - F.HOLD) / F.DOWN);
    }
    if (B.stage === 'A' && B.t >= S.A) {
      if (m === 'shield') for (let i = 0; i < 8; i++) { const a = i / 8 * 6.283; push(g, { k: 'drop', x: B.x + B.face * 40, y: fl - 70, vx: Math.cos(a) * 240, vy: Math.sin(a) * 240, r: 6, dmg: 2 }); }
      if (m === 'flood') { g.arenaHaz = 0; B.floodCd = BT.FLOOD.CD; B.lostSince = 0; }
      B.stage = 'R'; B.t = 0;
    }
  } else if (B.stage === 'R') {
    if (m === 'flood') B.y = Math.min(fl, B.y + 500 * dt);
    if (B.t >= S.R) {
      B.y = fl; B.fly = false; B.st = 'idle'; B.t = 0;
      if (B.phase === 0 && B.hp <= BT.P2) startShift(g);
    }
  }
}

function updateShots(g: Game, dt: number, box: Box | null): void {
  const B = g.boss, P = g.P, A = g.level.arena, fl = A.floorY, pcx = P.x + P.w / 2;
  for (const b of g.bproj) {
    b.t += dt; b.life -= dt;
    if (b.k === 'bub' && !b.back) b.vy += BT.BUB.G * dt;
    b.x += b.vx * dt; b.y += b.vy * dt;
    if (b.k === 'wave') {
      if (b.x < A.x0 || b.x > A.x1) b.life = 0;
      if (Math.abs(pcx - b.x) < 16 + P.w / 2 && P.y + P.h > b.y - b.h) hurtPlayer(g, b.dmg, b.x - b.vx);
      continue;
    }
    if (b.k === 'spear') {
      if (b.y >= fl) { b.y = fl; b.vy = 0; if (!b.stuck) { b.stuck = true; b.life = 0.3; sparks(g, b.x, fl - 4, 6, '#8ff0ff'); } }
      if (!b.stuck && Math.abs(pcx - b.x) < 8 + P.w / 2 && b.y > P.y && b.y - b.h < P.y + P.h) hurtPlayer(g, b.dmg, b.x);
      continue;
    }
    if (b.back) {   // bong bóng bị chém bay ngược về lõi ngực
      const dx = B.x - b.x, dy = (B.y - 76) - b.y, L = Math.hypot(dx, dy) || 1; b.vx = dx / L * 420; b.vy = dy / L * 420;
      if (L < 30) { b.life = 0; if (B.st !== 'shift') { B.inv = 0; hurtBoss(g, BT.BUB.REFL); } g.fx.push({ k: 'ring', x: b.x, y: b.y, life: 0.18, max: 0.18, big: 1, col: '#ff6a5a' }); }
      continue;
    }
    if (g.map.solidAt(b.x, b.y) || b.y > fl - 2) { b.life = 0; g.fx.push({ k: 'shothit', x: b.x, y: b.y, life: 0.15, max: 0.15 }); continue; }
    if (b.refl && box && b.x > box.x && b.x < box.x + box.w && b.y > box.y && b.y < box.y + box.h) { b.back = true; b.life = 3; sparks(g, b.x, b.y, 10, '#ffd34d'); g.hitstop = 0.05; continue; }
    if (Math.abs(b.x - pcx) < b.r + P.w / 2 && b.y > P.y + 6 && b.y < P.y + P.h) { b.life = 0; hurtPlayer(g, b.dmg, b.x - b.vx); }
  }
  for (let i = g.bproj.length - 1; i >= 0; i--) if (g.bproj[i].life <= 0) g.bproj.splice(i, 1);
}

/** Một bước của trùm: vào trận khi người chơi qua cổng, rồi chạy máy trạng thái. */
export function updateBoss(g: Game, dt: number): void {
  const B = g.boss, P = g.P, A = g.level.arena, fl = A.floorY, pcx = P.x + P.w / 2;
  if (B.st === 'wait') {
    if (pcx > A.closeAtX && !P.warp) {
      B.st = 'intro'; B.t = 0; B.y = fl - 340; say(g, BT.LINE_IN, 2.6);
      g.map.rect(A.gate.x, A.gate.y, A.gate.w, A.gate.h, 1); B.gateShut = true; g.shake = 6; g.sfx.land();
    }
    return;
  }
  B.t += dt; B.inv = Math.max(0, B.inv - dt); B.flash = Math.max(0, B.flash - dt); B.floodCd -= dt;
  if (B.st === 'intro') {   // nhảy từ trên trời đáp xuống
    const yy = fl - 340 + Math.min(1, B.t / 0.55) ** 2 * 340;
    if (B.y < fl - 2 && yy >= fl) { g.shake = 10; g.sfx.land(); sparks(g, B.x, fl - 4, 24, '#8ff0ff'); }
    B.y = Math.min(fl, yy); if (B.t > 2.8) { B.st = 'idle'; B.t = 0; }
  } else if (B.st === 'shift') {
    g.arenaWater = Math.min(BT.WATER_P2, Math.max(g.arenaWater, (B.t - 0.8) / 1.5 * BT.WATER_P2));
    if (B.t > BT.SHIFT) { B.st = 'idle'; B.t = 0; }
  } else if (B.st === 'idle') {
    const d = pcx - B.x, want = Math.abs(d) < 160 ? -Math.sign(d) : Math.abs(d) > 260 ? Math.sign(d) : 0;
    B.face = d < 0 ? -1 : 1;
    B.x = Math.max(A.x0 + 40, Math.min(A.x1 - 40, B.x + want * BT.WALK[B.phase] * dt));
    if (B.t > BT.IDLE[B.phase]) startMove(g, chooseMove(g));
  } else if (B.st === 'stun') {
    B.y = Math.min(fl, B.y + 600 * dt); if (B.t > B.stun) { B.st = 'idle'; B.t = 0; B.fly = false; }
  } else if (B.st === 'atk') atkStep(g, dt, pcx);
  else if (B.st === 'dead') {
    g.arenaWater = Math.max(0, g.arenaWater - 40 * dt); g.arenaHaz = 0;
    if (Math.random() < 0.05 && B.t < 2) { g.fx.push({ k: 'boom', x: B.x + (Math.random() - 0.5) * 60, y: B.y - Math.random() * 110, life: 0.42, max: 0.42 }); g.shake = 4; }
  }
  // va chạm thân
  if (B.st !== 'dead' && B.st !== 'intro' && Math.abs(pcx - B.x) < B.w / 2 + P.w / 2 - 4 && P.y + P.h > B.y - B.h && P.y < B.y) hurtPlayer(g, BT.TOUCH, B.x);
  // trúng kiếm (khiên nước chặn kiếm ở mặt trước)
  const box = slashBox(P);
  if (box && P.atk && !P.hitSet.has(B) && box.x < B.x + B.w / 2 && box.x + box.w > B.x - B.w / 2 && box.y < B.y && box.y + box.h > B.y - B.h) {
    P.hitSet.add(B);
    if (B.move === 'shield' && B.st === 'atk' && B.stage === 'A' && Math.sign(pcx - B.x) === B.face) sparks(g, B.x + B.face * 40, B.y - 70, 8, '#8ff0ff');
    else if (hurtBoss(g, P.atk.dmg)) { g.hitstop = T.HITSTOP * (P.atk.dmg > 1 ? T.FINISH_STOP : 1); g.shake = T.SHAKE; g.fx.push({ k: 'ring', x: B.x, y: B.y - 60, life: 0.16, max: 0.16, big: P.atk.dmg > 1 }); }
  }
  // trúng tên nỏ (chỉ tên nạp đầy phá được khiên, trùm choáng)
  for (const b of g.bolts) {
    if (b.life <= 0 || b.hit.has(B)) continue;
    const shielded = B.st === 'atk' && B.move === 'shield' && B.stage === 'A', sx = B.x + B.face * 40;
    if (shielded && Math.sign(b.x - B.x) === B.face && Math.abs(b.x - sx) < 16 && b.y > B.y - 140 && b.y < B.y) {
      b.hit.add(B);
      if (b.lv === 2) { hurtBoss(g, 4); B.st = 'stun'; B.t = 0; B.stun = BT.SHIELD.STUN; sparks(g, sx, b.y, 30, '#8ff0ff'); g.shake = 10; g.flashT = 0.05; }
      else { b.life = 0; sparks(g, sx, b.y, 8, '#8ff0ff'); }
      continue;
    }
    if (Math.abs(b.x - B.x) < B.w / 2 && b.y > B.y - B.h && b.y < B.y) {
      b.hit.add(B);
      if (hurtBoss(g, T.TV.DMG[b.lv])) { g.hitstop = Math.max(g.hitstop, T.TV.STOP[b.lv]); if (B.move === 'flood' && b.lv === 2 && B.fly) { B.st = 'stun'; B.t = 0; B.stun = 1.0; } }
      if (b.lv < 2) b.life = 0;
    }
  }
  updateShots(g, dt, box);
  // nước tím Đại Hồng Thủy: mất máu mỗi nhịp
  if (g.arenaHaz > g.arenaWater + 2 && P.y + P.h > fl - g.arenaHaz && pcx > A.x0 && pcx < A.x1) {
    P.hazT -= dt;
    if (P.hazT <= 0) {
      P.hazT = BT.FLOOD.TICK;
      if (P.inv <= 0) { P.hp -= BT.FLOOD.DMG; sparks(g, pcx, P.y + P.h / 2, 8, '#b57cff'); g.sfx.hit(false); if (P.hp <= 0) respawn(g); }
    }
  }
}
