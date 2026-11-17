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
        /// <summary>Đấu trường trùm phụ Thuồng Luồng (Bến Bạch Hạc).</summary>
        public SubArena sub;
        /// <summary>Cột đá Sơn Tinh: {cột x, hàng chân, số ô lúc đầu, số ô khi dựng hết}.</summary>
        public int[][] pillars;
    }

    public static class Levels
    {
        const int TS = 32, MW = 264, MH = 17;

        /// <summary>Màn 1 — Hồng Thủy: phố ngập, quảng trường, giếng nước dâng, mái nhà, Bến Bạch Hạc (Thuồng Luồng), chân núi Tản Viên, đấu trường Thủy Tinh.</summary>
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
                // Đoạn 5: Bến Bạch Hạc — đấu Thuồng Luồng (bờ kè, tường có cổng hàng 9–11, sông sâu, 3 cọc gỗ)
                new[] { 150, 12, 5, 3 }, new[] { 155, 1, 1, 8 }, new[] { 155, 12, 1, 3 }, new[] { 155, 0, 24, 1 }, new[] { 156, 12, 2, 3 },
                new[] { 158, 15, 18, 1, 0 }, new[] { 161, 11, 2, 5 }, new[] { 166, 11, 2, 5 }, new[] { 171, 11, 2, 5 }, new[] { 176, 12, 2, 3 },
                new[] { 178, 1, 1, 8 }, new[] { 178, 9, 1, 3 }, new[] { 178, 12, 1, 3 },
                // Đoạn 6: Chân núi Tản Viên — vách A, sông có cột đá Sơn Tinh, vách B, bậc xuống
                new[] { 188, 7, 10, 8 }, new[] { 208, 7, 10, 8 }, new[] { 218, 9, 3, 6 }, new[] { 221, 11, 3, 4 }, new[] { 224, 13, 3, 2 },
                // Đoạn 7: đấu trường Thủy Tinh
                new[] { 242, 1, 1, 11 }, new[] { 242, 0, MW - 242, 1 },
            },
            foes = new List<(FoeType, int, int)>
            {
                (FoeType.Drone, 24, 9), (FoeType.Turret, 30, 12), (FoeType.Turret, 54, 10), (FoeType.Drone, 60, 6), (FoeType.Turret, 79, 10),
                (FoeType.Drone, 100, 1), (FoeType.Turret, 110, 5), (FoeType.Drone, 130, 4), (FoeType.Turret, 142, 4),
                (FoeType.Drone, 185, 10), (FoeType.Turret, 195, 6), (FoeType.Drone, 203, 6), (FoeType.Turret, 215, 6), (FoeType.Drone, 222, 6), (FoeType.Turret, 232, 14), (FoeType.Drone, 236, 11),
            },
            checkpoints = new[] { new Checkpoint(3 * TS, 15 * TS), new Checkpoint(51 * TS, 11 * TS), new Checkpoint(85 * TS, 15 * TS), new Checkpoint(119 * TS, 6 * TS), new Checkpoint(147 * TS, 15 * TS),
                new Checkpoint(182 * TS, 15 * TS), new Checkpoint(238 * TS, 15 * TS) },
            zones = new[]
            {
                new WaterZone { x0 = 14 * TS, x1 = 19 * TS, y = 15 * TS + 8, deep = false },
                new WaterZone { x0 = 46 * TS, x1 = 50 * TS, y = 12 * TS }, new WaterZone { x0 = 56 * TS, x1 = 62 * TS, y = 12 * TS }, new WaterZone { x0 = 68 * TS, x1 = 76 * TS, y = 12 * TS },
                new WaterZone { x0 = 102 * TS, x1 = 105 * TS, y = 12 * TS }, new WaterZone { x0 = 112 * TS, x1 = 118 * TS, y = 12 * TS },
                new WaterZone { x0 = 124 * TS, x1 = 127 * TS, y = 12 * TS }, new WaterZone { x0 = 135 * TS, x1 = 139 * TS, y = 12 * TS },
                new WaterZone { x0 = 158 * TS, x1 = 176 * TS, y = 13 * TS }, new WaterZone { x0 = 198 * TS, x1 = 208 * TS, y = 13 * TS },
            },
            rise = new WaterRise { triggerX = 90 * TS, x0 = 88 * TS, x1 = 94 * TS, startY = 16 * TS, endY = 5 * TS, speed = 48, delay = 1f, lockTiles = new TileRect { x = 88, y = 12, w = 1, h = 3 } },
            arena = new BossArena { x0 = 243 * TS, x1 = 263 * TS, gateX = 242 * TS, closeAtX = 245 * TS, floorY = 15 * TS, gate = new TileRect { x = 242, y = 12, w = 1, h = 3 } },
            sub = new SubArena { x0 = 156 * TS, x1 = 178 * TS, closeAtX = 156 * TS + 16, surface = 13 * TS, platRow = 12, gate = new TileRect { x = 155, y = 9, w = 1, h = 3 }, exit = new TileRect { x = 178, y = 9, w = 1, h = 3 } },
            pillars = new[] { new[] { 186, 14, 2, 6 }, new[] { 200, 14, 2, 7 }, new[] { 204, 14, 2, 7 } },
        };
    }
}
