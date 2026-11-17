/**
 * LONG THẦN CHIẾN KÝ — điểm khởi động bản web.
 * © Võ Thái Anh · VTA Games
 */
import { STEP, VH } from './core/config';
import { loadAssets } from './core/assets';
import { WebAudioSfx } from './core/audio';
import { createGame, type Game } from './core/game';
import { Input, KEYMAP } from './core/input';
import { respawn, step, updateCamera } from './core/flow';
import { swapHero } from './entities/player';
import { chargeLv } from './entities/player';
import { HONG_THUY } from './levels/hongThuy';
import { draw } from './render/renderer';
import type { View } from './render/view';
import type { HeroId, Key } from './core/types';

const $ = <T extends HTMLElement>(id: string) => document.getElementById(id) as T;
const cv = $<HTMLCanvasElement>('c'), cx = cv.getContext('2d')!;
const input = new Input(), sfx = new WebAudioSfx();
const g: Game = createGame(HONG_THUY, input, sfx);
const view: View = { cx, VW: 640, cam: { x: 0, y: 0 }, alpha: 1 };
const usingTouch = matchMedia('(pointer:coarse)').matches || 'ontouchstart' in window;
(window as unknown as { __ltck: Game }).__ltck = g;   // để công cụ kiểm thử (Playwright) đọc trạng thái

// ---- màn hình: cao cố định 360, rộng theo tỉ lệ máy ----
cv.height = VH;
function fit(): void {
  view.VW = Math.round(Math.max(560, Math.min(800, VH * innerWidth / innerHeight)));
  cv.width = view.VW; cx.imageSmoothingEnabled = false;
  const s = Math.min(innerWidth / view.VW, innerHeight / VH);
  cv.style.width = view.VW * s + 'px'; cv.style.height = VH * s + 'px';
}
addEventListener('resize', fit); fit();

// ---- điều khiển ----
addEventListener('keydown', e => { sfx.unlock(); const k = KEYMAP[e.code]; if (k) { if (!e.repeat) input.set('keys', k, true); else input.keys[k] = 1; e.preventDefault(); } if (e.code === 'KeyR') respawn(g); });
addEventListener('keyup', e => { const k = KEYMAP[e.code]; if (k) { input.set('keys', k, false); e.preventDefault(); } });
addEventListener('pointerdown', () => sfx.unlock());
addEventListener('contextmenu', e => e.preventDefault());
if (usingTouch) $('pad').style.display = 'block';
document.querySelectorAll<HTMLElement>('.btn').forEach(b => {
  const k = b.dataset.k as Key;
  const on = (e: PointerEvent) => { e.preventDefault(); b.setPointerCapture?.(e.pointerId); input.set('touch', k, true); b.classList.add('on'); };
  const off = (e: PointerEvent) => { e.preventDefault(); input.set('touch', k, false); b.classList.remove('on'); };
  b.addEventListener('pointerdown', on); b.addEventListener('pointerup', off);
  b.addEventListener('pointercancel', off); b.addEventListener('lostpointercapture', off);
});

// ---- chọn / đổi nhân vật ----
g.onHeroChange = (h: HeroId) => {
  $('swapBtn').textContent = '⇄ Đổi sang ' + (h === 'xl' ? 'Thanh Vũ' : 'Xích Long');
  const atk = document.querySelector<HTMLElement>('.btn[data-k="attack"]'); if (atk) atk.textContent = h === 'tv' ? 'BẮN' : 'CHÉM';
};
const menuBtns = document.querySelectorAll<HTMLButtonElement>('#menu button');
menuBtns.forEach(b => { b.disabled = true; b.addEventListener('click', () => { sfx.unlock(); $('menu').style.display = 'none'; swapHero(g, b.dataset.hero as HeroId); }); });
$('swapBtn').addEventListener('click', e => { (e.currentTarget as HTMLElement).blur(); swapHero(g); });

// ---- chữ hướng dẫn góc trên ----
const hud = $('hud');
function updateHud(): void {
  const tv = g.hero === 'tv', P = g.P;
  const ctl = usingTouch ? '' : `<br><b>←→</b> chạy · <b>Z/K/Space</b> nhảy · <b>X/J</b> ${tv ? 'bắn (giữ để nạp)' : 'chém'} · <b>C/L/Shift</b> lướt · <b>Q/Tab</b> đổi nhân vật · <b>R</b> về điểm lưu`;
  const lv = tv && P.aiming ? ['', ' · Nạp ●', ' · Nạp ●● ĐẦY'][chargeLv(P.charge)] : '';
  hud.innerHTML = `<b>Long Thần Chiến Ký</b> — Màn 1: ${g.level.name} · <b>${tv ? 'Thanh Vũ' : 'Xích Long'}</b>${ctl}<br>Trúng: ${g.hits} · Hạ: ${g.kills}` +
    (!tv && P.atk && !P.atkAir ? ` · Combo ${P.combo + 1}/3` : '') + lv;
}

// ---- vòng lặp: vật lý bước cố định 120Hz, vẽ nội suy theo tần số màn hình ----
let last = performance.now(), acc = 0;
function frame(now: number): void {
  const dt = Math.min(0.05, (now - last) / 1000); last = now; acc += dt;
  input.poll();
  while (acc >= STEP) { acc -= STEP; step(g); }
  updateCamera(g, view.cam, view.VW, dt, usingTouch);
  view.alpha = g.hitstop > 0 ? 1 : acc / STEP;
  draw(g, view); updateHud();
  requestAnimationFrame(frame);
}

loadAssets(k => { $('loadTxt').textContent = `Đang tải… ${Math.round(k * 100)}%`; }).then(() => {
  $('loadTxt').textContent = 'Chọn nhân vật';
  menuBtns.forEach(b => (b.disabled = false));
});
requestAnimationFrame(frame);
