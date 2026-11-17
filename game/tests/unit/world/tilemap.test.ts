import { describe, expect, it } from 'vitest';
import { TileMap } from '../../../src/world/tilemap';

describe('TileMap', () => {
  it('vật rơi xuống được đặt đứng trên mặt gạch', () => {
    const m = new TileMap(10, 10, 32); m.rect(0, 8, 10, 2);
    const o = { x: 40, y: 200, w: 20, h: 40, vx: 0, vy: 500, ground: false };
    o.y += 30; m.collideY(o);   // chân lún vào hàng gạch 8
    expect(o.ground).toBe(true);
    expect(o.y + o.h).toBe(8 * 32);
    expect(o.vy).toBe(0);
  });
  it('đụng tường bên phải thì bị đẩy ra và trả về 1', () => {
    const m = new TileMap(10, 10, 32); m.rect(5, 0, 1, 10);
    const o = { x: 150, y: 50, w: 20, h: 40, vx: 300, vy: 0 };
    expect(m.collideX(o)).toBe(1);
    expect(o.x + o.w).toBeLessThanOrEqual(5 * 32);
  });
  it('giá trị 0 khoét trống ô đã tô', () => {
    const m = new TileMap(4, 4); m.rect(0, 0, 4, 4); m.rect(1, 1, 2, 1, 0);
    expect(m.solidCell(1, 1)).toBe(false);
    expect(m.solidCell(0, 1)).toBe(true);
  });
});
