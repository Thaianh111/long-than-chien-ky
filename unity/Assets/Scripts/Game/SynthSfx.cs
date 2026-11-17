// Âm thanh tạo bằng code (giống bản web dùng WebAudio): tạo sẵn các đoạn âm lúc khởi động rồi phát.
using System.Collections.Generic;
using LongThan.Core;
using UnityEngine;

namespace LongThan.Game
{
    public class SynthSfx : ISfx
    {
        const int SR = 44100;
        readonly AudioSource src;
        readonly Dictionary<string, AudioClip> clips = new Dictionary<string, AudioClip>();
        readonly Dictionary<string, List<AudioClip>> voices = new Dictionary<string, List<AudioClip>>();
        float lastShout;
        public bool muted;

        public SynthSfx(GameObject host)
        {
            src = host.AddComponent<AudioSource>(); src.playOnAwake = false;
            clips["swing1"] = Swish(1f); clips["swing115"] = Swish(1.15f); clips["swing14"] = Swish(1.4f);
            clips["hit0"] = HitClip(false); clips["hit1"] = HitClip(true);
            for (int lv = 0; lv < 3; lv++) clips["shoot" + lv] = Tone(lv == 2 ? 2 : 1, lv == 2 ? 900 : 1400, lv == 2 ? 120 : 300, 0.15f + lv * 0.08f, 0.12f + lv * 0.08f);
            clips["charge1"] = Tone(0, 520, 880, 0.2f, 0.1f); clips["charge2"] = Tone(0, 880, 1760, 0.2f, 0.1f);
            clips["warp"] = Tone(1, 1800, 220, Cfg.T.WARP_FALL + 0.02f, 0.08f);
            clips["land"] = Mix(Tone(3, 160, 50, 0.2f, 0.3f), (Tone(0, 880, 880, 0.2f, 0.12f), 0.08f), (Tone(0, 1320, 1320, 0.2f, 0.12f), 0.16f));
            clips["jump2"] = Tone(3, 300, 900, 0.14f, 0.15f);
            if (Cfg.T.VOICE > 0)   // tiếng hô thu sẵn: Resources/Voice/xl_0.mp3, xl_0b.mp3...
                foreach (var k in new[] { "xl_0", "xl_1", "xl_2", "xl_air", "tv_0", "tv_1", "tv_2" })
                    foreach (var v in new[] { "", "b", "c" }) { var c = Resources.Load<AudioClip>("Voice/" + k + v); if (c) { if (!voices.ContainsKey(k)) voices[k] = new List<AudioClip>(); voices[k].Add(c); } }
        }

        // kiểu sóng: 0 sin, 1 vuông, 2 răng cưa, 3 tam giác. Tần số và âm lượng giảm theo hàm mũ như WebAudio exponentialRamp.
        static float Wave(int type, float ph) => type == 0 ? Mathf.Sin(ph) : type == 1 ? Mathf.Sign(Mathf.Sin(ph)) : type == 2 ? 2 * (ph / (2 * Mathf.PI) % 1) - 1 : 2 * Mathf.Abs(2 * (ph / (2 * Mathf.PI) % 1) - 1) - 1;
        static float[] ToneData(int type, float f0, float f1, float dur, float gain)
        {
            int n = (int)(SR * (dur + 0.02f)); var d = new float[n]; float ph = 0;
            for (int i = 0; i < n; i++)
            {
                float t = i / (float)SR, k = Mathf.Clamp01(t / (dur * 0.85f));
                float f = f0 * Mathf.Pow(f1 / f0, k); ph += 2 * Mathf.PI * f / SR;
                float g = gain * Mathf.Pow(0.0001f / gain, Mathf.Clamp01(t / dur));
                d[i] = Wave(type, ph) * g;
            }
            return d;
        }
        static AudioClip Clip(string name, float[] d) { var c = AudioClip.Create(name, d.Length, 1, SR, false); c.SetData(d, 0); return c; }
        static AudioClip Tone(int type, float f0, float f1, float dur, float gain) => Clip("tone", ToneData(type, f0, f1, dur, gain));
        static AudioClip Mix(AudioClip a, params (AudioClip c, float delay)[] more)
        {
            var lists = new List<(float[] d, int off)>(); var da = new float[a.samples]; a.GetData(da, 0); lists.Add((da, 0));
            foreach (var (c, dl) in more) { var d = new float[c.samples]; c.GetData(d, 0); lists.Add((d, (int)(dl * SR))); }
            int n = 0; foreach (var (d, off) in lists) n = Mathf.Max(n, d.Length + off);
            var o = new float[n]; foreach (var (d, off) in lists) for (int i = 0; i < d.Length; i++) o[i + off] += d[i];
            return Clip("mix", o);
        }
        /// <summary>Tiếng vút: tiếng xì lọc dải quét từ cao xuống.</summary>
        static AudioClip Swish(float p)
        {
            int n = (int)(SR * 0.2f * p); var d = new float[n]; float lp = 0, hp = 0, prev = 0; var rnd = new System.Random(7);
            for (int i = 0; i < n; i++)
            {
                float t = i / (float)SR, f = Mathf.Lerp(3200 * p, 700, Mathf.Clamp01(t / (0.13f * p)));
                float a = 1 - Mathf.Exp(-2 * Mathf.PI * f / SR), x = (float)rnd.NextDouble() * 2 - 1;
                lp += a * (x - lp); hp = lp - prev; prev = lp;   // lọc dải thô: thông thấp rồi thông cao
                float env = t < 0.02f ? t / 0.02f : Mathf.Exp(-(t - 0.02f) * 30 / p);
                d[i] = hp * 2.5f * 0.32f * p * env;
            }
            return Clip("swish", d);
        }
        /// <summary>Tiếng trúng: nổ trầm + tiếng xoẹt cao.</summary>
        static AudioClip HitClip(bool big)
        {
            var a = ToneData(1, big ? 140 : 190, 40, big ? 0.25f : 0.14f, big ? 0.35f : 0.25f); var rnd = new System.Random(3); float prev = 0;
            for (int i = 0; i < a.Length && i < SR * 0.09f; i++) { float x = (float)rnd.NextDouble() * 2 - 1, h = x - prev; prev = x; a[i] += h * 0.3f * Mathf.Exp(-i / (SR * 0.02f)); }
            return Clip("hit", a);
        }

        void Play(string k) { if (!muted && clips.TryGetValue(k, out var c)) src.PlayOneShot(c); }
        public void Swing(float p) => Play(p > 1.3f ? "swing14" : p > 1.1f ? "swing115" : "swing1");
        public void Hit(bool big) => Play(big ? "hit1" : "hit0");
        public void Shoot(int lv) => Play("shoot" + lv);
        public void Charge(int lv) => Play("charge" + lv);
        public void Warp() => Play("warp");
        public void Land() => Play("land");
        public void Jump2() => Play("jump2");
        public void Shout(string key)
        {
            if (muted || Cfg.T.VOICE <= 0 || !voices.TryGetValue(key, out var l) || l.Count == 0) return;
            if (key == "tv_0" && Time.time - lastShout < 0.15f) return; lastShout = Time.time;
            src.PlayOneShot(l[Random.Range(0, l.Count)], Cfg.T.VOICE);
        }
    }
}
