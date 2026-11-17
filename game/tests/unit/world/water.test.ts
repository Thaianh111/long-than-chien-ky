import { describe, expect, it } from 'vitest';
import { deepAt, waterY } from '../../../src/world/water';
import { makeGame } from '../helpers';

describe('Nước', () => {
  it('chỗ không có nước trả về Infinity', () => {
    expect(waterY(makeGame(), 5 * 32)).toBe(Infinity);
  });
  it('vũng nông phố ngập chỉ làm chậm, hố quảng trường là nước sâu', () => {
    const g = makeGame();
    expect(deepAt(g, 16 * 32)).toBe(false);
    expect(deepAt(g, 47 * 32)).toBe(true);
    expect(waterY(g, 47 * 32)).toBe(12 * 32);
  });
  it('nước trong đấu trường lấy mức cao hơn giữa nước pha 2 và nước tím', () => {
    const g = makeGame(), A = g.level.arena; g.arenaWater = 64; g.arenaHaz = 176;
    expect(waterY(g, (A.x0 + A.x1) / 2)).toBe(A.floorY - 176);
  });
});
