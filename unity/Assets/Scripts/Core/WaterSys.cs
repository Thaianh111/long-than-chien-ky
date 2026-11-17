// Nước: hố nước tĩnh, giếng nước dâng đuổi theo, nước trong đấu trường. Dưới nước chậm hơn; nước sâu rút máu từ từ.
using System;

namespace LongThan.Core
{
    public static class WaterSys
    {
        /// <summary>Mặt nước (y thế giới) tại x; PositiveInfinity = chỗ này không có nước.</summary>
        public static float WaterY(Game g, float x)
        {
            var L = g.level; float y = float.PositiveInfinity;
            foreach (var z in L.zones) if (x >= z.x0 && x <= z.x1) y = Math.Min(y, z.y);
            if (g.lvWaterRise != null && x >= L.rise.x0 && x <= L.rise.x1) y = Math.Min(y, g.lvWaterRise.y);
            var A = L.arena;
            if (x >= A.x0 && x <= A.x1) { float h = Math.Max(g.arenaWater, g.arenaHaz); if (h > 0) y = Math.Min(y, A.floorY - h); }
            return y;
        }
        /// <summary>Nước sâu (rút máu) hay vũng nông (chỉ làm chậm).</summary>
        public static bool DeepAt(Game g, float x)
        {
            foreach (var z in g.level.zones) if (x >= z.x0 && x <= z.x1) return z.deep;
            return g.lvWaterRise != null && x >= g.level.rise.x0 && x <= g.level.rise.x1;
        }
        public static bool PlayerInWater(Game g) => g.P.y + g.P.h * 0.6f > WaterY(g, g.P.x + g.P.w / 2);

        /// <summary>Kích hoạt và dâng nước trong giếng khi người chơi bước vào.</summary>
        public static void UpdateRising(Game g, float dt)
        {
            var R = g.level.rise; float pcx = g.P.x + g.P.w / 2;
            if (g.lvWaterRise == null && pcx > R.triggerX && pcx < R.x1 && g.boss.st == BossSt.Wait)
            {
                g.lvWaterRise = new RiseState { y = R.startY };
                g.map.Rect(R.lockTiles.x, R.lockTiles.y, R.lockTiles.w, R.lockTiles.h, 1);   // cửa sập sau lưng
                g.shake = 8; g.sfx.Land(); BossSys.Say(g, new[] { "Cửa sập! Nước đang dâng — leo lên mau!" }, 2.2f, "Ngọc Diệp");
            }
            if (g.lvWaterRise != null) { g.lvWaterRise.t += dt; if (g.lvWaterRise.t > R.delay) g.lvWaterRise.y = Math.Max(R.endY, g.lvWaterRise.y - R.speed * dt); }
        }

        /// <summary>Đặt hệ số vật lý dưới nước trước bước người chơi.</summary>
        public static bool Before(Game g)
        {
            var P = g.P; var K = Cfg.T.WATER; bool wet = P.warp == null && PlayerInWater(g);
            if (wet) { g.physRun = K.RUN; g.physGrav = K.GRAV; g.physFall = K.FALL; } else { g.physRun = g.physGrav = g.physFall = 1; }
            if (wet && !P.wasWet && P.vy > 100)   // té xuống nước: bắn tung bọt
            {
                float x = P.x + P.w / 2, y = WaterY(g, x);
                for (int i = 0; i < 14; i++) g.parts.Add(new Particle { x = x, y = y, vx = (g.R() - 0.5f) * 300, vy = -150 - g.R() * 250, life = 0.4f, c = "#bdefff" });
            }
            P.wasWet = wet;
            return wet;
        }
        /// <summary>Nước sâu rút máu sau bước người chơi.</summary>
        public static void After(Game g, bool wet, float dt)
        {
            var P = g.P; float x = P.x + P.w / 2;
            if (wet && DeepAt(g, x) && P.y + P.h / 2 > WaterY(g, x))
            {
                P.drain += Cfg.T.WATER.DRAIN * dt;
                if (P.drain >= 1) { P.drain -= 1; P.hp -= 1; FxSys.Sparks(g, x, P.y + 10, 4, "#bdefff"); if (P.hp <= 0) Flow.Respawn(g); }
            }
        }
    }
}
