/**
 * Trùm phụ Thuồng Luồng (Bến Bạch Hạc) — lặn, lao mặt nước, nổi lưng, phun nước.
 * Thiết kế: design/gdd/boss-thuong-luong.md · Số liệu: src/data/boss-thuong-luong.json.
 * Toạ độ: x = mép trái khung hình 435×121 (khung gốc quay đầu TRÁI), y = mép trên khung.
 * face = -1: đầu bên trái; face = 1: lật hình, đầu bên phải.
 */
import { T, TL } from '../core/config';
import type { Game } from '../core/game';
import type { Box } from '../core/types';
import { sparks } from '../core/fx';
import { hurtPlayer, slashBox } from '../entities/player';
import { say } from './thuyTinh';

export type SubState = 'wait' | 'intro' | 'under' | 'atk' | 'dead';
export type SubMove = 'surge' | 'float' | 'spit';
export interface SubShot { x: number; y: number; vx: number; vy: number; life: number; back: boolean }
export interface SubBoss {
  st: SubState; move: SubMove | null; stage: 'W' | 'A' | 'R'; t: number; hp: number; phase: 0 | 1;
  x: number; y: number; face: number; dir: number; inv: number; flash: number; last: SubMove[]; first: boolean;
  n: number; again: boolean; gateShut: boolean; plat: boolean; shots: SubShot[];
}

export function createSub(g: Pick<Game, 'level'>): SubBoss {
  const A = g.level.sub;
  return {
    st: 'wait', move: null, stage: 'W', t: 0, hp: TL.HP, phase: 0, x: (A.x0 + A.x1) / 2 - TL.FW / 2, y: A.surface + TL.Y.UNDER,
    face: -1, dir: -1, inv: 0, flash: 0, last: [], first: false, n: 0, again: false, gateShut: false, plat: false, shots: [],
  };
}

/** Hộp đầu (chỗ ăn đòn) trong thế giới. */
export function headBox(S: SubBoss): Box {
  const H = TL.HEAD, x0 = S.face < 0 ? S.x + H.x0 : S.x + TL.FW - H.x1;
  return { x: x0, y: S.y + H.y0, w: H.x1 - H.x0, h: H.y1 - H.y0 };
}
function bodyBox(S: SubBoss): Box {
  const B = TL.BODY, x0 = S.face < 0 ? S.x + B.x0 : S.x + TL.FW - B.x1;
  return { x: x0, y: S.y + B.y0, w: B.x1 - B.x0, h: B.y1 - B.y0 };
}
/** Đầu đang nhô khỏi mặt nước (chỉ lúc này mới đánh trúng). */
export function headUp(g: Game): boolean { const S = g.sub; return S.y + (TL.HEAD.y0 + TL.HEAD.y1) / 2 < g.level.sub.surface; }
const hit = (a: Box, b: Box) => a.x < b.x + b.w && a.x + a.w > b.x && a.y < b.y + b.h && a.y + a.h > b.y;

/** Đặt / gỡ ô ẩn dưới lưng rắn khi nổi (giá trị 2 — đứng được nhưng không vẽ). */
function setPlat(g: Game, on: boolean): void {
  const S = g.sub, A = g.level.sub, TS = g.map.ts, row = A.platRow;
  if (!on) {
    for (let i = Math.floor(A.x0 / TS); i < Math.ceil(A.x1 / TS); i++) if (g.map.cells[row][i] === 2) g.map.cells[row][i] = 0;
    S.plat = false; return;
  }
  const P = TL.PLAT, a = S.face < 0 ? S.x + P.x0 : S.x + TL.FW - P.x1, b = a + (P.x1 - P.x0);
  for (let i = Math.max(Math.floor(A.x0 / TS), Math.floor(a / TS)); i <= Math.min(Math.ceil(A.x1 / TS) - 1, Math.floor(b / TS)); i++)
    if (g.map.cells[row][i] === 0) g.map.cells[row][i] = 2;
  S.plat = true;
}

/** Đưa trùm phụ về như lúc chưa vào trận (người chơi chết giữa trận). */
export function subReset(g: Game): void {
  const A = g.level.sub, G = A.gate;
  setPlat(g, false);
  if (g.sub.gateShut) g.map.rect(G.x, G.y, G.w, G.h, 0);
  g.sub = createSub(g);
}

