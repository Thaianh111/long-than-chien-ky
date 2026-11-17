import { KEYS, type Key } from './types';

type Flags = Record<Key, number>;
const zero = (): Flags => ({ left: 0, right: 0, jump: 0, attack: 0, dash: 0, swap: 0 });

/** Phím máy tính → nút trong game. */
export const KEYMAP: Record<string, Key> = {
  ArrowLeft: 'left', KeyA: 'left', ArrowRight: 'right', KeyD: 'right',
  KeyZ: 'jump', KeyK: 'jump', Space: 'jump', ArrowUp: 'jump', KeyW: 'jump',
  KeyX: 'attack', KeyJ: 'attack', KeyQ: 'swap', Tab: 'swap', KeyC: 'dash', KeyL: 'dash', ShiftLeft: 'dash', ShiftRight: 'dash',
};

/**
 * Gộp bàn phím, nút cảm ứng và tay cầm. Mỗi khung gọi poll(); held = đang giữ,
 * pressed/released = vừa bấm/vừa nhả (giữ tới khi vòng vật lý dùng xong rồi clearEdges()).
 */
export class Input {
  keys = zero(); touch = zero(); pad = zero(); tap = zero();
  held = zero(); prev = zero(); pressed: Record<Key, boolean> = { left: false, right: false, jump: false, attack: false, dash: false, swap: false };
  released: Record<Key, boolean> = { ...this.pressed };

  /** Đặt trạng thái một nút từ nguồn bất kỳ (bàn phím, cảm ứng). */
  set(src: 'keys' | 'touch', k: Key, on: boolean): void { this[src][k] = on ? 1 : 0; if (on) this.tap[k] = 1; }
  readGamepad(): void {
    const g = typeof navigator !== 'undefined' && navigator.getGamepads ? [...navigator.getGamepads()].find(Boolean) : null;
    this.pad = zero(); if (!g) return;
    const ax = g.axes[0] || 0, b = (i: number) => !!(g.buttons[i] && g.buttons[i].pressed);
    this.pad.left = ax < -0.4 || b(14) ? 1 : 0; this.pad.right = ax > 0.4 || b(15) ? 1 : 0;
    this.pad.jump = b(0) ? 1 : 0; this.pad.attack = b(2) || b(3) ? 1 : 0; this.pad.dash = b(1) || b(5) || b(7) ? 1 : 0;
  }
  poll(): void {
    this.readGamepad();
    for (const k of KEYS) {
      this.held[k] = this.keys[k] || this.touch[k] || this.pad[k] ? 1 : 0;
      this.pressed[k] = this.pressed[k] || !!this.tap[k] || (!!this.held[k] && !this.prev[k]); this.tap[k] = 0;
      this.released[k] = this.released[k] || (!this.held[k] && !!this.prev[k]); this.prev[k] = this.held[k];
    }
  }
  clearEdges(): void { for (const k of KEYS) { this.pressed[k] = false; this.released[k] = false; } }
}
