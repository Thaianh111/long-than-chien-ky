// Luồng một lượt chơi: hồi sinh, một bước vật lý cố định, camera.
using System;

namespace LongThan.Core
{
    public static class Flow
    {
        /// <summary>Hồi sinh ở điểm lưu gần nhất: hồi đầy máu, trùm về như cũ, giếng nước rút.</summary>
        public static void Respawn(Game g)
        {
            var fresh = Game.FreshPlayer(g.checkpoints[g.cpIndex]);
            CopyPlayer(fresh, g.P);   // giữ nguyên đối tượng P để các hệ thống đang chạy không cầm bản cũ
            PlayerSys.WarpIn(g);
            if (g.boss.st != BossSt.Wait && g.boss.st != BossSt.Dead) BossSys.Reset(g);
            if (g.sub.st != SubSt.Wait && g.sub.st != SubSt.Dead) SubSys.Reset(g);
            g.lvWaterRise = null;
            var L = g.level.rise.lockTiles; g.map.Rect(L.x, L.y, L.w, L.h, 0);
        }
        static void CopyPlayer(Player src, Player dst)
        {
            foreach (var f in typeof(Player).GetFields()) f.SetValue(dst, f.GetValue(src));
        }

        /// <summary>Một bước vật lý cố định (Cfg.STEP giây).</summary>
        public static void Step(Game g)
        {
            float dt = Cfg.STEP; var P = g.P;
            g.time += dt; P.ox = P.x; P.oy = P.y;
            FxSys.UpdateFx(g, dt);
            if (g.hitstop > 0) { g.hitstop -= dt; return; }   // khựng hình khi trúng đòn
            bool wet = WaterSys.Before(g);
            PlayerSys.Update(g, dt);
            WaterSys.After(g, wet, dt);
            if (g.talk != null) { g.talk.t += dt; if (g.talk.t > g.talk.dur) g.talk = null; }
            WaterSys.UpdateRising(g, dt);
            BossSys.Update(g, dt);
            SubSys.Update(g, dt, wet);
            PillarSys.Update(g, dt);
            g.P.inv = Math.Max(0, g.P.inv - dt);
            FoeSys.Update(g, dt); FoeSys.UpdateShots(g, dt); FoeSys.UpdateBolts(g, dt); FxSys.UpdateParts(g, dt);
            if (g.input.Pressed(Key.Swap)) PlayerSys.SwapHero(g);
            g.input.ClearEdges();
            g.shake = Math.Max(0, g.shake - dt * 30);
        }

        /// <summary>Camera bám người chơi; vào trận trùm thì khóa giữa đấu trường.</summary>
        public static void UpdateCamera(Game g, ref float camX, ref float camY, float VW, float dt, bool touch)
        {
            var P = g.P; var A = g.level.arena; float maxX = g.map.w * g.map.ts - VW;
            camX += (P.x + P.w / 2 + P.face * 80 - VW / 2 - camX) * Math.Min(1, dt * 6);
            camY += (P.y - Cfg.VH * (touch ? 0.38f : 0.5f) - camY) * Math.Min(1, dt * 4);
            if (g.boss.st != BossSt.Wait) camX += ((A.x0 + A.x1) / 2 - VW / 2 - camX) * Math.Min(1, dt * 5);
            var S = g.level.sub;
            if (g.sub.st != SubSt.Wait && g.sub.st != SubSt.Dead) camX += ((S.x0 + S.x1) / 2 - VW / 2 - camX) * Math.Min(1, dt * 5);
            camX = Math.Max(0, Math.Min(maxX, camX));
            camY = Math.Max(-80, Math.Min(g.map.h * g.map.ts - Cfg.VH + (touch ? 90 : 0), camY));
        }
    }
}
