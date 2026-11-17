// Quái thường (drone bay, tháp súng), đạn của quái, tên nỏ của Thanh Vũ.
using System;

namespace LongThan.Core
{
    public static class FoeSys
    {
        static void Kill(Game g, Foe f, string col)
        {
            f.dead = 2.5f; FxSys.Boom(g, f.x + f.w / 2, f.y + f.h / 2); FxSys.Sparks(g, f.x + f.w / 2, f.y + f.h / 2, 26, col); g.kills++;
        }

        public static void UpdateShots(Game g, float dt)
        {
            var P = g.P; var box = P.atk != null ? PlayerSys.SlashBox(P) : null;
            foreach (var b in g.eshots)
            {
                b.x += b.vx * dt; b.y += b.vy * dt; b.life -= dt; b.t += dt;
                if (g.map.SolidAt(b.x, b.y)) { b.life = 0; FxSys.ShotHit(g, b.x, b.y); continue; }
                if (box is Box bx && b.x > bx.x && b.x < bx.x + bx.w && b.y > bx.y && b.y < bx.y + bx.h) { b.life = 0; FxSys.ShotHit(g, b.x, b.y); FxSys.Sparks(g, b.x, b.y, 8, "#ffd34d"); continue; }   // chém đạn như Zero
                foreach (var t in g.bolts) if (Math.Abs(t.x - b.x) < 14 && Math.Abs(t.y - b.y) < 12) { b.life = 0; FxSys.ShotHit(g, b.x, b.y); break; }
                if (b.life > 0 && b.x > P.x && b.x < P.x + P.w && b.y > P.y + 6 && b.y < P.y + P.h) { b.life = 0; FxSys.ShotHit(g, b.x, b.y); PlayerSys.Hurt(g, Cfg.T.SHOT_DMG, b.x - b.vx); }
            }
            g.eshots.RemoveAll(b => b.life <= 0);
        }

        public static void Update(Game g, float dt)
        {
            var P = g.P; var T = Cfg.T; var box = PlayerSys.SlashBox(P);
            foreach (var f in g.foes)
            {
                if (f.dead > 0) { f.dead -= dt; if (f.dead <= 0) { f.ox = f.hx; f.x = f.hx; f.y = f.oy; f.hp = f.maxHp; f.vx = 0; f.vy = 0; f.kbx = 0; } continue; }
                f.t += dt; f.flash = Math.Max(0, f.flash - dt);
                float pcx = P.x + P.w / 2, pcy = P.y + P.h / 2;
                if (P.warp == null && pcx > f.x - 4 && pcx < f.x + f.w + 4 && P.y < f.y + f.h && P.y + P.h > f.y) PlayerSys.Hurt(g, T.TOUCH_DMG, f.x + f.w / 2);
                if (f.type == FoeType.Turret)
                {
                    f.dir = pcx < f.x + f.w / 2 ? -1 : 1; f.cd -= dt; f.fire = Math.Max(0, f.fire - dt);
                    if (f.cd <= 0 && Math.Abs(pcx - f.x) < T.TURRET_RANGE && Math.Abs(pcy - f.y) < 160 && P.warp == null)
                    {
                        f.cd = T.TURRET_CD; f.fire = 0.36f;
                        g.eshots.Add(new EnemyShot { x = f.x + f.w / 2 + f.dir * 16, y = f.y + 8, vx = f.dir * T.SHOT_SPEED, life = 3 });
                    }
                }
                else
                {
                    if (Math.Abs(pcx - f.x) < 300) f.ox += Math.Sign(pcx - f.x - f.w / 2) * 22 * dt;   // lừ đừ bám theo
                    f.vx *= (float)Math.Pow(0.02, dt); f.kbx = (f.kbx + f.vx * dt) * (float)Math.Pow(0.2, dt); f.x = f.ox + f.kbx;
                    f.y = f.oy + (float)Math.Sin(f.t * 2) * 10 + f.kby; f.kby *= (float)Math.Pow(0.05, dt);
                }
                if (box is Box bx && P.atk != null && !P.hitSet.Contains(f) && bx.x < f.x + f.w && bx.x + bx.w > f.x && bx.y < f.y + f.h && bx.y + bx.h > f.y)
                {
                    P.hitSet.Add(f); f.hp -= P.atk.dmg; f.flash = 0.12f;
                    f.vx = f.type == FoeType.Turret ? 0 : P.face * P.atk.knock; if (f.type == FoeType.Drone) f.kby = -16;
                    bool big = P.atk.dmg > 1; float cxh = f.x + f.w / 2, cyh = f.y + f.h / 2;
                    g.hitstop = T.HITSTOP * (big ? T.FINISH_STOP : 1); g.shake = T.SHAKE * (big ? 2.2f : 1);
                    // vết cắt sáng chéo qua quái + vòng nổ + mảnh văng theo hướng chém
                    g.fx.Add(new Fx { k = FxKind.Cut, x = cxh, y = cyh, a = (P.face > 0 ? 0 : (float)Math.PI) + (P.atk.a1 > P.atk.a0 ? 0.55f : -0.55f), L = big ? 120 : 80, life = 0.14f, max = 0.14f });
                    FxSys.Ring(g, cxh, cyh, big ? 0.22f : 0.16f, big);
                    if (big) g.flashT = 0.06f;
                    for (int i = 0; i < (big ? 18 : 10); i++)
                    {
                        float a = (P.face > 0 ? 0 : (float)Math.PI) + (g.R() - 0.5f) * 1.6f, sp = 200 + g.R() * 420;
                        g.parts.Add(new Particle { x = cxh, y = cyh, vx = (float)Math.Cos(a) * sp, vy = (float)Math.Sin(a) * sp - 120, life = 0.2f + g.R() * 0.3f, c = i % 3 != 0 ? "#ff6a2b" : "#fff1b8" });
                    }
                    g.sfx.Hit(big); g.hits++;
                    if (f.hp <= 0) Kill(g, f, "#ffd34d");
                }
            }
        }

