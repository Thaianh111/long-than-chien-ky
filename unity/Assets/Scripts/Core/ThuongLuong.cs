// Trùm phụ Thuồng Luồng (Bến Bạch Hạc) — lặn, lao mặt nước, nổi lưng, phun nước.
// Thiết kế: design/gdd/boss-thuong-luong.md · Số liệu: Resources/Data/boss-thuong-luong.json. Giống hệt game/src/boss/thuongLuong.ts.
// Toạ độ: x = mép trái khung hình 435×121 (khung gốc quay đầu TRÁI), y = mép trên khung. face = -1 đầu trái, 1 lật hình đầu phải.
using System;
using System.Collections.Generic;
using System.Linq;

namespace LongThan.Core
{
    public enum SubSt { Wait, Intro, Under, Atk, Dead }
    public enum SubMove { Surge, Float, Spit }
    public class SubShot { public float x, y, vx, vy, life; public bool back; }
    public class SubBoss
    {
        public SubSt st = SubSt.Wait; public SubMove? move; public Stage stage; public float t; public int hp, phase;
        public float x, y, face = -1, dir = -1, inv, flash; public List<SubMove> last = new List<SubMove>(); public bool first;
        public int n; public bool again, gateShut, plat; public List<SubShot> shots = new List<SubShot>();
    }

    public static class SubSys
    {
        static readonly string[] Keys = { "surge", "float", "spit" };
        static SubTuning TL => Cfg.TL;

        public static SubBoss Create(LevelDef L)
        {
            var A = L.sub;
            return new SubBoss { hp = TL.HP, x = (A.x0 + A.x1) / 2 - TL.FW / 2, y = A.surface + TL.Y.UNDER };
        }

        /// <summary>Hộp đầu (chỗ ăn đòn) trong thế giới.</summary>
        public static Box HeadBox(SubBoss S)
        {
            var H = TL.HEAD; float x0 = S.face < 0 ? S.x + H.x0 : S.x + TL.FW - H.x1;
            return new Box(x0, S.y + H.y0, H.x1 - H.x0, H.y1 - H.y0);
        }
        static Box BodyBox(SubBoss S)
        {
            var B = TL.BODY; float x0 = S.face < 0 ? S.x + B.x0 : S.x + TL.FW - B.x1;
            return new Box(x0, S.y + B.y0, B.x1 - B.x0, B.y1 - B.y0);
        }
        /// <summary>Đầu đang nhô khỏi mặt nước (chỉ lúc này mới đánh trúng).</summary>
        public static bool HeadUp(Game g) => g.sub.y + (TL.HEAD.y0 + TL.HEAD.y1) / 2 < g.level.sub.surface;
        static bool Hit(Box a, Box b) => a.x < b.x + b.w && a.x + a.w > b.x && a.y < b.y + b.h && a.y + a.h > b.y;

        /// <summary>Đặt / gỡ ô ẩn dưới lưng rắn khi nổi (giá trị 2 — đứng được nhưng không vẽ).</summary>
        static void SetPlat(Game g, bool on)
        {
            var S = g.sub; var A = g.level.sub; int TS = g.map.ts, row = A.platRow;
            int c0 = (int)Math.Floor(A.x0 / TS), c1 = (int)Math.Ceiling(A.x1 / TS);
            if (!on)
            {
                for (int i = c0; i < c1; i++) if (g.map.Get(i, row) == 2) g.map.Set(i, row, 0);
                S.plat = false; return;
            }
            var P = TL.PLAT; float a = S.face < 0 ? S.x + P.x0 : S.x + TL.FW - P.x1, b = a + (P.x1 - P.x0);
            for (int i = Math.Max(c0, (int)Math.Floor(a / TS)); i <= Math.Min(c1 - 1, (int)Math.Floor(b / TS)); i++)
                if (g.map.Get(i, row) == 0) g.map.Set(i, row, 2);
            S.plat = true;
        }

        /// <summary>Đưa trùm phụ về như lúc chưa vào trận (người chơi chết giữa trận).</summary>
        public static void Reset(Game g)
        {
            var G = g.level.sub.gate;
            SetPlat(g, false);
            if (g.sub.gateShut) g.map.Rect(G.x, G.y, G.w, G.h, 0);
            g.sub = Create(g.level);
        }

