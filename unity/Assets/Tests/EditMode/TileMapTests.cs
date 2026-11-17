using LongThan.Core;
using NUnit.Framework;

namespace LongThan.Tests
{
    public class TileMapTests
    {
        [Test] public void VatRoiDungTrenMatGach()
        {
            var m = new TileMap(10, 10, 32); m.Rect(0, 8, 10, 2);
            var o = new Body { x = 40, y = 230, w = 20, h = 40, vy = 500 };   // chân lún vào hàng gạch 8
            m.CollideY(o);
            Assert.IsTrue(o.ground); Assert.AreEqual(8 * 32, o.y + o.h); Assert.AreEqual(0, o.vy);
        }
        [Test] public void DungTuongPhaiBiDayRa()
        {
            var m = new TileMap(10, 10, 32); m.Rect(5, 0, 1, 10);
            var o = new Body { x = 150, y = 50, w = 20, h = 40, vx = 300 };
            Assert.AreEqual(1, m.CollideX(o)); Assert.LessOrEqual(o.x + o.w, 5 * 32);
        }
        [Test] public void GiaTri0KhoetTrong()
        {
            var m = new TileMap(4, 4); m.Rect(0, 0, 4, 4); m.Rect(1, 1, 2, 1, 0);
            Assert.IsFalse(m.SolidCell(1, 1)); Assert.IsTrue(m.SolidCell(0, 1));
        }
    }
}
