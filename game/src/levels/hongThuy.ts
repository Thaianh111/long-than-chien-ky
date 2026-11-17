/**
 * Màn 1 — Hồng Thủy. Bố cục và lý do thiết kế: design/levels/hong-thuy-man1.md
 * Đơn vị: ô 32px. Mỗi dòng [x, y, rộng, cao, giá trị] (giá trị 0 = khoét trống).
 */
import type { BossArena, Checkpoint, FoeType, WaterRise, WaterZone } from '../core/types';

export interface LevelDef {
  name: string; w: number; h: number; ts: number;
  tiles: [number, number, number, number, number?][];
  foes: [FoeType, number, number][];
  checkpoints: Checkpoint[];
  water: { zones: WaterZone[]; rise: WaterRise };
  arena: BossArena;
}

const TS = 32, MW = 180, MH = 17;

export const HONG_THUY: LevelDef = {
  name: 'Hồng Thủy', w: MW, h: MH, ts: TS,
  tiles: [
    [0, 15, MW, 2], [0, 0, 1, MH], [MW - 1, 0, 1, MH],          // nền, tường trái, tường phải
    // Đoạn 1: phố ngập
    [8, 13, 3, 2], [14, 15, 5, 1, 0], [28, 13, 4, 2],
    // Đoạn 2: quảng trường thành hồ (bệ cột A–D, hố nước 1–3, biển hiệu nổi)
    [35, 13, 3, 2], [38, 11, 8, 4], [50, 11, 6, 4], [58, 13, 2, 2], [62, 11, 6, 4], [70, 8, 4, 1], [72, 13, 2, 2], [76, 11, 5, 4], [81, 13, 3, 2],
    // Đoạn 3: giếng nước dâng (tường trái + 3 gờ bám)
    [88, 2, 1, 10], [92, 11, 2, 1], [89, 8, 2, 1], [92, 6, 2, 1],
    // Đoạn 4: mái nhà A–E
    [94, 4, 8, 11], [105, 6, 7, 9], [114, 11, 2, 1], [118, 6, 6, 9], [127, 8, 8, 7], [139, 5, 5, 10],
    // Đoạn 5: đấu trường Thủy Tinh (tường trái có cổng ở hàng 12–14, trần)
    [158, 1, 1, 11], [158, 0, MW - 158, 1],
  ],
  foes: [
    ['drone', 24, 9], ['turret', 30, 12], ['turret', 54, 10], ['drone', 60, 6], ['turret', 79, 10],
    ['drone', 100, 1], ['turret', 110, 5], ['drone', 130, 4], ['turret', 142, 4],
  ],
  checkpoints: [
    { x: 3 * TS, y: 15 * TS }, { x: 51 * TS, y: 11 * TS }, { x: 85 * TS, y: 15 * TS }, { x: 119 * TS, y: 6 * TS }, { x: 148 * TS, y: 15 * TS },
  ],
  water: {
    zones: [
      { x0: 14 * TS, x1: 19 * TS, y: 15 * TS + 8, deep: false },
      { x0: 46 * TS, x1: 50 * TS, y: 12 * TS }, { x0: 56 * TS, x1: 62 * TS, y: 12 * TS }, { x0: 68 * TS, x1: 76 * TS, y: 12 * TS },
      { x0: 102 * TS, x1: 105 * TS, y: 12 * TS }, { x0: 112 * TS, x1: 118 * TS, y: 12 * TS },
      { x0: 124 * TS, x1: 127 * TS, y: 12 * TS }, { x0: 135 * TS, x1: 139 * TS, y: 12 * TS },
    ],
    rise: { triggerX: 90 * TS, x0: 88 * TS, x1: 94 * TS, startY: 16 * TS, endY: 5 * TS, speed: 48, delay: 1.0, lockTiles: { x: 88, y: 12, w: 1, h: 3 } },
  },
  arena: { x0: 159 * TS, x1: 179 * TS, gateX: 158 * TS, closeAtX: 161 * TS, floorY: 15 * TS, gate: { x: 158, y: 12, w: 1, h: 3 } },
};
