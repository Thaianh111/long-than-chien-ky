import { BT } from '../core/config';
import type { BossArena } from '../core/types';

export type BossState = 'wait' | 'intro' | 'idle' | 'atk' | 'stun' | 'shift' | 'dead';
export type BossMove = 'wave' | 'dash' | 'bubbles' | 'shield' | 'spears' | 'flood';
export type BossStage = 'W' | 'A' | 'R';

/** Trùm Thủy Tinh. x = giữa thân, y = mặt sàn dưới chân. */
export interface Boss {
  st: BossState; x: number; y: number; w: number; h: number; hp: number; face: number; inv: number; t: number;
  phase: 0 | 1; last: BossMove[]; first: boolean; floodCd: number; lostSince: number; flash: number; fly: boolean; stun: number;
  move: BossMove | null; stage: BossStage; n: number; spots: number[]; gateShut: boolean;
}

/** Đạn của trùm: sóng nền, bong bóng (chém bay ngược được), giọt nước, giáo nước. */
export interface BossShot {
  k: 'wave' | 'bub' | 'drop' | 'spear'; x: number; y: number; vx: number; vy: number; t: number; life: number; dmg: number;
  r: number; w?: number; h: number; refl?: boolean; back?: boolean; stuck?: boolean;
}

export function createBoss(A: BossArena): Boss {
  return {
    st: 'wait', x: A.x1 - 120, y: A.floorY, w: 64, h: 120, hp: BT.HP, face: -1, inv: 0, t: 0, phase: 0, last: [], first: false,
    floodCd: 0, lostSince: 0, flash: 0, fly: false, stun: 0, move: null, stage: 'W', n: 0, spots: [], gateShut: false,
  };
}
