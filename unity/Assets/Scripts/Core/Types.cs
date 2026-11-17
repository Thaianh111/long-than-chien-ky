// Kiểu dữ liệu của một lượt chơi. Đơn vị: pixel (1 ô = 32px), trục y hướng XUỐNG (giống bản web).
using System.Collections.Generic;

namespace LongThan.Core
{
    public enum Hero { XichLong, ThanhVu }

    /// <summary>Vật có va chạm với ô gạch.</summary>
    public class Body { public float x, y, w, h, vx, vy; public bool ground; }
    public struct Box { public float x, y, w, h; public Box(float x, float y, float w, float h) { this.x = x; this.y = y; this.w = w; this.h = h; } }

    public class Arc { public float a0, a1, from, to, len, t; public bool big; public XY at; }
    public class Ghost { public float x, y, f, a; public string n; }
    public class Warp { public float t; public bool landed; }
    public class Swing { public float a, l, life; }

    /// <summary>Người chơi (dùng chung cho Xích Long và Thanh Vũ).</summary>
    public class Player : Body
    {
        public float ox, oy, face = 1, coyote, jumpBuf, dash, dashHeld, dj, wallLock, wjT, wjDir = 1;
        public int airDashes, airJumps, wall, combo, spinK, hp; public bool dashMomentum, comboQueued, atkAir, wasDash;
        public SlashStep atk; public float atkT; public HashSet<object> hitSet = new HashSet<object>();
        public Arc arc; public List<Swing> swing = new List<Swing>(); public float bladeA, bladeL = 1;
        public List<Ghost> ghosts = new List<Ghost>(); public float ghostT, sqx = 1, sqy = 1, vsx, vsy;
        public float gait, recover, turnT, lastFace = 1, airT, landT; public string recFrame;
        public float inv; public Warp warp;
        public bool aiming; public float charge, aimT, reload, shootT; public int lastLv;
        public bool wasWet; public float drain, hazT;
    }

    public enum FoeType { Drone, Turret }
    public class Foe : Body
    {
        public FoeType type; public float ox, oy, hx, flash, dead, t, dir = -1, cd, fire, kbx, kby; public int hp, maxHp;
    }
    public class Bolt { public float x, y, vx, life; public int lv; public HashSet<object> hit = new HashSet<object>(); }
    public class EnemyShot { public float x, y, vx, vy, life, t; }
    public class Particle { public float x, y, vx, vy, life; public string c; public bool nog; }
    public enum FxKind { Ring, Cut, Boom, ShotHit }
    public class Fx { public FxKind k; public float x, y, life, max, a, L; public bool big; public string col; }

    public class Checkpoint { public float x, y; public Checkpoint(float x, float y) { this.x = x; this.y = y; } }
    public class WaterZone { public float x0, x1, y; public bool deep = true; }
    public class TileRect { public int x, y, w, h; }
    public class WaterRise { public float triggerX, x0, x1, startY, endY, speed, delay; public TileRect lockTiles; }
    public class BossArena { public float x0, x1, gateX, closeAtX, floorY; public TileRect gate; }
    /// <summary>Đấu trường trùm phụ: x0/x1/closeAtX/surface tính px; platRow là hàng ô lưng rắn khi nổi; gate/exit tính ô.</summary>
    public class SubArena { public float x0, x1, closeAtX, surface; public int platRow; public TileRect gate, exit; }
    public class Talk { public string[] lines; public float t, dur; public string who; }
}
