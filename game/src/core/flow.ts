/** Luồng một lượt chơi: hồi sinh, một bước vật lý, camera. */
import { STEP, VH } from './config';
import { freshPlayer, type Game } from './game';
import { updateFx, updateParts } from './fx';
import { swapHero, updatePlayer, warpIn } from '../entities/player';
import { updateBolts, updateEshots, updateFoes } from '../entities/foes';
import { bossReset, updateBoss } from '../boss/thuyTinh';
import { updateRisingWater, waterAfter, waterBefore } from '../world/water';
import { subReset, updateSub } from '../boss/thuongLuong';
import { updatePillars } from '../world/pillars';

/** Hồi sinh ở điểm lưu gần nhất: hồi đầy máu, trùm về như cũ, giếng nước rút. */
export function respawn(g: Game): void {
  Object.assign(g.P, freshPlayer(g.checkpoints[g.cpIndex])); warpIn(g);   // giữ nguyên đối tượng P để các hệ thống đang chạy không cầm bản cũ
  if (g.boss.st !== 'wait' && g.boss.st !== 'dead') bossReset(g);
  if (g.sub.st !== 'wait' && g.sub.st !== 'dead') subReset(g);
  g.lvWaterRise = null;
  const L = g.level.water.rise.lockTiles; g.map.rect(L.x, L.y, L.w, L.h, 0);
}

/** Một bước vật lý cố định (STEP giây). */
export function step(g: Game): void {
  const dt = STEP, P = g.P;
  g.time += dt; P.ox = P.x; P.oy = P.y;
  updateFx(g, dt);
  if (g.hitstop > 0) { g.hitstop -= dt; return; }   // khựng hình khi trúng đòn
  const wet = waterBefore(g);
  updatePlayer(g, dt);
  waterAfter(g, wet, dt);
  if (g.talk) { g.talk.t += dt; if (g.talk.t > g.talk.dur) g.talk = null; }
  updateRisingWater(g, dt);
  updateBoss(g, dt);
  updateSub(g, dt, wet);
  updatePillars(g, dt);
  g.P.inv = Math.max(0, g.P.inv - dt);
  updateFoes(g, dt); updateEshots(g, dt); updateBolts(g, dt); updateParts(g, dt);
  if (g.input.pressed.swap) swapHero(g);
  g.input.clearEdges();
  g.shake = Math.max(0, g.shake - dt * 30);
}

/** Camera bám người chơi; vào trận trùm thì khóa giữa đấu trường. */
export function updateCamera(g: Game, cam: { x: number; y: number }, VW: number, dt: number, touch: boolean): void {
  const P = g.P, A = g.level.arena, maxX = g.map.w * g.map.ts - VW;
  cam.x += (P.x + P.w / 2 + P.face * 80 - VW / 2 - cam.x) * Math.min(1, dt * 6);
  cam.y += (P.y - VH * (touch ? 0.38 : 0.5) - cam.y) * Math.min(1, dt * 4);
  if (g.boss.st !== 'wait') cam.x += ((A.x0 + A.x1) / 2 - VW / 2 - cam.x) * Math.min(1, dt * 5);
  const S = g.level.sub;
  if (g.sub.st !== 'wait' && g.sub.st !== 'dead') cam.x += ((S.x0 + S.x1) / 2 - VW / 2 - cam.x) * Math.min(1, dt * 5);
  cam.x = Math.max(0, Math.min(maxX, cam.x));
  cam.y = Math.max(-80, Math.min(g.map.h * g.map.ts - VH + (touch ? 90 : 0), cam.y));
}