        /// <summary>Trừ máu trùm phụ. false nếu chưa vào trận, đầu đang dưới nước, hoặc vừa trúng.</summary>
        public static bool Hurt(Game g, int d, bool force = false)
        {
            var S = g.sub;
            if (S.st == SubSt.Wait || S.st == SubSt.Intro || S.st == SubSt.Dead || (!force && (S.inv > 0 || !HeadUp(g)))) return false;
            S.hp = Math.Max(0, S.hp - d); S.inv = TL.INV; S.flash = 0.1f;
            var h = HeadBox(S); FxSys.Sparks(g, h.x + h.w / 2, h.y + h.h / 2, 10, "#8ff0ff"); g.sfx.Hit(d > 1); g.hits++;
            if (S.phase == 0 && S.hp <= TL.P2) S.phase = 1;
            if (S.hp <= 0)
            {
                S.st = SubSt.Dead; S.t = 0; S.shots.Clear(); SetPlat(g, false); g.kills++; g.shake = 10; g.flashT = 0.06f;
                var E = g.level.sub.exit; g.map.Rect(E.x, E.y, E.w, E.h, 0); BossSys.Say(g, TL.LINE_OUT, 4.5f, "Ngọc Diệp");
            }
            return true;
        }

        /// <summary>Chọn chiêu: đòn đầu luôn Lao; không chiêu nào 3 lần liền; không Nổi 2 lần liền; người chơi lội nước thì hay Phun hơn.</summary>
        public static SubMove ChooseMove(Game g, bool wet = false)
        {
            var S = g.sub;
            if (!S.first) { S.first = true; S.last = new List<SubMove> { SubMove.Surge }; return SubMove.Surge; }
            var w = Keys.Select(k => TL.WEIGHT[S.phase].TryGetValue(k, out var v) ? v : 0).ToArray();
            if (wet) w[(int)SubMove.Spit] += 3;
            if (S.last.Count > 1 && S.last[0] == S.last[1]) w[(int)S.last[0]] = 0;
            if (S.last.Count > 0 && S.last[0] == SubMove.Float) w[(int)SubMove.Float] = 0;
            float sum = 0; foreach (var x in w) sum += Math.Max(0, x);
            float r = g.rng.Next() * sum; var pick = SubMove.Surge;
            for (int i = 0; i < w.Length; i++) { if (w[i] <= 0) continue; pick = (SubMove)i; r -= w[i]; if (r <= 0) break; }
            var nl = new List<SubMove> { pick }; if (S.last.Count > 0) nl.Add(S.last[0]); S.last = nl;
            return pick;
        }

        static void StartMove(Game g, SubMove m)
        {
            var S = g.sub; var A = g.level.sub; float pcx = g.P.x + g.P.w / 2, mid = (A.x0 + A.x1) / 2;
            S.st = SubSt.Atk; S.move = m; S.stage = Stage.W; S.t = 0; S.n = 0; S.y = A.surface + TL.Y.UNDER;
            if (m == SubMove.Surge) { S.dir = pcx > mid ? 1 : -1; S.face = S.dir; S.x = S.dir > 0 ? A.x0 - TL.FW - 40 : A.x1 + 40; }
            else if (m == SubMove.Float) { S.face = g.rng.Next() < 0.5f ? -1 : 1; S.x = mid - TL.FW / 2; }
            else { S.face = pcx < mid ? -1 : 1; S.x = S.face < 0 ? A.x1 - TL.HEAD.x1 - 20 : A.x0 + TL.HEAD.x1 + 20 - TL.FW; }
        }
        static void GoUnder(Game g)
        {
            var S = g.sub; var A = g.level.sub; S.st = SubSt.Under; S.t = 0; S.move = null; S.y = A.surface + TL.Y.UNDER;
            S.x = Math.Max(A.x0 - 100, Math.Min(A.x1 - TL.FW + 100, S.x));
        }
        /// <summary>Bọt nước báo hiệu (chỉ là hiệu ứng).</summary>
        static void Bubbles(Game g, float x, int n)
        {
            float y = g.level.sub.surface;
            for (int i = 0; i < n; i++) g.parts.Add(new Particle { x = x + (g.R() - 0.5f) * 80, y = y + 4, vx = (g.R() - 0.5f) * 40, vy = -60 - g.R() * 120, life = 0.3f + g.R() * 0.3f, c = "#bdefff", nog = true });
        }

