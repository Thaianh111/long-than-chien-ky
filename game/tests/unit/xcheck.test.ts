/** Dò khớp với bản Unity: chạy cùng chuỗi phím, in vị trí để so (XCHECK=1 npx vitest run tests/unit/xcheck.test.ts). */
import { it } from 'vitest';
import { step } from '../../src/core/flow';
import type { Key } from '../../src/core/types';
import { makeGame, placePlayer } from './helpers';

it.runIf((globalThis as unknown as { process?: { env: Record<string, string | undefined> } }).process?.env.XCHECK === '1')('xcheck', () => {
  const g = makeGame(1), o: string[] = [];
  const plan: [number, Key, boolean][] = [[0,'right',true],[60,'jump',true],[80,'jump',false],[130,'dash',true],[160,'dash',false],[200,'right',false],[210,'attack',true],[212,'attack',false],[300,'right',true],[340,'jump',true],[345,'jump',false],[360,'jump',true],[365,'jump',false]];
  for (let i = 0; i < 600; i++) {
    for (const [t, k, v] of plan) if (t === i) { g.input.held[k] = v ? 1 : 0; if (v) g.input.pressed[k] = true; else g.input.released[k] = true; }
    step(g); if (i % 50 === 49) o.push(`[${i},${+g.P.x.toFixed(2)},${+g.P.y.toFixed(2)}]`);
  }
  console.log('XCHECK [' + o.join(',') + ']');
  const h = makeGame(4), q: string[] = []; placePlayer(h, 166.5 * 32, 11 * 32);
  const st = (s: string) => s[0].toUpperCase() + s.slice(1);
  for (let i = 0; i < 4800; i++) {
    const j = i % 150 === 10, a = i % 40 === 5;
    h.input.held.jump = j ? 1 : 0; if (j) h.input.pressed.jump = true; if (a) h.input.pressed.attack = true; h.input.held.attack = a ? 1 : 0;
    step(h); if (i % 400 === 399) q.push(`[${i},"${st(h.sub.st)}","${h.sub.move ? st(h.sub.move) : ''}",${+h.sub.x.toFixed(1)},${+h.sub.y.toFixed(1)},${h.sub.hp},${h.P.hp}]`);
  }
  console.log('XSUB [' + q.join(',') + ']');
});
