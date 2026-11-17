/** Vẽ trùm phụ Thuồng Luồng (cắt theo mép đấu trường), cục nước phun, thanh máu. */
import { TL } from '../core/config';
import { FR, ready } from '../core/assets';
import type { Game } from '../core/game';
import { headUp } from '../boss/thuongLuong';
import type { View } from './view';

/** Tên khung bơi hiện tại. */
export function subFrame(g: Game): string {
  const S = g.sub, fast = S.st === 'atk' && S.move === 'surge' && S.stage === 'A';
  return 'tl_swim' + (1 + Math.floor(g.time * (fast ? 18 : 10)) % TL.FRAMES);
}

export function drawSub(g: Game, v: View, ox: number, oy: number): void {
  const S = g.sub, A = g.level.sub, cx = v.cx;
  if (S.st === 'wait' || (S.st === 'dead' && S.t > 2.5)) return;
  const img = FR[subFrame(g)];
  if (ready(img)) {
    const shake = S.st === 'atk' && S.move === 'float' && S.stage === 'R' ? Math.sin(g.time * 70) * 3 : 0;
    const sx = S.x + shake, cl = A.x0, cr = A.x1, bed = A.surface + TL.Y.BED;
    const vis = S.face < 0 ? [cl - sx, cr - sx] : [sx + TL.FW - cr, sx + TL.FW - cl];
    const u0 = Math.max(0, vis[0]), u1 = Math.min(TL.FW, vis[1]), hh = Math.min(TL.FH, bed - S.y);
    if (u1 > u0 && hh > 0) {
      const under = !headUp(g) && S.st !== 'dead';
      cx.save();
      if (S.face < 0) cx.translate(Math.round(sx - ox), Math.round(S.y - oy));
      else { cx.translate(Math.round(sx + TL.FW - ox), Math.round(S.y - oy)); cx.scale(-1, 1); }
      cx.globalAlpha = under ? 0.35 : S.st === 'dead' ? Math.max(0, 1 - S.t / 2.5) : 1;
      cx.drawImage(img, u0, 0, u1 - u0, hh, u0, 0, u1 - u0, hh);
      if (S.flash > 0 || (S.phase && !under && Math.sin(g.time * 5) > 0.6)) {
        cx.globalCompositeOperation = 'lighter'; cx.globalAlpha = S.flash > 0 ? 0.8 : 0.25;
        cx.drawImage(img, u0, 0, u1 - u0, hh, u0, 0, u1 - u0, hh);
      }
      cx.restore();
    }
  }
  for (const b of S.shots) {
    const x = Math.round(b.x - ox), y = Math.round(b.y - oy);
    cx.fillStyle = b.back ? '#ff6a5a' : 'rgba(143,240,255,.9)'; cx.beginPath(); cx.arc(x, y, TL.SPIT.SIZE, 0, 6.283); cx.fill();
    cx.fillStyle = '#fff'; cx.fillRect(x - 4, y - 5, 3, 3);
  }
}

export function drawSubHud(g: Game, v: View): void {
  const S = g.sub, cx = v.cx, VW = v.VW;
  if (S.st === 'wait' || (S.st === 'dead' && S.t > 2.2)) return;
  const n = TL.HP, fill = S.st === 'intro' ? Math.floor(Math.min(1, S.t / 1.2) * n) : S.hp;
  cx.fillStyle = '#000'; cx.fillRect(VW - 26, 74, 12, n * 5 + 4);
  for (let i = 0; i < n; i++) { cx.fillStyle = i < fill ? (S.phase ? '#c58bff' : '#3fe0c8') : '#2a2a3a'; cx.fillRect(VW - 24, 76 + (n - 1 - i) * 5, 8, 4); }
  cx.fillStyle = '#e8f6f8'; cx.font = 'bold 11px sans-serif'; cx.fillText('THUỒNG LUỒNG', VW - 112, 70);
}
