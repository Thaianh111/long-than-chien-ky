/** Âm thanh tạo bằng WebAudio (không cần file) + tiếng hô thu sẵn trong assets/voice. */
import { T } from './config';
import type { Sfx } from './game';

const VOICE_KEYS = ['xl_0', 'xl_1', 'xl_2', 'xl_air', 'tv_0', 'tv_1', 'tv_2'], VOICE_VARIANTS = ['', 'b', 'c'];

export class WebAudioSfx implements Sfx {
  private ac: AudioContext | null = null;
  private noise: AudioBuffer | null = null;
  private voices: Record<string, AudioBuffer[]> = {};
  private voiceLoading = false;
  private lastShout = 0;

  /** Gọi trong sự kiện chạm/bấm phím đầu tiên (trình duyệt chỉ cho mở âm thanh sau thao tác người dùng). */
  unlock(): void {
    if (!this.ac) { try { this.ac = new AudioContext(); } catch { return; } }
    if (this.ac.state === 'suspended') void this.ac.resume();
    if (T.VOICE) this.loadVoices();
  }
  private noiseBuf(ac: AudioContext): AudioBuffer {
    if (this.noise) return this.noise;
    const b = ac.createBuffer(1, ac.sampleRate * 0.4, ac.sampleRate), d = b.getChannelData(0);
    for (let i = 0; i < d.length; i++) d[i] = Math.random() * 2 - 1;
    return (this.noise = b);
  }
  private loadVoices(): void {
    const ac = this.ac; if (this.voiceLoading || !ac) return; this.voiceLoading = true;
    fetch('assets/voice/list.json').then(r => (r.ok ? r.json() : [])).then((files: string[]) => files.forEach(f => {
      const k = VOICE_KEYS.find(k => VOICE_VARIANTS.some(v => f === k + v + '.mp3')); if (!k) return;
      fetch('assets/voice/' + f).then(r => r.arrayBuffer()).then(ab => ac.decodeAudioData(ab)).then(buf => (this.voices[k] ||= []).push(buf)).catch(() => {});
    })).catch(() => {});
  }
  private tone(type: OscillatorType, f0: number, f1: number, dur: number, gain: number, delay = 0): void {
    const ac = this.ac; if (!ac) return; const t = ac.currentTime + delay, o = ac.createOscillator(), g = ac.createGain();
    o.type = type; o.frequency.setValueAtTime(f0, t); o.frequency.exponentialRampToValueAtTime(f1, t + dur * 0.85);
    g.gain.setValueAtTime(gain, t); g.gain.exponentialRampToValueAtTime(0.0001, t + dur); o.connect(g).connect(ac.destination); o.start(t); o.stop(t + dur + 0.02);
  }
  /** Tiếng vút: tiếng xì lọc dải quét từ cao xuống. */
  swing(p: number): void {
    const ac = this.ac; if (!ac) return; const t = ac.currentTime, n = ac.createBufferSource(), f = ac.createBiquadFilter(), g = ac.createGain();
    n.buffer = this.noiseBuf(ac); f.type = 'bandpass'; f.Q.value = 1.4; f.frequency.setValueAtTime(3200 * p, t); f.frequency.exponentialRampToValueAtTime(700, t + 0.13 * p);
    g.gain.setValueAtTime(0.0001, t); g.gain.exponentialRampToValueAtTime(0.32 * p, t + 0.02); g.gain.exponentialRampToValueAtTime(0.0001, t + 0.16 * p);
    n.connect(f).connect(g).connect(ac.destination); n.start(t); n.stop(t + 0.3);
  }
  /** Tiếng trúng: nổ trầm + tiếng xoẹt cao. */
  hit(big: boolean): void {
    const ac = this.ac; if (!ac) return; const t = ac.currentTime;
    const o = ac.createOscillator(), g = ac.createGain(); o.type = 'square';
    o.frequency.setValueAtTime(big ? 140 : 190, t); o.frequency.exponentialRampToValueAtTime(40, t + (big ? 0.22 : 0.12));
    g.gain.setValueAtTime(big ? 0.35 : 0.25, t); g.gain.exponentialRampToValueAtTime(0.0001, t + (big ? 0.25 : 0.14));
    o.connect(g).connect(ac.destination); o.start(t); o.stop(t + 0.3);
    const n = ac.createBufferSource(), f = ac.createBiquadFilter(), g2 = ac.createGain();
    n.buffer = this.noiseBuf(ac); f.type = 'highpass'; f.frequency.value = 2500; g2.gain.setValueAtTime(0.3, t); g2.gain.exponentialRampToValueAtTime(0.0001, t + 0.09);
    n.connect(f).connect(g2).connect(ac.destination); n.start(t); n.stop(t + 0.12);
  }
  shoot(lv: number): void { this.tone(lv === 2 ? 'sawtooth' : 'square', lv === 2 ? 900 : 1400, lv === 2 ? 120 : 300, 0.15 + lv * 0.08, 0.12 + lv * 0.08); }
  charge(lv: number): void { this.tone('sine', lv === 2 ? 880 : 520, lv === 2 ? 1760 : 880, 0.2, 0.1); }
  warp(): void { this.tone('square', 1800, 220, T.WARP_FALL + 0.02, 0.08); }
  land(): void { this.tone('triangle', 160, 50, 0.2, 0.3); this.tone('sine', 880, 880, 0.2, 0.12, 0.08); this.tone('sine', 1320, 1320, 0.2, 0.12, 0.16); }
  jump2(): void { this.tone('triangle', 300, 900, 0.14, 0.15); }
  shout(key: string): void {
    const ac = this.ac; if (!ac || !T.VOICE) return;
    const list = this.voices[key]; if (!list || !list.length) return;
    const t = ac.currentTime; if (key === 'tv_0' && t - this.lastShout < 0.15) return; this.lastShout = t;
    const src = ac.createBufferSource(), g = ac.createGain(); src.buffer = list[Math.floor(Math.random() * list.length)];
    g.gain.value = T.VOICE; src.connect(g).connect(ac.destination); src.start(t);
  }
}
