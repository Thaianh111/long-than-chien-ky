// Vẽ một khung hình (giống bản web): nền thị sai, ô gạch, cờ lưu, quái, trùm, người chơi, hiệu ứng, thanh máu.
// Chữ (HUD, thoại) vẽ riêng bằng OnGUI trong GameRunner.
using System.Collections.Generic;
using LongThan.Core;
using UnityEngine;
using G = LongThan.Core.Game;

namespace LongThan.Game
{
    public class WorldRenderer
    {
        readonly Painter p = new Painter();
        readonly Dictionary<string, Texture2D> tex = new Dictionary<string, Texture2D>();

        /// <summary>Ảnh khung f_&lt;tên&gt;.png trong Resources/Frames (null nếu thiếu).</summary>
        public Texture2D Frame(string n)
        {
            if (n == null) return null;
            if (!tex.TryGetValue(n, out var t)) { t = Resources.Load<Texture2D>("Frames/f_" + n); tex[n] = t; }
            return t;
        }
        Texture2D Bg(string n) { var k = "bg:" + n; if (!tex.TryGetValue(k, out var t)) { t = Resources.Load<Texture2D>("Bg/" + n); tex[k] = t; } return t; }
        static Color C(string s) => Painter.Col(s);
        static Color A(string s, float a) { var c = Painter.Col(s); c.a *= a; return c; }

        public void Draw(G g, float VW, float camX, float camY, float alpha)
        {
            float VH = Cfg.VH; var map = g.map; int TS = map.ts;
            p.Begin(VW, VH);
            float sx = g.shake > 0 ? (Random.value - 0.5f) * g.shake * 2 : 0, sy = g.shake > 0 ? (Random.value - 0.5f) * g.shake * 2 : 0;
            p.GradientRect(0, 0, VW, VH, C("#141a2e"), C("#2a2440"));
            p.Save(); p.Translate(Mathf.Round(sx), Mathf.Round(sy));
            // nền thành phố Warped City (Ansimuz, CC0), 3 lớp trôi chậm theo độ xa
            Parallax(Bg("skyline-a"), camX, VW, 0.05f, 0, 1.5f);
            Parallax(Bg("buildings-bg"), camX, VW, 0.15f, VH - 124 * 1.5f - 20, 1.5f);
            p.alpha = 0.85f; Parallax(Bg("near-buildings-bg"), camX, VW, 0.3f, VH - 209 * 1.5f + 40, 1.5f); p.alpha = 1;
            p.FillRect(0, 0, VW, VH, C("rgba(14,10,30,.55)"));

            float ox = Mathf.Round(camX), oy = Mathf.Round(camY);
            int i0 = Mathf.FloorToInt(ox / TS), i1 = i0 + Mathf.CeilToInt(VW / TS) + 2;
            for (int j = 0; j < map.h; j++) for (int i = i0; i < i1; i++)
            {
                int cv = map.Get(i, j); if (cv == 0 || cv == 2) continue;
                float X = i * TS - ox, Y = j * TS - oy;
                if (cv == 3)   // cột đá Sơn Tinh: đá nâu, vân vàng
                {
                    p.FillRect(X + 2, Y, TS - 4, TS, "#5b4a3a"); p.FillRect(X + 2, Y, 6, TS, "#72604b");
                    p.FillRect(X + 14, Y + 8, 4, 16, "#C9A227"); p.FillRect(X + 10, Y + 14, 12, 3, "#C9A227");
                    if (map.Get(i, j - 1) != 3) p.FillRect(X + 2, Y, TS - 4, 3, "#e0c060");
                    continue;
                }
                p.FillRect(X, Y, TS, TS, "#2e3346"); p.FillRect(X, Y, TS, 5, "#3c4259");
                p.FillRect(X, Y + TS - 2, TS, 2, "#262a3a"); p.FillRect(X + TS - 2, Y, 2, TS, "#262a3a");
                if (!map.SolidCell(i, j - 1) || map.Get(i, j - 1) == 2) p.FillRect(X, Y, TS, 2, "#C9A227");
            }
            for (int i = 0; i < g.checkpoints.Length; i++)
            {
                var c = g.checkpoints[i]; float X = c.x - ox, Y = c.y - oy;
                p.FillRect(X, Y - 52, 3, 52, "#888888"); p.FillRect(X + 3, Y - 52, 22, 14, i <= g.cpIndex ? "#d8352a" : "#555555");
            }
            DrawFoes(g, ox, oy);
            DrawBoss(g, ox, oy); DrawSub(g, ox, oy);
            if (!(g.P.inv > 0 && Mathf.FloorToInt(g.P.inv * 20) % 2 == 1)) DrawPlayer(g, camX, camY, alpha);   // nhấp nháy khi bất tử
            foreach (var pa in g.parts) p.FillRect(Mathf.Round(pa.x - ox), Mathf.Round(pa.y - oy), 3, 3, A(pa.c, Mathf.Min(1, pa.life * 3)));
            DrawBolts(g, ox, oy); DrawWater(g, VW, ox, oy); DrawFx(g, ox, oy);
            // thanh máu kiểu Mega Man
            int hpMax = Cfg.T.HP;
            p.FillRect(14, 74, 12, hpMax * 6 + 4, Color.black);
            for (int i = 0; i < hpMax; i++) p.FillRect(16, 76 + (hpMax - 1 - i) * 6, 8, 4, i < g.P.hp ? C(g.hero == Hero.ThanhVu ? "#7fe3ff" : "#ffb347") : C("#2a2a3a"));
            if (g.flashT > 0) p.FillRect(0, 0, VW, VH, A("#fff1d6", g.flashT / 0.06f * 0.35f));
            DrawBossHud(g, VW); DrawSubHud(g, VW);
            DrawTalkBox(g, VW);
            var B = g.boss;
            if (B.st == BossSt.Dead && B.t > 2.4f) p.FillRect(0, VH / 2 - 34, VW, 60, C("rgba(0,0,0,.5)"));
            p.Restore();
            p.End();
        }

