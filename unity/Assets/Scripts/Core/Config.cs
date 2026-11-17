// Số liệu chỉnh tay, đọc từ Resources/Data/*.json (giống hệt bản web: game/src/data).
using System.Collections.Generic;

namespace LongThan.Core
{
    public class XY { public float x, y; }
    /// <summary>Một nhát chém (combo hoặc chém xoay trên không).</summary>
    public class SlashStep { public float time, from, to, a0, a1, len, reach, h, knock, lunge; public int dmg, spins; }
    public class FoeHp { public int drone, turret; }
    public class WaterK { public float RUN, GRAV, FALL, DRAIN; }
    public class TvTuning
    {
        public float CHARGE1, CHARGE2, RELOAD, SHOOT_ANIM;
        public float[] SPEED, SIZE, STOP, RECOIL; public int[] DMG; public XY MUZZLE, MUZZLE_RUN;
    }

    /// <summary>Cảm giác điều khiển, sát thương, quái (tuning.json).</summary>
    public class Tuning
    {
        public FoeHp FOE_HP; public WaterK WATER;
        public int HP, SHOT_DMG, TOUCH_DMG, AIR_DASHES, AIR_JUMPS;
        public float HURT_INV, HURT_KNOCK, SHOT_SPEED, TURRET_CD, TURRET_RANGE;
        public float GRAVITY, MAX_FALL, FALL_GRAV, RUN_SPEED, ACCEL_GROUND, ACCEL_AIR, JUMP_VEL, JUMP_CUT, COYOTE, JUMP_BUFFER;
        public float DASH_SPEED, DASH_TIME, DASH_MIN, WALL_SLIDE, WALL_JUMP_VX, WALL_JUMP_VY, WALL_JUMP_LOCK, WALL_KICK_VX, WALL_KICK_LOCK;
        public float HITSTOP, FINISH_STOP, SHAKE, ARC_R, ARC_W, ARC_FADE, HIT_AT, AIR_JUMP_VEL, BLADE_LEN, TRAIL_R, BLADE_IDLE;
        public SlashStep[] COMBO; public SlashStep AIR_SLASH;
        public float SQUASH_K, SQUASH_D, STRIDE, RUN_BOB, GHOST_EVERY, RECOVER, IDLE_FPS, WALLJUMP_ANIM, WALLKICK_ANIM, DASH_PUSH, DASH_STOP;
        public float TAKEOFF_TIME, APEX_VY, LAND_TIME, LAND_MIN_VY, WARP_FALL, WARP_FORM, VOICE;
        public TvTuning TV;
    }

    public class BtWave { public float W, R, SPD, H, H2, GAP2; public int DMG; }
    public class BtDash { public float[] W, SPD; public float R; public int DMG; }
    public class BtBub { public float W, VY, G, R, SIZE; public int[] N; public float[] GAP, VX; public int DMG, REFL; }
    public class BtShield { public float W, STUN; public float[] HOLD, DROP; public int DMG; }
    public class BtSpear { public float[] W; public int[] N; public float SPD, R; public int DMG; }
    public class BtFlood { public float W, UP, HOLD, DOWN, PEAK, TICK, CD, R; public int DMG; }

    /// <summary>Trùm Thủy Tinh (boss-thuy-tinh.json, thiết kế: design/gdd/boss-thuy-tinh.md).</summary>
    public class BossTuning
    {
        public int HP, P2, TOUCH; public float INV, WATER_P2, SHIFT; public float[] WALK, IDLE;
        public BtWave WAVE; public BtDash DASH; public BtBub BUB; public BtShield SHIELD; public BtSpear SPEAR; public BtFlood FLOOD;
        public Dictionary<string, float>[] WEIGHT; public string[] LINE_IN, LINE_OUT;
    }

    public class AttackFrames { public string[] pre, act, post; public string rec; }
    /// <summary>Điểm neo và thứ tự khung hình (frames.json).</summary>
    public class FrameData
    {
        public Dictionary<string, float> FA, FY;
        public Dictionary<string, XY> MZ, SWING_AT;
        public Dictionary<string, string[]> ANIM;
        public Dictionary<string, AttackFrames> ATKF;
        public Dictionary<string, float[]> BTA;
    }

    /// <summary>Chỗ giữ số liệu dùng chung. Gọi Load một lần lúc khởi động (hoặc trong test).</summary>
    public static class Cfg
    {
        public static Tuning T; public static BossTuning BT; public static FrameData F;
        public const int FW = 150, FH = 110, SPR_W = 41, SPR_H = 84, PIVOT_X = 30, PIVOT_Y = 45;
        public const float VH = 360, STEP = 1f / 120f;

        public static void Load(string tuningJson, string bossJson, string framesJson)
        {
            T = Json.Bind<Tuning>(Json.Parse(tuningJson));
            BT = Json.Bind<BossTuning>(Json.Parse(bossJson));
            F = Json.Bind<FrameData>(Json.Parse(framesJson));
        }
        public static float Fy(string n) => F.FY.TryGetValue(n, out var v) ? v : 0;
        public static float Fa(string n) => F.FA.TryGetValue(n, out var v) ? v : 75;
    }
}
