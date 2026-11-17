using System.Linq;
using LongThan.Core;
using NUnit.Framework;

namespace LongThan.Tests
{
    /// <summary>Trùm phụ Thuồng Luồng và cột đá Sơn Tinh (giống game/tests/unit/boss/thuongLuong.test.ts, world/pillars.test.ts).</summary>
    public class SubBossTests
    {
        static Game Enter(uint seed = 1)
        {
            var g = TestData.MakeGame(seed); var A = g.level.sub;
            TestData.Place(g, A.closeAtX + 10, A.platRow * g.map.ts); SubSys.Update(g, Cfg.STEP); return g;
        }
        [Test] public void DonDauLuonLaLao()
        {
            for (uint s = 1; s <= 20; s++) Assert.AreEqual(SubMove.Surge, SubSys.ChooseMove(Enter(s)));
        }
        [Test] public void KhongLap3LanKhongNoi2Lan()
        {
            foreach (var ph in new[] { 0, 1 })
            {
                var g = Enter(5); g.sub.phase = ph;
                var seq = Enumerable.Range(0, 1000).Select(i => SubSys.ChooseMove(g, i % 7 == 0)).ToArray();
                for (int i = 2; i < seq.Length; i++) Assert.IsFalse(seq[i] == seq[i - 1] && seq[i] == seq[i - 2]);
                for (int i = 1; i < seq.Length; i++) Assert.IsFalse(seq[i] == SubMove.Float && seq[i - 1] == SubMove.Float);
            }
        }
        [Test] public void VaoTranThiCongDong()
        {
            var g = Enter(); Assert.AreEqual(SubSt.Intro, g.sub.st); Assert.IsTrue(g.map.SolidCell(g.level.sub.gate.x, g.level.sub.gate.y));
        }
        [Test] public void DauDuoiNuocKhongAnDon()
        {
            var g = Enter(); g.sub.st = SubSt.Under; g.sub.y = g.level.sub.surface + Cfg.TL.Y.UNDER;
            Assert.IsFalse(SubSys.Hurt(g, 2));
            g.sub.y = g.level.sub.surface + Cfg.TL.Y.FLOAT;
            Assert.IsTrue(SubSys.Hurt(g, 2)); Assert.AreEqual(Cfg.TL.HP - 2, g.sub.hp);
        }
        [Test] public void NoiLungCoODungLanThiHet()
        {
            var g = Enter(3); var A = g.level.sub; int TS = g.map.ts, row = A.platRow;
            int Count() { int n = 0; for (int i = (int)(A.x0 / TS); i < (int)(A.x1 / TS); i++) if (g.map.Get(i, row) == 2) n++; return n; }
            g.sub.st = SubSt.Under; g.sub.t = 99; g.sub.first = true; g.sub.last = new System.Collections.Generic.List<SubMove> { SubMove.Surge };
            bool floated = false;
            for (int i = 0; i < 120 * 60 && !floated; i++) { SubSys.Update(g, Cfg.STEP); if (g.sub.move == SubMove.Float && g.sub.stage == Stage.A && Count() > 0) floated = true; }
            Assert.IsTrue(floated); Assert.GreaterOrEqual(Count(), 6);
            for (int i = 0; i < 120 * 8 && g.sub.move == SubMove.Float; i++) SubSys.Update(g, Cfg.STEP);
            Assert.AreEqual(0, Count());
        }
        [Test] public void HaXongThiCongPhaiMo()
        {
            var g = Enter(); var E = g.level.sub.exit;
            Assert.IsTrue(g.map.SolidCell(E.x, E.y));
            g.sub.st = SubSt.Atk; g.sub.move = SubMove.Float; g.sub.y = g.level.sub.surface + Cfg.TL.Y.FLOAT;
            for (int i = 0; i < 30 && g.sub.st != SubSt.Dead; i++) { g.sub.inv = 0; SubSys.Hurt(g, 1); }
            Assert.AreEqual(SubSt.Dead, g.sub.st);
            for (int j = 0; j < E.h; j++) Assert.IsFalse(g.map.SolidCell(E.x, E.y + j));
        }
        [Test] public void BanTrungCotThiCotDung()
        {
            var g = TestData.MakeGame(); var c = g.pillars[1]; int TS = g.map.ts;
            g.bolts.Add(new Bolt { x = c.tx * TS + 10, y = PillarSys.Top(g, c) + 8, vx = 600, lv = 0, life = 1 });
            for (int i = 0; i < 240; i++) PillarSys.Update(g, Cfg.STEP);
            Assert.AreEqual(c.max, c.cur); Assert.AreEqual(3, g.map.Get(c.tx, c.@base - c.max + 1));
        }
        [Test] public void DungTrenCotThiDuocNang()
        {
            var g = TestData.MakeGame(); var c = g.pillars[1]; int TS = g.map.ts;
            TestData.Place(g, c.tx * TS + TS / 2, PillarSys.Top(g, c)); c.grow = true;
            int n = (int)System.Math.Ceiling((c.max - c.cur) * Cfg.T.PILLAR_STEP * 120) + 10;
            for (int i = 0; i < n; i++) PillarSys.Update(g, Cfg.STEP);
            Assert.AreEqual(PillarSys.Top(g, c), g.P.y + g.P.h, 0.001f);
            Assert.AreEqual((c.@base - c.max + 1) * TS, PillarSys.Top(g, c), 0.001f);
        }
    }
}
