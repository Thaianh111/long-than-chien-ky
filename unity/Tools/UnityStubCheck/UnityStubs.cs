// Lớp UnityEngine GIẢ (chỉ chữ ký hàm) để biên dịch thử. KHÔNG dùng trong Unity.
using System;
namespace UnityEngine
{
    public class Object { public string name; public HideFlags hideFlags; public static implicit operator bool(Object o) => o != null; public static T FindAnyObjectByType<T>() where T : Object => null; public static void DontDestroyOnLoad(Object o) { } }
    public enum HideFlags { None, HideAndDontSave }
    public class Component : Object { public GameObject gameObject; public T AddComponent<T>() where T : Component => default; }
    public class Behaviour : Component { }
    public class MonoBehaviour : Behaviour { }
    public class GameObject : Object { public string tag; public GameObject(string n) { } public T AddComponent<T>() where T : Component => default; }
    public enum CameraClearFlags { SolidColor }
    public class Camera : Behaviour { public static Camera main; public CameraClearFlags clearFlags; public Color backgroundColor; public bool orthographic; public int cullingMask; public Rect pixelRect; public static event Action<Camera> onPostRender; static void X() { onPostRender?.Invoke(null); } }
    public struct Color { public float r, g, b, a; public Color(float r, float g, float b, float a = 1) { this.r = r; this.g = g; this.b = b; this.a = a; } public static Color white, black; }
    public static class ColorUtility { public static bool TryParseHtmlString(string s, out Color c) { c = default; return true; } }
    public struct Vector2 { public float x, y; public Vector2(float x, float y) { this.x = x; this.y = y; } public static Vector2 operator *(float k, Vector2 v) => v; public static Vector2 operator +(Vector2 a, Vector2 b) => a; }
    public struct Vector3 { public float x, y, z; public Vector3(float x, float y, float z) { this.x = x; this.y = y; this.z = z; } }
    public struct Quaternion { public static Quaternion identity; }
    public struct Matrix4x4 { public static Matrix4x4 TRS(Vector3 p, Quaternion q, Vector3 s) => default; }
    public struct Rect { public float x, y, width, height, xMin, xMax, yMin, yMax; public Rect(float x, float y, float w, float h) { this.x = x; this.y = y; width = w; height = h; xMin = xMax = yMin = yMax = 0; } public bool Contains(Vector2 p) => true; }
    public class Texture : Object { public int width, height; }
    public class Texture2D : Texture { public static Texture2D whiteTexture; }
    public class TextAsset : Object { public string text; }
    public static class Resources { public static T Load<T>(string p) where T : Object => null; }
    public class Shader : Object { public static Shader Find(string n) => null; }
    public class Material : Object { public Material(Shader s) { } public Texture mainTexture; public void SetFloat(string n, float v) { } public bool SetPass(int p) => true; }
    public static class GL { public const int QUADS = 7, TRIANGLES = 4; public static void PushMatrix() { } public static void PopMatrix() { } public static void LoadPixelMatrix(float l, float r, float b, float t) { } public static void Begin(int m) { } public static void End() { } public static void Color(Color c) { } public static void TexCoord2(float x, float y) { } public static void Vertex(Vector3 v) { } }
    public static class Mathf { public const float PI = 3.14159f; public static float Sin(float f) => 0; public static float Cos(float f) => 0; public static float Pow(float a, float b) => 0; public static float Exp(float a) => 0; public static float Abs(float a) => 0; public static float Sign(float a) => 0; public static float Min(float a, float b) => 0; public static float Max(float a, float b) => 0; public static int Max(int a, int b) => 0; public static int Min(int a, int b) => 0; public static float Clamp(float v, float a, float b) => 0; public static float Clamp01(float v) => 0; public static float Round(float v) => 0; public static float Lerp(float a, float b, float t) => 0; public static int FloorToInt(float f) => 0; public static int CeilToInt(float f) => 0; }
    public static class Random { public static float value; public static int Range(int a, int b) => 0; }
    public static class Time { public static float time, unscaledDeltaTime; }
    public static class Application { public static int targetFrameRate; public static bool isMobilePlatform; }
    public static class QualitySettings { public static int vSyncCount; }
    public static class Screen { public static int width, height; }
    public class AudioSource : Behaviour { public bool playOnAwake; public void PlayOneShot(AudioClip c, float v = 1) { } }
    public class AudioClip : Object { public int samples; public static AudioClip Create(string n, int len, int ch, int f, bool s) => null; public bool SetData(float[] d, int o) => true; public bool GetData(float[] d, int o) => true; }
    public enum FontStyle { Bold } public enum TextAnchor { MiddleCenter } public enum ImagePosition { ImageAbove }
    public class GUIStyleState { public Color textColor; }
    public class GUIStyle { public GUIStyle(GUIStyle o) { } public int fontSize; public bool richText, wordWrap; public FontStyle fontStyle; public TextAnchor alignment; public ImagePosition imagePosition; public GUIStyleState normal = new GUIStyleState(); }
    public class GUISkin { public GUIStyle label, button; }
    public class GUIContent { public GUIContent(string t, Texture i) { } }
    public static class GUI { public static GUISkin skin; public static Matrix4x4 matrix; public static Color color; public static void Label(Rect r, string t, GUIStyle s) { } public static bool Button(Rect r, string t) => false; public static bool Button(Rect r, GUIContent c, GUIStyle s) => false; public static void Box(Rect r, string t) { } public static void Box(Rect r, string t, GUIStyle s) { } }
    public enum RuntimeInitializeLoadType { AfterSceneLoad }
    public class RuntimeInitializeOnLoadMethodAttribute : Attribute { public RuntimeInitializeOnLoadMethodAttribute(RuntimeInitializeLoadType t) { } }
    public enum KeyCode { LeftArrow, A, RightArrow, D, Z, K, Space, UpArrow, W, X, J, C, L, LeftShift, RightShift, Q, Tab, R }
    public enum TouchPhase { Began, Ended, Canceled }
    public struct Touch { public Vector2 position; public TouchPhase phase; }
    public static class Input { public static bool GetKey(KeyCode k) => false; public static bool GetKeyDown(KeyCode k) => false; public static bool touchSupported; public static int touchCount; public static Touch GetTouch(int i) => default; }
}
namespace UnityEngine.Rendering
{
    public enum BlendMode { One = 1, SrcAlpha = 5, OneMinusSrcAlpha = 10 }
    public struct ScriptableRenderContext { }
    public static class RenderPipelineManager { public static event Action<ScriptableRenderContext, UnityEngine.Camera> endCameraRendering; static void X() { endCameraRendering?.Invoke(default, null); } }
}
