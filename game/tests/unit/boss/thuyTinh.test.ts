import { describe, expect, it } from 'vitest';
import { chooseMove, spearSpots, hurtBoss } from '../../../src/boss/thuyTinh';
import { BT } from '../../../src/core/config';
import { makeGame, placePlayer } from '../helpers';

describe('Thủy Tinh — chọn chiêu', () => {
  it('đòn đầu tiên luôn là Sóng Thần', () => {
    for (let s = 1; s <= 20; s++) expect(chooseMove(makeGame(s))).toBe('wave');
  });
  it('không chiêu nào lặp 3 lần liền, không khiên 2 lần liền (1.000 lần chọn)', () => {
    const g = makeGame(7); g.boss.phase = 1; g.boss.lostSince = 99;
    const seq = Array.from({ length: 1000 }, () => chooseMove(g));
    for (let i = 2; i < seq.length; i++) expect(seq[i] === seq[i - 1] && seq[i] === seq[i - 2]).toBe(false);
    for (let i = 1; i < seq.length; i++) expect(seq[i] === 'shield' && seq[i - 1] === 'shield').toBe(false);
  });
  it('Đại Hồng Thủy không bao giờ ra ở pha 1', () => {
    const g = makeGame(3); g.boss.lostSince = 99;
    const seq = Array.from({ length: 500 }, () => chooseMove(g));
    expect(seq).not.toContain('flood');
  });
  it('người chơi ở xa (> 300px) thì hay bị Đinh Ba Lao hơn', () => {
    const count = (far: boolean) => {
      const g = makeGame(11); placePlayer(g, far ? g.boss.x - 450 : g.boss.x - 200, g.level.arena.floorY); chooseMove(g);
      let n = 0; for (let i = 0; i < 600; i++) { if (chooseMove(g) === 'dash') n++; } return n;
    };
    expect(count(true)).toBeGreaterThan(count(false));
  });
});

describe('Thủy Tinh — mưa giáo', () => {
  it('luôn chừa ít nhất 2 khe trống rộng ≥ 64px (1.000 hạt giống)', () => {
    for (let s = 1; s <= 1000; s++) {
      const g = makeGame(s), A = g.level.arena, n = BT.SPEAR.N[s % 2];
      const xs = [A.x0, ...spearSpots(g, n).sort((a, b) => a - b), A.x1];
      const gaps = xs.slice(1).map((x, i) => x - xs[i] - 16).filter(w => w >= 64);   // trừ bề rộng giáo 16px
      expect(gaps.length).toBeGreaterThanOrEqual(2);
    }
  });
});

describe('Thủy Tinh — máu và chuyển pha', () => {
  it('trùm chưa vào trận thì không mất máu', () => {
    const g = makeGame(); expect(hurtBoss(g, 5)).toBe(false); expect(g.boss.hp).toBe(BT.HP);
  });
  it('máu xuống tới nửa thì chuyển pha 2 đúng 1 lần, xóa hết đạn', () => {
    const g = makeGame(); g.boss.st = 'idle'; g.bproj.push({ k: 'drop', x: 0, y: 0, vx: 0, vy: 0, t: 0, life: 1, dmg: 2, r: 6, h: 0 });
    hurtBoss(g, BT.HP - BT.P2);
    expect(g.boss.st).toBe('shift'); expect(g.boss.phase).toBe(1); expect(g.bproj.length).toBe(0);
  });
  it('máu không xuống dưới 0 và trùm chuyển sang gục ngã', () => {
    const g = makeGame(); g.boss.st = 'idle'; g.boss.phase = 1; g.boss.hp = 2;
    hurtBoss(g, 4); expect(g.boss.hp).toBe(0); expect(g.boss.st).toBe('dead');
  });
});
