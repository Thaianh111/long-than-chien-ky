using System.Linq;
using LongThan.Core;
using NUnit.Framework;

namespace LongThan.Tests
{
    public class BossTests
    {
        [Test] public void DonDauLuonLaSongThan()
        {
            for (uint s = 1; s <= 20; s++) Assert.AreEqual(BossMove.Wave, BossSys.ChooseMove(TestData.MakeGame(s)));
        }
        [Test] public void KhongLap3LanKhongKhien2Lan()
        {
            var g = TestData.MakeGame(7); g.boss.phase = 1; g.boss.lostSince = 99;
            var seq = Enumerable.Range(0, 1000).Select(_ => BossSys.ChooseMove(g)).ToArray();
            for (int i = 2; i < seq.Length; i++) Assert.IsFalse(seq[i] == seq[i - 1] && seq[i] == seq[i - 2], "lặp 3 lần ở " + i);
            for (int i = 1; i < seq.Length; i++) Assert.IsFalse(seq[i] == BossMove.Shield && seq[i - 1] == BossMove.Shield);
        }
        [Test] public void DaiHongThuyKhongRaOPha1()
        {
            var g = TestData.MakeGame(3); g.boss.lostSince = 99;
            Assert.IsFalse(Enumerable.Range(0, 500).Select(_ => BossSys.ChooseMove(g)).Contains(BossMove.Flood));
        }
        [Test] public void OXaThiHayBiLaoHon()
        {
            int Count(bool far)
            {
                var g = TestData.MakeGame(11); TestData.Place(g, far ? g.boss.x - 450 : g.boss.x - 200, g.level.arena.floorY); BossSys.ChooseMove(g);
                return Enumerable.Range(0, 600).Count(_ => BossSys.ChooseMove(g) == BossMove.Dash);
            }
            Assert.Greater(Count(true), Count(false));
        }
        [Test] public void MuaGiaoLuonChua2KheNe()
        {
            for (uint s = 1; s <= 1000; s++)
            {
                var g = TestData.MakeGame(s); var A = g.level.arena; int n = Cfg.BT.SPEAR.N[s % 2];
                var xs = new[] { A.x0 }.Concat(BossSys.SpearSpots(g, n).OrderBy(x => x)).Concat(new[] { A.x1 }).ToArray();
                int gaps = 0; for (int i = 1; i < xs.Length; i++) if (xs[i] - xs[i - 1] - 16 >= 64) gaps++;
                Assert.GreaterOrEqual(gaps, 2, "hạt giống " + s);
                Assert.AreEqual(n, xs.Length - 2, "đúng số giáo");
            }
        }
        [Test] public void ChuaVaoTranThiKhongMatMau()
        {
            var g = TestData.MakeGame(); Assert.IsFalse(BossSys.Hurt(g, 5)); Assert.AreEqual(Cfg.BT.HP, g.boss.hp);
        }
        [Test] public void NuaMauThiChuyenPha2VaXoaDan()
        {
            var g = TestData.MakeGame(); g.boss.st = BossSt.Idle; g.bproj.Add(new BossShot());
            BossSys.Hurt(g, Cfg.BT.HP - Cfg.BT.P2);
            Assert.AreEqual(BossSt.Shift, g.boss.st); Assert.AreEqual(1, g.boss.phase); Assert.AreEqual(0, g.bproj.Count);
        }
        [Test] public void MauKhongAmVaGucNga()
        {
            var g = TestData.MakeGame(); g.boss.st = BossSt.Idle; g.boss.phase = 1; g.boss.hp = 2;
            BossSys.Hurt(g, 4); Assert.AreEqual(0, g.boss.hp); Assert.AreEqual(BossSt.Dead, g.boss.st);
        }
    }
}
