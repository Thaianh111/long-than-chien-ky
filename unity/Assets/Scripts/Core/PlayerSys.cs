// Người chơi: chạy, nhảy 2 bậc, lướt, bám/đạp tường, chém combo (Xích Long), bắn nỏ nạp lực (Thanh Vũ).
using System;
using System.Collections.Generic;
using static LongThan.Core.Key;

namespace LongThan.Core
{
    public static class PlayerSys
    {
        static float EaseOut(float t) => 1 - (float)Math.Pow(1 - t, 3);
        static float Clamp01(float v) => Math.Min(1, Math.Max(0, v));

        public static void Squash(Player P, float sx, float sy) { P.sqx = sx; P.sqy = sy; P.vsx = 0; P.vsy = 0; }

        static bool TouchingWall(Game g, int dir)
        {
            var P = g.P; float x = dir > 0 ? P.x + P.w + 1 : P.x - 1;
            return g.map.SolidAt(x, P.y + 8) && g.map.SolidAt(x, P.y + P.h - 12);
        }

        static void StartAttack(Game g)
        {
            var P = g.P; var T = Cfg.T; bool air = !P.ground;
            var step = air ? T.AIR_SLASH : T.COMBO[P.combo];
            P.atk = step; P.atkT = 0; P.hitSet = new HashSet<object>(); P.comboQueued = false; P.atkAir = air; P.swing.Clear(); P.spinK = 0;
            string key = air ? "air" : P.combo.ToString();
            P.arc = new Arc { a0 = step.a0, a1 = step.a1, from = step.from, to = step.to, len = step.len, big = step.dmg > 1, at = Cfg.F.SWING_AT.TryGetValue(key, out var at) ? at : Cfg.F.SWING_AT["0"] };
            g.sfx.Swing(step.dmg > 1 ? 1.4f : air ? 1.15f : 1);
            g.sfx.Shout(air ? "xl_air" : "xl_" + P.combo);
            if (!air && step.lunge > 0) P.vx = P.face * step.lunge * 2.2f;
        }

        static float BladeAngle(Player P)
        {
            var a = P.atk;
            if (a == null) return Cfg.T.BLADE_IDLE;
            if (P.atkT < a.from) return a.a0;
            if (P.atkT > a.to) return a.a1;
            return a.a0 + (a.a1 - a.a0) * EaseOut((P.atkT - a.from) / (a.to - a.from));
        }

        static void UpdateAnim(Game g, float dt)
        {
            var P = g.P; var T = Cfg.T;
            P.vsx += (-(P.sqx - 1) * T.SQUASH_K - P.vsx * T.SQUASH_D) * dt; P.sqx += P.vsx * dt;   // lò xo co giãn luôn bật về 1
            P.vsy += (-(P.sqy - 1) * T.SQUASH_K - P.vsy * T.SQUASH_D) * dt; P.sqy += P.vsy * dt;
            if (P.ground && P.dash <= 0 && Math.Abs(P.vx) > 40)
            {
                int before = (int)Math.Floor(P.gait);
                P.gait += Math.Abs(P.vx) * dt / T.STRIDE;
                int now = (int)Math.Floor(P.gait); if (now != before && now % 4 == 0) FxSys.Dust(g, P.x + P.w / 2 - P.face * 8, P.y + P.h, 3);
            }
            else P.gait = 0;
            if (P.face != P.lastFace) { if (P.ground && P.atk == null) { P.turnT = 0.08f; Squash(P, 0.7f, 1.04f); } P.lastFace = P.face; }
            P.turnT = Math.Max(0, P.turnT - dt);
            // vừa dứt lướt trên đất mà không bấm hướng -> trượt gót thắng lại
            if (P.wasDash && P.dash <= 0 && P.ground && P.atk == null && !g.input.Held(Left) && !g.input.Held(Right)) { P.recFrame = g.hero == Hero.ThanhVu ? "tv_d3" : "xl_dash3"; P.recover = T.DASH_STOP; }
            P.wasDash = P.dash > 0;
            P.recover = Math.Max(0, P.recover - dt);
            P.wjT = Math.Max(0, P.wjT - dt); if (P.ground || P.wall != 0 || P.atk != null || P.dash > 0) P.wjT = 0;
            P.dj = Math.Max(0, P.dj - dt);
            P.landT = Math.Max(0, P.landT - dt); if (!P.ground || P.atk != null || P.dash > 0) P.landT = 0;
            if (P.atk != null || P.dash > 0 || (!P.ground && P.recover > 0)) P.recover = 0;
            P.airT = P.ground ? 0 : P.airT + dt;
        }

