# Cắt bộ khung Thanh Vũ mới (Sorceress AutoSprite, nền hồng/xanh lá) → khung 150x110 của game.
import json, sys, numpy as np
from PIL import Image
from scipy import ndimage
SRC = sys.argv[2] if len(sys.argv) > 2 else '/tmp/tvpack/x'
D = {s['sprite']: s for s in json.load(open(f'{SRC}/sprite_sheets.json'))['sprites']}
NM = {'idle': 'Standing_ready_with_the_crossbow_hel', 'run': 'Running_to_the_right_in_place_athle',
      'runb': 'Running_to_the_right_in_place_exactl', 'aim': 'Standing_raises_the_crossbow_to_the',
      'fire': 'Strict_side_profile_view_facing_righ', 'chg': 'Strict_side_profile_view_facing_righ_2',
      'jump': 'Crouches_jumps_straight_up_with_kne', 'air': 'In_mid-air_with_knees_tucked_aims_t',
      'land': 'Lands_from_above_into_a_low_crouch_w', 'dash': 'Bursts_into_a_fast_low_ground_dash_t',
      'wall': 'An_invisible_wall_is_on_the_LEFT', 'hurt': 'Gets_hit_from_the_front_recoils_bac'}
GREEN = {'fire', 'chg'}
_cache = {}
def raw(k, i):
    s = D[NM[k]]; c = s['sheet']['cols']
    if k not in _cache: _cache[k] = Image.open(f"{SRC}/{s['image']}").convert('RGB')
    return np.asarray(_cache[k].crop(((i % c) * 512, (i // c) * 512, (i % c) * 512 + 512, (i // c) * 512 + 512))).astype(int)
def key(k, a):
    r, g, b = a[..., 0], a[..., 1], a[..., 2]
    if k in GREEN: bg = (g > r + 22) & (g > b + 22)
    else: bg = (r > g + 55) & (b > g + 45)              # mọi sắc hồng tím (cả vạch tường đậm)
    m = ndimage.binary_opening(~bg, iterations=1)
    lab, n = ndimage.label(m)
    if n:
        sz = ndimage.sum(m, lab, range(1, n + 1)); sl = ndimage.find_objects(lab)
        m = np.isin(lab, [i + 1 for i, s in enumerate(sz) if s > 400 and sl[i][1].start < 455])   # bỏ vệt mũi tên rời bên phải
    if k == 'dash':                                       # bỏ bụi: chỉ giữ khối lớn nhất
        lab, n = ndimage.label(m)
        if n: sz = ndimage.sum(m, lab, range(1, n + 1)); m = lab == (int(np.argmax(sz)) + 1)
        r_, g_, b_ = a[..., 0], a[..., 1], a[..., 2]
        dust = (r_ > 170) & (g_ > 110) & (b_ < 120) & (np.arange(512)[:, None] > 380)
        m = m & ~dust; m = ndimage.binary_opening(m, iterations=2)
        lab, n = ndimage.label(m)
        if n: sz = ndimage.sum(m, lab, range(1, n + 1)); m = lab == (int(np.argmax(sz)) + 1)
    out = a.copy()
    if k in GREEN:
        gm = np.maximum(r, b); out[..., 1] = np.where(g > gm + 10, np.minimum(g, gm + 10), g)
    else:                                                 # khử ám hồng ở viền
        sp = (r > g + 25) & (b > g + 25)
        out[..., 0] = np.where(sp, np.minimum(r, g + 25), r); out[..., 2] = np.where(sp, np.minimum(b, np.maximum(g + 25, b - 40)), b)
    return out, m

K = 0.183                      # 512px → khung game (nhân vật cao ~82px)
FW, FH, AX, GY = 150, 110, 75, 108
CLIP_K = {'run': 1.04, 'aim': 1.06, 'fire': 1.07, 'chg': 0.98, 'jump': 1.2, 'air': 1.33, 'wall': 1.3, 'dash': 0.99, 'hurt': 0.98}
PLAN = {
    'mau': ('idle', 0),
    **{f'i{j+1}': ('idle', i) for j, i in enumerate([0, 8, 16, 24])},
    **{f'r{j+1}': ('run', i) for j, i in enumerate([3, 4, 5, 6, 7, 9, 10, 11])},
    **{f'rb{j+1}': ('runb', i) for j, i in enumerate([0, 1, 2, 4, 5, 6, 7, 9])},
    'b1': ('aim', 0), 'b2': ('idle', 4), 'b3': ('fire', 21), 'b4': ('fire', 24),
    **{f'c{j+1}': ('chg', i) for j, i in enumerate([0, 4, 8, 12])},
    'j1': ('jump', 3), 'j2': ('jump', 7), 'j3': ('jump', 9), 'j4': ('jump', 16), 'j5': ('jump', 23),
    'aj1': ('jump', 4), 'aj2': ('air', 8), 'aj3': ('air', 12), 'aj4': ('air', 16), 'aj5': ('air', 24),
    'd1': ('dash', 3), 'd2': ('dash', 4), 'd3': ('dash', 13),
    'w1': ('wall', 3), 'w2': ('wall', 6), 'w3': ('wall', 11),
}
AIR = {'j2', 'j3', 'j4', 'aj2', 'aj3', 'aj4', 'w1', 'w2', 'w3'}
_clip = {}
def stance_x(m):
    ys, _ = np.where(m); top, bot = ys.min(), ys.max()
    band = m[int(top + (bot - top) * 0.62):bot + 1]
    cols = np.where(band.sum(0) >= 4)[0]
    return float(cols.mean()) if len(cols) else 256.0
def clip_ref(k):
    """Tâm chân (x) và vạch chân (y) chung cho cả clip — camera đứng yên nên dùng chung để khỏi giật."""
    if k in _clip: return _clip[k]
    n = D[NM[k]]['sheet']['frames']; xs, ys = [], []
    for i in range(0, n, 2):
        _, m = key(k, raw(k, i)); yy, _ = np.where(m)
        if not len(yy): continue
        xs.append(stance_x(m)); ys.append(yy.max())
    _clip[k] = (float(np.median(xs)), float(np.median(ys))); return _clip[k]

def build(name, out_dir):
    k, i = PLAN[name]; a, m = key(k, raw(k, i))
    cx, gy = clip_ref(k)
    if name in AIR: gy = float(np.where(m)[0].max()); cx = stance_x(m) if k != 'wall' else cx
    a = a.copy(); a[~m] = (14, 16, 26)
    rgb = Image.fromarray(a.astype(np.uint8)); al = Image.fromarray((m * 255).astype(np.uint8))
    kk = K * CLIP_K.get(k, 1); W = round(512 * kk)
    rgb = rgb.resize((W, W), Image.LANCZOS); al = al.resize((W, W), Image.BOX).point(lambda v: 255 if v > 120 else 0)
    rgb.putalpha(al)
    canvas = Image.new('RGBA', (FW, FH), (0, 0, 0, 0))
    canvas.alpha_composite(rgb, (round(AX - cx * kk), round(GY - gy * kk)))
    canvas.save(f'{out_dir}/f_tv_{name}.png')
    return canvas

if __name__ == '__main__':
    out = sys.argv[1]
    for n in PLAN: build(n, out)
    print(len(PLAN), 'khung')