/** Trừ máu trùm phụ. false nếu chưa vào trận, đầu đang dưới nước, hoặc vừa trúng. */
export function hurtSub(g: Game, d: number, force = false): boolean {
  const S = g.sub;
  if (S.st === 'wait' || S.st === 'intro' || S.st === 'dead' || (!force && (S.inv > 0 || !headUp(g)))) return false;
  S.hp = Math.max(0, S.hp - d); S.inv = TL.INV; S.flash = 0.1;
  const h = headBox(S); sparks(g, h.x + h.w / 2, h.y + h.h / 2, 10, '#8ff0ff'); g.sfx.hit(d > 1); g.hits++;
  if (S.phase === 0 && S.hp <= TL.P2) S.phase = 1;
  if (S.hp <= 0) {
    S.st = 'dead'; S.t = 0; S.shots.length = 0; setPlat(g, false); g.kills++; g.shake = 10; g.flashT = 0.06;
    const E = g.level.sub.exit; g.map.rect(E.x, E.y, E.w, E.h, 0); say(g, TL.LINE_OUT, 4.5, 'Ngọc Diệp');
  }
  return true;
}

/**
 * Chọn chiêu: đòn đầu luôn là Lao; không chiêu nào 3 lần liền; không Nổi 2 lần liền;
 * người chơi đang lội nước thì hay Phun hơn.
 */
export function chooseSubMove(g: Game, wet = false): SubMove {
  const S = g.sub;
  if (!S.first) { S.first = true; S.last = ['surge']; return 'surge'; }
  const w = { ...(TL.WEIGHT[S.phase] as Record<SubMove, number>) };
  if (wet) w.spit += 3;
  if (S.last[0] && S.last[0] === S.last[1]) w[S.last[0]] = 0;
  if (S.last[0] === 'float') w.float = 0;
  const keys = Object.keys(w) as SubMove[];
  let sum = 0; for (const k of keys) sum += Math.max(0, w[k]);
  let r = g.rng() * sum, pick: SubMove = 'surge';
  for (const k of keys) { if (w[k] <= 0) continue; pick = k; r -= w[k]; if (r <= 0) break; }
  S.last = [pick, S.last[0]].filter(Boolean) as SubMove[];
  return pick;
}

function startMove(g: Game, m: SubMove): void {
  const S = g.sub, A = g.level.sub, pcx = g.P.x + g.P.w / 2, mid = (A.x0 + A.x1) / 2;
  S.st = 'atk'; S.move = m; S.stage = 'W'; S.t = 0; S.n = 0; S.y = A.surface + TL.Y.UNDER;
  if (m === 'surge') {   // lao ra từ phía xa người chơi
    S.dir = pcx > mid ? 1 : -1; S.face = S.dir;
    S.x = S.dir > 0 ? A.x0 - TL.FW - 40 : A.x1 + 40;
  } else if (m === 'float') {
    S.face = g.rng() < 0.5 ? -1 : 1; S.x = mid - TL.FW / 2;
  } else {               // phun: đầu trồi ở mép xa người chơi, thân khuất ngoài đấu trường
    S.face = pcx < mid ? -1 : 1;
    S.x = S.face < 0 ? A.x1 - TL.HEAD.x1 - 20 : A.x0 + TL.HEAD.x1 + 20 - TL.FW;
  }
}
function goUnder(g: Game): void {
  const S = g.sub; S.st = 'under'; S.t = 0; S.move = null; S.y = g.level.sub.surface + TL.Y.UNDER;
  const A = g.level.sub; S.x = Math.max(A.x0 - 100, Math.min(A.x1 - TL.FW + 100, S.x));
}

/** Bọt nước báo hiệu (chỉ là hiệu ứng, dùng Math.random để không đụng dãy ngẫu nhiên của luật chơi). */
function bubbles(g: Game, x: number, n: number): void {
  const y = g.level.sub.surface;
  for (let i = 0; i < n; i++) g.parts.push({ x: x + (Math.random() - 0.5) * 80, y: y + 4, vx: (Math.random() - 0.5) * 40, vy: -60 - Math.random() * 120, life: 0.3 + Math.random() * 0.3, c: '#bdefff', nog: 1 });
}

