/** Vẽ người chơi: dáng theo khung, bóng mờ khi lướt, vệt chém bán nguyệt, hiệu ứng xuất hiện, tên nỏ. */
import { ANIM, FA, FH, FW, FY, SPR_H, SPR_W, T } from '../core/config';
import { FR, SPR, ready } from '../core/assets';
import type { Game } from '../core/game';
import { pickFrame } from '../entities/frames';
import { chargeLv, muzzle } from '../entities/player';
import type { View } from './view';

const easeOut = (t: number) => 1 - Math.pow(1 - t, 3);
const tintCache: Record<string, HTMLCanvasElement> = {};

/** Khung tô một màu (bóng mờ khi lướt: Xích Long đỏ cam, Thanh Vũ xanh băng), tạo một lần rồi giữ lại. */
function tinted(name: string): HTMLCanvasElement | null {
  if (tintCache[name]) return tintCache[name];
  const img = FR[name]; if (!ready(img)) return null;
  const c = document.createElement('canvas'); c.width = FW; c.height = FH; const g = c.getContext('2d')!;
  g.drawImage(img, 0, 0); g.globalCompositeOperation = 'source-atop'; g.fillStyle = name.startsWith('tv_') ? '#2fc8ff' : '#ff5a1f'; g.fillRect(0, 0, FW, FH);
  return (tintCache[name] = c);
}
function drawPose(cx: CanvasRenderingContext2D, name: string): void {
  const fr = FR[name];
  if (ready(fr)) cx.drawImage(fr, -FA[name], -FH + 2 + (FY[name] || 0));
  else if (ready(SPR)) cx.drawImage(SPR, -SPR_W / 2, -SPR_H);
}

/** Vệt chém hình lưỡi liềm nhọn hai đầu, mở theo lưỡi đao rồi thu lại từ đuôi. */
function crescent(cx: CanvasRenderingContext2D, a0: number, a1: number, R: number, W: number, tail: number, head: number, shape: number): void {
  const N = 28; cx.beginPath();
  for (let i = 0; i <= N; i++) { const u = tail + (head - tail) * i / N, a = a0 + (a1 - a0) * u; cx.lineTo(Math.cos(a) * R, Math.sin(a) * R); }
  for (let i = N; i >= 0; i--) {
    const u = tail + (head - tail) * i / N, a = a0 + (a1 - a0) * u, k = (u - tail) / Math.max(0.001, head - tail);
    const w = W * Math.pow(Math.sin(Math.PI * Math.pow(k, 0.75)), shape);
    cx.lineTo(Math.cos(a) * (R - w), Math.sin(a) * (R - w));
  }
  cx.closePath(); cx.fill();
}
function drawSwing(g: Game, cx: CanvasRenderingContext2D): void {
  const A = g.P.arc; if (!A || A.t < A.from) return;
  const open = easeOut(Math.min(1, (A.t - A.from) / (A.to - A.from))), fade = Math.max(0, (A.t - A.to) / T.ARC_FADE);
  const span = Math.abs(A.a1 - A.a0), maxTail = Math.min(1, (Math.PI * 1.65) / span);
  const head = open, tail = Math.min(head, Math.max(fade * fade, head - maxTail));
  if (head - tail < 0.02) return;
  const R = T.BLADE_LEN * T.ARC_R * (1 + (A.len - 1) * 0.45), W = T.ARC_W * (A.big ? 1.35 : 1) * (1 - fade * 0.5);
  cx.save(); cx.translate(A.at.x, A.at.y);
  cx.globalAlpha = 0.35 * (1 - fade); cx.fillStyle = '#ff3d14'; crescent(cx, A.a0, A.a1, R + 5, W + 9, tail, head, 0.8);
  cx.globalAlpha = 0.95 * (1 - fade * 0.6); cx.fillStyle = '#ff8a2b'; crescent(cx, A.a0, A.a1, R, W, tail, head, 0.9);
  cx.globalAlpha = 1 - fade * 0.5; cx.fillStyle = '#fff6d0'; crescent(cx, A.a0, A.a1, R - 1, W * 0.42, tail, head, 1);
  cx.restore(); cx.globalAlpha = 1;
}

