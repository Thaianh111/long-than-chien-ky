// Màn chơi. Bố cục và lý do thiết kế màn 1: design/levels/hong-thuy-man1.md
using System.Collections.Generic;

namespace LongThan.Core
{
    public class LevelDef
    {
        public string name; public int w, h, ts;
        /// <summary>Mỗi dòng {x, y, rộng, cao, giá trị}; giá trị 0 = khoét trống.</summary>
        public int[][] tiles;
        public List<(FoeType type, int tx, int ty)> foes;
        public Checkpoint[] checkpoints;
        public WaterZone[] zones; public WaterRise rise; public BossArena arena;
    }

    public static class Levels
    {
        const int TS = 32, MW = 180, MH = 17;

        /// <summary>Màn 1 — Hồng Thủy: phố ngập, quảng trường, giếng nước dâng, mái nhà, đấu trường Thủy Tinh.</summary>
        public static LevelDef HongThuy() => new LevelDef
        {
            name = "Hồng Thủy", w = MW, h = MH, ts = TS,
            tiles = new[]
            {
                new[] { 0, 15, MW, 2 }, new[] { 0, 0, 1, MH }, new[] { MW - 1, 0, 1, MH },
                // Đoạn 1: phố ngập
                new[] { 8, 13, 3, 2 }, new[] { 14, 15, 5, 1, 0 }, new[] { 28, 13, 4, 2 },
                // Đoạn 2: quảng trường thành hồ
                new[] { 35, 13, 3, 2 }, new[] { 38, 11, 8, 4 }, new[] { 50, 11, 6, 4 }, new[] { 58, 13, 2, 2 }, new[] { 62, 11, 6, 4 },
                new[] { 70, 8, 4, 1 }, new[] { 72, 13, 2, 2 }, new[] { 76, 11, 5, 4 }, new[] { 81, 13, 3, 2 },
                // Đoạn 3: giếng nước dâng
                new[] { 88, 2, 1, 10 }, new[] { 92, 11, 2, 1 }, new[] { 89, 8, 2, 1 }, new[] { 92, 6, 2, 1 },
                // Đoạn 4: mái nhà A–E
                new[] { 94, 4, 8, 11 }, new[] { 105, 6, 7, 9 }, new[] { 114, 11, 2, 1 }, new[] { 118, 6, 6, 9 }, new[] { 127, 8, 8, 7 }, new[] { 139, 5, 5, 10 },
                // Đoạn 5: đấu trường Thủy Tinh
                new[] { 158, 1, 1, 11 }, new[] { 158, 0, MW - 158, 1 },
            },
            foes = new List<(FoeType, int, int)>
            {
                (FoeType.Drone, 24, 9), (FoeType.Turret, 30, 12), (FoeType.Turret, 54, 10), (FoeType.Drone, 60, 6), (FoeType.Turret, 79, 10),
                (FoeType.Drone, 100, 1), (FoeType.Turret, 110, 5), (FoeType.Drone, 130, 4), (FoeType.Turret, 142, 4),
            },
            checkpoints = new[] { new Checkpoint(3 * TS, 15 * TS), new Checkpoint(51 * TS, 11 * TS), new Checkpoint(85 * TS, 15 * TS), new Checkpoint(119 * TS, 6 * TS), new Checkpoint(148 * TS, 15 * TS) },
            zones = new[]
            {
                new WaterZone { x0 = 14 * TS, x1 = 19 * TS, y = 15 * TS + 8, deep = false },
                new WaterZone { x0 = 46 * TS, x1 = 50 * TS, y = 12 * TS }, new WaterZone { x0 = 56 * TS, x1 = 62 * TS, y = 12 * TS }, new WaterZone { x0 = 68 * TS, x1 = 76 * TS, y = 12 * TS },
                new WaterZone { x0 = 102 * TS, x1 = 105 * TS, y = 12 * TS }, new WaterZone { x0 = 112 * TS, x1 = 118 * TS, y = 12 * TS },
                new WaterZone { x0 = 124 * TS, x1 = 127 * TS, y = 12 * TS }, new WaterZone { x0 = 135 * TS, x1 = 139 * TS, y = 12 * TS },
            },
            rise = new WaterRise { triggerX = 90 * TS, x0 = 88 * TS, x1 = 94 * TS, startY = 16 * TS, endY = 5 * TS, speed = 48, delay = 1f, lockTiles = new TileRect { x = 88, y = 12, w = 1, h = 3 } },
            arena = new BossArena { x0 = 159 * TS, x1 = 179 * TS, gateX = 158 * TS, closeAtX = 161 * TS, floorY = 15 * TS, gate = new TileRect { x = 158, y = 12, w = 1, h = 3 } },
        };
    }
}