        void Parallax(Texture2D im, float camX, float VW, float f, float y, float k)
        {
            if (!im) return;
            float w = im.width * k, x0 = -((camX * f) % w);
            for (float x = x0 - w; x < VW; x += w) p.DrawImage(im, Mathf.Round(x), y, w, im.height * k);
        }

        void DrawFoes(G g, float ox, float oy)
        {
            foreach (var f in g.foes)
            {
                if (f.dead > 0) continue;
                float X = Mathf.Round(f.x - ox), Y = Mathf.Round(f.y - oy);
                var img = f.type == FoeType.Drone ? Frame("en_drone" + (1 + Mathf.FloorToInt(f.t * 8) % 4)) : Frame("en_turret" + (f.fire > 0 ? 1 + Mathf.FloorToInt((0.36f - f.fire) / 0.06f) % 6 : 1));
                p.Save(); p.Translate(X + f.w / 2, Y + f.h); p.Scale(f.type == FoeType.Turret ? f.dir : 1, 1);
                if (img)
                {
                    float dy = -img.height + (f.type == FoeType.Drone ? 11 : 1);
                    p.DrawImage(img, -Mathf.Round(img.width / 2f), dy);
                    if (f.flash > 0) { p.additive = true; p.alpha = 0.85f; p.DrawImage(img, -Mathf.Round(img.width / 2f), dy); }
                }
                p.Restore();
                p.FillRect(X, Y - 9, f.w, 4, Color.black); p.FillRect(X, Y - 9, f.w * f.hp / f.maxHp, 4, "#ffcf5a");
            }
            foreach (var b in g.eshots)
            {
                var im = Frame("fx_shot" + (1 + Mathf.FloorToInt(b.t * 12) % 3)); if (!im) continue;
                p.Save(); p.Translate(Mathf.Round(b.x - ox), Mathf.Round(b.y - oy)); p.Scale(b.vx > 0 ? -1 : 1, 1); p.DrawImage(im, -15, -11); p.Restore();
            }
        }

