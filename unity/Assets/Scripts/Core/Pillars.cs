// Cột đá Sơn Tinh (đoạn Chân núi Tản Viên): chém hoặc bắn vào cột thì cột dựng lên từng ô, nâng người đứng trên đỉnh theo.
// Ô cột có giá trị 3 (đặc, vẽ thành đá). Giống hệt game/src/world/pillars.ts.
namespace LongThan.Core
{
    public class Pillar { public int tx, @base, cur, max; public bool grow; public float t; }

    public static class PillarSys
    {
        public static System.Collections.Generic.List<Pillar> Create(Game g)
        {
            var list = new System.Collections.Generic.List<Pillar>();
            foreach (var d in g.level.pillars)
            {
                int tx = d[0], b = d[1], start = d[2], max = d[3];
                for (int j = 0; j < start; j++) g.map.Set(tx, b - j, 3);
                list.Add(new Pillar { tx = tx, @base = b, cur = start, max = max });
            }
            return list;
        }
        /// <summary>Mép trên của cột (px).</summary>
        public static float Top(Game g, Pillar c) => (c.@base - c.cur + 1) * g.map.ts;

        static void Trigger(Game g, Pillar c)
        {
            if (c.grow || c.cur >= c.max) return;
            c.grow = true; c.t = 0; g.shake = System.Math.Max(g.shake, 4); g.sfx.Land();
            FxSys.Sparks(g, (c.tx + 0.5f) * g.map.ts, Top(g, c), 14, "#C9A227");
        }

        public static void Update(Game g, float dt)
        {
            var P = g.P; int TS = g.map.ts; var box = PlayerSys.SlashBox(P);
            foreach (var c in g.pillars)
            {
                float x0 = c.tx * TS, top = Top(g, c), bot = (c.@base + 1) * TS;
                if (box.HasValue && P.atk != null && !P.hitSet.Contains(c))
                {
                    var b = box.Value;
                    if (b.x < x0 + TS && b.x + b.w > x0 && b.y < bot && b.y + b.h > top - 8) { P.hitSet.Add(c); Trigger(g, c); }
                }
                foreach (var bl in g.bolts) if (bl.life > 0 && bl.x > x0 - 6 && bl.x < x0 + TS + 6 && bl.y > top - 6 && bl.y < bot) { bl.life = 0; Trigger(g, c); }
                if (!c.grow) continue;
                c.t += dt;
                while (c.grow && c.t >= Cfg.T.PILLAR_STEP)
                {
                    c.t -= Cfg.T.PILLAR_STEP;
                    int row = c.@base - c.cur; if (row < 0) { c.grow = false; break; }
                    if (g.map.Get(c.tx, row) == 0) g.map.Set(c.tx, row, 3);
                    c.cur++;
                    float y = row * TS;   // ô mới mọc vào người chơi → nâng người chơi lên
                    if (P.x < x0 + TS && P.x + P.w > x0 && P.y < y + TS && P.y + P.h > y) { P.y = y - P.h; P.oy = System.Math.Min(P.oy, P.y); if (P.vy > 0) P.vy = 0; }
                    if (c.cur >= c.max) c.grow = false;
                }
            }
        }
    }
}