function atkStep(g: Game, dt: number): void {
  const S = g.sub, A = g.level.sub, P = g.P, sf = A.surface, p = S.phase, m = S.move!;
  if (m === 'surge') {
    if (S.stage === 'W') {
      if (Math.random() < 0.5) bubbles(g, S.dir > 0 ? A.x0 + 30 : A.x1 - 30, 2);
      if (S.t >= TL.SURGE.W) { S.stage = 'A'; S.t = 0; g.shake = 6; g.sfx.land(); }
    } else {
      const k = Math.min(1, S.t / TL.SURGE.RISE);
      S.y = sf + TL.Y.UNDER + (TL.Y.SURGE - TL.Y.UNDER) * k;
      S.x += S.dir * TL.SURGE.SPD[p] * dt;
      if (Math.random() < 0.6) bubbles(g, S.x + (S.dir > 0 ? 0 : TL.FW), 1);
      if (S.dir > 0 ? S.x > A.x1 + 40 : S.x + TL.FW < A.x0 - 40) {
        if (p && !S.again && g.rng() < TL.SURGE.DOUBLE) {   // pha 2: lao ngược lại thêm 1 lượt
          S.again = true; S.dir = -S.dir; S.face = S.dir; S.stage = 'W'; S.t = TL.SURGE.W * 0.4; S.y = sf + TL.Y.UNDER;
        } else { S.again = false; goUnder(g); }
      }
    }
  } else if (m === 'float') {
    if (S.stage === 'W') {
      if (Math.random() < 0.6) bubbles(g, S.x + TL.FW / 2, 2);
      if (S.t >= TL.FLOAT.W) { S.stage = 'A'; S.t = 0; }
    } else if (S.stage === 'A') {
      const k = Math.min(1, S.t / TL.FLOAT.RISE);
      S.y = sf + TL.Y.UNDER + (TL.Y.FLOAT - TL.Y.UNDER) * k;
      if (k >= 1 && !S.plat) { setPlat(g, true); sparks(g, S.x + TL.FW / 2, sf, 16, '#bdefff'); }
      if (S.t >= TL.FLOAT.RISE + TL.FLOAT.HOLD[p]) { S.stage = 'R'; S.t = 0; }
    } else {
      if (S.t >= TL.FLOAT.SHAKE) { setPlat(g, false); goUnder(g); }
    }
  } else {
    if (S.stage === 'W') {
      if (Math.random() < 0.5) bubbles(g, headBox(S).x + 50, 2);
      const k = Math.min(1, S.t / TL.SPIT.W); S.y = sf + TL.Y.UNDER + (TL.Y.SPIT - TL.Y.UNDER) * k;
      if (S.t >= TL.SPIT.W) { S.stage = 'A'; S.t = 0; }
    } else if (S.stage === 'A') {
      if (S.n < TL.SPIT.N[p] && S.t >= S.n * TL.SPIT.GAP) {
        const h = headBox(S), mx = S.face < 0 ? h.x + 8 : h.x + h.w - 8, my = h.y + h.h * 0.55;
        const pcx = P.x + P.w / 2, d = Math.sign(pcx - mx) || -S.face, dist = Math.min(520, Math.abs(pcx - mx));
        S.shots.push({ x: mx, y: my, vx: d * Math.max(140, dist / 520 * TL.SPIT.VX * 1.7), vy: -TL.SPIT.VY * (0.85 + 0.1 * (S.n % 3)), life: 4, back: false });
        S.n++; g.sfx.shoot(0);
      }
      if (S.n >= TL.SPIT.N[p] && S.t >= S.n * TL.SPIT.GAP + 0.3) { S.stage = 'R'; S.t = 0; }
    } else {
      S.y += 260 * dt;
      if (S.t >= TL.SPIT.R) goUnder(g);
    }
  }
}

function updateShots(g: Game, dt: number, box: Box | null): void {
  const S = g.sub, P = g.P, sf = g.level.sub.surface, r = TL.SPIT.SIZE;
  for (const b of S.shots) {
    b.life -= dt;
    if (b.back) {
      const h = headBox(S), tx = h.x + h.w / 2, ty = h.y + h.h / 2, dx = tx - b.x, dy = ty - b.y, L = Math.hypot(dx, dy) || 1;
      b.vx = dx / L * 460; b.vy = dy / L * 460; b.x += b.vx * dt; b.y += b.vy * dt;
      if (L < 26) { b.life = 0; hurtSub(g, TL.DMG.REFL, true); g.fx.push({ k: 'ring', x: b.x, y: b.y, life: 0.18, max: 0.18, big: 1, col: '#ff6a5a' }); }
      continue;
    }
    b.vy += TL.SPIT.G * dt; b.x += b.vx * dt; b.y += b.vy * dt;
    if (g.map.solidAt(b.x, b.y) || b.y > sf + 6) { b.life = 0; g.fx.push({ k: 'shothit', x: b.x, y: Math.min(b.y, sf), life: 0.15, max: 0.15 }); continue; }
    if (box && b.x > box.x && b.x < box.x + box.w && b.y > box.y && b.y < box.y + box.h) { b.back = true; b.life = 3; sparks(g, b.x, b.y, 10, '#ffd34d'); g.hitstop = 0.05; continue; }
    if (b.x + r > P.x && b.x - r < P.x + P.w && b.y + r > P.y + 6 && b.y - r < P.y + P.h) { b.life = 0; hurtPlayer(g, TL.DMG.SPIT, b.x - b.vx); }
  }
  for (let i = S.shots.length - 1; i >= 0; i--) if (S.shots[i].life <= 0) S.shots.splice(i, 1);
}

