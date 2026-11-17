// Bút vẽ 2D kiểu Canvas (giống bản web): tọa độ pixel logic, gốc trên-trái, y hướng xuống.
// Vẽ tức thời bằng GL trong lúc camera render, nên thứ tự vẽ = thứ tự gọi hàm.
using System.Collections.Generic;
using UnityEngine;

namespace LongThan.Game
{
    public class Painter
    {
        struct M { public float a, b, c, d, e, f; }   // ma trận affine 2D: x' = a*x + c*y + e, y' = b*x + d*y + f
        M m = new M { a = 1, d = 1 };
        readonly Stack<(M, float, bool)> stack = new Stack<(M, float, bool)>();
        public float alpha = 1;
        public bool additive;

        readonly Material normal, add, silNormal, silAdd;
        Material curMat; Texture curTex; int curMode = -1;   // GL.QUADS hoặc GL.TRIANGLES
        static readonly Dictionary<string, Color> colors = new Dictionary<string, Color>();

        public Painter()
        {
            var sh = Shader.Find("LongThan/Painter");
            Material Make(bool addBlend, bool sil)
            {
                var mat = new Material(sh) { hideFlags = HideFlags.HideAndDontSave };
                mat.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
                mat.SetFloat("_DstBlend", (float)(addBlend ? UnityEngine.Rendering.BlendMode.One : UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha));
                mat.SetFloat("_Silhouette", sil ? 1 : 0);
                return mat;
            }
            normal = Make(false, false); add = Make(true, false); silNormal = Make(false, true); silAdd = Make(true, true);
        }

        /// <summary>Bắt đầu khung vẽ: màn logic rộng vw × cao vh.</summary>
        public void Begin(float vw, float vh)
        {
            GL.PushMatrix(); GL.LoadPixelMatrix(0, vw, vh, 0);
            m = new M { a = 1, d = 1 }; stack.Clear(); alpha = 1; additive = false; curMat = null; curTex = null; curMode = -1;
        }
        public void End() { Flush(); GL.PopMatrix(); }

        // ---------- biến đổi ----------
        public void Save() => stack.Push((m, alpha, additive));
        public void Restore() { if (stack.Count > 0) { var s = stack.Pop(); m = s.Item1; alpha = s.Item2; additive = s.Item3; } }
        public void Translate(float x, float y) { m.e += m.a * x + m.c * y; m.f += m.b * x + m.d * y; }
        public void Scale(float sx, float sy) { m.a *= sx; m.b *= sx; m.c *= sy; m.d *= sy; }
        public void Rotate(float r)
        {
            float cs = Mathf.Cos(r), sn = Mathf.Sin(r);
            var n = m; n.a = m.a * cs + m.c * sn; n.b = m.b * cs + m.d * sn; n.c = m.c * cs - m.a * sn; n.d = m.d * cs - m.b * sn; m = n;
        }
        Vector3 P(float x, float y) => new Vector3(m.a * x + m.c * y + m.e, m.b * x + m.d * y + m.f, 0);

        // ---------- màu ----------
        /// <summary>Đọc màu kiểu web: "#rrggbb" hoặc "rgba(r,g,b,a)".</summary>
        public static Color Col(string s)
        {
            if (s == null) return Color.white;
            if (colors.TryGetValue(s, out var c)) return c;
            if (s.StartsWith("#")) ColorUtility.TryParseHtmlString(s, out c);
            else if (s.StartsWith("rgba"))
            {
                var p = s.Substring(5, s.Length - 6).Split(',');
                var ic = System.Globalization.CultureInfo.InvariantCulture;
                c = new Color(float.Parse(p[0], ic) / 255f, float.Parse(p[1], ic) / 255f, float.Parse(p[2], ic) / 255f, float.Parse(p[3], ic));
            }
            colors[s] = c; return c;
        }
        Color Mul(Color c) { c.a *= Mathf.Clamp01(alpha); return c; }

        // ---------- gom lệnh GL ----------
        void Use(Texture tex, bool silhouette, int mode)
        {
            var mat = silhouette ? (additive ? silAdd : silNormal) : (additive ? add : normal);
            tex = tex ? tex : Texture2D.whiteTexture;
            if (mat == curMat && tex == curTex && mode == curMode) return;
            Flush();
            mat.mainTexture = tex; mat.SetPass(0); GL.Begin(mode);
            curMat = mat; curTex = tex; curMode = mode;
        }
        void Flush() { if (curMode >= 0) GL.End(); curMode = -1; curMat = null; curTex = null; }

        void Quad(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, Color c, Rect uv)
        {
            GL.Color(c);
            GL.TexCoord2(uv.xMin, uv.yMax); GL.Vertex(p0);
            GL.TexCoord2(uv.xMax, uv.yMax); GL.Vertex(p1);
            GL.TexCoord2(uv.xMax, uv.yMin); GL.Vertex(p2);
            GL.TexCoord2(uv.xMin, uv.yMin); GL.Vertex(p3);
        }

        // ---------- hình ----------
        public void FillRect(float x, float y, float w, float h, Color c)
        {
            Use(null, false, GL.QUADS); Quad(P(x, y), P(x + w, y), P(x + w, y + h), P(x, y + h), Mul(c), new Rect(0, 0, 1, 1));
        }
        public void FillRect(float x, float y, float w, float h, string c) => FillRect(x, y, w, h, Col(c));

