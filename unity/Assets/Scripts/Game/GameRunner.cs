// Điểm khởi động Unity: tự tạo khi bấm Play ở bất kỳ cảnh nào (không cần kéo thả gì).
// Vòng lặp: vật lý bước cố định 120Hz (Core.Flow.Step), vẽ nội suy theo tần số màn hình.
// © Võ Thái Anh · VTA Games
using LongThan.Core;
using UnityEngine;
using UnityEngine.Rendering;
using CoreKey = LongThan.Core.Key;
using G = LongThan.Core.Game;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace LongThan.Game
{
    public class GameRunner : MonoBehaviour
    {
        G g; WorldRenderer view; SynthSfx sfx; Camera cam;
        readonly bool[] now = new bool[6], tapped = new bool[6], touchHeld = new bool[6];
        float acc, camX, camY, VW = 640, renderAlpha = 1;
        bool started, usingTouch;
        GUIStyle hudStyle, talkStyle, bigStyle, btnStyle;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Boot()
        {
            if (FindAnyObjectByType<GameRunner>()) return;
            var go = new GameObject("LongThanChienKy"); DontDestroyOnLoad(go); go.AddComponent<GameRunner>();
        }

        void Awake()
        {
            Application.targetFrameRate = 120; QualitySettings.vSyncCount = 1;
            Cfg.Load(Resources.Load<TextAsset>("Data/tuning").text, Resources.Load<TextAsset>("Data/boss-thuy-tinh").text, Resources.Load<TextAsset>("Data/frames").text);
            sfx = new SynthSfx(gameObject);
            g = G.Create(Levels.HongThuy(), new InputState(), sfx, (uint)System.Environment.TickCount);
            view = new WorldRenderer();
            cam = Camera.main;
            if (!cam) { var cgo = new GameObject("Main Camera"); cam = cgo.AddComponent<Camera>(); cgo.tag = "MainCamera"; }
            cam.clearFlags = CameraClearFlags.SolidColor; cam.backgroundColor = Color.black; cam.orthographic = true; cam.cullingMask = 0;
            usingTouch = Application.isMobilePlatform;
            Camera.onPostRender += OnCamPost;                              // đường render mặc định (Built-in)
            RenderPipelineManager.endCameraRendering += OnSrpCamEnd;       // URP / HDRP
        }
        void OnDestroy() { Camera.onPostRender -= OnCamPost; RenderPipelineManager.endCameraRendering -= OnSrpCamEnd; }

        // ---------- màn hình: cao cố định 360 px logic, rộng 560–800 theo tỉ lệ máy; viền đen phần dư ----------
        Rect ViewRect()
        {
            float sw = Screen.width, sh = Screen.height;
            VW = Mathf.Round(Mathf.Clamp(Cfg.VH * sw / sh, 560, 800));
            float s = Mathf.Min(sw / VW, sh / Cfg.VH), w = VW * s, h = Cfg.VH * s;
            return new Rect((sw - w) / 2, (sh - h) / 2, w, h);
        }

        // ---------- điều khiển ----------
        void ReadInput()
        {
            for (int i = 0; i < 6; i++) now[i] = touchHeld[i];
#if ENABLE_INPUT_SYSTEM
            var kb = Keyboard.current;
            if (kb != null)
            {
                bool K(params UnityEngine.InputSystem.Key[] ks) { foreach (var k in ks) if (kb[k].isPressed) return true; return false; }
                now[0] |= K(UnityEngine.InputSystem.Key.LeftArrow, UnityEngine.InputSystem.Key.A);
                now[1] |= K(UnityEngine.InputSystem.Key.RightArrow, UnityEngine.InputSystem.Key.D);
                now[2] |= K(UnityEngine.InputSystem.Key.Z, UnityEngine.InputSystem.Key.K, UnityEngine.InputSystem.Key.Space, UnityEngine.InputSystem.Key.UpArrow, UnityEngine.InputSystem.Key.W);
                now[3] |= K(UnityEngine.InputSystem.Key.X, UnityEngine.InputSystem.Key.J);
                now[4] |= K(UnityEngine.InputSystem.Key.C, UnityEngine.InputSystem.Key.L, UnityEngine.InputSystem.Key.LeftShift, UnityEngine.InputSystem.Key.RightShift);
                now[5] |= K(UnityEngine.InputSystem.Key.Q, UnityEngine.InputSystem.Key.Tab);
                if (kb.rKey.wasPressedThisFrame && started) Flow.Respawn(g);
            }
            var gp = Gamepad.current;
            if (gp != null)
            {
                var st = gp.leftStick.ReadValue();
                now[0] |= st.x < -0.4f || gp.dpad.left.isPressed; now[1] |= st.x > 0.4f || gp.dpad.right.isPressed;
                now[2] |= gp.buttonSouth.isPressed; now[3] |= gp.buttonWest.isPressed || gp.buttonNorth.isPressed;
                now[4] |= gp.buttonEast.isPressed || gp.rightShoulder.isPressed || gp.rightTrigger.isPressed;
            }
            if (Touchscreen.current != null) usingTouch = true;
#else
            bool K(params KeyCode[] ks) { foreach (var k in ks) if (Input.GetKey(k)) return true; return false; }
            now[0] |= K(KeyCode.LeftArrow, KeyCode.A); now[1] |= K(KeyCode.RightArrow, KeyCode.D);
            now[2] |= K(KeyCode.Z, KeyCode.K, KeyCode.Space, KeyCode.UpArrow, KeyCode.W);
            now[3] |= K(KeyCode.X, KeyCode.J); now[4] |= K(KeyCode.C, KeyCode.L, KeyCode.LeftShift, KeyCode.RightShift);
            now[5] |= K(KeyCode.Q, KeyCode.Tab);
            if (Input.GetKeyDown(KeyCode.R) && started) Flow.Respawn(g);
            if (Input.touchSupported && Input.touchCount > 0) usingTouch = true;
#endif
        }

        // Nút cảm ứng (tọa độ màn hình thật, gốc trên-trái): trái, phải, lướt, chém/bắn, nhảy.
        Rect[] TouchRects()
        {
            float u = Mathf.Min(Screen.width, Screen.height) / 360f, sw = Screen.width, sh = Screen.height, m = sw * 0.03f;
            float b = 84 * u, j = 88 * u, d = 74 * u;
            return new[]
            {
                new Rect(m, sh - sh * 0.08f - b, b, b),                                   // trái
                new Rect(m + 96 * u, sh - sh * 0.08f - b, b, b),                          // phải
                new Rect(sw - m - j, sh - sh * 0.06f - j, j, j),                          // nhảy
                new Rect(sw - m - 98 * u - b, sh - sh * 0.20f - b, b, b),                 // chém / bắn
                new Rect(sw - m - 196 * u - d, sh - sh * 0.06f - d, d, d),                // lướt
            };
        }
        static readonly CoreKey[] TouchKeys = { CoreKey.Left, CoreKey.Right, CoreKey.Jump, CoreKey.Attack, CoreKey.Dash };
        void ReadTouches()
        {
            for (int i = 0; i < 6; i++) touchHeld[i] = false;
            if (!usingTouch || !started) return;
            var rects = TouchRects();
            void Hit(Vector2 pos, bool began)
            {
                var gp = new Vector2(pos.x, Screen.height - pos.y);   // đổi sang gốc trên-trái
                for (int r = 0; r < rects.Length; r++) if (rects[r].Contains(gp)) { touchHeld[(int)TouchKeys[r]] = true; if (began) tapped[(int)TouchKeys[r]] = true; }
            }
#if ENABLE_INPUT_SYSTEM
            if (Touchscreen.current != null)
                foreach (var t in Touchscreen.current.touches) if (t.isInProgress) Hit(t.position.ReadValue(), t.press.wasPressedThisFrame);
#else
            for (int i = 0; i < Input.touchCount; i++) { var t = Input.GetTouch(i); if (t.phase != TouchPhase.Ended && t.phase != TouchPhase.Canceled) Hit(t.position, t.phase == TouchPhase.Began); }
#endif
        }

        void Update()
        {
            var vr = ViewRect(); cam.pixelRect = vr;
            ReadTouches(); ReadInput();
            if (!started) return;
            g.input.Poll(now, tapped); System.Array.Clear(tapped, 0, 6);
            float dt = Mathf.Min(0.05f, Time.unscaledDeltaTime); acc += dt;
            while (acc >= Cfg.STEP) { acc -= Cfg.STEP; Flow.Step(g); }
            Flow.UpdateCamera(g, ref camX, ref camY, VW, dt, usingTouch);
            renderAlpha = g.hitstop > 0 ? 1 : acc / Cfg.STEP;
        }

        void OnCamPost(Camera c) { if (c == cam) Render(); }
        void OnSrpCamEnd(ScriptableRenderContext ctx, Camera c) { if (c == cam) Render(); }
        void Render() { if (g != null) view.Draw(g, VW, camX, camY, renderAlpha); }

        // ---------- chữ, menu, nút cảm ứng ----------
        void Styles()
        {
            if (hudStyle != null) return;
            hudStyle = new GUIStyle(GUI.skin.label) { fontSize = 11, richText = true, wordWrap = true }; hudStyle.normal.textColor = new Color(0.95f, 0.9f, 0.77f);
            talkStyle = new GUIStyle(hudStyle) { fontSize = 13 };
            bigStyle = new GUIStyle(GUI.skin.label) { fontSize = 22, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter }; bigStyle.normal.textColor = new Color(1, 0.83f, 0.3f);
            btnStyle = new GUIStyle(GUI.skin.button) { fontSize = 15, fontStyle = FontStyle.Bold, imagePosition = ImagePosition.ImageAbove };
        }

        void OnGUI()
        {
            Styles();
            var vr = ViewRect(); float s = vr.width / VW;
            var old = GUI.matrix;
            GUI.matrix = Matrix4x4.TRS(new Vector3(vr.x, vr.y, 0), Quaternion.identity, new Vector3(s, s, 1));   // vẽ theo tọa độ logic VW×360
            if (!started) { Menu(); GUI.matrix = old; return; }
            bool tv = g.hero == Hero.ThanhVu; var P = g.P;
            string ctl = usingTouch ? "" : $"\n<b>←→</b> chạy · <b>Z/K/Space</b> nhảy · <b>X/J</b> {(tv ? "bắn (giữ để nạp)" : "chém")} · <b>C/L/Shift</b> lướt · <b>Q/Tab</b> đổi nhân vật · <b>R</b> về điểm lưu";
            string lv = tv && P.aiming ? new[] { "", " · Nạp ●", " · Nạp ●● ĐẦY" }[PlayerSys.ChargeLv(P.charge)] : "";
            string combo = !tv && P.atk != null && !P.atkAir ? $" · Combo {P.combo + 1}/3" : "";
            GUI.Label(new Rect(10, 6, VW * 0.62f, 60), $"<color=#ffcf5a><b>Long Thần Chiến Ký</b></color> — Màn 1: {g.level.name} · <b>{(tv ? "Thanh Vũ" : "Xích Long")}</b>{ctl}\nTrúng: {g.hits} · Hạ: {g.kills}{combo}{lv}", hudStyle);
            if (GUI.Button(new Rect(VW / 2 - 70, 6, 140, 22), "⇄ Đổi sang " + (tv ? "Xích Long" : "Thanh Vũ"))) PlayerSys.SwapHero(g);
            var B = g.boss;
            if (B.st != BossSt.Wait && !(B.st == BossSt.Dead && B.t > 2.2f)) GUI.Label(new Rect(VW - 100, 58, 90, 16), "<b>THỦY TINH</b>", hudStyle);
            if (g.talk != null)
            {
                var t = g.talk; float a = Mathf.Clamp01(Mathf.Min(t.t * 4, (t.dur - t.t) * 4)); var c = GUI.color; GUI.color = new Color(1, 1, 1, a);
                GUI.Label(new Rect(54, Cfg.VH - 92, VW - 110, 18), $"<b><color={(t.who != null ? "#ffb0c8" : "#8ff0ff")}>{t.who ?? "Thủy Tinh"}</color></b>", talkStyle);
                GUI.Label(new Rect(54, Cfg.VH - 75, VW - 110, 40), string.Join("\n", t.lines), talkStyle);
                GUI.color = c;
            }
            if (B.st == BossSt.Dead && B.t > 2.4f)
            {
                GUI.Label(new Rect(0, Cfg.VH / 2 - 30, VW, 30), "HẠ THỦY TINH — QUA MÀN 1", bigStyle);
                GUI.Label(new Rect(VW / 2 - 140, Cfg.VH / 2 + 2, 280, 20), "Nhận được vũ khí Thủy Tiễn (sắp có)", talkStyle);
            }
            GUI.matrix = old;
            if (usingTouch) TouchButtons();
        }

        void Menu()
        {
            GUI.Box(new Rect(0, 0, VW, Cfg.VH), "");
            GUI.Label(new Rect(0, 40, VW, 30), "LONG THẦN CHIẾN KÝ", bigStyle);
            GUI.Label(new Rect(VW / 2 - 60, 74, 120, 20), "Chọn nhân vật", talkStyle);
            var xl = new GUIContent("XÍCH LONG\nKiếm sĩ · đao laze", view.Frame("xl_id1"));
            var tvc = new GUIContent("THANH VŨ\nXạ thủ · Thần Nỏ", view.Frame("tv_mau"));
            if (GUI.Button(new Rect(VW / 2 - 170, 110, 160, 150), xl, btnStyle)) StartAs(Hero.XichLong);
            if (GUI.Button(new Rect(VW / 2 + 10, 110, 160, 150), tvc, btnStyle)) StartAs(Hero.ThanhVu);
        }
        void StartAs(Hero h) { started = true; PlayerSys.SwapHero(g, h); }

        void TouchButtons()
        {
            var rects = TouchRects(); string[] labels = { "◀", "▶", "NHẢY", g.hero == Hero.ThanhVu ? "BẮN" : "CHÉM", "LƯỚT" };
            var c = GUI.color;
            for (int i = 0; i < rects.Length; i++) { GUI.color = new Color(1, 1, 1, touchHeld[(int)TouchKeys[i]] ? 0.9f : 0.5f); GUI.Box(rects[i], labels[i], btnStyle); }
            GUI.color = c;
        }
    }
}
