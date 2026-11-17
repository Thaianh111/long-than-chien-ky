/**
 * Cột đá Sơn Tinh (đoạn Chân núi Tản Viên): chém hoặc bắn vào cột thì cột dựng lên từng ô,
 * nâng người đang đứng trên đỉnh theo — dùng làm bậc leo và đường qua sông.
 * Ô cột có giá trị 3 (đặc, vẽ thành đá).
 */
import { T } from '../core/config';
import type { Game } from '../core/game';
import { sparks } from '../core/fx';
import { slashBox } from '../entities/player';

export interface Pillar { tx: number; base: number; cur: number; max: number; grow: boolean; t: number }

export function createPillars(g: Pick<Game, 'level' | 'map'>): Pillar[] {
  return g.level.pillars.map(([tx, base, start, max]) => {
    for (let j = 0; j < start; j++) g.map.cells[base - j][tx] = 3;
    return { tx, base, cur: start, max, grow: false, t: 0 };
  });
}

/** Mép trên của cột (px). */
export const pillarTop = (g: Game, c: Pillar): number => (c.base - c.cur + 1) * g.map.ts;

function trigger(g: Game, c: Pillar): void {
  if (c.grow || c.cur >= c.max) return;
  c.grow = true; c.t = 0; g.shake = Math.max(g.shake, 4); g.sfx.land();
  sparks(g, (c.tx + 0.5) * g.map.ts, pillarTop(g, c), 14, '#C9A227');
}

export function updatePillars(g: Game, dt: number): void {
  const P = g.P, TS = g.map.ts, box = slashBox(P);
  for (const c of g.pillars) {
    const x0 = c.tx * TS, top = pillarTop(g, c), bot = (c.base + 1) * TS;
    if (box && P.atk && !P.hitSet.has(c) && box.x < x0 + TS && box.x + box.w > x0 && box.y < bot && box.y + box.h > top - 8) { P.hitSet.add(c); trigger(g, c); }
    for (const b of g.bolts) if (b.life > 0 && b.x > x0 - 6 && b.x < x0 + TS + 6 && b.y > top - 6 && b.y < bot) { b.life = 0; trigger(g, c); }
    if (!c.grow) continue;
    c.t += dt;
    while (c.grow && c.t >= T.PILLAR_STEP) {
      c.t -= T.PILLAR_STEP;
      const row = c.base - c.cur; if (row < 0) { c.grow = false; break; }
      if (g.map.cells[row][c.tx] === 0) g.map.cells[row][c.tx] = 3;
      c.cur++;
      const y = row * TS;   // ô mới mọc vào người chơi → nâng người chơi lên
      if (P.x < x0 + TS && P.x + P.w > x0 && P.y < y + TS && P.y + P.h > y) { P.y = y - P.h; P.oy = Math.min(P.oy, P.y); if (P.vy > 0) P.vy = 0; }
      if (c.cur >= c.max) c.grow = false;
    }
  }
}