        /// <summary>Tô đa giác lồi (quạt tam giác).</summary>
        public void FillPoly(IList<Vector2> pts, Color c)
        {
            if (pts.Count < 3) return;
            Use(null, false, GL.TRIANGLES); GL.Color(Mul(c));
            var p0 = P(pts[0].x, pts[0].y);
            for (int i = 1; i < pts.Count - 1; i++) { GL.Vertex(p0); GL.Vertex(P(pts[i].x, pts[i].y)); GL.Vertex(P(pts[i + 1].x, pts[i + 1].y)); }
        }
        /// <summary>Tô dải giữa hai đường (ngoài và trong) có cùng số điểm — dùng cho vệt chém lưỡi liềm.</summary>
        public void FillStrip(IList<Vector2> outer, IList<Vector2> inner, Color c)
        {
            Use(null, false, GL.TRIANGLES); GL.Color(Mul(c));
            for (int i = 0; i < outer.Count - 1; i++)
            {
                var a = P(outer[i].x, outer[i].y); var b = P(outer[i + 1].x, outer[i + 1].y); var d = P(inner[i].x, inner[i].y); var e = P(inner[i + 1].x, inner[i + 1].y);
                GL.Vertex(a); GL.Vertex(b); GL.Vertex(e); GL.Vertex(a); GL.Vertex(e); GL.Vertex(d);
            }
        }
        public void FillCircle(float cx, float cy, float r, Color c, int seg = 20)
        {
            Use(null, false, GL.TRIANGLES); GL.Color(Mul(c)); var o = P(cx, cy);
            for (int i = 0; i < seg; i++)
            {
                float a0 = i * Mathf.PI * 2 / seg, a1 = (i + 1) * Mathf.PI * 2 / seg;
                GL.Vertex(o); GL.Vertex(P(cx + Mathf.Cos(a0) * r, cy + Mathf.Sin(a0) * r)); GL.Vertex(P(cx + Mathf.Cos(a1) * r, cy + Mathf.Sin(a1) * r));
            }
        }
        /// <summary>Viền elip (rx = ry là đường tròn).</summary>
        public void StrokeEllipse(float cx, float cy, float rx, float ry, float width, Color c, int seg = 28)
        {
            Use(null, false, GL.QUADS); c = Mul(c);
            for (int i = 0; i < seg; i++)
            {
                float a0 = i * Mathf.PI * 2 / seg, a1 = (i + 1) * Mathf.PI * 2 / seg, h = width / 2;
                Quad(P(cx + Mathf.Cos(a0) * (rx - h), cy + Mathf.Sin(a0) * (ry - h)), P(cx + Mathf.Cos(a1) * (rx - h), cy + Mathf.Sin(a1) * (ry - h)),
                     P(cx + Mathf.Cos(a1) * (rx + h), cy + Mathf.Sin(a1) * (ry + h)), P(cx + Mathf.Cos(a0) * (rx + h), cy + Mathf.Sin(a0) * (ry + h)), c, new Rect(0, 0, 1, 1));
            }
        }
        public void StrokeRect(float x, float y, float w, float h, float lw, Color c)
        {
            FillRect(x - lw / 2, y - lw / 2, w + lw, lw, c); FillRect(x - lw / 2, y + h - lw / 2, w + lw, lw, c);
            FillRect(x - lw / 2, y, lw, h, c); FillRect(x + w - lw / 2, y, lw, h, c);
        }
        public void GradientRect(float x, float y, float w, float h, Color top, Color bottom)
        {
            Use(null, false, GL.QUADS);
            GL.TexCoord2(0, 0); GL.Color(Mul(top)); GL.Vertex(P(x, y)); GL.Vertex(P(x + w, y));
            GL.Color(Mul(bottom)); GL.Vertex(P(x + w, y + h)); GL.Vertex(P(x, y + h));
        }

        // ---------- ảnh ----------
        /// <summary>Vẽ ảnh tại (dx, dy) cỡ gốc (hoặc dw×dh). silhouette = tô cả hình bằng màu tint.</summary>
        public void DrawImage(Texture2D tex, float dx, float dy, float dw = -1, float dh = -1, Color? tint = null, bool silhouette = false)
        {
            if (!tex) return;
            if (dw < 0) { dw = tex.width; dh = tex.height; }
            Use(tex, silhouette, GL.QUADS);
            Quad(P(dx, dy), P(dx + dw, dy), P(dx + dw, dy + dh), P(dx, dy + dh), Mul(tint ?? Color.white), new Rect(0, 0, 1, 1));
        }
        /// <summary>Vẽ một phần ảnh: vùng nguồn (sx, sy, sw, sh) tính bằng pixel ảnh, gốc trên-trái.</summary>
        public void DrawImagePart(Texture2D tex, float sx, float sy, float sw, float sh, float dx, float dy, float dw, float dh, Color? tint = null, bool silhouette = false)
        {
            if (!tex) return;
            Use(tex, silhouette, GL.QUADS);
            var uv = new Rect(sx / tex.width, 1 - (sy + sh) / tex.height, sw / tex.width, sh / tex.height);
            Quad(P(dx, dy), P(dx + dw, dy), P(dx + dw, dy + dh), P(dx, dy + dh), Mul(tint ?? Color.white), uv);
        }
    }
}
