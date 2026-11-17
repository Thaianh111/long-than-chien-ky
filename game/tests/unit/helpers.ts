import { createGame, seededRng, type Game } from '../../src/core/game';
import { Input } from '../../src/core/input';
import { HONG_THUY } from '../../src/levels/hongThuy';

/** Tạo một lượt chơi màn Hồng Thủy với số ngẫu nhiên cố định và không âm thanh. */
export function makeGame(seed = 1): Game { return createGame(HONG_THUY, new Input(), undefined, seededRng(seed)); }
/** Đặt người chơi đứng ngay vị trí (x giữa thân, chân ở y). */
export function placePlayer(g: Game, x: number, footY: number): void { g.P.x = x - g.P.w / 2; g.P.y = footY - g.P.h; g.P.vx = 0; g.P.vy = 0; }