        static void AtkStep(Game g, float dt)
        {
            var S = g.sub; var A = g.level.sub; var P = g.P; float sf = A.surface; int p = S.phase; var m = S.move.Value;
            if (m == SubMove.Surge)
            {
                if (S.stage == Stage.W)
                {
                    if (g.R() < 0.5f) Bubbles(g, S.dir > 0 ? A.x0 + 30 : A.x1 - 30, 2);
                    if (S.t >= TL.SURGE.W) { S.stage = Stage.A; S.t = 0; g.shake = 6; g.sfx.Land(); }
                }
                else
                {
                    float k = Math.Min(1, S.t / TL.SURGE.RISE);
                    S.y = sf + TL.Y.UNDER + (TL.Y.SURGE - TL.Y.UNDER) * k;
                    S.x += S.dir * TL.SURGE.SPD[p] * dt;
                    if (g.R() < 0.6f) Bubbles(g, S.x + (S.dir > 0 ? 0 : TL.FW), 1);
                    if (S.dir > 0 ? S.x > A.x1 + 40 : S.x + TL.FW < A.x0 - 40)
                    {
                        if (p == 1 && !S.again && g.rng.Next() < TL.SURGE.DOUBLE) { S.again = true; S.dir = -S.dir; S.face = S.dir; S.stage = Stage.W; S.t = TL.SURGE.W * 0.4f; S.y = sf + TL.Y.UNDER; }
                        else { S.again = false; GoUnder(g); }
                    }
                }
            }
            else if (m == SubMove.Float)
            {
                if (S.stage == Stage.W)
                {
                    if (g.R() < 0.6f) Bubbles(g, S.x + TL.FW / 2, 2);
                    if (S.t >= TL.FLOAT.W) { S.stage = Stage.A; S.t = 0; }
                }
                else if (S.stage == Stage.A)
                {
                    float k = Math.Min(1, S.t / TL.FLOAT.RISE);
                    S.y = sf + TL.Y.UNDER + (TL.Y.FLOAT - TL.Y.UNDER) * k;
                    if (k >= 1 && !S.plat) { SetPlat(g, true); FxSys.Sparks(g, S.x + TL.FW / 2, sf, 16, "#bdefff"); }
                    if (S.t >= TL.FLOAT.RISE + TL.FLOAT.HOLD[p]) { S.stage = Stage.R; S.t = 0; }
                }
                else if (S.t >= TL.FLOAT.SHAKE) { SetPlat(g, false); GoUnder(g); }
            }
            else
            {
                if (S.stage == Stage.W)
                {
                    if (g.R() < 0.5f) Bubbles(g, HeadBox(S).x + 50, 2);
                    float k = Math.Min(1, S.t / TL.SPIT.W); S.y = sf + TL.Y.UNDER + (TL.Y.SPIT - TL.Y.UNDER) * k;
                    if (S.t >= TL.SPIT.W) { S.stage = Stage.A; S.t = 0; }
                }
                else if (S.stage == Stage.A)
                {
                    if (S.n < TL.SPIT.N[p] && S.t >= S.n * TL.SPIT.GAP)
                    {
                        var h = HeadBox(S); float mx = S.face < 0 ? h.x + 8 : h.x + h.w - 8, my = h.y + h.h * 0.55f;
                        float pcx = P.x + P.w / 2, d = Math.Sign(pcx - mx); if (d == 0) d = -S.face;
                        float dist = Math.Min(520, Math.Abs(pcx - mx));
                        S.shots.Add(new SubShot { x = mx, y = my, vx = d * Math.Max(140, dist / 520 * TL.SPIT.VX * 1.7f), vy = -TL.SPIT.VY * (0.85f + 0.1f * (S.n % 3)), life = 4 });
                        S.n++; g.sfx.Shoot(0);
                    }
                    if (S.n >= TL.SPIT.N[p] && S.t >= S.n * TL.SPIT.GAP + 0.3f) { S.stage = Stage.R; S.t = 0; }
                }
                else { S.y += 260 * dt; if (S.t >= TL.SPIT.R) GoUnder(g); }
            }
        }

        static void UpdateShots(Game g, float dt, Box? box)
        {
            var S = g.sub; var P = g.P; float sf = g.level.sub.surface, r = TL.SPIT.SIZE;
            foreach (var b in S.shots)
            {
                b.life -= dt;
                if (b.back)
                {
                    var h = HeadBox(S); float tx = h.x + h.w / 2, ty = h.y + h.h / 2, dx = tx - b.x, dy = ty - b.y, L = (float)Math.Sqrt(dx * dx + dy * dy); if (L == 0) L = 1;
                    b.vx = dx / L * 460; b.vy = dy / L * 460; b.x += b.vx * dt; b.y += b.vy * dt;
                    if (L < 26) { b.life = 0; Hurt(g, TL.DMG.REFL, true); FxSys.Ring(g, b.x, b.y, 0.18f, true, "#ff6a5a"); }
                    continue;
                }
                b.vy += TL.SPIT.G * dt; b.x += b.vx * dt; b.y += b.vy * dt;
                if (g.map.SolidAt(b.x, b.y) || b.y > sf + 6) { b.life = 0; FxSys.ShotHit(g, b.x, Math.Min(b.y, sf)); continue; }
                if (box.HasValue) { var bx = box.Value; if (b.x > bx.x && b.x < bx.x + bx.w && b.y > bx.y && b.y < bx.y + bx.h) { b.back = true; b.life = 3; FxSys.Sparks(g, b.x, b.y, 10, "#ffd34d"); g.hitstop = 0.05f; continue; } }
                if (b.x + r > P.x && b.x - r < P.x + P.w && b.y + r > P.y + 6 && b.y - r < P.y + P.h) { b.life = 0; PlayerSys.Hurt(g, TL.DMG.SPIT, b.x - b.vx); }
            }
            S.shots.RemoveAll(b => b.life <= 0);
        }

