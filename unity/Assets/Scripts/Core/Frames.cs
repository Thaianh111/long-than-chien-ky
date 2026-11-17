using System;

namespace LongThan.Core
{
    /// <summary>Chọn tên khung hình theo trạng thái (file Resources/Frames/f_&lt;tên&gt;.png).</summary>
    public static class Frames
    {
        static string Seq(string name, float t, float fps = 10) { var a = Cfg.F.ANIM[name]; return a[(int)Math.Floor(t * fps) % a.Length]; }
        public static bool TvRunning(Player P) => P.ground && P.dash <= 0 && Math.Abs(P.vx) > 40;

        /// <summary>Khung của nhát chém (pre = lấy đà, act = vung, post = cuối nhát, rec = thu đao).</summary>
        public static string Attack(Player P, bool endPose = false)
        {
            var a = P.atk; if (a == null) return null;
            var F = Cfg.F.ATKF.TryGetValue(P.atkAir ? "air" : P.combo.ToString(), out var f) ? f : Cfg.F.ATKF["0"];
            if (endPose) return F.rec;
            if (P.atkT < a.from) return F.pre[Math.Min(F.pre.Length - 1, (int)Math.Floor(P.atkT / a.from * F.pre.Length))];
            if (P.atkT <= a.to) return F.act[Math.Min(F.act.Length - 1, (int)Math.Floor((P.atkT - a.from) / (a.to - a.from) * F.act.Length))];
            return F.post[Math.Min(F.post.Length - 1, (int)Math.Floor((P.atkT - a.to) / (a.time - a.to) * F.post.Length))];
        }

        public static string Pick(Game g)
        {
            var P = g.P; var T = Cfg.T; float time = g.time; var A = Cfg.F.ANIM;
            if (g.hero == Hero.ThanhVu)
            {
                bool shooting = P.aiming || P.shootT > 0;
                if (P.wall != 0) return shooting ? "tv_w2" : "tv_w1";
                if (P.wjT > 0 && !shooting) return "tv_w3";
                if (P.dash > 0 && !shooting) return P.ground && P.dashHeld < T.DASH_PUSH ? "tv_d1" : "tv_d2";
                if (P.recover > 0 && P.recFrame == "tv_d3" && !P.aiming) return "tv_d3";
                if (!P.ground)
                {
                    string sh = shooting ? "tv_aj" : "tv_j";
                    if (P.vy < 0 && P.airT < T.TAKEOFF_TIME && !P.aiming) return "tv_j1";
                    return sh + (P.vy < -T.APEX_VY ? 2 : P.vy < T.APEX_VY ? 3 : 4);
                }
                if (TvRunning(P) && shooting) return A["tv_runshoot"][(int)Math.Floor(P.gait) % A["tv_runshoot"].Length];
                if (P.shootT > 0) return P.shootT > T.TV.SHOOT_ANIM - 0.07f ? "tv_b3" : "tv_b4";
                if (P.aiming) return P.charge >= T.TV.CHARGE1 ? Seq("tv_charge", time, 12) : P.aimT < 0.05f ? "tv_b1" : "tv_b2";   // tụ lực: đầu nỏ rực sáng
                if (P.landT > 0 && Math.Abs(P.vx) <= 40) return "tv_j5";
                if (P.ground && P.dash <= 0 && Math.Abs(P.vx) > 40) return A["tv_run"][(int)Math.Floor(P.gait) % A["tv_run"].Length];
                return Seq("tv_idle", time, T.IDLE_FPS);
            }
            if (P.atk != null) return Attack(P);
            if (P.recover > 0 && P.recFrame != null) return P.recFrame;
            if (P.dash > 0) return P.ground && P.dashHeld < T.DASH_PUSH ? Seq("dash_start", time) : Seq("dash", time);
            if (P.wall != 0) return Seq("wall", time);
            if (!P.ground)
            {
                if (P.wjT > 0) return P.wjT > T.WALLJUMP_ANIM - T.WALLKICK_ANIM ? A["wall_kick"][0] : A["wall_jump"][0];
                if (P.vy < 0 && P.airT < T.TAKEOFF_TIME) return Seq("takeoff", P.airT);
                return Seq(P.vy < -T.APEX_VY ? "jump_up" : P.vy < T.APEX_VY ? "apex" : "jump_down", P.airT);
            }
            if (P.landT > 0 && Math.Abs(P.vx) <= 40) return Seq("land", P.landT);
            if (Math.Abs(P.vx) > 40) return A["run"][(int)Math.Floor(P.gait) % A["run"].Length];
            return Seq("idle", time, T.IDLE_FPS);
        }
    }
}
