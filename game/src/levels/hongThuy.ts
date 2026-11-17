/**
 * Màn 1 — Hồng Thủy. Bố cục và lý do thiết kế: design/levels/hong-thuy-man1.md
 * Đơn vị: ô 32px. Mỗi dòng [x, y, rộng, cao, giá trị] (giá trị 0 = khoét trống).
 */
import type { BossArena, Checkpoint, FoeType, PillarDef, SubArena, WaterRise, WaterZone } from '../core/types';

export interface LevelDef {
  name: string; w: number; h: number; ts: number;
  tiles: [number, number, number, number, number?][];
  foes: [FoeType, number, number][];
  checkpoints: Checkpoint[];
  water: { zones: WaterZone[]; rise: WaterRise };
  arena: BossArena;
  /** Đấu trường trùm phụ Thuồng Luồng (Bến Bạch Hạc). */
  sub: SubArena;
  /** Cột đá Sơn Tinh: [cột x, hàng chân, số ô lúc đầu, số ô khi dựng hết]. */
  pillars: PillarDef[];
}

const TS = 32, MW = 264, MH = 17;

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
    // Đoạn 5: Bến Bạch Hạc — đấu Thuồng Luồng (bờ kè, tường có cổng hàng 9–11, sông sâu, 3 cọc gỗ)
    [150, 12, 5, 3], [155, 1, 1, 8], [155, 12, 1, 3], [155, 0, 24, 1], [156, 12, 2, 3],
    [158, 15, 18, 1, 0], [161, 11, 2, 5], [166, 11, 2, 5], [171, 11, 2, 5], [176, 12, 2, 3],
    [178, 1, 1, 8], [178, 9, 1, 3], [178, 12, 1, 3],
    // Đoạn 6: Chân núi Tản Viên — vách A, sông có cột đá Sơn Tinh, vách B, bậc xuống
    [188, 7, 10, 8], [208, 7, 10, 8], [218, 9, 3, 6], [221, 11, 3, 4], [224, 13, 3, 2],
    // Đoạn 7: đấu trường Thủy Tinh (tường trái có cổng ở hàng 12–14, trần)
    [242, 1, 1, 11], [242, 0, MW - 242, 1],
  ],
  foes: [
    ['drone', 24, 9], ['turret', 30, 12], ['turret', 54, 10], ['drone', 60, 6], ['turret', 79, 10],
    ['drone', 100, 1], ['turret', 110, 5], ['drone', 130, 4], ['turret', 142, 4],
    ['drone', 185, 10], ['turret', 195, 6], ['drone', 203, 6], ['turret', 215, 6], ['drone', 222, 6], ['turret', 232, 14], ['drone', 236, 11],
  ],
  checkpoints: [
    { x: 3 * TS, y: 15 * TS }, { x: 51 * TS, y: 11 * TS }, { x: 85 * TS, y: 15 * TS }, { x: 119 * TS, y: 6 * TS }, { x: 147 * TS, y: 15 * TS },
    { x: 182 * TS, y: 15 * TS }, { x: 238 * TS, y: 15 * TS },
  ],
  water: {
    zones: [
      { x0: 14 * TS, x1: 19 * TS, y: 15 * TS + 8, deep: false },
      { x0: 46 * TS, x1: 50 * TS, y: 12 * TS }, { x0: 56 * TS, x1: 62 * TS, y: 12 * TS }, { x0: 68 * TS, x1: 76 * TS, y: 12 * TS },
      { x0: 102 * TS, x1: 105 * TS, y: 12 * TS }, { x0: 112 * TS, x1: 118 * TS, y: 12 * TS },
      { x0: 124 * TS, x1: 127 * TS, y: 12 * TS }, { x0: 135 * TS, x1: 139 * TS, y: 12 * TS },
      { x0: 158 * TS, x1: 176 * TS, y: 13 * TS }, { x0: 198 * TS, x1: 208 * TS, y: 13 * TS },
    ],
    rise: { triggerX: 90 * TS, x0: 88 * TS, x1: 94 * TS, startY: 16 * TS, endY: 5 * TS, speed: 48, delay: 1.0, lockTiles: { x: 88, y: 12, w: 1, h: 3 } },
  },
  arena: { x0: 243 * TS, x1: 263 * TS, gateX: 242 * TS, closeAtX: 245 * TS, floorY: 15 * TS, gate: { x: 242, y: 12, w: 1, h: 3 } },
  sub: { x0: 156 * TS, x1: 178 * TS, closeAtX: 156 * TS + 16, surface: 13 * TS, platRow: 12, gate: { x: 155, y: 9, w: 1, h: 3 }, exit: { x: 178, y: 9, w: 1, h: 3 } },
  pillars: [[186, 14, 2, 6], [200, 14, 2, 7], [204, 14, 2, 7]],
};
