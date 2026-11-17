import { describe, expect, it } from 'vitest';
import { pillarTop, updatePillars } from '../../../src/world/pillars';
import { T } from '../../../src/core/config';
import { makeGame, placePlayer } from '../helpers';

describe('Cột đá Sơn Tinh', () => {
  it('tên bắn trúng thì cột dựng tới đủ chiều cao', () => {
    const g = makeGame(), c = g.pillars[1], TS = g.map.ts;
    expect(c.cur).toBe(c.max - 5);
    g.bolts.push({ x: c.tx * TS + 10, y: pillarTop(g, c) + 8, vx: 600, lv: 0, life: 1, hit: new Set() });
    for (let i = 0; i < 240; i++) updatePillars(g, 1 / 120);
    expect(c.cur).toBe(c.max);
    expect(g.map.cells[c.base - c.max + 1][c.tx]).toBe(3);
  });
  it('đứng trên đỉnh cột thì được nâng lên theo', () => {
    const g = makeGame(), c = g.pillars[1], TS = g.map.ts;
    placePlayer(g, c.tx * TS + TS / 2, pillarTop(g, c));
    c.grow = true; const n = Math.ceil((c.max - c.cur) * T.PILLAR_STEP * 120) + 10;
    for (let i = 0; i < n; i++) updatePillars(g, 1 / 120);
    expect(g.P.y + g.P.h).toBe(pillarTop(g, c));
    expect(pillarTop(g, c)).toBe((c.base - c.max + 1) * TS);
  });
});