        /// <summary>Xuất hiện kiểu tia năng lượng rơi xuống (Mega Man X8).</summary>
        public static void WarpIn(Game g)
        {
            var P = g.P; P.warp = new Warp(); P.vx = 0; P.vy = 0; P.atk = null; P.arc = null; P.aiming = false; P.dash = 0;
            g.sfx.Warp();
        }
        static void UpdateWarp(Game g, float dt)
        {
            var P = g.P; var T = Cfg.T; var W = P.warp; W.t += dt; P.vx = 0;
            P.vy = Math.Min(P.vy + T.GRAVITY * dt, T.MAX_FALL); P.y += P.vy * dt; g.map.CollideY(P);
            if (!W.landed && W.t >= T.WARP_FALL)
            {
                W.landed = true; float cx0 = P.x + P.w / 2, by = P.y + P.h; string col = g.hero == Hero.ThanhVu ? "#7fe3ff" : "#ff8a2b";
                FxSys.Ring(g, cx0, by - 4, 0.3f, true, col);
                for (int i = 0; i < 18; i++)
                {
                    float a = -(float)Math.PI * g.R(), sp = 120 + g.R() * 260;
                    g.parts.Add(new Particle { x = cx0, y = by - 2, vx = (float)Math.Cos(a) * sp, vy = (float)Math.Sin(a) * sp, life = 0.3f + g.R() * 0.25f, c = i % 2 == 1 ? col : "#ffffff" });
                }
                g.shake = Math.Max(g.shake, 3); g.sfx.Land();
            }
            if (W.t >= T.WARP_FALL + T.WARP_FORM) { P.warp = null; Squash(P, 1.12f, 0.9f); }
        }

        /// <summary>Đổi nhân vật (Xích Long ⇄ Thanh Vũ), có hiệu ứng xuất hiện.</summary>
        public static void SwapHero(Game g, Hero? to = null)
        {
            var P = g.P;
            g.hero = to ?? (g.hero == Hero.XichLong ? Hero.ThanhVu : Hero.XichLong);
            P.atk = null; P.arc = null; P.combo = 0; P.aiming = false; P.shootT = 0; P.recover = 0;
            WarpIn(g); g.onHeroChange?.Invoke(g.hero);
        }

        /// <summary>Bị trúng đòn: mất máu, giật lùi, nhấp nháy bất tử. Hết máu thì hồi sinh ở điểm lưu.</summary>
        public static void Hurt(Game g, int dmg, float fromX)
        {
            var P = g.P; var T = Cfg.T;
            if (P.warp != null || P.inv > 0) return;
            P.hp -= dmg; P.inv = T.HURT_INV; float d = P.x + P.w / 2 < fromX ? -1 : 1;
            P.vx = d * T.HURT_KNOCK; P.vy = -260; P.dash = 0; P.atk = null; P.arc = null; P.aiming = false;
            g.shake = T.SHAKE * 1.6f; g.flashT = 0.05f; FxSys.Sparks(g, P.x + P.w / 2, P.y + P.h / 2, 14, "#ff3b6b"); g.sfx.Hit(true);
            if (P.hp <= 0) { FxSys.Sparks(g, P.x + P.w / 2, P.y + P.h / 2, 40, "#ffd34d"); Flow.Respawn(g); }
        }

