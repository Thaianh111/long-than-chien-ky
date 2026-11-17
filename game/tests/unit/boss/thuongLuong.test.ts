import { describe, expect, it } from 'vitest';
import { chooseSubMove, headBox, hurtSub, updateSub } from '../../../src/boss/thuongLuong';
import { TL } from '../../../src/core/config';
import { makeGame, placePlayer } from '../helpers';

/** Vào trận: đứng trên bờ trái đấu trường rồi chạy qua đoạn mở màn. */
function enter(seed = 1) {
  const g = makeGame(seed), A = g.level.sub;
  placePlayer(g, A.closeAtX + 10, A.platRow * g.map.ts);
  updateSub(g, 1 / 120); return g;
}

describe('Thuồng Luồng — chọn chiêu', () => {
  it('đòn đầu tiên luôn là Lao mặt nước', () => {
    for (let s = 1; s <= 20; s++) expect(chooseSubMove(enter(s))).toBe('surge');
  });
  it('không chiêu nào 3 lần liền, không Nổi 2 lần liền (1.000 lần chọn, cả 2 pha)', () => {
    for (const ph of [0, 1] as const) {
      const g = enter(5); g.sub.phase = ph;
      const seq = Array.from({ length: 1000 }, (_, i) => chooseSubMove(g, i % 7 === 0));
      for (let i = 2; i < seq.length; i++) expect(seq[i] === seq[i - 1] && seq[i] === seq[i - 2]).toBe(false);
      for (let i = 1; i < seq.length; i++) expect(seq[i] === 'float' && seq[i - 1] === 'float').toBe(false);
    }
  });
});

describe('Thuồng Luồng — trận đấu', () => {
  it('vào trận thì cổng trái đóng', () => {
    const g = enter(), G = g.level.sub.gate;
    expect(g.sub.st).toBe('intro');
    expect(g.map.solidCell(G.x, G.y)).toBe(true);
  });
  it('đầu dưới nước không ăn đòn, nhô lên thì ăn', () => {
    const g = enter(); g.sub.st = 'under'; g.sub.y = g.level.sub.surface + TL.Y.UNDER;
    expect(hurtSub(g, 2)).toBe(false);
    g.sub.y = g.level.sub.surface + TL.Y.FLOAT;
    expect(hurtSub(g, 2)).toBe(true);
    expect(g.sub.hp).toBe(TL.HP - 2);
  });
  it('lúc Nổi lưng thì có ô đứng được, lặn rồi thì hết ô', () => {
    const g = enter(3), A = g.level.sub, TS = g.map.ts, row = A.platRow;
    const count = () => { let n = 0; for (let i = A.x0 / TS; i < A.x1 / TS; i++) if (g.map.cells[row][i] === 2) n++; return n; };
    g.sub.st = 'under'; g.sub.t = 99; g.sub.first = true; g.sub.last = ['surge'];
    let floated = false;
    for (let i = 0; i < 120 * 60 && !floated; i++) { updateSub(g, 1 / 120); if (g.sub.move === 'float' && g.sub.stage === 'A' && count() > 0) floated = true; }
    expect(floated).toBe(true);
    expect(count()).toBeGreaterThanOrEqual(6);
    for (let i = 0; i < 120 * 8 && g.sub.move === 'float'; i++) updateSub(g, 1 / 120);
    expect(count()).toBe(0);
  });
  it('hạ xong thì cổng phải mở, người chơi đi tiếp được', () => {
    const g = enter(), E = g.level.sub.exit;
    expect(g.map.solidCell(E.x, E.y)).toBe(true);
    g.sub.st = 'atk'; g.sub.move = 'float'; g.sub.y = g.level.sub.surface + TL.Y.FLOAT;
    for (let i = 0; i < 30 && (g.sub.st as string) !== 'dead'; i++) { g.sub.inv = 0; hurtSub(g, 1); }
    expect(g.sub.st).toBe('dead');
    for (let j = 0; j < E.h; j++) expect(g.map.solidCell(E.x, E.y + j)).toBe(false);
  });
  it('hộp đầu nằm đúng phía mặt (lật hình khi quay phải)', () => {
    const g = enter(); g.sub.x = 1000;
    g.sub.face = -1; expect(headBox(g.sub).x).toBe(1000 + TL.HEAD.x0);
    g.sub.face = 1; expect(headBox(g.sub).x + headBox(g.sub).w).toBe(1000 + TL.FW - TL.HEAD.x0);
  });
});