        public static void UpdateBolts(Game g, float dt)
        {
            var T = Cfg.T;
            foreach (var b in g.bolts)
            {
                b.x += b.vx * dt; b.life -= dt;
                if (g.map.SolidAt(b.x, b.y)) { b.life = 0; FxSys.Sparks(g, b.x - Math.Sign(b.vx) * 4, b.y, 6 + b.lv * 4, "#7fe3ff"); continue; }
                if (g.R() < 0.6f) g.parts.Add(new Particle { x = b.x - Math.Sign(b.vx) * 10, y = b.y + (g.R() - 0.5f) * (4 + b.lv * 6), vx = -b.vx * 0.05f, life = 0.12f + b.lv * 0.05f, c = b.lv > 0 ? "#bff4ff" : "#7fe3ff", nog = true });
                float r = T.TV.SIZE[b.lv];
                foreach (var f in g.foes)
                {
                    if (f.dead > 0 || b.hit.Contains(f)) continue;
                    if (b.x + r > f.x && b.x - r < f.x + f.w && b.y + r > f.y && b.y - r < f.y + f.h)
                    {
                        b.hit.Add(f); f.hp -= T.TV.DMG[b.lv]; f.flash = 0.12f; f.vx = f.type == FoeType.Turret ? 0 : Math.Sign(b.vx) * (160 + b.lv * 140);
                        if (f.type == FoeType.Drone) f.kby = -8 - b.lv * 6;
                        g.hitstop = Math.Max(g.hitstop, T.TV.STOP[b.lv]); g.shake = Math.Max(g.shake, T.SHAKE * (0.5f + b.lv * 0.5f));
                        FxSys.Ring(g, b.x, b.y, 0.16f, b.lv == 2, "#7fe3ff");
                        FxSys.Sparks(g, b.x, b.y, 8 + b.lv * 6, "#7fe3ff"); g.sfx.Hit(b.lv == 2); g.hits++;
                        if (f.hp <= 0) Kill(g, f, "#bff4ff");
                        if (b.lv < 2) { b.life = 0; break; }   // tên nạp đầy xuyên qua quái
                    }
                }
            }
            g.bolts.RemoveAll(b => b.life <= 0);
        }
    }
}
