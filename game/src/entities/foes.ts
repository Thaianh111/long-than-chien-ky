/** Quái thường (drone bay, tháp súng), đạn của quái, tên nỏ của Thanh Vũ. */
import { T } from '../core/config';
import type { Game } from '../core/game';
import type { Foe } from '../core/types';
import { sparks } from '../core/fx';
import { hurtPlayer, slashBox } from './player';

export function boom(g: Game, f: { x: number; y: number; w: number; h: number }): void {
  g.fx.push({ k: 'boom', x: f.x + f.w / 2, y: f.y + f.h / 2, life: 0.42, max: 0.42 });
}
function killFoe(g: Game, f: Foe, col: string): void {
  f.dead = 2.5; boom(g, f); sparks(g, f.x + f.w / 2, f.y + f.h / 2, 26, col); g.kills++;
}

export function updateEshots(g: Game, dt: number): void {
  const P = g.P, box = P.atk ? slashBox(P) : null;
  for (const b of g.eshots) {
    b.x += b.vx * dt; b.y += b.vy * dt; b.life -= dt; b.t += dt;
    const pop = () => { b.life = 0; g.fx.push({ k: 'shothit', x: b.x, y: b.y, life: 0.15, max: 0.15 }); };
    if (g.map.solidAt(b.x, b.y)) { pop(); continue; }
    if (box && b.x > box.x && b.x < box.x + box.w && b.y > box.y && b.y < box.y + box.h) { pop(); sparks(g, b.x, b.y, 8, '#ffd34d'); continue; }   // chém đạn như Zero
    for (const t of g.bolts) if (Math.abs(t.x - b.x) < 14 && Math.abs(t.y - b.y) < 12) { pop(); break; }
    if (b.life > 0 && b.x > P.x && b.x < P.x + P.w && b.y > P.y + 6 && b.y < P.y + P.h) { pop(); hurtPlayer(g, T.SHOT_DMG, b.x - b.vx); }
  }
  for (let i = g.eshots.length - 1; i >= 0; i--) if (g.eshots[i].life <= 0) g.eshots.splice(i, 1);
}

export function updateFoes(g: Game, dt: number): void {
  const P = g.P, box = slashBox(P);
  for (const f of g.foes) {
    if (f.dead > 0) { f.dead -= dt; if (f.dead <= 0) Object.assign(f, { ox: f.hx, x: f.hx, y: f.oy, hp: f.maxHp, vx: 0, vy: 0, kbx: 0 }); continue; }
    f.t += dt; f.flash = Math.max(0, f.flash - dt);
    const pcx = P.x + P.w / 2, pcy = P.y + P.h / 2;
    if (!P.warp && pcx > f.x - 4 && pcx < f.x + f.w + 4 && P.y < f.y + f.h && P.y + P.h > f.y) hurtPlayer(g, T.TOUCH_DMG, f.x + f.w / 2);
    if (f.type === 'turret') {
      f.dir = pcx < f.x + f.w / 2 ? -1 : 1; f.cd -= dt; f.fire = Math.max(0, f.fire - dt);
      if (f.cd <= 0 && Math.abs(pcx - f.x) < T.TURRET_RANGE && Math.abs(pcy - f.y) < 160 && !P.warp) {
        f.cd = T.TURRET_CD; f.fire = 0.36;
        g.eshots.push({ x: f.x + f.w / 2 + f.dir * 16, y: f.y + 8, vx: f.dir * T.SHOT_SPEED, vy: 0, life: 3, t: 0 });
      }
    } else {
      if (Math.abs(pcx - f.x) < 300) f.ox += Math.sign(pcx - f.x - f.w / 2) * 22 * dt;   // lừ đừ bám theo
      f.vx *= Math.pow(0.02, dt); f.kbx = (f.kbx + f.vx * dt) * Math.pow(0.2, dt); f.x = f.ox + f.kbx;
      f.y = f.oy + Math.sin(f.t * 2) * 10 + f.kby; f.kby *= Math.pow(0.05, dt);
    }
    if (box && P.atk && !P.hitSet.has(f) && box.x < f.x + f.w && box.x + box.w > f.x && box.y < f.y + f.h && box.y + box.h > f.y) {
      P.hitSet.add(f); f.hp -= P.atk.dmg; f.flash = 0.12;
      f.vx = f.type === 'turret' ? 0 : P.face * P.atk.knock; if (f.type === 'drone') f.kby = -16;
      const big = P.atk.dmg > 1, cxh = f.x + f.w / 2, cyh = f.y + f.h / 2;
      g.hitstop = T.HITSTOP * (big ? T.FINISH_STOP : 1); g.shake = T.SHAKE * (big ? 2.2 : 1);
      // vết cắt sáng chéo qua quái + vòng nổ + mảnh văng theo hướng chém
      g.fx.push({ k: 'cut', x: cxh, y: cyh, a: (P.face > 0 ? 0 : Math.PI) + (P.atk.a1 > P.atk.a0 ? 0.55 : -0.55), L: big ? 120 : 80, life: 0.14, max: 0.14 });
      g.fx.push({ k: 'ring', x: cxh, y: cyh, life: big ? 0.22 : 0.16, max: big ? 0.22 : 0.16, big });
      if (big) g.flashT = 0.06;
      for (let i = 0; i < (big ? 18 : 10); i++) {
        const a = (P.face > 0 ? 0 : Math.PI) + (Math.random() - 0.5) * 1.6, sp = 200 + Math.random() * 420;
        g.parts.push({ x: cxh, y: cyh, vx: Math.cos(a) * sp, vy: Math.sin(a) * sp - 120, life: 0.2 + Math.random() * 0.3, c: i % 3 ? '#ff6a2b' : '#fff1b8' });
      }
      g.sfx.hit(big); g.hits++;
      if (f.hp <= 0) killFoe(g, f, '#ffd34d');
    }
  }
}

