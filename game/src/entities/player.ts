/**
 * Người chơi: chạy, nhảy (2 bậc), lướt, bám/đạp tường, chém combo (Xích Long), bắn nỏ nạp lực (Thanh Vũ).
 * Thông số ở src/data/tuning.json.
 */
import { MZ, PIVOT, SPR_H, SPR_W, SWING_AT, T, type SlashStep } from '../core/config';
import type { Game } from '../core/game';
import type { Box, HeroId, Player } from '../core/types';
import { dust, sparks } from '../core/fx';
import { atkFrame, pickFrame, tvRunning } from './frames';
import { respawn } from '../core/flow';

const easeOut = (t: number) => 1 - Math.pow(1 - t, 3);

export function squash(P: Player, sx: number, sy: number): void { P.sqx = sx; P.sqy = sy; P.vsx = 0; P.vsy = 0; }

const touchingWall = (g: Game, dir: number): boolean => {
  const P = g.P, x = dir > 0 ? P.x + P.w + 1 : P.x - 1;
  return g.map.solidAt(x, P.y + 8) && g.map.solidAt(x, P.y + P.h - 12);
};

function startAttack(g: Game): void {
  const P = g.P, air = !P.ground;
  const step: SlashStep = air ? T.AIR_SLASH : T.COMBO[P.combo];
  P.atk = step; P.atkT = 0; P.hitSet = new Set(); P.comboQueued = false; P.atkAir = air; P.swing = []; P.spinK = 0;
  P.arc = { a0: step.a0, a1: step.a1, from: step.from, to: step.to, len: step.len, big: step.dmg > 1, t: 0, at: SWING_AT[air ? 'air' : String(P.combo)] || SWING_AT['0'] };
  g.sfx.swing(step.dmg > 1 ? 1.4 : air ? 1.15 : 1);
  g.sfx.shout(air ? 'xl_air' : 'xl_' + P.combo);
  if (!air && step.lunge) P.vx = P.face * step.lunge * 2.2;
}

function bladeAngle(P: Player): number {
  const a = P.atk;
  if (!a) return T.BLADE_IDLE;
  if (P.atkT < a.from) return a.a0;
  if (P.atkT > a.to) return a.a1;
  return a.a0 + (a.a1 - a.a0) * easeOut((P.atkT - a.from) / (a.to - a.from));
}

function updateAnim(g: Game, dt: number): void {
  const P = g.P;
  P.vsx += (-(P.sqx - 1) * T.SQUASH_K - P.vsx * T.SQUASH_D) * dt; P.sqx += P.vsx * dt;   // lò xo co giãn luôn bật về 1
  P.vsy += (-(P.sqy - 1) * T.SQUASH_K - P.vsy * T.SQUASH_D) * dt; P.sqy += P.vsy * dt;
  if (P.ground && !P.dash && Math.abs(P.vx) > 40) {
    const before = Math.floor(P.gait);
    P.gait += Math.abs(P.vx) * dt / T.STRIDE;
    const now = Math.floor(P.gait); if (now !== before && now % 4 === 0) dust(g, P.x + P.w / 2 - P.face * 8, P.y + P.h, 3);
  } else P.gait = 0;
  if (P.face !== P.lastFace) { if (P.ground && !P.atk) { P.turnT = 0.08; squash(P, 0.7, 1.04); } P.lastFace = P.face; }
  P.turnT = Math.max(0, P.turnT - dt);
  // vừa dứt lướt trên đất mà không bấm hướng -> trượt gót thắng lại
  if (P.wasDash && !P.dash && P.ground && !P.atk && !g.input.held.left && !g.input.held.right) { P.recFrame = g.hero === 'tv' ? 'tv_d3' : 'xl_dash3'; P.recover = T.DASH_STOP; }
  P.wasDash = !!P.dash;
  P.recover = Math.max(0, P.recover - dt);
  P.wjT = Math.max(0, P.wjT - dt); if (P.ground || P.wall || P.atk || P.dash) P.wjT = 0;
  P.dj = Math.max(0, P.dj - dt);
  P.landT = Math.max(0, P.landT - dt); if (!P.ground || P.atk || P.dash) P.landT = 0;
  if (P.atk || P.dash || (!P.ground && P.recover)) P.recover = 0;
  P.airT = P.ground ? 0 : P.airT + dt;
}