        void DrawFx(G g, float ox, float oy)
        {
            foreach (var e in g.fx)
            {
                float k = e.life / e.max; p.Save(); p.Translate(Mathf.Round(e.x - ox), Mathf.Round(e.y - oy));
                if (e.k == FxKind.Cut)   // vết cắt: đường sáng mảnh kéo dài rồi thu lại
                {
                    p.Rotate(e.a); float L = e.L * (1.2f - k * 0.4f);
                    p.FillRect(-L / 2, -2 * k - 0.5f, L, 4 * k + 1, A("#fff6d0", k));
                    p.FillRect(-L / 2, -5 * k, L, 10 * k, A("#ff8a2b", k * 0.5f));
                }
                else if (e.k == FxKind.Boom || e.k == FxKind.ShotHit)
                {
                    int n = e.k == FxKind.Boom ? 6 : 3, i = Mathf.Min(n - 1, Mathf.FloorToInt((1 - k) * n));
                    var im = Frame((e.k == FxKind.Boom ? "fx_boom" : "fx_shothit") + (i + 1));
                    if (im) p.DrawImage(im, -Mathf.Round(im.width / 2f), -Mathf.Round(im.height / 2f));
                }
                else
                {
                    float r = (1 - k) * (e.big ? 46 : 30) + 6;
                    p.StrokeEllipse(0, 0, r, r, 3 * k + 1, A(e.col ?? "#ffd34d", k));
                }
                p.Restore();
            }
        }

        // ---------------- người chơi ----------------
        static float EaseOut(float t) => 1 - Mathf.Pow(1 - t, 3);
        static readonly List<Vector2> outer = new List<Vector2>(), inner = new List<Vector2>();

        /// <summary>Vệt chém hình lưỡi liềm nhọn hai đầu, mở theo lưỡi đao rồi thu lại từ đuôi.</summary>
        void Crescent(float a0, float a1, float R, float W, float tail, float head, float shape, Color c)
        {
            const int N = 28; outer.Clear(); inner.Clear();
            for (int i = 0; i <= N; i++)
            {
                float u = tail + (head - tail) * i / N, a = a0 + (a1 - a0) * u, k = (u - tail) / Mathf.Max(0.001f, head - tail);
                float w = W * Mathf.Pow(Mathf.Sin(Mathf.PI * Mathf.Pow(k, 0.75f)), shape);
                outer.Add(new Vector2(Mathf.Cos(a) * R, Mathf.Sin(a) * R)); inner.Add(new Vector2(Mathf.Cos(a) * (R - w), Mathf.Sin(a) * (R - w)));
            }
            p.FillStrip(outer, inner, c);
        }
        void DrawSwing(Player P)
        {
            var Ar = P.arc; var T = Cfg.T; if (Ar == null || Ar.t < Ar.from) return;
            float open = EaseOut(Mathf.Min(1, (Ar.t - Ar.from) / (Ar.to - Ar.from))), fade = Mathf.Max(0, (Ar.t - Ar.to) / T.ARC_FADE);
            float span = Mathf.Abs(Ar.a1 - Ar.a0), maxTail = Mathf.Min(1, (Mathf.PI * 1.65f) / span);
            float head = open, tail = Mathf.Min(head, Mathf.Max(fade * fade, head - maxTail));
            if (head - tail < 0.02f) return;
            float R = T.BLADE_LEN * T.ARC_R * (1 + (Ar.len - 1) * 0.45f), W = T.ARC_W * (Ar.big ? 1.35f : 1) * (1 - fade * 0.5f);
            p.Save(); p.Translate(Ar.at.x, Ar.at.y);
            Crescent(Ar.a0, Ar.a1, R + 5, W + 9, tail, head, 0.8f, A("#ff3d14", 0.35f * (1 - fade)));
            Crescent(Ar.a0, Ar.a1, R, W, tail, head, 0.9f, A("#ff8a2b", 0.95f * (1 - fade * 0.6f)));
            Crescent(Ar.a0, Ar.a1, R - 1, W * 0.42f, tail, head, 1, A("#fff6d0", 1 - fade * 0.5f));
            p.Restore();
        }
        void Pose(string name, float alpha = 1, bool sil = false, Color? tint = null)
        {
            var fr = Frame(name); float fy = Cfg.Fy(name);
            if (fr) p.DrawImage(fr, -Cfg.Fa(name), -Cfg.FH + 2 + fy, -1, -1, tint ?? new Color(1, 1, 1, alpha), sil);
        }

