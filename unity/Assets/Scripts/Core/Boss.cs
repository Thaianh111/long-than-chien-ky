// Trùm Thủy Tinh — máy trạng thái và 6 chiêu. Thiết kế: design/gdd/boss-thuy-tinh.md
using System;
using System.Collections.Generic;
using System.Linq;

namespace LongThan.Core
{
    public enum BossSt { Wait, Intro, Idle, Atk, Stun, Shift, Dead }
    public enum BossMove { Wave, Dash, Bubbles, Shield, Spears, Flood }
    public enum Stage { W, A, R }

    /// <summary>Trùm. x = giữa thân, y = mặt sàn dưới chân.</summary>
    public class Boss
    {
        public BossSt st = BossSt.Wait; public float x, y, w = 64, h = 120, face = -1, inv, t, floodCd, lostSince, flash, stun;
        public int hp, phase; public List<BossMove> last = new List<BossMove>(); public bool first, fly, gateShut;
        public BossMove? move; public Stage stage; public int n; public List<float> spots = new List<float>();
        public static Boss Create(BossArena A) => new Boss { x = A.x1 - 120, y = A.floorY, hp = Cfg.BT.HP };
    }
    public enum ShotKind { Wave, Bub, Drop, Spear }
    /// <summary>Đạn của trùm: sóng nền, bong bóng (chém bay ngược được), giọt nước, giáo nước.</summary>
    public class BossShot { public ShotKind k; public float x, y, vx, vy, t, life = 6, r = 8, w, h; public int dmg = 2; public bool refl, back, stuck; }

    public static class BossSys
    {
        static readonly string[] Keys = { "wave", "dash", "bubbles", "shield", "spears", "flood" };

        public static void Say(Game g, string[] lines, float dur, string who = null) => g.talk = new Talk { lines = lines, dur = dur, who = who };

        /// <summary>Đưa trùm và đấu trường về như lúc chưa vào trận (người chơi chết thì trùm hồi đầy máu).</summary>
        public static void Reset(Game g)
        {
            var A = g.level.arena; var G = A.gate;
            if (g.boss.gateShut) g.map.Rect(G.x, G.y, G.w, G.h, 0);
            g.boss = Boss.Create(A); g.bproj.Clear(); g.arenaWater = 0; g.arenaHaz = 0; g.talk = null;
        }

        static void StartShift(Game g) { var B = g.boss; B.st = BossSt.Shift; B.t = 0; B.phase = 1; g.bproj.Clear(); g.shake = 10; g.flashT = 0.06f; }

        /// <summary>Trừ máu trùm. Trả về false nếu trùm đang bất tử (vào trận, chuyển pha, chết, vừa trúng).</summary>
        public static bool Hurt(Game g, int d)
        {
            var B = g.boss;
            if (B.st == BossSt.Intro || B.st == BossSt.Shift || B.st == BossSt.Dead || B.st == BossSt.Wait || B.inv > 0) return false;
            B.hp = Math.Max(0, B.hp - d); B.inv = Cfg.BT.INV; B.flash = 0.1f; B.lostSince += d;
            FxSys.Sparks(g, B.x, B.y - 70, 10, "#8ff0ff"); g.sfx.Hit(d > 1); g.hits++;
            if (B.hp <= 0) { B.st = BossSt.Dead; B.t = 0; g.bproj.Clear(); Say(g, Cfg.BT.LINE_OUT, 4); g.kills++; }
            else if (B.phase == 0 && B.hp <= Cfg.BT.P2 && B.st != BossSt.Atk) StartShift(g);
            return true;
        }

        static void Push(Game g, BossShot s) => g.bproj.Add(s);

