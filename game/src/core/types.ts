import type { SlashStep } from './config';

export type HeroId = 'xl' | 'tv';
export type Key = 'left' | 'right' | 'jump' | 'attack' | 'dash' | 'swap';
export const KEYS: Key[] = ['left', 'right', 'jump', 'attack', 'dash', 'swap'];

/** Hộp chữ nhật trong thế giới (px). */
export interface Box { x: number; y: number; w: number; h: number }
/** Vật thể có va chạm với ô gạch. */
export interface Body extends Box { vx: number; vy: number; ground?: boolean }

export interface Arc { a0: number; a1: number; from: number; to: number; len: number; big: boolean; t: number; at: { x: number; y: number } }
export interface Ghost { x: number; y: number; f: number; a: number; n: string }
export interface Warp { t: number; landed?: boolean }

/** Người chơi (dùng chung cho Xích Long và Thanh Vũ). */
export interface Player extends Body {
  ox: number; oy: number; face: number; ground: boolean; coyote: number; jumpBuf: number;
  dash: number; dashHeld: number; airDashes: number; airJumps: number; dj: number; dashMomentum: boolean;
  wall: number; wallLock: number; wjT: number; wjDir: number;
  atk: SlashStep | null; atkT: number; atkAir: boolean; combo: number; comboQueued: boolean; hitSet: Set<object>; spinK: number;
  arc: Arc | null; swing: { a: number; l: number; life: number }[]; bladeA: number; bladeL: number;
  ghosts: Ghost[]; ghostT: number; sqx: number; sqy: number; vsx: number; vsy: number;
  gait: number; recover: number; recFrame: string | null; turnT: number; lastFace: number; airT: number; landT: number; wasDash: boolean;
  hp: number; inv: number; warp: Warp | null;
  aiming: boolean; charge: number; aimT: number; lastLv: number; reload: number; shootT: number;
  wasWet: boolean; drain: number; hazT: number;
}

export type FoeType = 'drone' | 'turret';
export interface Foe extends Body {
  type: FoeType; ox: number; oy: number; hx: number; hp: number; maxHp: number; flash: number; dead: number;
  t: number; dir: number; cd: number; fire: number; kbx: number; kby: number;
}
export interface Bolt { x: number; y: number; vx: number; lv: number; life: number; hit: Set<object> }
export interface EnemyShot { x: number; y: number; vx: number; vy: number; life: number; t: number }
export interface Particle { x: number; y: number; vx: number; vy: number; life: number; c: string; nog?: number }
export type Fx =
  | { k: 'ring'; x: number; y: number; life: number; max: number; big?: boolean | number; col?: string }
  | { k: 'cut'; x: number; y: number; life: number; max: number; a: number; L: number }
  | { k: 'boom' | 'shothit'; x: number; y: number; life: number; max: number };

export interface Checkpoint { x: number; y: number }
export interface WaterZone { x0: number; x1: number; y: number; deep?: boolean }
export interface WaterRise { triggerX: number; x0: number; x1: number; startY: number; endY: number; speed: number; delay: number; lockTiles: { x: number; y: number; w: number; h: number } }
export interface BossArena { x0: number; x1: number; gateX: number; closeAtX: number; floorY: number; gate: { x: number; y: number; w: number; h: number } }

export interface Talk { lines: string[]; t: number; dur: number; who?: string }