        void DrawPlayer(G g, float camX, float camY, float alpha)
        {
            var P = g.P; var T = Cfg.T;
            float rx = P.ox + (P.x - P.ox) * alpha, ry = P.oy + (P.y - P.oy) * alpha;   // nội suy giữa 2 bước vật lý
            if (P.warp != null) { DrawWarp(g, rx, ry, camX, camY); return; }
            float sx = Mathf.Round(rx + P.w / 2 - camX), sy = Mathf.Round(ry + P.h - camY);
            foreach (var gh in P.ghosts)   // bóng mờ khi lướt: Xích Long đỏ cam, Thanh Vũ xanh băng
            {
                p.Save(); p.additive = true; p.Translate(Mathf.Round(gh.x + P.w / 2 - camX), Mathf.Round(gh.y + P.h - camY)); p.Scale(gh.f, 1);
                Pose(gh.n, 1, true, A(gh.n.StartsWith("tv_") ? "#2fc8ff" : "#ff5a1f", gh.a * 0.5f)); p.Restore();
            }
            string name = Frames.Pick(g); var A_ = Cfg.F.ANIM;
            float rot = 0;
            if (name == A_["run"][Mathf.FloorToInt(P.gait) % A_["run"].Length] && P.ground && Mathf.Abs(P.vx) > 40) rot = 0.02f;
            if (!P.ground && P.wall == 0 && P.atk == null && P.dash <= 0) rot = Mathf.Clamp(P.vy / 20000f, -0.03f, 0.04f);
            if (P.dash > 0) rot = 0;
            p.Save(); p.Translate(sx, sy); p.Scale(name == A_["wall_kick"][0] ? P.wjDir : P.face, 1); p.Rotate(rot); p.Scale(P.sqx, P.sqy);
            DrawSwing(P); Pose(name);
            p.Restore();
        }

        /// <summary>Tia năng lượng rơi xuống rồi hiện người từ chân lên (kiểu Mega Man X8).</summary>
        void DrawWarp(G g, float rx, float ry, float camX, float camY)
        {
            var P = g.P; var T = Cfg.T; var W = P.warp; bool tv = g.hero == Hero.ThanhVu;
            string col = tv ? "#7fe3ff" : "#ff8a2b", glow = tv ? "#1fb7ff" : "#ff3d14";
            float sx = Mathf.Round(rx + P.w / 2 - camX), by = Mathf.Round(ry + P.h - camY);
            if (W.t < T.WARP_FALL)
            {
                float k0 = W.t / T.WARP_FALL, head = -40 + (by + 40) * (k0 * k0), L = 140;
                p.FillRect(sx - 7, head - L, 14, L, A(glow, 0.35f));
                p.FillRect(sx - 4, head - L * 0.8f, 8, L * 0.8f, A(col, 0.9f));
                p.FillRect(sx - 2, head - L * 0.6f, 4, L * 0.6f + 6, Color.white);
                return;
            }
            float k = Mathf.Min(1, (W.t - T.WARP_FALL) / T.WARP_FORM), colH = (1 - k) * 90 + 10;
            p.FillRect(sx - 6 - (1 - k) * 6, by - colH, 12 + (1 - k) * 12, colH, A(col, (1 - k) * 0.8f));
            p.FillRect(sx - 2, by - colH, 4, colH, new Color(1, 1, 1, 1 - k));
            string name = Frames.Pick(g); var img = Frame(name); if (!img) return;
            // chỉ hiện phần dưới của khung (từ chân lên theo k)
            float top = Cfg.FH - 2, hShow = Cfg.FH * Mathf.Max(0.02f, k), fy = Cfg.Fy(name);
            float srcY = Mathf.Clamp(top - fy - hShow, 0, img.height);
            p.Save(); p.Translate(sx, by); p.Scale(P.face, 1);
            float dy = -top + fy + srcY, hh = img.height - srcY;
            p.DrawImagePart(img, 0, srcY, img.width, hh, -Cfg.Fa(name), dy, img.width, hh);
            p.additive = true; p.DrawImagePart(img, 0, srcY, img.width, hh, -Cfg.Fa(name), dy, img.width, hh, A(tv ? "#2fc8ff" : "#ff5a1f", (1 - k) * 0.9f), true);
            p.Restore();
        }