        /// <summary>Chọn chiêu theo trọng số: đòn đầu luôn Sóng Thần, không lặp 3 lần liền, không khiên 2 lần liền, Đại Hồng Thủy chỉ ở pha 2.</summary>
        public static BossMove ChooseMove(Game g)
        {
            var B = g.boss; var P = g.P;
            if (!B.first) { B.first = true; B.last = new List<BossMove> { BossMove.Wave }; return BossMove.Wave; }
            var w = Keys.Select(k => Cfg.BT.WEIGHT[B.phase].TryGetValue(k, out var v) ? v : 0).ToArray();
            float d = Math.Abs(B.x - (P.x + P.w / 2));
            if (d < 120) { w[(int)BossMove.Wave] += 2; w[(int)BossMove.Bubbles] -= 1; }
            if (d > 300) w[(int)BossMove.Dash] += 3;
            if (P.wall != 0) { w[(int)BossMove.Spears] += 2; w[(int)BossMove.Dash] -= 2; }
            if (B.phase == 1 && B.floodCd <= 0 && B.lostSince >= 6) w[(int)BossMove.Flood] += 6;
            if (B.last.Count >= 2 && B.last[0] == B.last[1]) w[(int)B.last[0]] = 0;
            if (B.last.Count >= 1 && B.last[0] == BossMove.Shield) w[(int)BossMove.Shield] = 0;
            float sum = 0; for (int i = 0; i < w.Length; i++) { w[i] = Math.Max(0, w[i]); sum += w[i]; }
            float r = g.rng.Next() * sum; var pick = BossMove.Wave;
            for (int i = 0; i < w.Length; i++) { if (w[i] <= 0) continue; pick = (BossMove)i; r -= w[i]; if (r <= 0) break; }
            B.last = B.last.Count > 0 ? new List<BossMove> { pick, B.last[0] } : new List<BossMove> { pick };
            return pick;
        }

        /// <summary>Vị trí n giáo: chia đấu trường thành n + 2 ô, bỏ trống 2 ô KHÔNG kề nhau → luôn có 2 khe rộng ≥ 64px.</summary>
        public static List<float> SpearSpots(Game g, int n)
        {
            var A = g.level.arena; float L = A.x0 + 16, R = A.x1 - 16; int slots = n + 2; float gap = (R - L) / slots;
            int a = (int)(g.rng.Next() * slots), b = a;
            while (Math.Abs(b - a) < 2) b = (int)(g.rng.Next() * slots);
            var outp = new List<float>();
            for (int i = 0; i < slots; i++) if (i != a && i != b) outp.Add(L + gap * (i + 0.5f) + (g.rng.Next() - 0.5f) * 24);
            return outp;
        }

        public static void StartMove(Game g, BossMove m)
        {
            var B = g.boss; var P = g.P;
            B.st = BossSt.Atk; B.move = m; B.t = 0; B.stage = Stage.W; B.face = P.x + P.w / 2 < B.x ? -1 : 1; B.n = 0;
            if (m == BossMove.Spears) B.spots = SpearSpots(g, Cfg.BT.SPEAR.N[B.phase]);
            if (m == BossMove.Flood) B.fly = false;
        }

        static (float W, float A, float R) StageTime(Game g, BossMove m)
        {
            int p = g.boss.phase; var BT = Cfg.BT;
            switch (m)
            {
                case BossMove.Wave: return (BT.WAVE.W, p == 1 ? BT.WAVE.GAP2 + 0.05f : 0.05f, BT.WAVE.R);
                case BossMove.Dash: return (BT.DASH.W[p], 9, BT.DASH.R);
                case BossMove.Bubbles: return (BT.BUB.W, BT.BUB.N[p] * BT.BUB.GAP[p], BT.BUB.R);
                case BossMove.Shield: return (BT.SHIELD.W, BT.SHIELD.HOLD[p], 0.5f);
                case BossMove.Spears: return (BT.SPEAR.W[p], 0.5f, BT.SPEAR.R);
                default: return (BT.FLOOD.W, BT.FLOOD.UP + BT.FLOOD.HOLD + BT.FLOOD.DOWN, BT.FLOOD.R);
            }
        }

        static void SpawnWave(Game g)
        {
            var B = g.boss; var BT = Cfg.BT;
            Push(g, new BossShot { k = ShotKind.Wave, x = B.x + B.face * 40, y = g.level.arena.floorY, vx = B.face * BT.WAVE.SPD, w = 32, h = B.phase == 1 ? BT.WAVE.H2 : BT.WAVE.H, dmg = BT.WAVE.DMG });
            g.shake = 5; g.sfx.Land();
        }

