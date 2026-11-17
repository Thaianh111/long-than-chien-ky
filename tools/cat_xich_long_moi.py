# Cắt bộ khung Xích Long mới (Sorceress AutoSprite, nền xanh) → khung 150x110 của game.
import json, sys, numpy as np
from PIL import Image
from scipy import ndimage
SRC = '/tmp/sp2'
D = {s['sprite']: s for s in json.load(open(f'{SRC}/sprite_sheets.json'))['sprites']}
NM = {'run':'Side_view_character_runs_to_the_rig','idle':'Character_stands_in_a_ready_stance_f','jump':'Character_crouches_jumps_straight_u',
      'dashA':'Strict_side_profile_view_facing_righ','dashB':'Strict_side_profile_view_facing_righ_2','s1':'Character_pulls_the_katana_back_then',
      's2':'Character_steps_forward_and_swings_t','s3':'Character_raises_the_katana_high_wit','air':'Character_jumps_into_the_air_and_spi',
      'wall':'An_invisible_wall_is_on_the_right','hurt':'Character_gets_hit_from_the_front_r'}
_cache = {}
def raw(k, i):
    s = D[NM[k]]; c = s['sheet']['cols']
    if k not in _cache: _cache[k] = Image.open(f"{SRC}/{s['image']}").convert('RGB')
    return np.asarray(_cache[k].crop(((i % c) * 512, (i // c) * 512, (i % c) * 512 + 512, (i // c) * 512 + 512))).astype(int)
def key(a):
    r, g, b = a[..., 0], a[..., 1], a[..., 2]
    bg = (g > r + 45) & (g > b + 45)                       # mọi sắc xanh lá (cả vạch tường xanh đậm)
    m = ~bg
    m = ndimage.binary_opening(m, iterations=1)
    lab, n = ndimage.label(m)
    if n:
        sz = ndimage.sum(m, lab, range(1, n + 1)); m = np.isin(lab, [i + 1 for i, s in enumerate(sz) if s > 60])
    out = a.copy()
    gmax = np.maximum(r, b); spill = g > gmax + 10        # khử ám xanh ở viền
    out[..., 1] = np.where(spill, np.minimum(g, gmax + 10), g)
    return out.astype(np.uint8), m
def body_mask(a, m):
    r, g, b = a[..., 0], a[..., 1], a[..., 2]
    glow = (r > 200) & (g > 150) & (b < 140)                # vệt sáng vàng cam của lưỡi đao
    return m & ~glow

K = 0.18                       # 512px → khung game (nhân vật cao ~88px)
FW, FH, AX, GY = 150, 110, 75, 108
AIR = {'jump2', 'jump3', 'jump4', 'as1', 'as2', 'as3', 'as4', 'as5', 'w1', 'w2', 'w3'}
PLAN = {
    'id1': ('idle', 0), 'id2': ('idle', 8), 'id3': ('idle', 16), 'id4': ('idle', 24),
    **{f'run{j+1}': ('run', i) for j, i in enumerate([0, 2, 5, 7, 10, 12, 14, 17])},
    'jump1': ('jump', 6), 'jump2': ('run', 2), 'jump3': ('wall', 4), 'jump4': ('wall', 0), 'jump5': ('jump', 7),
    'dash1': ('dashA', 0), 'dash2': ('dashB', 4), 'dash3': ('dashA', 5),
    's1_1': ('s1', 9), 's1_2': ('s1', 3), 's1_3': ('s1', 1), 's1_4': ('s1', 0), 's1_5': ('s3', 2),
    's2_1': ('s2', 7), 's2_2': ('s2', 9), 's2_3': ('s2', 11), 's2_4': ('s1', 4), 's2_5': ('s1', 3),
    's3_1': ('s3', 7), 's3_2': ('s3', 8), 's3_3': ('s3', 6), 's3_4': ('s3', 5), 's3_5': ('s3', 4), 's3_6': ('s3', 2),
    'as1': ('air', 7), 'as2': ('air', 8), 'as3': ('air', 9), 'as4': ('air', 10), 'as5': ('air', 11),
    'w1': ('wall', 2), 'w2': ('wall', 6), 'w3': ('wall', 27),
}
MIRROR = {'w3'}
CLIP_K = {'wall': 1.38, 'air': 1.22, 'jump': 1.12}   # clip quay xa hơn nên nhân vật nhỏ hơn — phóng lại cho cùng cỡ
_clip = {}
def clip_ref(k):
    """Tâm thân (x) và vạch chân (y) chung cho cả clip — camera đứng yên nên dùng chung để khỏi giật."""
    if k in _clip: return _clip[k]
    n = D[NM[k]]['sheet']['frames']; xs, ys = [], []
    for i in range(0, n, 2):
        a, m = key(raw(k, i)); bm = body_mask(a, m); yy, _ = np.where(bm)
        if not len(yy): continue
        top, bot = yy.min(), yy.max(); band = bm[int(top + (bot - top) * 0.3):int(top + (bot - top) * 0.6)]
        cols = np.where(band.sum(0) >= 8)[0]
        if len(cols): xs.append(cols.mean())
        ys.append(bot)
    _clip[k] = (float(np.median(xs)), float(np.median(ys))); return _clip[k]

def build(name, out_dir):
    k, i = PLAN[name]; a, m = key(raw(k, i)); bm = body_mask(a, m)
    cx, gy = clip_ref(k)
    if name in AIR: gy = float(np.where(bm)[0].max())          # khung trên không: chân là đáy thân chính khung đó
    a = a.copy(); a[~m] = (14, 16, 26)
    rgb = Image.fromarray(a[..., :3].astype(np.uint8)); al = Image.fromarray((m * 255).astype(np.uint8))
    kk = K * CLIP_K.get(k, 1); W = round(512 * kk)
    rgb = rgb.resize((W, W), Image.LANCZOS); al = al.resize((W, W), Image.BOX).point(lambda v: 255 if v > 120 else 0)
    rgb.putalpha(al)
    if name in MIRROR: rgb = rgb.transpose(Image.FLIP_LEFT_RIGHT); cx = 512 - cx
    canvas = Image.new('RGBA', (FW, FH), (0, 0, 0, 0))
    canvas.alpha_composite(rgb, (round(AX - cx * kk), round(GY - gy * kk)))
    canvas.save(f'{out_dir}/f_xl_{name}.png')
    return canvas

if __name__ == '__main__':
    out = sys.argv[1]
    for n in PLAN: build(n, out)
    print(len(PLAN), 'khung')