/** Tia năng lượng rơi xuống rồi hiện người từ chân lên (kiểu Mega Man X8). */
function drawWarp(g: Game, v: View, rx: number, ry: number): void {
  const { cx, cam } = v, P = g.P, W = P.warp!, tv = g.hero === 'tv', col = tv ? '#7fe3ff' : '#ff8a2b', glow = tv ? '#1fb7ff' : '#ff3d14';
  const sx = Math.round(rx + P.w / 2 - cam.x), by = Math.round(ry + P.h - cam.y);
  if (W.t < T.WARP_FALL) {
    const k = W.t / T.WARP_FALL, head = -40 + (by + 40) * (k * k), L = 140;
    cx.globalAlpha = 0.35; cx.fillStyle = glow; cx.fillRect(sx - 7, head - L, 14, L);
    cx.globalAlpha = 0.9; cx.fillStyle = col; cx.fillRect(sx - 4, head - L * 0.8, 8, L * 0.8);
    cx.globalAlpha = 1; cx.fillStyle = '#ffffff'; cx.fillRect(sx - 2, head - L * 0.6, 4, L * 0.6 + 6);
    return;
  }
  const k = Math.min(1, (W.t - T.WARP_FALL) / T.WARP_FORM), colH = (1 - k) * 90 + 10;
  cx.globalAlpha = (1 - k) * 0.8; cx.fillStyle = col; cx.fillRect(sx - 6 - (1 - k) * 6, by - colH, 12 + (1 - k) * 12, colH);
  cx.globalAlpha = 1 - k; cx.fillStyle = '#ffffff'; cx.fillRect(sx - 2, by - colH, 4, colH);
  const name = pickFrame(g), img = FR[name], t = tinted(name);
  if (ready(img)) {
    cx.save(); cx.translate(sx, by); cx.scale(P.face, 1);
    const top = FH - 2, hShow = FH * Math.max(0.02, k);
    cx.beginPath(); cx.rect(-FW, -hShow, FW * 2, hShow + 2); cx.clip();
    cx.globalAlpha = 1; cx.drawImage(img, -FA[name], -top + (FY[name] || 0));
    if (t) { cx.globalCompositeOperation = 'lighter'; cx.globalAlpha = (1 - k) * 0.9; cx.drawImage(t, -FA[name], -top + (FY[name] || 0)); }
    cx.restore();
  }
  cx.globalAlpha = 1;
}

export function drawPlayer(g: Game, v: View): void {
  const { cx, cam, alpha } = v, P = g.P;
  const rx = P.ox + (P.x - P.ox) * alpha, ry = P.oy + (P.y - P.oy) * alpha;   // nội suy giữa 2 bước vật lý
  if (P.warp) { drawWarp(g, v, rx, ry); return; }
  const sx = Math.round(rx + P.w / 2 - cam.x), sy = Math.round(ry + P.h - cam.y);
  cx.save(); cx.globalCompositeOperation = 'lighter';
  for (const gh of P.ghosts) {
    const t = tinted(gh.n); if (!t) continue;
    cx.save(); cx.globalAlpha = gh.a * 0.5; cx.translate(Math.round(gh.x + P.w / 2 - cam.x), Math.round(gh.y + P.h - cam.y)); cx.scale(gh.f, 1);
    cx.drawImage(t, -FA[gh.n], -FH + 2 + (FY[gh.n] || 0)); cx.restore();
  }
  cx.restore();
  const name = pickFrame(g);
  let rot = 0;
  if (name === ANIM.run[Math.floor(P.gait) % ANIM.run.length] && P.ground && Math.abs(P.vx) > 40) rot = 0.02;
  if (!P.ground && !P.wall && !P.atk && !P.dash) rot = Math.max(-0.03, Math.min(0.04, P.vy / 20000));
  if (P.dash) rot = 0;
  cx.save();
  cx.translate(sx, sy); cx.scale(name === ANIM.wall_kick[0] ? P.wjDir : P.face, 1);
  cx.rotate(rot); cx.scale(P.sqx, P.sqy);
  drawSwing(g, cx); drawPose(cx, name);
  cx.restore();
}

export function drawBolts(g: Game, v: View, ox: number, oy: number): void {
  const cx = v.cx;
  for (const b of g.bolts) {
    const d = Math.sign(b.vx), L = [20, 30, 48][b.lv], h = [3, 5, 9][b.lv], x = Math.round(b.x - ox), y = Math.round(b.y - oy);
    cx.save(); cx.translate(x, y); cx.scale(d, 1);
    cx.globalAlpha = 0.35; cx.fillStyle = '#1fb7ff'; cx.fillRect(-L - 4, -h - 2, L + 8, h * 2 + 4);
    cx.globalAlpha = 0.9; cx.fillStyle = '#7fe3ff'; cx.fillRect(-L, -h, L + 2, h * 2);
    cx.globalAlpha = 1; cx.fillStyle = '#ffffff'; cx.fillRect(-L * 0.7, -Math.max(1, h / 3), L * 0.7 + 4, Math.max(2, h * 2 / 3));
    cx.beginPath(); cx.moveTo(6 + h, 0); cx.lineTo(0, -h - 1); cx.lineTo(0, h + 1); cx.closePath(); cx.fill();   // mũi tên
    if (b.lv === 2) { cx.globalAlpha = 0.25 + 0.15 * Math.sin(g.time * 40); cx.fillStyle = '#bff4ff'; cx.beginPath(); cx.arc(0, 0, h * 2.2, 0, 6.283); cx.fill(); }
    cx.restore();
  }
  const P = g.P;
  if (g.hero === 'tv' && P.aiming && chargeLv(P.charge) > 0) {   // đầu nỏ sáng khi đang nạp
    const m = muzzle(g), lv = chargeLv(P.charge), r = (lv === 2 ? 7 : 4) + Math.sin(g.time * 30) * 1.5;
    cx.globalAlpha = 0.85; cx.fillStyle = lv === 2 ? '#ffffff' : '#7fe3ff'; cx.beginPath(); cx.arc(Math.round(m.x - ox), Math.round(m.y - oy), r, 0, 6.283); cx.fill(); cx.globalAlpha = 1;
  }
}