        void DrawBolts(G g, float ox, float oy)
        {
            float[] Ls = { 20, 30, 48 }, Hs = { 3, 5, 9 };
            foreach (var b in g.bolts)
            {
                float d = Mathf.Sign(b.vx), L = Ls[b.lv], h = Hs[b.lv];
                p.Save(); p.Translate(Mathf.Round(b.x - ox), Mathf.Round(b.y - oy)); p.Scale(d, 1);
                p.FillRect(-L - 4, -h - 2, L + 8, h * 2 + 4, A("#1fb7ff", 0.35f));
                p.FillRect(-L, -h, L + 2, h * 2, A("#7fe3ff", 0.9f));
                p.FillRect(-L * 0.7f, -Mathf.Max(1, h / 3), L * 0.7f + 4, Mathf.Max(2, h * 2 / 3), Color.white);
                p.FillPoly(new[] { new Vector2(6 + h, 0), new Vector2(0, -h - 1), new Vector2(0, h + 1) }, Color.white);   // mũi tên
                if (b.lv == 2) p.FillCircle(0, 0, h * 2.2f, A("#bff4ff", 0.25f + 0.15f * Mathf.Sin(g.time * 40)));
                p.Restore();
            }
            var P = g.P;
            if (g.hero == Hero.ThanhVu && P.aiming && PlayerSys.ChargeLv(P.charge) > 0)   // đầu nỏ sáng khi đang nạp
            {
                var m = PlayerSys.Muzzle(g); int lv = PlayerSys.ChargeLv(P.charge); float r = (lv == 2 ? 7 : 4) + Mathf.Sin(g.time * 30) * 1.5f;
                p.FillCircle(Mathf.Round(m.x - ox), Mathf.Round(m.y - oy), r, A(lv == 2 ? "#ffffff" : "#7fe3ff", 0.85f));
            }
        }

        // ---------------- trùm ----------------
        /// <summary>Chọn khung hình trùm theo trạng thái và chiêu.</summary>
        public static string BossPose(G g)
        {
            var B = g.boss; var st = B.st; var m = B.move; var sg = B.stage; float t = B.t, fl = g.level.arena.floorY;
            string idle = "bt_i" + (1 + Mathf.FloorToInt(g.time / 0.45f) % 4);
            if (st == BossSt.Intro) return B.y < fl - 2 ? "bt_jump3" : t < 0.9f ? "bt_jump1" : idle;
            if (st == BossSt.Shift) return t < 0.3f ? "bt_hurt1" : "bt_hurt2";
            if (st == BossSt.Stun) return "bt_hurt1";
            if (st == BossSt.Dead) return t < 0.7f ? "bt_die1" : t < 1.4f ? "bt_die2" : "bt_die3";
            if (st == BossSt.Atk)
            {
                switch (m)
                {
                    case BossMove.Dash: return sg == Stage.W ? "bt_dash1" : sg == Stage.A ? (Mathf.FloorToInt(g.time * 12) % 2 == 1 ? "bt_dash2" : "bt_dash3") : t < 0.8f ? "bt_dash3" : "bt_dash4";
                    case BossMove.Wave: return sg == Stage.W ? (t < 0.25f ? "bt_cast1" : "bt_cast2") : sg == Stage.A || t < 0.35f ? "bt_cast3" : "bt_cast4";
                    case BossMove.Spears: return sg == Stage.W ? "bt_cast2" : "bt_cast3";
                    case BossMove.Shield: return sg == Stage.W ? "bt_cast1" : "bt_cast4";
                    case BossMove.Bubbles: return sg == Stage.W ? "bt_cast1" : "bt_dash4";
                    case BossMove.Flood: return sg == Stage.R ? (B.y < fl - 2 ? "bt_jump3" : "bt_jump1") : sg == Stage.W ? "bt_jump2" : "bt_cast2";
                }
            }
            if (B.flash > 0.05f) return "bt_hurt1";
            return idle;
        }