        static void AtkStep(Game g, float dt, float pcx)
        {
            var B = g.boss; var P = g.P; var A = g.level.arena; var BT = Cfg.BT; float fl = A.floorY; var m = B.move.Value; int p = B.phase; var S = StageTime(g, m);
            if (B.stage == Stage.W && B.t >= S.W)
            {
                B.stage = Stage.A; B.t = 0; B.n = 0;
                if (m == BossMove.Wave) { SpawnWave(g); B.n = 1; }
                if (m == BossMove.Spears) foreach (var x in B.spots) Push(g, new BossShot { k = ShotKind.Spear, x = x, y = fl - 360, vy = BT.SPEAR.SPD, w = 16, h = 48, dmg = BT.SPEAR.DMG });
            }
            else if (B.stage == Stage.A)
            {
                if (m == BossMove.Wave && p == 1 && B.n == 1 && B.t > BT.WAVE.GAP2) { B.n = 2; SpawnWave(g); }
                if (m == BossMove.Dash)
                {
                    B.x += B.face * BT.DASH.SPD[p] * dt;
                    if (B.x < A.x0 + 34 || B.x > A.x1 - 34)
                    {
                        B.x = Math.Max(A.x0 + 34, Math.Min(A.x1 - 34, B.x)); B.stage = Stage.R; B.t = 0; g.shake = 8; g.sfx.Land(); FxSys.Sparks(g, B.x + B.face * 40, fl - 40, 14, "#e8f6f8");
                    }
                    if (Math.Abs(pcx - B.x) < 60 && P.y + P.h > fl - 60 && P.y < fl - 20) PlayerSys.Hurt(g, BT.DASH.DMG, B.x);
                }
                if (m == BossMove.Bubbles && B.n < BT.BUB.N[p] && B.t >= B.n * BT.BUB.GAP[p])
                {
                    float s = Math.Sign(pcx - B.x); if (s == 0) s = B.face;
                    Push(g, new BossShot { k = ShotKind.Bub, x = B.x + B.face * 30, y = B.y - 80, vx = BT.BUB.VX[B.n] * s, vy = -BT.BUB.VY, r = BT.BUB.SIZE, dmg = BT.BUB.DMG, refl = true }); B.n++; g.sfx.Shoot(0);
                }
                if (m == BossMove.Shield && B.t >= (B.n + 1) * BT.SHIELD.DROP[p] && B.t < S.A)
                {
                    B.n++; Push(g, new BossShot { k = ShotKind.Drop, x = B.x + B.face * 20, y = fl - 76, vx = B.face * 300, r = 6, dmg = BT.SHIELD.DMG });
                }
                if (m == BossMove.Flood)
                {
                    float t = B.t, bse = BT.WATER_P2; var F = BT.FLOOD;
                    B.fly = true; B.y += ((fl - 185) - B.y) * Math.Min(1, dt * 4);
                    g.arenaHaz = t < F.UP ? bse + (F.PEAK - bse) * t / F.UP : t < F.UP + F.HOLD ? F.PEAK : Math.Max(bse, F.PEAK - (F.PEAK - bse) * (t - F.UP - F.HOLD) / F.DOWN);
                }
                if (B.stage == Stage.A && B.t >= S.A)
                {
                    if (m == BossMove.Shield) for (int i = 0; i < 8; i++) { float a = i / 8f * 6.283f; Push(g, new BossShot { k = ShotKind.Drop, x = B.x + B.face * 40, y = fl - 70, vx = (float)Math.Cos(a) * 240, vy = (float)Math.Sin(a) * 240, r = 6, dmg = 2 }); }
                    if (m == BossMove.Flood) { g.arenaHaz = 0; B.floodCd = BT.FLOOD.CD; B.lostSince = 0; }
                    B.stage = Stage.R; B.t = 0;
                }
            }
            else
            {
                if (m == BossMove.Flood) B.y = Math.Min(fl, B.y + 500 * dt);
                if (B.t >= S.R)
                {
                    B.y = fl; B.fly = false; B.st = BossSt.Idle; B.t = 0;
                    if (B.phase == 0 && B.hp <= BT.P2) StartShift(g);
                }
            }
        }

