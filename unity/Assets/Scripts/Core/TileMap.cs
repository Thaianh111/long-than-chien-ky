using System;

namespace LongThan.Core
{
    /// <summary>Bản đồ ô gạch: 0 = trống, 1 = gạch, 2 = đặc nhưng không vẽ (lưng Thuồng Luồng), 3 = cột đá Sơn Tinh. Mỗi ô ts×ts px.</summary>
    public class TileMap
    {
        public readonly int w, h, ts;
        readonly byte[,] cells;
        public TileMap(int w, int h, int ts = 32) { this.w = w; this.h = h; this.ts = ts; cells = new byte[h, w]; }

        /// <summary>Tô một khối ô (tính theo ô) bằng giá trị v.</summary>
        public void Rect(int x, int y, int rw, int rh, int v = 1)
        {
            for (int j = y; j < y + rh; j++) for (int i = x; i < x + rw; i++)
                if (i >= 0 && j >= 0 && i < w && j < h) cells[j, i] = (byte)v;
        }
        public bool SolidCell(int i, int j) => i >= 0 && i < w && j >= 0 && j < h && cells[j, i] != 0;
        /// <summary>Giá trị ô (0 nếu ngoài bản đồ).</summary>
        public int Get(int i, int j) => i >= 0 && i < w && j >= 0 && j < h ? cells[j, i] : 0;
        public void Set(int i, int j, int v) { if (i >= 0 && i < w && j >= 0 && j < h) cells[j, i] = (byte)v; }
        public bool SolidAt(float px, float py) => SolidCell((int)Math.Floor(px / ts), (int)Math.Floor(py / ts));

        /// <summary>Đẩy vật ra khỏi tường theo trục ngang. Trả về -1/1 nếu đụng tường trái/phải, 0 nếu không.</summary>
        public int CollideX(Body o)
        {
            float top = o.y + 1, bot = o.y + o.h - 1;
            if (o.vx > 0)
            {
                float r = o.x + o.w;
                for (float y = top; y <= bot; y += 12) if (SolidAt(r, y) || SolidAt(r, bot)) { o.x = (float)Math.Floor(r / ts) * ts - o.w - 0.01f; o.vx = 0; return 1; }
            }
            else if (o.vx < 0)
            {
                for (float y = top; y <= bot; y += 12) if (SolidAt(o.x, y) || SolidAt(o.x, bot)) { o.x = ((float)Math.Floor(o.x / ts) + 1) * ts + 0.01f; o.vx = 0; return -1; }
            }
            return 0;
        }
        /// <summary>Đẩy vật ra khỏi sàn/trần theo trục dọc, đặt o.ground khi đứng trên gạch.</summary>
        public void CollideY(Body o)
        {
            float l = o.x + 1, r = o.x + o.w - 1;
            o.ground = false;
            if (o.vy > 0)
            {
                float b = o.y + o.h;
                if (SolidAt(l, b) || SolidAt(r, b)) { o.y = (float)Math.Floor(b / ts) * ts - o.h; o.vy = 0; o.ground = true; }
            }
            else if (o.vy < 0)
            {
                if (SolidAt(l, o.y) || SolidAt(r, o.y)) { o.y = ((float)Math.Floor(o.y / ts) + 1) * ts; o.vy = 0; }
            }
        }
    }
}
