/** Vẽ trùm Thủy Tinh (khung Gemini), đạn của trùm, nước, thanh máu trùm, hộp thoại. */
import { BT, BTA, VH } from '../core/config';
import { FR, ready } from '../core/assets';
import type { Game } from '../core/game';
import type { View } from './view';

/** Chọn khung hình trùm theo trạng thái và chiêu. */
export function bossPose(g: Game): string {
  const B = g.boss, st = B.st, m = B.move, sg = B.stage, t = B.t, fl = g.level.arena.floorY;
  const idle = 'bt_i' + (1 + Math.floor(g.time / 0.45) % 4);
  if (st === 'intro') return B.y < fl - 2 ? 'bt_jump3' : t < 0.9 ? 'bt_jump1' : idle;
  if (st === 'shift') return t < 0.3 ? 'bt_hurt1' : 'bt_hurt2';
  if (st === 'stun') return 'bt_hurt1';
  if (st === 'dead') return t < 0.7 ? 'bt_die1' : t < 1.4 ? 'bt_die2' : 'bt_die3';
  if (st === 'atk') {
    if (m === 'dash') return sg === 'W' ? 'bt_dash1' : sg === 'A' ? (Math.floor(g.time * 12) % 2 ? 'bt_dash2' : 'bt_dash3') : t < 0.8 ? 'bt_dash3' : 'bt_dash4';
    if (m === 'wave') return sg === 'W' ? (t < 0.25 ? 'bt_cast1' : 'bt_cast2') : sg === 'A' || t < 0.35 ? 'bt_cast3' : 'bt_cast4';
    if (m === 'spears') return sg === 'W' ? 'bt_cast2' : 'bt_cast3';
    if (m === 'shield') return sg === 'W' ? 'bt_cast1' : 'bt_cast4';
    if (m === 'bubbles') return sg === 'W' ? 'bt_cast1' : 'bt_dash4';
    if (m === 'flood') return sg === 'R' ? (B.y < fl - 2 ? 'bt_jump3' : 'bt_jump1') : sg === 'W' ? 'bt_jump2' : 'bt_cast2';
  }
  if (B.flash > 0.05) return 'bt_hurt1';
  return idle;
}

export function drawBoss(g: Game, v: View, ox: number, oy: number): void {
  const B = g.boss, cx = v.cx, time = g.time, fl = g.level.arena.floorY;
  if (B.st === 'wait' || (B.st === 'dead' && B.t > 2.2)) return;
  const X = Math.round(B.x - ox), Y = Math.round(B.y - oy), f = B.face, sh = B.st === 'atk' && B.stage === 'W' ? Math.sin(time * 60) * 1.5 : 0;
  // báo hiệu: vòng gợn chỗ giáo sẽ rơi, vệt sáng hướng lao
  if (B.st === 'atk' && B.move === 'spears' && B.stage === 'W') for (const x of B.spots) {
    cx.strokeStyle = '#8ff0ff'; cx.globalAlpha = 0.5 + 0.5 * Math.sin(time * 20); cx.beginPath(); cx.ellipse(x - ox, fl - oy - 2, 18, 4, 0, 0, 6.283); cx.stroke(); cx.globalAlpha = 1;
  }
  if (B.st === 'atk' && B.move === 'dash' && B.stage === 'W') { cx.fillStyle = 'rgba(143,240,255,.5)'; cx.fillRect(f > 0 ? X : X - 600, Y - 3, 600, 2); }
  const pose = bossPose(g), spr = FR[pose], an = BTA[pose] || [60, 148];
  if (ready(spr)) {   // hình Gemini quay mặt TRÁI; an = [tâm thân x, chân y]
    if (B.st === 'atk' && B.move === 'dash' && B.stage === 'A') {
      cx.save(); cx.globalAlpha = 0.3; cx.globalCompositeOperation = 'lighter';
      for (let i = 1; i <= 3; i++) { cx.save(); cx.translate(X - f * i * 26, Y); cx.scale(-f, 1); cx.drawImage(spr, -an[0], -an[1]); cx.restore(); }
      cx.restore();
    }
    cx.save(); cx.translate(X + sh, Y); cx.scale(-f, 1);
    cx.drawImage(spr, -an[0], -an[1]);
    if (B.flash > 0 || (B.phase && B.st !== 'dead' && Math.sin(time * 5) > 0.6)) {   // trúng đòn chớp trắng, pha 2 ánh tím
      cx.globalCompositeOperation = 'lighter'; cx.globalAlpha = B.flash > 0 ? 0.8 : 0.25;
      cx.filter = B.flash > 0 ? 'brightness(2)' : 'hue-rotate(60deg)'; cx.drawImage(spr, -an[0], -an[1]); cx.filter = 'none';
    }
    cx.restore();
  } else { cx.fillStyle = B.flash > 0 ? '#fff' : '#1B4F8A'; cx.fillRect(X - B.w / 2, Y - B.h, B.w, B.h); }
  // khiên nước
  if (B.st === 'atk' && B.move === 'shield' && B.stage === 'A') {
    const sx = X + f * 40; cx.globalAlpha = 0.45 + 0.15 * Math.sin(time * 12); cx.fillStyle = '#5fd3ff'; cx.fillRect(sx - 12, Y - 140, 24, 140);
    cx.globalAlpha = 0.9; cx.fillStyle = '#e8f6f8'; for (let yy = 0; yy < 140; yy += 10) cx.fillRect(sx - 12 + ((yy + time * 80) % 24), Y - 140 + yy, 3, 6); cx.globalAlpha = 1;
  }
  for (const b of g.bproj) {
    const x = Math.round(b.x - ox), y = Math.round(b.y - oy), s = Math.sign(b.vx);
    if (b.k === 'wave') {
      cx.fillStyle = 'rgba(95,211,255,.75)'; cx.beginPath(); cx.moveTo(x - 20, y); cx.quadraticCurveTo(x - 6 * s, y - b.h * 1.2, x + 18 * s, y - b.h * 0.6); cx.lineTo(x + 20 * s, y); cx.fill();
      cx.fillStyle = '#e8f6f8'; cx.fillRect(x - 4, y - b.h, 8, 4);
    } else if (b.k === 'spear') {
      cx.fillStyle = '#8ff0ff'; cx.fillRect(x - 3, y - b.h, 6, b.h); cx.fillStyle = '#e8f6f8'; cx.beginPath(); cx.moveTo(x - 7, y - 10); cx.lineTo(x, y + 4); cx.lineTo(x + 7, y - 10); cx.fill();
    } else {
      cx.fillStyle = b.back ? '#ff6a5a' : 'rgba(143,240,255,.85)'; cx.beginPath(); cx.arc(x, y, b.r, 0, 6.283); cx.fill(); cx.fillStyle = '#fff'; cx.fillRect(x - b.r * 0.4, y - b.r * 0.5, 3, 3);
    }
  }
}