        void DrawBoss(G g, float ox, float oy)
        {
            var B = g.boss; float time = g.time, fl = g.level.arena.floorY;
            if (B.st == BossSt.Wait || (B.st == BossSt.Dead && B.t > 2.2f)) return;
            float X = Mathf.Round(B.x - ox), Y = Mathf.Round(B.y - oy), f = B.face, sh = B.st == BossSt.Atk && B.stage == Stage.W ? Mathf.Sin(time * 60) * 1.5f : 0;
            bool atkW = B.st == BossSt.Atk && B.stage == Stage.W, atkA = B.st == BossSt.Atk && B.stage == Stage.A;
            if (atkW && B.move == BossMove.Spears) foreach (var x in B.spots) p.StrokeEllipse(x - ox, fl - oy - 2, 18, 4, 1, A("#8ff0ff", 0.5f + 0.5f * Mathf.Sin(time * 20)));   // báo hiệu chỗ giáo rơi
            if (atkW && B.move == BossMove.Dash) p.FillRect(f > 0 ? X : X - 600, Y - 3, 600, 2, C("rgba(143,240,255,.5)"));   // vệt sáng hướng lao
            string pose = BossPose(g); var spr = Frame(pose);
            float ax = 60, ay = 148; if (Cfg.F.BTA.TryGetValue(pose, out var an)) { ax = an[0]; ay = an[1]; }
            if (spr)   // hình Gemini quay mặt TRÁI
            {
                if (atkA && B.move == BossMove.Dash)
                    for (int i = 1; i <= 3; i++) { p.Save(); p.additive = true; p.alpha = 0.3f; p.Translate(X - f * i * 26, Y); p.Scale(-f, 1); p.DrawImage(spr, -ax, -ay); p.Restore(); }
                p.Save(); p.Translate(X + sh, Y); p.Scale(-f, 1);
                p.DrawImage(spr, -ax, -ay);
                if (B.flash > 0) { p.additive = true; p.alpha = 0.8f; p.DrawImage(spr, -ax, -ay); }                   // trúng đòn chớp trắng
                else if (B.phase == 1 && B.st != BossSt.Dead && Mathf.Sin(time * 5) > 0.6f) { p.additive = true; p.DrawImage(spr, -ax, -ay, -1, -1, A("#b57cff", 0.35f), true); }   // pha 2 ánh tím
                p.Restore();
            }
            else p.FillRect(X - B.w / 2, Y - B.h, B.w, B.h, B.flash > 0 ? "#ffffff" : "#1B4F8A");
            if (atkA && B.move == BossMove.Shield)   // khiên nước
            {
                float sx = X + f * 40;
                p.FillRect(sx - 12, Y - 140, 24, 140, A("#5fd3ff", 0.45f + 0.15f * Mathf.Sin(time * 12)));
                for (int yy = 0; yy < 140; yy += 10) p.FillRect(sx - 12 + ((yy + time * 80) % 24), Y - 140 + yy, 3, 6, A("#e8f6f8", 0.9f));
            }
            foreach (var b in g.bproj)
            {
                float x = Mathf.Round(b.x - ox), y = Mathf.Round(b.y - oy), s = Mathf.Sign(b.vx);
                if (b.k == ShotKind.Wave)
                {
                    var pts = new List<Vector2> { new Vector2(x - 20, y) };
                    for (int i = 1; i <= 8; i++) { float u = i / 8f; var a = new Vector2(x - 20, y); var c1 = new Vector2(x - 6 * s, y - b.h * 1.2f); var e = new Vector2(x + 18 * s, y - b.h * 0.6f); pts.Add((1 - u) * (1 - u) * a + 2 * (1 - u) * u * c1 + u * u * e); }
                    pts.Add(new Vector2(x + 20 * s, y));
                    p.FillPoly(pts, C("rgba(95,211,255,.75)")); p.FillRect(x - 4, y - b.h, 8, 4, "#e8f6f8");
                }
                else if (b.k == ShotKind.Spear)
                {
                    p.FillRect(x - 3, y - b.h, 6, b.h, "#8ff0ff");
                    p.FillPoly(new[] { new Vector2(x - 7, y - 10), new Vector2(x, y + 4), new Vector2(x + 7, y - 10) }, C("#e8f6f8"));
                }
                else { p.FillCircle(x, y, b.r, b.back ? C("#ff6a5a") : C("rgba(143,240,255,.85)")); p.FillRect(x - b.r * 0.4f, y - b.r * 0.5f, 3, 3, Color.white); }
            }
        }

