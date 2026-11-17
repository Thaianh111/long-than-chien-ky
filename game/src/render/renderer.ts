/** Vẽ một khung hình: nền thị sai, ô gạch, cờ lưu, quái, trùm, người chơi, hiệu ứng, thanh máu. */
import { T, VH } from '../core/config';
import { BG, FR, ready } from '../core/assets';
import type { Game } from '../core/game';
import { drawBolts, drawPlayer } from './player';
import { drawBoss, drawBossHud, drawTalk, drawWater } from './boss';
import type { View } from './view';

function parallax(v: View, im: HTMLImageElement | undefined, f: number, y: number, k: number): void {
  if (!ready(im)) return;
  const w = im.width * k, x0 = -((v.cam.x * f) % w);
  for (let x = x0 - w; x < v.VW; x += w) v.cx.drawImage(im, Math.round(x), y, w, im.height * k);
}

export function draw(g: Game, v: View): void {
  const { cx, VW, cam } = v, map = g.map, TS = map.ts;
  const sx = g.shake ? (Math.random() - 0.5) * g.shake * 2 : 0, sy = g.shake ? (Math.random() - 0.5) * g.shake * 2 : 0;
  const gr = cx.createLinearGradient(0, 0, 0, VH); gr.addColorStop(0, '#141a2e'); gr.addColorStop(1, '#2a2440');
  cx.fillStyle = gr; cx.fillRect(0, 0, VW, VH);
  cx.save(); cx.translate(Math.round(sx), Math.round(sy));
  // nền thành phố Warped City (Ansimuz, CC0), 3 lớp trôi chậm theo độ xa
  parallax(v, BG['skyline-a'], 0.05, 0, 1.5);
  parallax(v, BG['buildings-bg'], 0.15, VH - 124 * 1.5 - 20, 1.5);
  cx.globalAlpha = 0.85; parallax(v, BG['near-buildings-bg'], 0.3, VH - 209 * 1.5 + 40, 1.5); cx.globalAlpha = 1;
  cx.fillStyle = 'rgba(14,10,30,.55)'; cx.fillRect(0, 0, VW, VH);

  const ox = Math.round(cam.x), oy = Math.round(cam.y), i0 = Math.floor(ox / TS), i1 = i0 + Math.ceil(VW / TS) + 2;
  for (let j = 0; j < map.h; j++) for (let i = i0; i < i1; i++) {
    if (!map.solidCell(i, j)) continue;
    const X = i * TS - ox, Y = j * TS - oy;
    cx.fillStyle = '#2e3346'; cx.fillRect(X, Y, TS, TS);
    cx.fillStyle = '#3c4259'; cx.fillRect(X, Y, TS, 5);
    cx.fillStyle = '#262a3a'; cx.fillRect(X, Y + TS - 2, TS, 2); cx.fillRect(X + TS - 2, Y, 2, TS);
    if (!map.solidCell(i, j - 1)) { cx.fillStyle = '#C9A227'; cx.fillRect(X, Y, TS, 2); }
  }
  g.checkpoints.forEach((c, i) => { const X = c.x - ox, Y = c.y - oy; cx.fillStyle = '#888'; cx.fillRect(X, Y - 52, 3, 52); cx.fillStyle = i <= g.cpIndex ? '#d8352a' : '#555'; cx.fillRect(X + 3, Y - 52, 22, 14); });

  for (const f of g.foes) {
    if (f.dead > 0) continue;
    const X = Math.round(f.x - ox), Y = Math.round(f.y - oy);
    const img = f.type === 'drone' ? FR['en_drone' + (1 + Math.floor(f.t * 8) % 4)] : FR['en_turret' + (f.fire > 0 ? 1 + Math.floor((0.36 - f.fire) / 0.06) % 6 : 1)];
    cx.save(); cx.translate(X + f.w / 2, Y + f.h); cx.scale(f.type === 'turret' ? f.dir : 1, 1);
    if (ready(img)) {
      const dy = -img.height + (f.type === 'drone' ? 11 : 1);
      cx.drawImage(img, -Math.round(img.width / 2), dy);
      if (f.flash > 0) { cx.globalCompositeOperation = 'lighter'; cx.globalAlpha = 0.85; cx.drawImage(img, -Math.round(img.width / 2), dy); }
    }
    cx.restore();
    cx.fillStyle = '#000'; cx.fillRect(X, Y - 9, f.w, 4); cx.fillStyle = '#ffcf5a'; cx.fillRect(X, Y - 9, f.w * f.hp / f.maxHp, 4);
  }
  for (const b of g.eshots) {
    const im = FR['fx_shot' + (1 + Math.floor(b.t * 12) % 3)];
    if (ready(im)) { cx.save(); cx.translate(Math.round(b.x - ox), Math.round(b.y - oy)); cx.scale(-Math.sign(b.vx) || 1, 1); cx.drawImage(im, -15, -11); cx.restore(); }
  }
  drawBoss(g, v, ox, oy);
  if (!(g.P.inv > 0 && Math.floor(g.P.inv * 20) % 2)) drawPlayer(g, v);   // nhấp nháy khi bất tử
  for (const p of g.parts) { cx.globalAlpha = Math.min(1, p.life * 3); cx.fillStyle = p.c; cx.fillRect(Math.round(p.x - ox), Math.round(p.y - oy), 3, 3); }
  cx.globalAlpha = 1; drawBolts(g, v, ox, oy); drawWater(g, v, ox, oy);
  for (const e of g.fx) {
    const k = e.life / e.max; cx.save(); cx.translate(Math.round(e.x - ox), Math.round(e.y - oy));
    if (e.k === 'cut') {   // vết cắt: đường sáng mảnh kéo dài rồi thu lại
      cx.rotate(e.a); const L = e.L * (1.2 - k * 0.4);
      cx.globalAlpha = k; cx.fillStyle = '#fff6d0'; cx.fillRect(-L / 2, -2 * k - 0.5, L, 4 * k + 1);
      cx.globalAlpha = k * 0.5; cx.fillStyle = '#ff8a2b'; cx.fillRect(-L / 2, -5 * k, L, 10 * k);
    } else if (e.k === 'boom' || e.k === 'shothit') {
      const n = e.k === 'boom' ? 6 : 3, i = Math.min(n - 1, Math.floor((1 - k) * n)), im = FR[(e.k === 'boom' ? 'fx_boom' : 'fx_shothit') + (i + 1)];
      if (ready(im)) cx.drawImage(im, -Math.round(im.width / 2), -Math.round(im.height / 2));
    } else if (e.k === 'ring') {
      const r = (1 - k) * (e.big ? 46 : 30) + 6; cx.globalAlpha = k; cx.strokeStyle = e.col || '#ffd34d'; cx.lineWidth = 3 * k + 1;
      cx.beginPath(); cx.arc(0, 0, r, 0, 6.283); cx.stroke();
    }
    cx.restore();
  }
  cx.globalAlpha = 1;
  // thanh máu kiểu Mega Man
  cx.fillStyle = '#000'; cx.fillRect(14, 74, 12, T.HP * 6 + 4);
  for (let i = 0; i < T.HP; i++) { cx.fillStyle = i < g.P.hp ? (g.hero === 'tv' ? '#7fe3ff' : '#ffb347') : '#2a2a3a'; cx.fillRect(16, 76 + (T.HP - 1 - i) * 6, 8, 4); }
  if (g.flashT > 0) { cx.globalAlpha = g.flashT / 0.06 * 0.35; cx.fillStyle = '#fff1d6'; cx.fillRect(0, 0, VW, VH); cx.globalAlpha = 1; }
  drawBossHud(g, v); drawTalk(g, v);
  const B = g.boss;
  if (B.st === 'dead' && B.t > 2.4) {
    cx.fillStyle = 'rgba(0,0,0,.5)'; cx.fillRect(0, VH / 2 - 34, VW, 60);
    cx.textAlign = 'center'; cx.fillStyle = '#ffd34d'; cx.font = 'bold 22px sans-serif'; cx.fillText('HẠ THỦY TINH — QUA MÀN 1', VW / 2, VH / 2);
    cx.font = '13px sans-serif'; cx.fillStyle = '#f3e6c4'; cx.fillText('Nhận được vũ khí Thủy Tiễn (sắp có)', VW / 2, VH / 2 + 18); cx.textAlign = 'left';
  }
  cx.restore();
}
