import type { Game } from './game';

/** Tia lửa văng tứ phía. */
export function sparks(g: Game, x: number, y: number, n: number, c: string): void {
  for (let i = 0; i < n; i++) { const a = Math.random() * 6.28, s = 80 + Math.random() * 320; g.parts.push({ x, y, vx: Math.cos(a) * s, vy: Math.sin(a) * s - 80, life: 0.25 + Math.random() * 0.3, c }); }
}
/** Bụi dưới chân khi chạy, nhảy, đáp. */
export function dust(g: Game, x: number, y: number, n = 6): void {
  for (let i = 0; i < n; i++) g.parts.push({ x: x + (Math.random() - 0.5) * 20, y, vx: (Math.random() - 0.5) * 120, vy: -Math.random() * 80, life: 0.3, c: '#9aa3b5' });
}
export function updateFx(g: Game, dt: number): void {
  for (const e of g.fx) e.life -= dt;
  for (let i = g.fx.length - 1; i >= 0; i--) if (g.fx[i].life <= 0) g.fx.splice(i, 1);
  g.flashT = Math.max(0, g.flashT - dt);
}
export function updateParts(g: Game, dt: number): void {
  for (const p of g.parts) { p.life -= dt; if (!p.nog) p.vy += 1000 * dt; p.x += p.vx * dt; p.y += p.vy * dt; }
  for (let i = g.parts.length - 1; i >= 0; i--) if (g.parts[i].life <= 0) g.parts.splice(i, 1);
}