/** Xuất hiện kiểu tia năng lượng rơi xuống (Mega Man X8). */
export function warpIn(g: Game): void {
  const P = g.P;
  P.warp = { t: 0 }; P.vx = 0; P.vy = 0; P.atk = null; P.arc = null; P.aiming = false; P.dash = 0;
  g.sfx.warp();
}
function updateWarp(g: Game, dt: number): void {
  const P = g.P, W = P.warp!; W.t += dt; P.vx = 0;
  P.vy = Math.min(P.vy + T.GRAVITY * dt, T.MAX_FALL); P.y += P.vy * dt; g.map.collideY(P);
  if (!W.landed && W.t >= T.WARP_FALL) {
    W.landed = true; const cx0 = P.x + P.w / 2, by = P.y + P.h, col = g.hero === 'tv' ? '#7fe3ff' : '#ff8a2b';
    g.fx.push({ k: 'ring', x: cx0, y: by - 4, life: 0.3, max: 0.3, big: true, col });
    for (let i = 0; i < 18; i++) {
      const a = -Math.PI * Math.random(), sp = 120 + Math.random() * 260;
      g.parts.push({ x: cx0, y: by - 2, vx: Math.cos(a) * sp, vy: Math.sin(a) * sp, life: 0.3 + Math.random() * 0.25, c: i % 2 ? col : '#ffffff' });
    }
    g.shake = Math.max(g.shake, 3); g.sfx.land();
  }
  if (W.t >= T.WARP_FALL + T.WARP_FORM) { P.warp = null; squash(P, 1.12, 0.9); }
}

/** Đổi nhân vật (Xích Long ⇄ Thanh Vũ), có hiệu ứng xuất hiện. */
export function swapHero(g: Game, to?: HeroId): void {
  const P = g.P;
  g.hero = to || (g.hero === 'xl' ? 'tv' : 'xl');
  P.atk = null; P.arc = null; P.combo = 0; P.aiming = false; P.shootT = 0; P.recover = 0;
  warpIn(g); g.onHeroChange?.(g.hero);
}

/** Bị trúng đòn: mất máu, giật lùi, nhấp nháy bất tử. Hết máu thì hồi sinh ở điểm lưu. */
export function hurtPlayer(g: Game, dmg: number, fromX: number): void {
  const P = g.P;
  if (P.warp || P.inv > 0) return;
  P.hp -= dmg; P.inv = T.HURT_INV; const d = P.x + P.w / 2 < fromX ? -1 : 1;
  P.vx = d * T.HURT_KNOCK; P.vy = -260; P.dash = 0; P.atk = null; P.arc = null; P.aiming = false;
  g.shake = T.SHAKE * 1.6; g.flashT = 0.05; sparks(g, P.x + P.w / 2, P.y + P.h / 2, 14, '#ff3b6b'); g.sfx.hit(true);
  if (P.hp <= 0) { sparks(g, P.x + P.w / 2, P.y + P.h / 2, 40, '#ffd34d'); respawn(g); }
}

