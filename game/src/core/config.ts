/**
 * Số liệu chỉnh tay của game, đọc từ file JSON trong src/data (không viết cứng trong code).
 * - tuning.json: cảm giác điều khiển, sát thương, quái thường.
 * - boss-thuy-tinh.json: trùm Thủy Tinh (thiết kế: design/gdd/boss-thuy-tinh.md).
 * - frames.json: điểm neo và thứ tự khung hình của nhân vật, trùm.
 */
import tuning from '../data/tuning.json';
import bossThuyTinh from '../data/boss-thuy-tinh.json';
import frames from '../data/frames.json';

/** Một nhát chém (combo hoặc chém xoay trên không). */
export interface SlashStep {
  time: number; from: number; to: number; a0: number; a1: number; len: number;
  reach: number; h: number; dmg: number; lunge: number; knock: number; spins?: number;
}

export type Tuning = typeof tuning & { COMBO: SlashStep[]; AIR_SLASH: SlashStep };
export const T = tuning as Tuning;
export type BossTuning = typeof bossThuyTinh;
export const BT: BossTuning = bossThuyTinh;

export interface AttackFrames { pre: string[]; act: string[]; post: string[]; rec: string }
export const FA: Record<string, number> = frames.FA;
export const FY: Record<string, number> = frames.FY;
export const MZ: Record<string, { x: number; y: number }> = frames.MZ;
export const SWING_AT: Record<string, { x: number; y: number }> = frames.SWING_AT;
export const ANIM: Record<string, string[]> = frames.ANIM;
export const ATKF: Record<string, AttackFrames> = frames.ATKF;
export const BTA = frames.BTA as unknown as Record<string, [number, number]>;

/** Kích thước khung nhân vật chính (px) — xem art/STYLE_BIBLE.md. */
export const FW = 150, FH = 110;
/** Sprite dự phòng khi khung hình chưa tải xong. */
export const SPR_W = 41, SPR_H = 84, PIVOT = { x: 30, y: 45 };
/** Màn hình logic: cao cố định 360px, rộng 560–800 tùy tỉ lệ máy. */
export const VH = 360;
/** Bước vật lý cố định 120 lần/giây; vẽ nội suy giữa 2 bước. */
export const STEP = 1 / 120;
