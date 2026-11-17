/** Tải hình. Thiếu file thì game vẫn chạy (vẽ hình dự phòng), không chặn khởi động. */
import { ANIM, BTA, FA, TL } from './config';

export const FR: Record<string, HTMLImageElement> = {};
export const BG: Record<string, HTMLImageElement> = {};
export let SPR: HTMLImageElement | null = null;

const FOE_FRAMES = ['en_drone1', 'en_drone2', 'en_drone3', 'en_drone4', 'en_turret1', 'en_turret2', 'en_turret3', 'en_turret4', 'en_turret5', 'en_turret6',
  'fx_boom1', 'fx_boom2', 'fx_boom3', 'fx_boom4', 'fx_boom5', 'fx_boom6', 'fx_shot1', 'fx_shot2', 'fx_shot3', 'fx_shothit1', 'fx_shothit2', 'fx_shothit3'];

function load(src: string): Promise<HTMLImageElement> {
  return new Promise(res => { const im = new Image(); im.onload = () => res(im); im.onerror = () => res(im); im.src = src; });
}
/** Mọi tên khung cần tải: lấy từ bảng neo (FA, BTA) và bảng hoạt ảnh, cộng quái/hiệu ứng. */
export function frameNames(): string[] {
  const s = new Set<string>([...Object.keys(FA), ...Object.keys(BTA), ...FOE_FRAMES, ...Array.from({ length: TL.FRAMES }, (_, i) => 'tl_swim' + (i + 1))]);
  for (const list of Object.values(ANIM)) list.forEach(n => s.add(n));
  return [...s];
}
export async function loadAssets(onProgress?: (k: number) => void): Promise<void> {
  const names = frameNames(), bgs = ['skyline-a', 'buildings-bg', 'near-buildings-bg'];
  let done = 0; const total = names.length + bgs.length + 1, tick = () => onProgress?.(++done / total);
  await Promise.all([
    ...names.map(n => load(`assets/frames/f_${n}.png`).then(im => { FR[n] = im; tick(); })),
    ...bgs.map(n => load(`assets/bg/${n}.png`).then(im => { BG[n] = im; tick(); })),
    load('assets/xich-long.png').then(im => { SPR = im; tick(); }),
  ]);
}
export const ready = (im: HTMLImageElement | undefined | null): im is HTMLImageElement => !!im && im.complete && im.naturalWidth > 0;