/** Một bước của trùm phụ. */
export function updateSub(g: Game, dt: number, wet = false): void {
  const S = g.sub, A = g.level.sub, P = g.P, pcx = P.x + P.w / 2;
  if (S.st === 'wait') {
    if (pcx > A.closeAtX && pcx < A.x1 && !P.warp) {
      S.st = 'intro'; S.t = 0; g.map.rect(A.gate.x, A.gate.y, A.gate.w, A.gate.h, 1); S.gateShut = true;
      g.shake = 6; g.sfx.land(); say(g, TL.LINE_IN, 3.2, 'Ngọc Diệp');
    }
    return;
  }
  S.t += dt; S.inv = Math.max(0, S.inv - dt); S.flash = Math.max(0, S.flash - dt);
  if (S.st === 'dead') { S.y += 40 * dt; if (S.t < 1.6 && Math.random() < 0.06) g.fx.push({ k: 'boom', x: S.x + Math.random() * TL.FW, y: S.y + 40 + Math.random() * 40, life: 0.42, max: 0.42 }); return; }
  if (S.st === 'intro') {   // trồi lên giữa sông gầm một tiếng rồi lặn
    const sf = A.surface, up = S.t > 0.8 && S.t < TL.INTRO - 0.5;
    S.y += ((sf + (up ? TL.Y.FLOAT : TL.Y.UNDER)) - S.y) * Math.min(1, dt * 5);
    if (S.t > 0.8 && S.t < 0.9) { g.shake = 8; bubbles(g, S.x + 100, 20); }
    if (S.t >= TL.INTRO) goUnder(g);
  } else if (S.st === 'under') {
    const want = pcx - TL.FW / 2; S.x += Math.sign(want - S.x) * Math.min(Math.abs(want - S.x), TL.FOLLOW * dt);
    if (S.t >= TL.UNDER[S.phase]) startMove(g, chooseSubMove(g, wet));
  } else atkStep(g, dt);

  // va chạm: đầu lúc lao/phun, thân lúc lao
  const pb: Box = { x: P.x, y: P.y, w: P.w, h: P.h };
  if (S.st === 'atk' && S.stage !== 'W' && (S.move === 'surge' || S.move === 'spit')) {
    const hb = headBox(S); if (headUp(g) && hit(pb, hb)) hurtPlayer(g, TL.DMG.TOUCH, hb.x + hb.w / 2);
    if (S.move === 'surge') { const bb = bodyBox(S); if (bb.y < A.surface && hit(pb, bb)) hurtPlayer(g, TL.DMG.TOUCH, bb.x + bb.w / 2); }
  }
  // trúng kiếm vào đầu
  const box = slashBox(P);
  if (box && P.atk && !P.hitSet.has(S) && hit(box, headBox(S))) {
    if (hurtSub(g, P.atk.dmg)) { P.hitSet.add(S); g.hitstop = T.HITSTOP * (P.atk.dmg > 1 ? T.FINISH_STOP : 1); g.shake = T.SHAKE; const h = headBox(S); g.fx.push({ k: 'ring', x: h.x + h.w / 2, y: h.y + h.h / 2, life: 0.16, max: 0.16, big: P.atk.dmg > 1 }); }
  }
  // trúng tên nỏ
  for (const b of g.bolts) {
    if (b.life <= 0 || b.hit.has(S)) continue;
    const h = headBox(S);
    if (b.x > h.x && b.x < h.x + h.w && b.y > h.y && b.y < h.y + h.h && headUp(g)) {
      b.hit.add(S); if (hurtSub(g, T.TV.DMG[b.lv])) g.hitstop = Math.max(g.hitstop, T.TV.STOP[b.lv]);
      if (b.lv < 2) b.life = 0;
    }
  }
  updateShots(g, dt, box);
}