        // ---------------- Thanh Vũ: Thần Nỏ ----------------
        /// <summary>Cấp nạp hiện tại: 0 thường, 1 nạp, 2 nạp đầy.</summary>
        public static int ChargeLv(float charge) => charge >= Cfg.T.TV.CHARGE2 ? 2 : charge >= Cfg.T.TV.CHARGE1 ? 1 : 0;
        /// <summary>Vị trí đầu nỏ trong thế giới.</summary>
        public static XY Muzzle(Game g)
        {
            var P = g.P; var T = Cfg.T;
            XY M = !P.ground ? (Cfg.F.MZ.TryGetValue(Frames.Pick(g), out var m) ? m : Cfg.F.MZ["tv_aj3"]) : Frames.TvRunning(P) ? T.TV.MUZZLE_RUN : T.TV.MUZZLE;
            return new XY { x = P.x + P.w / 2 + P.face * M.x, y = P.y + P.h + M.y };
        }
        static void UpdateShoot(Game g, float dt)
        {
            var P = g.P; var T = Cfg.T; var inp = g.input;
            P.reload = Math.Max(0, P.reload - dt); P.shootT = Math.Max(0, P.shootT - dt);
            if (inp.Pressed(Attack) && !P.aiming && P.reload <= 0) { P.aiming = true; P.charge = 0; P.aimT = 0; P.lastLv = 0; }
            if (!P.aiming) return;
            P.charge += dt; P.aimT += dt;
            int lv = ChargeLv(P.charge); var mz = Muzzle(g);
            if (lv > P.lastLv) { P.lastLv = lv; g.sfx.Charge(lv); FxSys.Ring(g, mz.x, mz.y, 0.2f, lv == 2, "#7fe3ff"); }
            if (lv > 0 && g.R() < (lv == 2 ? 0.9f : 0.5f))   // hạt năng lượng hút vào đầu nỏ
            {
                float a = g.R() * 6.28f, r = 26 + g.R() * 14;
                g.parts.Add(new Particle { x = mz.x + (float)Math.Cos(a) * r, y = mz.y + (float)Math.Sin(a) * r, vx = -(float)Math.Cos(a) * r * 5, vy = -(float)Math.Sin(a) * r * 5, life = 0.18f, c = lv == 2 ? "#ffffff" : "#7fe3ff", nog = true });
            }
            if (!inp.Held(Attack))   // nhả nút = bắn
            {
                g.bolts.Add(new Bolt { x = mz.x, y = mz.y, vx = P.face * T.TV.SPEED[lv], lv = lv, life = 1.4f });
                P.aiming = false; P.charge = 0; P.shootT = T.TV.SHOOT_ANIM; P.reload = T.TV.RELOAD;
                if (P.ground) P.vx -= P.face * T.TV.RECOIL[lv];
                Squash(P, lv == 2 ? 0.86f : 0.95f, lv == 2 ? 1.08f : 1.02f);
                FxSys.Ring(g, mz.x, mz.y, 0.12f + lv * 0.05f, lv == 2, "#bff4ff");
                if (lv == 2) g.shake = T.SHAKE * 1.2f;
                g.sfx.Shoot(lv); g.sfx.Shout("tv_" + lv);
            }
        }

