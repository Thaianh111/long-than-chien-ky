using LongThan.Core;
using NUnit.Framework;

namespace LongThan.Tests
{
    public class WaterTests
    {
        [Test] public void ChoKhongCoNuoc() => Assert.IsTrue(float.IsPositiveInfinity(WaterSys.WaterY(TestData.MakeGame(), 5 * 32)));
        [Test] public void VungNongVaHoSau()
        {
            var g = TestData.MakeGame();
            Assert.IsFalse(WaterSys.DeepAt(g, 16 * 32)); Assert.IsTrue(WaterSys.DeepAt(g, 47 * 32)); Assert.AreEqual(12 * 32, WaterSys.WaterY(g, 47 * 32));
        }
        [Test] public void NuocDauTruongLayMucCaoHon()
        {
            var g = TestData.MakeGame(); var A = g.level.arena; g.arenaWater = 64; g.arenaHaz = 176;
            Assert.AreEqual(A.floorY - 176, WaterSys.WaterY(g, (A.x0 + A.x1) / 2));
        }
    }

    public class PlayerTests
    {
        [Test] public void RoiXuongDungTrenNen()
        {
            var g = TestData.MakeGame(); TestData.Place(g, 5 * 32, 10 * 32);
            for (int i = 0; i < 240; i++) Flow.Step(g);
            Assert.IsTrue(g.P.ground); Assert.AreEqual(15 * 32, g.P.y + g.P.h, 0.01f);
        }
        [Test] public void CapNapNo()
        {
            TestData.Ensure();
            Assert.AreEqual(0, PlayerSys.ChargeLv(0)); Assert.AreEqual(1, PlayerSys.ChargeLv(Cfg.T.TV.CHARGE1)); Assert.AreEqual(2, PlayerSys.ChargeLv(Cfg.T.TV.CHARGE2));
        }
        [Test] public void TrungDonMatMauVaBatTu()
        {
            var g = TestData.MakeGame(); TestData.Place(g, 5 * 32, 15 * 32);
            PlayerSys.Hurt(g, 3, 0); Assert.AreEqual(Cfg.T.HP - 3, g.P.hp); Assert.Greater(g.P.inv, 0);
            PlayerSys.Hurt(g, 3, 0); Assert.AreEqual(Cfg.T.HP - 3, g.P.hp);
        }
        [Test] public void HetMauHoiSinhODiemLuu()
        {
            var g = TestData.MakeGame(); g.cpIndex = 2; TestData.Place(g, 30 * 32, 15 * 32);
            var same = g.P; PlayerSys.Hurt(g, 99, 0);
            Assert.AreEqual(Cfg.T.HP, g.P.hp); Assert.AreEqual(g.checkpoints[2].x, g.P.x); Assert.IsNotNull(g.P.warp);
            Assert.IsTrue(ReferenceEquals(same, g.P), "giữ nguyên đối tượng người chơi");
        }
        [Test] public void ChayToiVaChemTrungQuai()
        {
            var g = TestData.MakeGame(); var f = g.foes[0]; f.y = f.oy = 15 * 32 - f.h; f.x = f.ox = f.hx = 10 * 32;
            TestData.Place(g, 10 * 32 - 30, 15 * 32); g.P.ground = true; g.P.face = 1;
            int hp0 = f.hp; g.input.pressed[(int)Key.Attack] = true;
            for (int i = 0; i < 60; i++) Flow.Step(g);
            Assert.Greater(hp0, f.hp, "quái phải mất máu");
        }
    }
}