export function updateBolts(g: Game, dt: number): void {
  for (const b of g.bolts) {
    b.x += b.vx * dt; b.life -= dt;
    if (g.map.solidAt(b.x, b.y)) { b.life = 0; sparks(g, b.x - Math.sign(b.vx) * 4, b.y, 6 + b.lv * 4, '#7fe3ff'); continue; }
    if (Math.random() < 0.6) g.parts.push({ x: b.x - Math.sign(b.vx) * 10, y: b.y + (Math.random() - 0.5) * (4 + b.lv * 6), vx: -b.vx * 0.05, vy: 0, life: 0.12 + b.lv * 0.05, c: b.lv ? '#bff4ff' : '#7fe3ff', nog: 1 });
    const r = T.TV.SIZE[b.lv];
    for (const f of g.foes) {
      if (f.dead > 0 || b.hit.has(f)) continue;
      if (b.x + r > f.x && b.x - r < f.x + f.w && b.y + r > f.y && b.y - r < f.y + f.h) {
        b.hit.add(f); f.hp -= T.TV.DMG[b.lv]; f.flash = 0.12; f.vx = f.type === 'turret' ? 0 : Math.sign(b.vx) * (160 + b.lv * 140);
        if (f.type === 'drone') f.kby = -8 - b.lv * 6;
        g.hitstop = Math.max(g.hitstop, T.TV.STOP[b.lv]); g.shake = Math.max(g.shake, T.SHAKE * (0.5 + b.lv * 0.5));
        g.fx.push({ k: 'ring', x: b.x, y: b.y, life: 0.16, max: 0.16, big: b.lv === 2, col: '#7fe3ff' });
        sparks(g, b.x, b.y, 8 + b.lv * 6, '#7fe3ff'); g.sfx.hit(b.lv === 2); g.hits++;
        if (f.hp <= 0) killFoe(g, f, '#bff4ff');
        if (b.lv < 2) { b.life = 0; break; }   // tên nạp đầy xuyên qua quái
      }
    }
  }
  for (let i = g.bolts.length - 1; i >= 0; i--) if (g.bolts[i].life <= 0) g.bolts.splice(i, 1);
}
