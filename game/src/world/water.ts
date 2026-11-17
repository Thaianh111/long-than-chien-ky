/**
 * Nước trong màn: hố nước tĩnh, giếng nước dâng đuổi theo, nước trong đấu trường trùm.
 * Dưới nước: chạy/rơi chậm hơn; nước sâu rút máu từ từ (không chết ngay).
 */
import { T } from '../core/config';
import type { Game } from '../core/game';
import { sparks } from '../core/fx';
import { say } from '../boss/thuyTinh';
import { respawn } from '../core/flow';

/** Mặt nước (y thế giới) tại x; Infinity = chỗ này không có nước. */
export function waterY(g: Game, x: number): number {
  const W = g.level.water, A = g.level.arena;
  let y = Infinity;
  for (const z of W.zones) if (x >= z.x0 && x <= z.x1) y = Math.min(y, z.y);
  if (g.lvWaterRise && x >= W.rise.x0 && x <= W.rise.x1) y = Math.min(y, g.lvWaterRise.y);
  if (x >= A.x0 && x <= A.x1) { const h = Math.max(g.arenaWater, g.arenaHaz); if (h > 0) y = Math.min(y, A.floorY - h); }
  return y;
}
/** Chỗ này là nước sâu (rút máu) hay vũng nông (chỉ làm chậm). */
export function deepAt(g: Game, x: number): boolean {
  const W = g.level.water;
  for (const z of W.zones) if (x >= z.x0 && x <= z.x1) return z.deep !== false;
  return !!(g.lvWaterRise && x >= W.rise.x0 && x <= W.rise.x1);
}
export const playerInWater = (g: Game): boolean => g.P.y + g.P.h * 0.6 > waterY(g, g.P.x + g.P.w / 2);

/** Kích hoạt và dâng nước trong giếng khi người chơi bước vào. */
export function updateRisingWater(g: Game, dt: number): void {
  const R = g.level.water.rise, pcx = g.P.x + g.P.w / 2;
  if (!g.lvWaterRise && pcx > R.triggerX && pcx < R.x1 && g.boss.st === 'wait') {
    g.lvWaterRise = { y: R.startY, t: 0 };
    const L = R.lockTiles; g.map.rect(L.x, L.y, L.w, L.h, 1);   // cửa sập sau lưng
    g.shake = 8; g.sfx.land(); say(g, ['Cửa sập! Nước đang dâng — leo lên mau!'], 2.2, 'Ngọc Diệp');
  }
  if (g.lvWaterRise) { g.lvWaterRise.t += dt; if (g.lvWaterRise.t > R.delay) g.lvWaterRise.y = Math.max(R.endY, g.lvWaterRise.y - R.speed * dt); }
}

/** Đặt hệ số vật lý dưới nước trước bước người chơi, và rút máu ở nước sâu sau bước đó. */
export function waterBefore(g: Game): boolean {
  const P = g.P, wet = !P.warp && playerInWater(g), K = T.WATER;
  g.phys = wet ? { run: K.RUN, grav: K.GRAV, fall: K.FALL } : { run: 1, grav: 1, fall: 1 };
  if (wet && !P.wasWet && P.vy > 100) {   // té xuống nước: bắn tung bọt
    const x = P.x + P.w / 2, y = waterY(g, x);
    for (let i = 0; i < 14; i++) g.parts.push({ x, y, vx: (Math.random() - 0.5) * 300, vy: -150 - Math.random() * 250, life: 0.4, c: '#bdefff' });
  }
  P.wasWet = wet;
  return wet;
}
export function waterAfter(g: Game, wet: boolean, dt: number): void {
  const P = g.P, x = P.x + P.w / 2;
  if (wet && deepAt(g, x) && P.y + P.h / 2 > waterY(g, x)) {
    P.drain += T.WATER.DRAIN * dt;
    if (P.drain >= 1) { P.drain -= 1; P.hp -= 1; sparks(g, x, P.y + 10, 4, '#bdefff'); if (P.hp <= 0) respawn(g); }
  }
}