/** Mặt nước gợn sóng: xanh (thường), xanh tím (pha 2), tím (nước Ma Chủng gây sát thương). */
export function drawWater(g: Game, v: View, ox: number, oy: number): void {
  const cx = v.cx, W = g.level.water, A = g.level.arena, segs: [number, number, number, number][] = [];
  for (const z of W.zones) segs.push([z.x0, z.x1, z.y, 0]);
  if (g.lvWaterRise) segs.push([W.rise.x0, W.rise.x1, g.lvWaterRise.y, 0]);
  if (g.arenaWater > 0) segs.push([A.x0, A.x1, A.floorY - g.arenaWater, g.boss.phase ? 1 : 0]);
  if (g.arenaHaz > g.arenaWater) segs.push([A.x0, A.x1, A.floorY - g.arenaHaz, 2]);
  for (const [x0, x1, y, kind] of segs) {
    const X0 = Math.max(0, x0 - ox), X1 = Math.min(v.VW, x1 - ox); if (X1 <= X0) continue;
    cx.fillStyle = kind === 2 ? 'rgba(120,50,170,.55)' : kind === 1 ? 'rgba(70,90,170,.5)' : 'rgba(40,120,170,.5)';
    for (let X = X0; X < X1; X += 4) cx.fillRect(X, y - oy + Math.sin((X + ox) * 0.05 + g.time * 4) * 2.5, 4, VH);
    cx.fillStyle = kind === 2 ? '#d7a8ff' : '#bdefff';
    for (let X = X0; X < X1; X += 4) cx.fillRect(X, y - oy + Math.sin((X + ox) * 0.05 + g.time * 4) * 2.5, 4, 2);
  }
}

export function drawBossHud(g: Game, v: View): void {
  const B = g.boss, cx = v.cx, VW = v.VW;
  if (B.st === 'wait' || (B.st === 'dead' && B.t > 2.2)) return;
  const n = BT.HP, fill = B.st === 'intro' ? Math.floor(Math.min(1, B.t / 1.0) * n) : B.hp;
  cx.fillStyle = '#000'; cx.fillRect(VW - 26, 74, 12, n * 4 + 4);
  for (let i = 0; i < n; i++) { cx.fillStyle = i < fill ? (B.phase ? '#c58bff' : '#5fd3ff') : '#2a2a3a'; cx.fillRect(VW - 24, 76 + (n - 1 - i) * 4, 8, 3); }
  cx.fillStyle = '#e8f6f8'; cx.font = 'bold 11px sans-serif'; cx.fillText('THỦY TINH', VW - 92, 70);
}

export function drawTalk(g: Game, v: View): void {
  const t = g.talk, cx = v.cx, VW = v.VW; if (!t) return;
  cx.globalAlpha = Math.max(0, Math.min(1, t.t * 4, (t.dur - t.t) * 4));
  cx.fillStyle = 'rgba(8,12,30,.85)'; cx.fillRect(40, VH - 96, VW - 80, 58);
  cx.strokeStyle = '#3FB8C4'; cx.lineWidth = 2; cx.strokeRect(40, VH - 96, VW - 80, 58);
  cx.fillStyle = t.who ? '#ffb0c8' : '#8ff0ff'; cx.font = 'bold 12px sans-serif'; cx.fillText(t.who || 'Thủy Tinh', 54, VH - 78);
  cx.fillStyle = '#f3e6c4'; cx.font = '13px sans-serif'; t.lines.forEach((l, i) => cx.fillText(l, 54, VH - 60 + i * 17));
  cx.globalAlpha = 1;
}
