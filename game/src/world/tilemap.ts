import type { Body } from '../core/types';

/** Bản đồ ô gạch: 1 = gạch đặc, 0 = trống. Mỗi ô TS×TS px. */
export class TileMap {
  readonly cells: Uint8Array[];
  constructor(readonly w: number, readonly h: number, readonly ts = 32) {
    this.cells = Array.from({ length: h }, () => new Uint8Array(w));
  }
  /** Tô một khối ô (tính theo ô) bằng giá trị v. */
  rect(x: number, y: number, w: number, h: number, v = 1): void {
    for (let j = y; j < y + h; j++) for (let i = x; i < x + w; i++)
      if (i >= 0 && j >= 0 && i < this.w && j < this.h) this.cells[j][i] = v;
  }
  /** Ô tại (i, j) có phải gạch đặc không. */
  solidCell(i: number, j: number): boolean {
    return i >= 0 && i < this.w && j >= 0 && j < this.h && this.cells[j][i] === 1;
  }
  /** Điểm (px, py) theo pixel có nằm trong gạch đặc không. */
  solidAt(px: number, py: number): boolean {
    return this.solidCell(Math.floor(px / this.ts), Math.floor(py / this.ts));
  }
  /** Đẩy vật ra khỏi tường theo trục ngang. Trả về -1/1 nếu đụng tường trái/phải, 0 nếu không. */
  collideX(o: Body): number {
    const TS = this.ts, top = o.y + 1, bot = o.y + o.h - 1;
    if (o.vx > 0) {
      const r = o.x + o.w;
      for (let y = top; y <= bot; y += 12) if (this.solidAt(r, y) || this.solidAt(r, bot)) { o.x = Math.floor(r / TS) * TS - o.w - 0.01; o.vx = 0; return 1; }
    } else if (o.vx < 0) {
      for (let y = top; y <= bot; y += 12) if (this.solidAt(o.x, y) || this.solidAt(o.x, bot)) { o.x = (Math.floor(o.x / TS) + 1) * TS + 0.01; o.vx = 0; return -1; }
    }
    return 0;
  }
  /** Đẩy vật ra khỏi sàn/trần theo trục dọc, đặt o.ground khi đứng trên gạch. */
  collideY(o: Body): void {
    const TS = this.ts, l = o.x + 1, r = o.x + o.w - 1;
    o.ground = false;
    if (o.vy > 0) {
      const b = o.y + o.h;
      if (this.solidAt(l, b) || this.solidAt(r, b)) { o.y = Math.floor(b / TS) * TS - o.h; o.vy = 0; o.ground = true; }
    } else if (o.vy < 0) {
      if (this.solidAt(l, o.y) || this.solidAt(r, o.y)) { o.y = (Math.floor(o.y / TS) + 1) * TS; o.vy = 0; }
    }
  }
}
