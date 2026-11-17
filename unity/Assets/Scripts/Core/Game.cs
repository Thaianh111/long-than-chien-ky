// Trạng thái một lượt chơi. Mọi hệ thống nhận Game làm tham số (không dùng biến toàn cục) để test được.
using System;
using System.Collections.Generic;
using System.Linq;

namespace LongThan.Core
{
    /// <summary>Âm thanh mà logic gọi tới (tách riêng để test chạy không cần Unity).</summary>
    public interface ISfx
    {
        void Swing(float p); void Hit(bool big); void Shoot(int lv); void Charge(int lv);
        void Warp(); void Land(); void Jump2(); void Shout(string key);
    }
    public class SilentSfx : ISfx
    {
        public void Swing(float p) { } public void Hit(bool big) { } public void Shoot(int lv) { } public void Charge(int lv) { }
        public void Warp() { } public void Land() { } public void Jump2() { } public void Shout(string key) { }
    }

    public enum Key { Left, Right, Jump, Attack, Dash, Swap }

    /// <summary>Trạng thái nút: held = đang giữ; pressed/released = vừa bấm/nhả, xóa sau mỗi bước vật lý.</summary>
    public class InputState
    {
        public readonly bool[] held = new bool[6], pressed = new bool[6], released = new bool[6];
        readonly bool[] prev = new bool[6];
        public bool Held(Key k) => held[(int)k];
        public bool Pressed(Key k) => pressed[(int)k];
        public bool Released(Key k) => released[(int)k];
        /// <summary>Cập nhật từ trạng thái phím thật (cộng dồn cạnh bấm/nhả tới khi bước vật lý dùng).</summary>
        public void Poll(bool[] now, bool[] tapped)
        {
            for (int i = 0; i < 6; i++)
            {
                held[i] = now[i];
                pressed[i] = pressed[i] || tapped[i] || (held[i] && !prev[i]);
                released[i] = released[i] || (!held[i] && prev[i]);
                prev[i] = held[i];
            }
        }
        public void ClearEdges() { for (int i = 0; i < 6; i++) { pressed[i] = false; released[i] = false; } }
    }

    /// <summary>Số ngẫu nhiên có hạt giống (mulberry32) — cùng hạt cho cùng dãy, để test tất định.</summary>
    public class Rng
    {
        uint a;
        public Rng(uint seed) { a = seed; }
        public float Next() { a += 0x6d2b79f5; uint t = a; t = (t ^ (t >> 15)) * (t | 1); t ^= t + (t ^ (t >> 7)) * (t | 61); return Math.Min(0.9999999f, (float)((t ^ (t >> 14)) / 4294967296.0)); }
    }

    public class Game
    {
        public LevelDef level; public TileMap map; public Checkpoint[] checkpoints; public int cpIndex;
        public Hero hero = Hero.XichLong; public Player P; public InputState input; public ISfx sfx; public Rng rng;
        /// <summary>Ngẫu nhiên cho hiệu ứng (không ảnh hưởng luật chơi).</summary>
        public readonly Random fxRand = new Random();
        public List<Foe> foes = new List<Foe>(); public List<Bolt> bolts = new List<Bolt>(); public List<EnemyShot> eshots = new List<EnemyShot>();
        public List<Particle> parts = new List<Particle>(); public List<Fx> fx = new List<Fx>();
        public Boss boss; public List<BossShot> bproj = new List<BossShot>();
        /// <summary>Trùm phụ Thuồng Luồng và các cột đá Sơn Tinh.</summary>
        public SubBoss sub; public List<Pillar> pillars = new List<Pillar>();
        public float arenaWater, arenaHaz; public RiseState lvWaterRise; public Talk talk;
        /// <summary>Hệ số vật lý hiện tại (dưới nước nhỏ hơn 1).</summary>
        public float physRun = 1, physGrav = 1, physFall = 1;
        public float hitstop, shake, flashT, time; public int hits, kills;
        public Action<Hero> onHeroChange;

        public float R() => (float)fxRand.NextDouble();

        public static Game Create(LevelDef level, InputState input, ISfx sfx = null, uint seed = 1)
        {
            var g = new Game { level = level, input = input, sfx = sfx ?? new SilentSfx(), rng = new Rng(seed) };
            g.map = new TileMap(level.w, level.h, level.ts);
            foreach (var t in level.tiles) g.map.Rect(t[0], t[1], t[2], t[3], t.Length > 4 ? t[4] : 1);
            g.checkpoints = level.checkpoints;
            g.P = FreshPlayer(level.checkpoints[0]);
            g.foes = level.foes.Select(f => MakeFoe(f.type, f.tx, f.ty, level.ts, g.rng)).ToList();
            g.boss = Boss.Create(level.arena);
            g.sub = SubSys.Create(level);
            g.pillars = PillarSys.Create(g);
            return g;
        }

        public static Foe MakeFoe(FoeType type, int tx, int ty, int ts, Rng rng)
        {
            float w = type == FoeType.Drone ? 34 : 36, h = type == FoeType.Drone ? 30 : 44;
            int hp = type == FoeType.Drone ? Cfg.T.FOE_HP.drone : Cfg.T.FOE_HP.turret;
            float ox = tx * ts, oy = ty * ts + ts - h;
            return new Foe { type = type, ox = ox, oy = oy, hx = ox, x = ox, y = oy, w = w, h = h, hp = hp, maxHp = hp, t = rng.Next() * 6, cd = 1 + rng.Next() };
        }

        public static Player FreshPlayer(Checkpoint cp)
        {
            var T = Cfg.T;
            return new Player { w = 22, h = 78, x = cp.x, y = cp.y - 78, ox = cp.x, oy = cp.y - 78, airDashes = T.AIR_DASHES, airJumps = T.AIR_JUMPS, bladeA = T.BLADE_IDLE, hp = T.HP };
        }
    }

    public class RiseState { public float y, t; }
}
