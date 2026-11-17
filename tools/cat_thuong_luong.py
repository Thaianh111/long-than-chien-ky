# Cắt hoạt ảnh bơi Thuồng Luồng (Sorceress AutoSprite, nền cam) → khung tl_swim1..16 cho game.
# Hình gốc quay đầu sang TRÁI. In ra hộp chung và vị trí đầu từng khung để ghi vào frames.json.
import json, sys, numpy as np
from PIL import Image
from scipy import ndimage
SRC, OUT = sys.argv[1], sys.argv[2]
S = json.load(open(f'{SRC}/sprite_sheets.json'))['sprites'][0]
sheet = Image.open(f"{SRC}/{S['image']}").convert('RGB'); c = S['sheet']['cols']; FWs, FHs = S['frameWidth'], S['frameHeight']
K = 0.85
PICK = list(range(0, 31, 2))
def frame(i):
    a = np.asarray(sheet.crop(((i % c) * FWs, (i // c) * FHs, (i % c) * FWs + FWs, (i // c) * FHs + FHs))).astype(int)
    r, g, b = a[..., 0], a[..., 1], a[..., 2]
    bg = (r > 200) & (g > 90) & (g < 185) & (b < 90) & (r - g > 55)       # nền cam
    m = ndimage.binary_opening(~bg, iterations=1)
    lab, n = ndimage.label(m)
    if n: sz = ndimage.sum(m, lab, range(1, n + 1)); m = np.isin(lab, [j + 1 for j, s in enumerate(sz) if s > 150])
    out = a.copy(); warm = (r > g + 40) & (r > b + 60) & m                 # khử ám cam ở viền
    out[..., 0] = np.where(warm, (r + g) // 2, r)
    return out.astype(np.uint8), m
fr = [frame(i) for i in PICK]
ys, xs = np.where(np.any([m for _, m in fr], axis=0))
y0, y1, x0, x1 = ys.min() - 2, ys.max() + 3, max(0, xs.min() - 2), min(FWs, xs.max() + 3)
W, H = round((x1 - x0) * K), round((y1 - y0) * K)
heads = []
for j, (a, m) in enumerate(fr):
    rgba = np.dstack([a, (m * 255).astype(np.uint8)])[y0:y1, x0:x1]
    im = Image.fromarray(rgba, 'RGBA'); al = im.getchannel('A').resize((W, H), Image.BOX).point(lambda v: 255 if v > 110 else 0)
    im = im.convert('RGB').resize((W, H), Image.LANCZOS); im.putalpha(al); im.save(f'{OUT}/f_tl_swim{j + 1}.png')
    mm = np.asarray(al) > 0; cols = np.where(mm.any(0))[0]; sx = cols.min()
    hy = np.where(mm[:, sx:sx + 40].any(1))[0]; heads.append([int(sx), int((hy.min() + hy.max()) / 2)])
    top = [int(np.where(mm[:, x])[0].min()) if mm[:, x].any() else H for x in range(0, W, 8)]
print(json.dumps({'W': W, 'H': H, 'heads': heads}))