        /// <summary>Một bước của trùm phụ.</summary>
        public static void Update(Game g, float dt, bool wet = false)
        {
            var S = g.sub; var A = g.level.sub; var P = g.P; float pcx = P.x + P.w / 2;
            if (S.st == SubSt.Wait)
            {
                if (pcx > A.closeAtX && pcx < A.x1 && P.warp == null)
                {
                    S.st = SubSt.Intro; S.t = 0; g.map.Rect(A.gate.x, A.gate.y, A.gate.w, A.gate.h, 1); S.gateShut = true;
                    g.shake = 6; g.sfx.Land(); BossSys.Say(g, TL.LINE_IN, 3.2f, "Ngọc Diệp");
                }
                return;
            }
            S.t += dt; S.inv = Math.Max(0, S.inv - dt); S.flash = Math.Max(0, S.flash - dt);
            if (S.st == SubSt.Dead) { S.y += 40 * dt; if (S.t < 1.6f && g.R() < 0.06f) FxSys.Boom(g, S.x + g.R() * TL.FW, S.y + 40 + g.R() * 40); return; }
            if (S.st == SubSt.Intro)
            {
                float sf = A.surface; bool up = S.t > 0.8f && S.t < TL.INTRO - 0.5f;
                S.y += ((sf + (up ? TL.Y.FLOAT : TL.Y.UNDER)) - S.y) * Math.Min(1, dt * 5);
                if (S.t > 0.8f && S.t < 0.9f) { g.shake = 8; Bubbles(g, S.x + 100, 20); }
                if (S.t >= TL.INTRO) GoUnder(g);
            }
            else if (S.st == SubSt.Under)
            {
                float want = pcx - TL.FW / 2; S.x += Math.Sign(want - S.x) * Math.Min(Math.Abs(want - S.x), TL.FOLLOW * dt);
                if (S.t >= TL.UNDER[S.phase]) StartMove(g, ChooseMove(g, wet));
            }
            else AtkStep(g, dt);

            // va chạm: đầu lúc lao/phun, thân lúc lao
            var pb = new Box(P.x, P.y, P.w, P.h);
            if (S.st == SubSt.Atk && S.stage != Stage.W && (S.move == SubMove.Surge || S.move == SubMove.Spit))
            {
                var hb = HeadBox(S); if (HeadUp(g) && Hit(pb, hb)) PlayerSys.Hurt(g, TL.DMG.TOUCH, hb.x + hb.w / 2);
                if (S.move == SubMove.Surge) { var bb = BodyBox(S); if (bb.y < A.surface && Hit(pb, bb)) PlayerSys.Hurt(g, TL.DMG.TOUCH, bb.x + bb.w / 2); }
            }
            // trúng kiếm vào đầu
            var box = PlayerSys.SlashBox(P);
            if (box.HasValue && P.atk != null && !P.hitSet.Contains(S) && Hit(box.Value, HeadBox(S)))
            {
                if (Hurt(g, P.atk.dmg))
                {
                    P.hitSet.Add(S); g.hitstop = Cfg.T.HITSTOP * (P.atk.dmg > 1 ? Cfg.T.FINISH_STOP : 1); g.shake = Cfg.T.SHAKE;
                    var h = HeadBox(S); FxSys.Ring(g, h.x + h.w / 2, h.y + h.h / 2, 0.16f, P.atk.dmg > 1);
                }
            }
            // trúng tên nỏ
            foreach (var b in g.bolts)
            {
                if (b.life <= 0 || b.hit.Contains(S)) continue;
                var h = HeadBox(S);
                if (b.x > h.x && b.x < h.x + h.w && b.y > h.y && b.y < h.y + h.h && HeadUp(g))
                {
                    b.hit.Add(S); if (Hurt(g, Cfg.T.TV.DMG[b.lv])) g.hitstop = Math.Max(g.hitstop, Cfg.T.TV.STOP[b.lv]);
                    if (b.lv < 2) b.life = 0;
                }
            }
            UpdateShots(g, dt, box);
        }
    }
}