        static void UpdateShots(Game g, float dt, Box? box)
        {
            var B = g.boss; var P = g.P; var A = g.level.arena; var BT = Cfg.BT; float fl = A.floorY, pcx = P.x + P.w / 2;
            foreach (var b in g.bproj.ToArray())
            {
                b.t += dt; b.life -= dt;
                if (b.k == ShotKind.Bub && !b.back) b.vy += BT.BUB.G * dt;
                b.x += b.vx * dt; b.y += b.vy * dt;
                if (b.k == ShotKind.Wave)
                {
                    if (b.x < A.x0 || b.x > A.x1) b.life = 0;
                    if (Math.Abs(pcx - b.x) < 16 + P.w / 2 && P.y + P.h > b.y - b.h) PlayerSys.Hurt(g, b.dmg, b.x - b.vx);
                    continue;
                }
                if (b.k == ShotKind.Spear)
                {
                    if (b.y >= fl) { b.y = fl; b.vy = 0; if (!b.stuck) { b.stuck = true; b.life = 0.3f; FxSys.Sparks(g, b.x, fl - 4, 6, "#8ff0ff"); } }
                    if (!b.stuck && Math.Abs(pcx - b.x) < 8 + P.w / 2 && b.y > P.y && b.y - b.h < P.y + P.h) PlayerSys.Hurt(g, b.dmg, b.x);
                    continue;
                }
                if (b.back)   // bong bóng bị chém bay ngược về lõi ngực
                {
                    float dx = B.x - b.x, dy = (B.y - 76) - b.y, L = (float)Math.Sqrt(dx * dx + dy * dy); if (L == 0) L = 1;
                    b.vx = dx / L * 420; b.vy = dy / L * 420;
                    if (L < 30) { b.life = 0; if (B.st != BossSt.Shift) { B.inv = 0; Hurt(g, BT.BUB.REFL); } FxSys.Ring(g, b.x, b.y, 0.18f, true, "#ff6a5a"); }
                    continue;
                }
                if (g.map.SolidAt(b.x, b.y) || b.y > fl - 2) { b.life = 0; FxSys.ShotHit(g, b.x, b.y); continue; }
                if (b.refl && box is Box bx && b.x > bx.x && b.x < bx.x + bx.w && b.y > bx.y && b.y < bx.y + bx.h) { b.back = true; b.life = 3; FxSys.Sparks(g, b.x, b.y, 10, "#ffd34d"); g.hitstop = 0.05f; continue; }
                if (Math.Abs(b.x - pcx) < b.r + P.w / 2 && b.y > P.y + 6 && b.y < P.y + P.h) { b.life = 0; PlayerSys.Hurt(g, b.dmg, b.x - b.vx); }
            }
            g.bproj.RemoveAll(b => b.life <= 0);
        }