// ---------------- Thanh Vũ: Thần Nỏ ----------------
/** Cấp nạp hiện tại: 0 thường, 1 nạp, 2 nạp đầy. */
export function chargeLv(charge: number): number { return charge >= T.TV.CHARGE2 ? 2 : charge >= T.TV.CHARGE1 ? 1 : 0; }
/** Vị trí đầu nỏ trong thế giới. */
export function muzzle(g: Game): { x: number; y: number } {
  const P = g.P, M = !P.ground ? (MZ[pickFrame(g)] || MZ.tv_aj3) : tvRunning(P) ? T.TV.MUZZLE_RUN : T.TV.MUZZLE;
  return { x: P.x + P.w / 2 + P.face * M.x, y: P.y + P.h + M.y };
}
function updateShoot(g: Game, dt: number): void {
  const P = g.P, inp = g.input;
  P.reload = Math.max(0, P.reload - dt); P.shootT = Math.max(0, P.shootT - dt);
  if (inp.pressed.attack && !P.aiming && P.reload <= 0) { P.aiming = true; P.charge = 0; P.aimT = 0; P.lastLv = 0; }
  if (!P.aiming) return;
  P.charge += dt; P.aimT += dt;
  const lv = chargeLv(P.charge), m = muzzle(g);
  if (lv > P.lastLv) { P.lastLv = lv; g.sfx.charge(lv); g.fx.push({ k: 'ring', x: m.x, y: m.y, life: 0.2, max: 0.2, big: lv === 2, col: '#7fe3ff' }); }
  if (lv > 0 && Math.random() < (lv === 2 ? 0.9 : 0.5)) {   // hạt năng lượng hút vào đầu nỏ
    const a = Math.random() * 6.28, r = 26 + Math.random() * 14;
    g.parts.push({ x: m.x + Math.cos(a) * r, y: m.y + Math.sin(a) * r, vx: -Math.cos(a) * r * 5, vy: -Math.sin(a) * r * 5, life: 0.18, c: lv === 2 ? '#ffffff' : '#7fe3ff', nog: 1 });
  }
  if (!inp.held.attack) {   // nhả nút = bắn
    const L = lv;
    g.bolts.push({ x: m.x, y: m.y, vx: P.face * T.TV.SPEED[L], lv: L, life: 1.4, hit: new Set() });
    P.aiming = false; P.charge = 0; P.shootT = T.TV.SHOOT_ANIM; P.reload = T.TV.RELOAD;
    if (P.ground) P.vx -= P.face * T.TV.RECOIL[L];
    squash(P, L === 2 ? 0.86 : 0.95, L === 2 ? 1.08 : 1.02);
    g.fx.push({ k: 'ring', x: m.x, y: m.y, life: 0.12 + L * 0.05, max: 0.12 + L * 0.05, big: L === 2, col: '#bff4ff' });
    if (L === 2) g.shake = T.SHAKE * 1.2;
    g.sfx.shoot(L); g.sfx.shout('tv_' + L);
  }
}

