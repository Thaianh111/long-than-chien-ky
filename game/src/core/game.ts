import { T } from './config';
import type { Bolt, Checkpoint, EnemyShot, Foe, FoeType, Fx, HeroId, Particle, Player, Talk } from './types';
import { TileMap } from '../world/tilemap';
import type { LevelDef } from '../levels/hongThuy';
import { createBoss, type Boss, type BossShot } from '../boss/state';
import type { Input } from './input';

/** Âm thanh mà logic game gọi tới (tách riêng để test chạy không cần WebAudio). */
export interface Sfx {
  swing(p: number): void; hit(big: boolean): void; shoot(lv: number): void; charge(lv: number): void;
  warp(): void; land(): void; jump2(): void; shout(key: string): void;
}
export const silentSfx: Sfx = { swing() {}, hit() {}, shoot() {}, charge() {}, warp() {}, land() {}, jump2() {}, shout() {} };

/** Toàn bộ trạng thái một lượt chơi. Mọi hệ thống nhận Game làm tham số (không dùng biến toàn cục). */
export interface Game {
  level: LevelDef; map: TileMap; checkpoints: Checkpoint[]; cpIndex: number;
  hero: HeroId; P: Player; input: Input; sfx: Sfx; rng: () => number;
  foes: Foe[]; bolts: Bolt[]; eshots: EnemyShot[]; parts: Particle[]; fx: Fx[];
  boss: Boss; bproj: BossShot[];
  arenaWater: number; arenaHaz: number; lvWaterRise: { y: number; t: number } | null; talk: Talk | null;
  /** Hệ số vật lý hiện tại (dưới nước < 1). */
  phys: { run: number; grav: number; fall: number };
  hitstop: number; shake: number; flashT: number; hits: number; kills: number; time: number;
  onHeroChange?: (h: HeroId) => void;
}

/** Bộ sinh số ngẫu nhiên có hạt giống (mulberry32) — cùng hạt cho ra cùng dãy, để test tất định. */
export function seededRng(seed: number): () => number {
  let a = seed >>> 0;
  return () => { a = (a + 0x6d2b79f5) >>> 0; let t = a; t = Math.imul(t ^ (t >>> 15), t | 1); t ^= t + Math.imul(t ^ (t >>> 7), t | 61); return ((t ^ (t >>> 14)) >>> 0) / 4294967296; };
}

export function makeFoe(type: FoeType, tx: number, ty: number, ts: number, rng: () => number): Foe {
  const w = type === 'drone' ? 34 : 36, h = type === 'drone' ? 30 : 44, hp = type === 'drone' ? T.FOE_HP.drone : T.FOE_HP.turret;
  const ox = tx * ts, oy = ty * ts + ts - h;
  return { type, ox, oy, hx: ox, x: ox, y: oy, w, h, hp, maxHp: hp, vx: 0, vy: 0, flash: 0, dead: 0, t: rng() * 6, dir: -1, cd: 1 + rng(), fire: 0, kbx: 0, kby: 0 };
}

export function freshPlayer(cp: Checkpoint): Player {
  const w = 22, h = 78;
  return {
    w, h, x: cp.x, y: cp.y - h, ox: cp.x, oy: cp.y - h, vx: 0, vy: 0, face: 1, ground: false, coyote: 0, jumpBuf: 0,
    dash: 0, dashHeld: 0, airDashes: T.AIR_DASHES, airJumps: T.AIR_JUMPS, dj: 0, dashMomentum: false, wall: 0, wallLock: 0, wjT: 0, wjDir: 1,
    atk: null, atkT: 0, atkAir: false, combo: 0, comboQueued: false, hitSet: new Set(), spinK: 0, arc: null, swing: [], bladeA: T.BLADE_IDLE, bladeL: 1,
    ghosts: [], ghostT: 0, sqx: 1, sqy: 1, vsx: 0, vsy: 0, gait: 0, recover: 0, recFrame: null, turnT: 0, lastFace: 1, airT: 0, landT: 0, wasDash: false,
    hp: T.HP, inv: 0, warp: null, aiming: false, charge: 0, aimT: 0, lastLv: 0, reload: 0, shootT: 0, wasWet: false, drain: 0, hazT: 0,
  };
}

export function createGame(level: LevelDef, input: Input, sfx: Sfx = silentSfx, rng: () => number = Math.random): Game {
  const map = new TileMap(level.w, level.h, level.ts);
  for (const [x, y, w, h, v] of level.tiles) map.rect(x, y, w, h, v ?? 1);
  return {
    level, map, checkpoints: level.checkpoints, cpIndex: 0, hero: 'xl', P: freshPlayer(level.checkpoints[0]), input, sfx, rng,
    foes: level.foes.map(([t, x, y]) => makeFoe(t, x, y, level.ts, rng)), bolts: [], eshots: [], parts: [], fx: [],
    boss: createBoss(level.arena), bproj: [], arenaWater: 0, arenaHaz: 0, lvWaterRise: null, talk: null,
    phys: { run: 1, grav: 1, fall: 1 }, hitstop: 0, shake: 0, flashT: 0, hits: 0, kills: 0, time: 0,
  };
}
