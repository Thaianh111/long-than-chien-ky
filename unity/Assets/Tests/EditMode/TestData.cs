// Nạp số liệu JSON cho test: trong Unity đọc từ Resources, ngoài Unity đọc file.
using LongThan.Core;

namespace LongThan.Tests
{
    public static class TestData
    {
        public static string Dir;
        static bool loaded;
        static string Read(string name)
        {
#if UNITY_5_3_OR_NEWER
            return UnityEngine.Resources.Load<UnityEngine.TextAsset>("Data/" + name).text;
#else
            return System.IO.File.ReadAllText(System.IO.Path.Combine(Dir, name + ".json"));
#endif
        }
        public static void Ensure()
        {
            if (loaded) return; loaded = true;
            Cfg.Load(Read("tuning"), Read("boss-thuy-tinh"), Read("frames"));
        }
        /// <summary>Một lượt chơi màn Hồng Thủy, số ngẫu nhiên cố định, không âm thanh.</summary>
        public static Game MakeGame(uint seed = 1) { Ensure(); return Game.Create(Levels.HongThuy(), new InputState(), null, seed); }
        /// <summary>Đặt người chơi đứng ngay vị trí (x giữa thân, chân ở footY).</summary>
        public static void Place(Game g, float x, float footY) { g.P.x = x - g.P.w / 2; g.P.y = footY - g.P.h; g.P.vx = 0; g.P.vy = 0; }
    }
}
