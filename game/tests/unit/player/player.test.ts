import { describe, expect, it } from 'vitest';
import { step } from '../../../src/core/flow';
import { chargeLv, hurtPlayer } from '../../../src/entities/player';
import { T } from '../../../src/core/config';
import { makeGame, placePlayer } from '../helpers';

describe('Người chơi', () => {
  it('đứng giữa không trung thì rơi xuống và đứng trên nền', () => {
    const g = makeGame(); placePlayer(g, 5 * 32, 10 * 32);
    for (let i = 0; i < 240; i++) step(g);
    expect(g.P.ground).toBe(true);
    expect(g.P.y + g.P.h).toBe(15 * 32);
  });
  it('cấp nạp nỏ theo thời gian giữ nút', () => {
    expect(chargeLv(0)).toBe(0);
    expect(chargeLv(T.TV.CHARGE1)).toBe(1);
    expect(chargeLv(T.TV.CHARGE2)).toBe(2);
  });
  it('trúng đòn thì mất máu và được bất tử, đòn thứ hai trong lúc bất tử không tính', () => {
    const g = makeGame(); placePlayer(g, 5 * 32, 15 * 32);
    hurtPlayer(g, 3, 0); expect(g.P.hp).toBe(T.HP - 3); expect(g.P.inv).toBeGreaterThan(0);
    hurtPlayer(g, 3, 0); expect(g.P.hp).toBe(T.HP - 3);
  });
  it('hết máu thì hồi sinh ở điểm lưu với máu đầy', () => {
    const g = makeGame(); g.cpIndex = 2; placePlayer(g, 30 * 32, 15 * 32);
    hurtPlayer(g, 99, 0);
    expect(g.P.hp).toBe(T.HP); expect(g.P.x).toBe(g.checkpoints[2].x); expect(g.P.warp).not.toBeNull();
  });
});