        /// <summary>Mặt nước gợn sóng: xanh (thường), xanh tím (pha 2), tím (nước Ma Chủng gây sát thương).</summary>
        void DrawWater(G g, float VW, float ox, float oy)
        {
            var L = g.level; var segs = new List<(float x0, float x1, float y, int kind)>();
            foreach (var z in L.zones) segs.Add((z.x0, z.x1, z.y, 0));
            if (g.lvWaterRise != null) segs.Add((L.rise.x0, L.rise.x1, g.lvWaterRise.y, 0));
            if (g.arenaWater > 0) segs.Add((L.arena.x0, L.arena.x1, L.arena.floorY - g.arenaWater, g.boss.phase == 1 ? 1 : 0));
            if (g.arenaHaz > g.arenaWater) segs.Add((L.arena.x0, L.arena.x1, L.arena.floorY - g.arenaHaz, 2));
            foreach (var (x0, x1, y, kind) in segs)
            {
                float X0 = Mathf.Max(0, x0 - ox), X1 = Mathf.Min(VW, x1 - ox); if (X1 <= X0) continue;
                var body = C(kind == 2 ? "rgba(120,50,170,.55)" : kind == 1 ? "rgba(70,90,170,.5)" : "rgba(40,120,170,.5)");
                var top = C(kind == 2 ? "#d7a8ff" : "#bdefff");
                for (float X = X0; X < X1; X += 4)
                {
                    float wy = y - oy + Mathf.Sin((X + ox) * 0.05f + g.time * 4) * 2.5f;
                    p.FillRect(X, wy, 4, Cfg.VH, body); p.FillRect(X, wy, 4, 2, top);
                }
            }
        }