        /// <summary>Một bước vật lý của người chơi.</summary>
        public static void Update(Game g, float dt)
        {
            var P = g.P; var T = Cfg.T; var inp = g.input; var map = g.map;
            if (P.warp != null) { UpdateWarp(g, dt); UpdateAnim(g, dt); return; }
            // nước làm chậm: hệ số do WaterSys đặt vào g.phys* trước mỗi bước
            float RUN = T.RUN_SPEED * g.physRun, DASH = T.DASH_SPEED * g.physRun, GRAV = T.GRAVITY * g.physGrav, MAXF = T.MAX_FALL * g.physFall;
            int dirIn = (inp.Held(Right) ? 1 : 0) - (inp.Held(Left) ? 1 : 0);
            P.coyote = P.ground ? T.COYOTE : Math.Max(0, P.coyote - dt);
            P.jumpBuf = inp.Pressed(Jump) ? T.JUMP_BUFFER : Math.Max(0, P.jumpBuf - dt);
            P.wallLock = Math.Max(0, P.wallLock - dt);

            // --- chém (Xích Long) / bắn nỏ (Thanh Vũ) ---
            if (g.hero == Hero.ThanhVu) UpdateShoot(g, dt);
            else if (inp.Pressed(Attack))
            {
                if (P.atk == null) { if (P.ground && P.combo > 2) P.combo = 0; StartAttack(g); }
                else if (!P.atkAir && P.combo < 2) P.comboQueued = true;
            }
            if (g.hero == Hero.XichLong && P.atk != null)
            {
                P.atkT += dt;
                if (P.atkT >= P.atk.time)
                {
                    if (P.comboQueued && P.ground && !P.atkAir) { P.combo++; StartAttack(g); }
                    else { P.recFrame = Frames.Attack(P, true); P.recover = T.RECOVER; P.atk = null; P.combo = 0; }
                }
            }
            // vệt chém: lưu góc lưỡi đao theo thời gian
            float target = BladeAngle(P);
            P.bladeA = P.atk != null ? target : P.bladeA + (target - P.bladeA) * Math.Min(1, dt * 14);
            P.bladeL = P.atk != null ? P.atk.len : P.bladeL + (1 - P.bladeL) * Math.Min(1, dt * 10);
            if (P.atk != null && P.atkT >= P.atk.from && P.atkT <= P.atk.to + 0.02f) P.swing.Add(new Swing { a = P.bladeA, l = P.bladeL, life = 0.12f });
            foreach (var s in P.swing) s.life -= dt;
            P.swing.RemoveAll(s => s.life <= 0);
            if (P.atk != null && P.atk.spins > 0)
            {
                int k = (int)Math.Floor(Math.Max(0, P.atkT - P.atk.from) / (P.atk.to - P.atk.from) * P.atk.spins);
                if (k != P.spinK && k < P.atk.spins) { P.spinK = k; P.hitSet = new HashSet<object>(); }
            }
            if (P.arc != null) { P.arc.t += dt; if (P.arc.t > P.arc.to + T.ARC_FADE) P.arc = null; }
            bool groundSlashing = P.atk != null && !P.atkAir;

            // --- lướt ---
            if (inp.Pressed(Dash) && P.dash <= 0 && !groundSlashing)
            {
                if (P.ground) { P.dash = T.DASH_TIME; P.dashHeld = 0; Squash(P, 1.18f, 0.88f); FxSys.Dust(g, P.x + P.w / 2 - P.face * 10, P.y + P.h); }
                else if (P.airDashes > 0 && P.wall == 0) { P.airDashes--; P.dash = T.DASH_TIME * 0.8f; P.dashHeld = 0; P.vy = 0; Squash(P, 1.18f, 0.88f); }
                if (P.dash > 0 && dirIn != 0) P.face = dirIn;
            }
            if (P.dash > 0)
            {
                P.dashHeld += dt; P.dash -= dt;
                if ((!inp.Held(Dash) && P.dashHeld > T.DASH_MIN) || (dirIn != 0 && dirIn != P.face)) P.dash = 0;
                P.vx = P.face * DASH;
                if (!P.ground) P.vy = Math.Min(P.vy, 0);
                if (P.ground) P.dashMomentum = true;
                P.ghostT -= dt; if (P.ghostT <= 0) { P.ghostT = T.GHOST_EVERY; P.ghosts.Add(new Ghost { x = P.x, y = P.y, f = P.face, a = 0.6f, n = Frames.Pick(g) }); }
                if (P.dash <= 0) P.dash = 0;
            }

            // --- chạy ---
            if (P.dash <= 0 && P.wallLock <= 0)
            {
                if (groundSlashing) P.vx *= (float)Math.Pow(0.0005, dt);
                else
                {
                    float top = P.dashMomentum && !P.ground ? DASH : RUN, tgt = dirIn * top, acc = P.ground ? T.ACCEL_GROUND : T.ACCEL_AIR;
                    if (P.vx < tgt) P.vx = Math.Min(tgt, P.vx + acc * dt);
                    else if (P.vx > tgt) P.vx = Math.Max(tgt, P.vx - acc * dt);
                    if (dirIn != 0 && P.atk == null) P.face = dirIn;
                }
            }

            // --- bám tường ---
            P.wall = 0;
            if (!P.ground && P.dash <= 0 && dirIn != 0 && TouchingWall(g, dirIn) && P.vy > -120) P.wall = dirIn;
            if (g.hero == Hero.ThanhVu && P.wall != 0) P.face = -P.wall;   // Thanh Vũ quay lưng vào tường để bắn ra ngoài

            // --- nhảy ---
            if (P.jumpBuf > 0)
            {
                if (P.coyote > 0)
                {
                    P.vy = -T.JUMP_VEL; P.coyote = 0; P.jumpBuf = 0; Squash(P, 0.82f, 1.2f); FxSys.Dust(g, P.x + P.w / 2, P.y + P.h);
                    P.dashMomentum = P.dash > 0 || inp.Held(Dash); P.dash = 0; P.ground = false;
                }
                else if (!P.ground && (P.wall != 0 || TouchingWall(g, -1) || TouchingWall(g, 1)))
                {
                    int w = P.wall != 0 ? P.wall : (TouchingWall(g, 1) ? 1 : -1); bool climb = dirIn == w;   // giữ hướng vào tường = leo lên chính bức tường
                    P.dashMomentum = inp.Held(Dash);
                    P.vx = -w * (P.dashMomentum ? DASH : climb ? T.WALL_KICK_VX : T.WALL_JUMP_VX);
                    P.vy = -T.WALL_JUMP_VY; Squash(P, 0.85f, 1.15f); P.face = climb && !P.dashMomentum ? w : -w;
                    P.wallLock = climb && !P.dashMomentum ? T.WALL_KICK_LOCK : T.WALL_JUMP_LOCK; P.jumpBuf = 0; P.wall = 0; P.wjT = T.WALLJUMP_ANIM; P.wjDir = w;
                    FxSys.Sparks(g, P.x + (w > 0 ? P.w : 0), P.y + P.h - 8, 6, "#ffd34d");
                }
                else if (!P.ground && inp.Pressed(Jump) && P.airJumps > 0)   // nhảy bậc 2, vòng lửa dưới chân
                {
                    P.airJumps--; P.vy = -T.AIR_JUMP_VEL; P.jumpBuf = 0; P.dash = 0; P.airT = 0;
                    Squash(P, 0.78f, 1.22f); P.dj = 0.22f; g.sfx.Jump2();
                    FxSys.Ring(g, P.x + P.w / 2, P.y + P.h, 0.25f);
                    FxSys.Sparks(g, P.x + P.w / 2, P.y + P.h, 8, "#ffb347");
                }
            }
            if (inp.Released(Jump) && P.vy < 0) P.vy *= T.JUMP_CUT;

            // --- trọng lực ---
            if (!(P.dash > 0 && !P.ground))
            {
                P.vy = Math.Min(P.vy + GRAV * (P.vy > 0 ? T.FALL_GRAV : 1) * dt, MAXF);
                if (P.wall != 0 && P.vy > T.WALL_SLIDE) { P.vy = T.WALL_SLIDE; if (g.R() < 0.3f) FxSys.Sparks(g, P.x + (P.wall > 0 ? P.w : 0), P.y + 12, 1, "#ffb347"); }
            }

            P.x += P.vx * dt; map.CollideX(P);
            bool wasGround = P.ground; float fallV = P.vy;
            P.y += P.vy * dt; map.CollideY(P);
            if (P.ground || P.wall != 0) P.airJumps = T.AIR_JUMPS;
            if (P.ground)
            {
                P.airDashes = T.AIR_DASHES; if (P.dash <= 0) P.dashMomentum = false;
                if (!wasGround)
                {
                    FxSys.Dust(g, P.x + P.w / 2, P.y + P.h); if (fallV > T.LAND_MIN_VY) P.landT = T.LAND_TIME;
                    float k = Math.Min(1, fallV / MAXF); Squash(P, 1 + 0.22f * k, 1 - 0.24f * k);
                }
            }
            if (P.ground && P.atkAir) { P.atk = null; P.combo = 0; }

            for (int i = 0; i < g.checkpoints.Length; i++) if (P.x > g.checkpoints[i].x - 16 && i > g.cpIndex) g.cpIndex = i;
            if (P.y > g.map.h * g.map.ts + 80) Flow.Respawn(g);

            UpdateAnim(g, dt);
            foreach (var gh in P.ghosts) gh.a -= dt * 2.4f;
            P.ghosts.RemoveAll(gh => gh.a <= 0);
        }

        /// <summary>Điểm tay cầm đao trong thế giới.</summary>
        public static XY HandPos(Player P)
        {
            float sx = P.x + P.w / 2 - Cfg.SPR_W / 2f, sy = P.y + P.h - Cfg.SPR_H;
            return new XY { x = sx + (P.face > 0 ? Cfg.PIVOT_X : Cfg.SPR_W - Cfg.PIVOT_X), y = sy + Cfg.PIVOT_Y };
        }
        /// <summary>Vùng trúng của nhát chém; chỉ có khi lưỡi đao đã quét qua HIT_AT phần vệt.</summary>
        public static Box? SlashBox(Player P)
        {
            var a = P.atk; if (a == null) return null;
            float on = a.from + (a == Cfg.T.AIR_SLASH ? 0 : (a.to - a.from) * Cfg.T.HIT_AT);
            if (P.atkT < on || P.atkT > a.to + 0.02f) return null;
            var hp = HandPos(P); float x = P.face > 0 ? hp.x - 14 : hp.x - a.reach + 14;
            return new Box(x, hp.y - a.h / 2, a.reach, a.h);
        }
    }
}