        /// <summary>Một bước của trùm: vào trận khi người chơi qua cổng, rồi chạy máy trạng thái.</summary>
        public static void Update(Game g, float dt)
        {
            var B = g.boss; var P = g.P; var A = g.level.arena; var BT = Cfg.BT; var T = Cfg.T; float fl = A.floorY, pcx = P.x + P.w / 2;
            if (B.st == BossSt.Wait)
            {
                if (pcx > A.closeAtX && P.warp == null)
                {
                    B.st = BossSt.Intro; B.t = 0; B.y = fl - 340; Say(g, BT.LINE_IN, 2.6f);
                    g.map.Rect(A.gate.x, A.gate.y, A.gate.w, A.gate.h, 1); B.gateShut = true; g.shake = 6; g.sfx.Land();
                }
                return;
            }
            B.t += dt; B.inv = Math.Max(0, B.inv - dt); B.flash = Math.Max(0, B.flash - dt); B.floodCd -= dt;
            switch (B.st)
            {
                case BossSt.Intro:   // nhảy từ trên trời đáp xuống
                {
                    float k = Math.Min(1, B.t / 0.55f), yy = fl - 340 + k * k * 340;
                    if (B.y < fl - 2 && yy >= fl) { g.shake = 10; g.sfx.Land(); FxSys.Sparks(g, B.x, fl - 4, 24, "#8ff0ff"); }
                    B.y = Math.Min(fl, yy); if (B.t > 2.8f) { B.st = BossSt.Idle; B.t = 0; }
                    break;
                }
                case BossSt.Shift:
                    g.arenaWater = Math.Min(BT.WATER_P2, Math.Max(g.arenaWater, (B.t - 0.8f) / 1.5f * BT.WATER_P2));
                    if (B.t > BT.SHIFT) { B.st = BossSt.Idle; B.t = 0; }
                    break;
                case BossSt.Idle:
                {
                    float d = pcx - B.x, want = Math.Abs(d) < 160 ? -Math.Sign(d) : Math.Abs(d) > 260 ? Math.Sign(d) : 0;
                    B.face = d < 0 ? -1 : 1;
                    B.x = Math.Max(A.x0 + 40, Math.Min(A.x1 - 40, B.x + want * BT.WALK[B.phase] * dt));
                    if (B.t > BT.IDLE[B.phase]) StartMove(g, ChooseMove(g));
                    break;
                }
                case BossSt.Stun:
                    B.y = Math.Min(fl, B.y + 600 * dt); if (B.t > B.stun) { B.st = BossSt.Idle; B.t = 0; B.fly = false; }
                    break;
                case BossSt.Atk: AtkStep(g, dt, pcx); break;
                case BossSt.Dead:
                    g.arenaWater = Math.Max(0, g.arenaWater - 40 * dt); g.arenaHaz = 0;
                    if (g.R() < 0.05f && B.t < 2) { FxSys.Boom(g, B.x + (g.R() - 0.5f) * 60, B.y - g.R() * 110); g.shake = 4; }
                    break;
            }
            // va chạm thân
            if (B.st != BossSt.Dead && B.st != BossSt.Intro && Math.Abs(pcx - B.x) < B.w / 2 + P.w / 2 - 4 && P.y + P.h > B.y - B.h && P.y < B.y) PlayerSys.Hurt(g, BT.TOUCH, B.x);
            // trúng kiếm (khiên nước chặn kiếm ở mặt trước)
            var box = PlayerSys.SlashBox(P);
            if (box is Box bx && P.atk != null && !P.hitSet.Contains(B) && bx.x < B.x + B.w / 2 && bx.x + bx.w > B.x - B.w / 2 && bx.y < B.y && bx.y + bx.h > B.y - B.h)
            {
                P.hitSet.Add(B);
                if (B.move == BossMove.Shield && B.st == BossSt.Atk && B.stage == Stage.A && Math.Sign(pcx - B.x) == B.face) FxSys.Sparks(g, B.x + B.face * 40, B.y - 70, 8, "#8ff0ff");
                else if (Hurt(g, P.atk.dmg)) { g.hitstop = T.HITSTOP * (P.atk.dmg > 1 ? T.FINISH_STOP : 1); g.shake = T.SHAKE; FxSys.Ring(g, B.x, B.y - 60, 0.16f, P.atk.dmg > 1); }
            }
            // trúng tên nỏ (chỉ tên nạp đầy phá được khiên, trùm choáng)
            foreach (var b in g.bolts)
            {
                if (b.life <= 0 || b.hit.Contains(B)) continue;
                bool shielded = B.st == BossSt.Atk && B.move == BossMove.Shield && B.stage == Stage.A; float sx = B.x + B.face * 40;
                if (shielded && Math.Sign(b.x - B.x) == B.face && Math.Abs(b.x - sx) < 16 && b.y > B.y - 140 && b.y < B.y)
                {
                    b.hit.Add(B);
                    if (b.lv == 2) { Hurt(g, 4); B.st = BossSt.Stun; B.t = 0; B.stun = BT.SHIELD.STUN; FxSys.Sparks(g, sx, b.y, 30, "#8ff0ff"); g.shake = 10; g.flashT = 0.05f; }
                    else { b.life = 0; FxSys.Sparks(g, sx, b.y, 8, "#8ff0ff"); }
                    continue;
                }
                if (Math.Abs(b.x - B.x) < B.w / 2 && b.y > B.y - B.h && b.y < B.y)
                {
                    b.hit.Add(B);
                    if (Hurt(g, T.TV.DMG[b.lv])) { g.hitstop = Math.Max(g.hitstop, T.TV.STOP[b.lv]); if (B.move == BossMove.Flood && b.lv == 2 && B.fly) { B.st = BossSt.Stun; B.t = 0; B.stun = 1f; } }
                    if (b.lv < 2) b.life = 0;
                }
            }
            UpdateShots(g, dt, box);
            // nước tím Đại Hồng Thủy: mất máu mỗi nhịp
            if (g.arenaHaz > g.arenaWater + 2 && P.y + P.h > fl - g.arenaHaz && pcx > A.x0 && pcx < A.x1)
            {
                P.hazT -= dt;
                if (P.hazT <= 0)
                {
                    P.hazT = BT.FLOOD.TICK;
                    if (P.inv <= 0) { P.hp -= BT.FLOOD.DMG; FxSys.Sparks(g, pcx, P.y + P.h / 2, 8, "#b57cff"); g.sfx.Hit(false); if (P.hp <= 0) Flow.Respawn(g); }
                }
            }
        }
    }
}