        /// <summary>Thuồng Luồng: vẽ phần hình nằm trong đấu trường (cắt theo mép), cục nước phun.</summary>
        void DrawSub(G g, float ox, float oy)
        {
            var S = g.sub; var Ar = g.level.sub; var TL = Cfg.TL;
            if (S.st == SubSt.Wait || (S.st == SubSt.Dead && S.t > 2.5f)) return;
            bool fast = S.st == SubSt.Atk && S.move == SubMove.Surge && S.stage == Stage.A;
            var img = Frame("tl_swim" + (1 + Mathf.FloorToInt(g.time * (fast ? 18 : 10)) % TL.FRAMES));
            if (img)
            {
                float shake = S.st == SubSt.Atk && S.move == SubMove.Float && S.stage == Stage.R ? Mathf.Sin(g.time * 70) * 3 : 0;
                float sx = S.x + shake, cl = Ar.x0, cr = Ar.x1, bed = Ar.surface + TL.Y.BED;
                float v0 = S.face < 0 ? cl - sx : sx + TL.FW - cr, v1 = S.face < 0 ? cr - sx : sx + TL.FW - cl;
                float u0 = Mathf.Max(0, v0), u1 = Mathf.Min(TL.FW, v1), hh = Mathf.Min(TL.FH, bed - S.y);
                if (u1 > u0 && hh > 0)
                {
                    bool under = !SubSys.HeadUp(g) && S.st != SubSt.Dead;
                    float sc = img.width / TL.FW;   // phòng khi ảnh nhập khác cỡ
                    p.Save();
                    if (S.face < 0) p.Translate(Mathf.Round(sx - ox), Mathf.Round(S.y - oy));
                    else { p.Translate(Mathf.Round(sx + TL.FW - ox), Mathf.Round(S.y - oy)); p.Scale(-1, 1); }
                    p.alpha = under ? 0.35f : S.st == SubSt.Dead ? Mathf.Max(0, 1 - S.t / 2.5f) : 1;
                    p.DrawImagePart(img, u0 * sc, 0, (u1 - u0) * sc, hh * sc, u0, 0, u1 - u0, hh);
                    if (S.flash > 0 || (S.phase == 1 && !under && Mathf.Sin(g.time * 5) > 0.6f))
                    { p.additive = true; p.alpha = S.flash > 0 ? 0.8f : 0.25f; p.DrawImagePart(img, u0 * sc, 0, (u1 - u0) * sc, hh * sc, u0, 0, u1 - u0, hh); }
                    p.Restore();
                }
            }
            foreach (var b in S.shots)
            {
                float x = Mathf.Round(b.x - ox), y = Mathf.Round(b.y - oy);
                p.FillCircle(x, y, TL.SPIT.SIZE, b.back ? C("#ff6a5a") : C("rgba(143,240,255,.9)")); p.FillRect(x - 4, y - 5, 3, 3, Color.white);
            }
        }
        void DrawSubHud(G g, float VW)
        {
            var S = g.sub; if (S.st == SubSt.Wait || (S.st == SubSt.Dead && S.t > 2.2f)) return;
            int n = Cfg.TL.HP, fill = S.st == SubSt.Intro ? Mathf.FloorToInt(Mathf.Min(1, S.t / 1.2f) * n) : S.hp;
            p.FillRect(VW - 26, 74, 12, n * 5 + 4, Color.black);
            for (int i = 0; i < n; i++) p.FillRect(VW - 24, 76 + (n - 1 - i) * 5, 8, 4, i < fill ? C(S.phase == 1 ? "#c58bff" : "#3fe0c8") : C("#2a2a3a"));
        }

        void DrawBossHud(G g, float VW)
        {
            var B = g.boss; if (B.st == BossSt.Wait || (B.st == BossSt.Dead && B.t > 2.2f)) return;
            int n = Cfg.BT.HP, fill = B.st == BossSt.Intro ? Mathf.FloorToInt(Mathf.Min(1, B.t / 1f) * n) : B.hp;
            p.FillRect(VW - 26, 74, 12, n * 4 + 4, Color.black);
            for (int i = 0; i < n; i++) p.FillRect(VW - 24, 76 + (n - 1 - i) * 4, 8, 3, i < fill ? C(B.phase == 1 ? "#c58bff" : "#5fd3ff") : C("#2a2a3a"));
        }
        void DrawTalkBox(G g, float VW)
        {
            var t = g.talk; if (t == null) return;
            float a = Mathf.Clamp01(Mathf.Min(t.t * 4, (t.dur - t.t) * 4)), VH = Cfg.VH;
            p.FillRect(40, VH - 96, VW - 80, 58, A("rgba(8,12,30,.85)", a)); p.StrokeRect(40, VH - 96, VW - 80, 58, 2, A("#3FB8C4", a));
        }
    }
}