/** Một bước vật lý của người chơi. */
export function updatePlayer(g: Game, dt: number): void {
  const P = g.P, inp = g.input, held = inp.held, pressed = inp.pressed, map = g.map;
  if (P.warp) { updateWarp(g, dt); updateAnim(g, dt); return; }
  // nước làm chậm: hệ số do world/water.ts đặt vào g.phys trước mỗi bước
  const RUN = T.RUN_SPEED * g.phys.run, DASH = T.DASH_SPEED * g.phys.run, GRAV = T.GRAVITY * g.phys.grav, MAXF = T.MAX_FALL * g.phys.fall;
  const dirIn = (held.right ? 1 : 0) - (held.left ? 1 : 0);
  P.coyote = P.ground ? T.COYOTE : Math.max(0, P.coyote - dt);
  P.jumpBuf = pressed.jump ? T.JUMP_BUFFER : Math.max(0, P.jumpBuf - dt);
  P.wallLock = Math.max(0, P.wallLock - dt);

  // --- chém (Xích Long) / bắn nỏ (Thanh Vũ) ---
  if (g.hero === 'tv') updateShoot(g, dt);
  else if (pressed.attack) {
    if (!P.atk) { if (P.ground && P.combo > 2) P.combo = 0; startAttack(g); }
    else if (!P.atkAir && P.combo < 2) P.comboQueued = true;
  }
  if (g.hero === 'xl' && P.atk) {
    P.atkT += dt;
    if (P.atkT >= P.atk.time) {
      if (P.comboQueued && P.ground && !P.atkAir) { P.combo++; startAttack(g); }
      else { P.recFrame = atkFrame(P, true); P.recover = T.RECOVER; P.atk = null; P.combo = 0; }
    }
  }
  // vệt chém: lưu góc lưỡi đao theo thời gian
  const target = bladeAngle(P);
  P.bladeA = P.atk ? target : P.bladeA + (target - P.bladeA) * Math.min(1, dt * 14);
  P.bladeL = P.atk ? P.atk.len : P.bladeL + (1 - P.bladeL) * Math.min(1, dt * 10);
  if (P.atk && P.atkT >= P.atk.from && P.atkT <= P.atk.to + 0.02) P.swing.push({ a: P.bladeA, l: P.bladeL, life: 0.12 });
  for (const s of P.swing) s.life -= dt;
  P.swing = P.swing.filter(s => s.life > 0);
  if (P.atk && P.atk.spins) { const k = Math.floor(Math.max(0, P.atkT - P.atk.from) / (P.atk.to - P.atk.from) * P.atk.spins); if (k !== P.spinK && k < P.atk.spins) { P.spinK = k; P.hitSet = new Set(); } }
  if (P.arc) { P.arc.t += dt; if (P.arc.t > P.arc.to + T.ARC_FADE) P.arc = null; }
  const groundSlashing = !!P.atk && !P.atkAir;

  // --- lướt ---
  if (pressed.dash && !P.dash && !groundSlashing) {
    if (P.ground) { P.dash = T.DASH_TIME; P.dashHeld = 0; squash(P, 1.18, 0.88); dust(g, P.x + P.w / 2 - P.face * 10, P.y + P.h); }
    else if (P.airDashes > 0 && !P.wall) { P.airDashes--; P.dash = T.DASH_TIME * 0.8; P.dashHeld = 0; P.vy = 0; squash(P, 1.18, 0.88); }
    if (P.dash && dirIn) P.face = dirIn;
  }
  if (P.dash) {
    P.dashHeld += dt; P.dash -= dt;
    if ((!held.dash && P.dashHeld > T.DASH_MIN) || (dirIn && dirIn !== P.face)) P.dash = 0;
    P.vx = P.face * DASH;
    if (!P.ground) P.vy = Math.min(P.vy, 0);
    if (P.ground) P.dashMomentum = true;
    P.ghostT -= dt; if (P.ghostT <= 0) { P.ghostT = T.GHOST_EVERY; P.ghosts.push({ x: P.x, y: P.y, f: P.face, a: 0.6, n: pickFrame(g) }); }
    if (P.dash <= 0) P.dash = 0;
  }

  // --- chạy ---
  if (!P.dash && P.wallLock <= 0) {
    if (groundSlashing) P.vx *= Math.pow(0.0005, dt);
    else {
      const top = P.dashMomentum && !P.ground ? DASH : RUN, tgt = dirIn * top, acc = P.ground ? T.ACCEL_GROUND : T.ACCEL_AIR;
      if (P.vx < tgt) P.vx = Math.min(tgt, P.vx + acc * dt);
      else if (P.vx > tgt) P.vx = Math.max(tgt, P.vx - acc * dt);
      if (dirIn && !P.atk) P.face = dirIn;
    }
  }

  // --- bám tường ---
  P.wall = 0;
  if (!P.ground && !P.dash && dirIn && touchingWall(g, dirIn) && P.vy > -120) P.wall = dirIn;
  if (g.hero === 'tv' && P.wall) P.face = -P.wall;   // Thanh Vũ quay lưng vào tường để bắn ra ngoài

  // --- nhảy ---
  if (P.jumpBuf > 0) {
    if (P.coyote > 0) {
      P.vy = -T.JUMP_VEL; P.coyote = 0; P.jumpBuf = 0; squash(P, 0.82, 1.2); dust(g, P.x + P.w / 2, P.y + P.h);
      P.dashMomentum = !!(P.dash || held.dash); P.dash = 0; P.ground = false;
    } else if (!P.ground && (P.wall || touchingWall(g, -1) || touchingWall(g, 1))) {
      const w = P.wall || (touchingWall(g, 1) ? 1 : -1), climb = dirIn === w;   // giữ hướng vào tường = leo lên chính bức tường
      P.dashMomentum = !!held.dash;
      P.vx = -w * (P.dashMomentum ? DASH : climb ? T.WALL_KICK_VX : T.WALL_JUMP_VX);
      P.vy = -T.WALL_JUMP_VY; squash(P, 0.85, 1.15); P.face = climb && !P.dashMomentum ? w : -w;
      P.wallLock = climb && !P.dashMomentum ? T.WALL_KICK_LOCK : T.WALL_JUMP_LOCK; P.jumpBuf = 0; P.wall = 0; P.wjT = T.WALLJUMP_ANIM; P.wjDir = w;
      sparks(g, P.x + (w > 0 ? P.w : 0), P.y + P.h - 8, 6, '#ffd34d');
    } else if (!P.ground && pressed.jump && P.airJumps > 0) {   // nhảy bậc 2, vòng lửa dưới chân
      P.airJumps--; P.vy = -T.AIR_JUMP_VEL; P.jumpBuf = 0; P.dash = 0; P.airT = 0;
      squash(P, 0.78, 1.22); P.dj = 0.22; g.sfx.jump2();
      g.fx.push({ k: 'ring', x: P.x + P.w / 2, y: P.y + P.h, life: 0.25, max: 0.25 });
      sparks(g, P.x + P.w / 2, P.y + P.h, 8, '#ffb347');
    }
  }
  if (inp.released.jump && P.vy < 0) P.vy *= T.JUMP_CUT;

  // --- trọng lực ---
  if (!(P.dash && !P.ground)) {
    P.vy = Math.min(P.vy + GRAV * (P.vy > 0 ? T.FALL_GRAV : 1) * dt, MAXF);
    if (P.wall && P.vy > T.WALL_SLIDE) { P.vy = T.WALL_SLIDE; if (Math.random() < 0.3) sparks(g, P.x + (P.wall > 0 ? P.w : 0), P.y + 12, 1, '#ffb347'); }
  }

  P.x += P.vx * dt; map.collideX(P);
  const wasGround = P.ground, fallV = P.vy;
  P.y += P.vy * dt; map.collideY(P);
  if (P.ground || P.wall) P.airJumps = T.AIR_JUMPS;
  if (P.ground) {
    P.airDashes = T.AIR_DASHES; if (!P.dash) P.dashMomentum = false;
    if (!wasGround) { dust(g, P.x + P.w / 2, P.y + P.h); if (fallV > T.LAND_MIN_VY) P.landT = T.LAND_TIME; const k = Math.min(1, fallV / MAXF); squash(P, 1 + 0.22 * k, 1 - 0.24 * k); }
  }
  if (P.ground && P.atkAir) { P.atk = null; P.combo = 0; }

  g.checkpoints.forEach((c, i) => { if (P.x > c.x - 16 && i > g.cpIndex) g.cpIndex = i; });
  if (P.y > g.map.h * g.map.ts + 80) respawn(g);

  updateAnim(g, dt);
  P.ghosts.forEach(gh => gh.a -= dt * 2.4); P.ghosts = P.ghosts.filter(gh => gh.a > 0);
}

/** Điểm tay cầm đao trong thế giới. */
export function handPos(P: Player): { x: number; y: number } {
  const sx = P.x + P.w / 2 - SPR_W / 2, sy = P.y + P.h - SPR_H;
  return { x: sx + (P.face > 0 ? PIVOT.x : SPR_W - PIVOT.x), y: sy + PIVOT.y };
}
/** Vùng trúng của nhát chém; chỉ có khi lưỡi đao đã quét qua HIT_AT phần vệt. */
export function slashBox(P: Player): Box | null {
  const a = P.atk; if (!a) return null;
  const on = a.from + (a === T.AIR_SLASH ? 0 : (a.to - a.from) * T.HIT_AT);
  if (P.atkT < on || P.atkT > a.to + 0.02) return null;
  const hp = handPos(P), x = P.face > 0 ? hp.x - 14 : hp.x - a.reach + 14;
  return { x, y: hp.y - a.h / 2, w: a.reach, h: a.h };
}
