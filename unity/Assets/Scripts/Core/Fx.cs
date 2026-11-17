using System;

namespace LongThan.Core
{
    /// <summary>Hạt và hiệu ứng ngắn (tia lửa, bụi, vòng nổ).</summary>
    public static class FxSys
    {
        /// <summary>Tia lửa văng tứ phía.</summary>
        public static void Sparks(Game g, float x, float y, int n, string c)
        {
            for (int i = 0; i < n; i++)
            {
                float a = g.R() * 6.28f, s = 80 + g.R() * 320;
                g.parts.Add(new Particle { x = x, y = y, vx = (float)Math.Cos(a) * s, vy = (float)Math.Sin(a) * s - 80, life = 0.25f + g.R() * 0.3f, c = c });
            }
        }
        /// <summary>Bụi dưới chân khi chạy, nhảy, đáp.</summary>
        public static void Dust(Game g, float x, float y, int n = 6)
        {
            for (int i = 0; i < n; i++) g.parts.Add(new Particle { x = x + (g.R() - 0.5f) * 20, y = y, vx = (g.R() - 0.5f) * 120, vy = -g.R() * 80, life = 0.3f, c = "#9aa3b5" });
        }
        public static void Ring(Game g, float x, float y, float life, bool big = false, string col = null) =>
            g.fx.Add(new Fx { k = FxKind.Ring, x = x, y = y, life = life, max = life, big = big, col = col });
        public static void Boom(Game g, float x, float y) => g.fx.Add(new Fx { k = FxKind.Boom, x = x, y = y, life = 0.42f, max = 0.42f });
        public static void ShotHit(Game g, float x, float y) => g.fx.Add(new Fx { k = FxKind.ShotHit, x = x, y = y, life = 0.15f, max = 0.15f });

        public static void UpdateFx(Game g, float dt)
        {
            foreach (var e in g.fx) e.life -= dt;
            g.fx.RemoveAll(e => e.life <= 0);
            g.flashT = Math.Max(0, g.flashT - dt);
        }
        public static void UpdateParts(Game g, float dt)
        {
            foreach (var p in g.parts) { p.life -= dt; if (!p.nog) p.vy += 1000 * dt; p.x += p.vx * dt; p.y += p.vy * dt; }
            g.parts.RemoveAll(p => p.life <= 0);
        }
    }
}
