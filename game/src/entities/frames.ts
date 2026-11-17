import { ANIM, ATKF, T } from '../core/config';
import type { Game } from '../core/game';
import type { Player } from '../core/types';

const seq = (name: string, t: number, fps = 10): string => { const a = ANIM[name]; return a[Math.floor(t * fps) % a.length]; };

/** Thanh Vũ đang chạy trên đất. */
export const tvRunning = (P: Player): boolean => P.ground && !P.dash && Math.abs(P.vx) > 40;

/** Khung hình của nhát chém hiện tại (pre = lấy đà, act = vung, post = cuối nhát, rec = thu đao). */
export function atkFrame(P: Player, endPose = false): string | null {
  const a = P.atk; if (!a) return null;
  const F = ATKF[P.atkAir ? 'air' : String(P.combo)] || ATKF['0'];
  if (endPose) return F.rec;
  if (P.atkT < a.from) return F.pre[Math.min(F.pre.length - 1, Math.floor(P.atkT / a.from * F.pre.length))];
  if (P.atkT <= a.to) return F.act[Math.min(F.act.length - 1, Math.floor((P.atkT - a.from) / (a.to - a.from) * F.act.length))];
  return F.post[Math.min(F.post.length - 1, Math.floor((P.atkT - a.to) / (a.time - a.to) * F.post.length))];
}

/** Chọn tên khung hình theo trạng thái người chơi (tên file frames/f_<tên>.png). */
export function pickFrame(g: Game): string {
  const P = g.P, time = g.time;
  if (g.hero === 'tv') {
    if (P.wall) return P.aiming || P.shootT > 0 ? 'tv_w2' : 'tv_w1';
    if (P.wjT > 0 && !P.aiming && !P.shootT) return 'tv_w3';
    if (P.dash && !P.aiming && !P.shootT) return P.ground && P.dashHeld < T.DASH_PUSH ? 'tv_d1' : 'tv_d2';
    if (P.recover > 0 && P.recFrame === 'tv_d3' && !P.aiming) return 'tv_d3';
    if (!P.ground && !P.wall) {
      const sh = P.aiming || P.shootT > 0 ? 'tv_aj' : 'tv_j';
      if (P.vy < 0 && P.airT < T.TAKEOFF_TIME && !P.aiming) return 'tv_j1';
      return sh + (P.vy < -T.APEX_VY ? 2 : P.vy < T.APEX_VY ? 3 : 4);
    }
    if (tvRunning(P) && (P.aiming || P.shootT > 0)) return ANIM.tv_runshoot[Math.floor(P.gait) % ANIM.tv_runshoot.length];
    if (P.shootT > 0) return P.shootT > T.TV.SHOOT_ANIM - 0.07 ? 'tv_b3' : 'tv_b4';
    if (P.aiming) return P.aimT < 0.05 ? 'tv_b1' : 'tv_b2';
    if (P.landT > 0 && Math.abs(P.vx) <= 40) return 'tv_j5';
    if (P.ground && !P.dash && Math.abs(P.vx) > 40) return ANIM.tv_run[Math.floor(P.gait) % ANIM.tv_run.length];
    return seq('tv_idle', time, T.IDLE_FPS);
  }
  if (P.atk) return atkFrame(P) as string;
  if (P.recover > 0 && P.recFrame) return P.recFrame;
  if (P.dash) return P.ground && P.dashHeld < T.DASH_PUSH ? seq('dash_start', time) : seq('dash', time);
  if (P.wall) return seq('wall', time);
  if (!P.ground) {
    if (P.wjT > 0) return P.wjT > T.WALLJUMP_ANIM - T.WALLKICK_ANIM ? ANIM.wall_kick[0] : ANIM.wall_jump[0];
    if (P.vy < 0 && P.airT < T.TAKEOFF_TIME) return seq('takeoff', P.airT);
    return seq(P.vy < -T.APEX_VY ? 'jump_up' : P.vy < T.APEX_VY ? 'apex' : 'jump_down', P.airT);
  }
  if (P.landT > 0 && Math.abs(P.vx) <= 40) return seq('land', P.landT);
  if (Math.abs(P.vx) > 40) return ANIM.run[Math.floor(P.gait) % ANIM.run.length];
  return seq('idle', time, T.IDLE_FPS);
}
