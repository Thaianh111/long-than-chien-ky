"""Cắt một tấm hình nhiều dáng (nền xanh lá #00FF00, hoặc magenta #FF00FF với --nen hong) thành các khung pixel 150x110 cho Long Thần Chiến Ký.

Cách dùng:
    python3 tools/cat_khung.py <anh_tam.png> <thu_muc_ra> <ten> [--cao 84] [--hang 1] [--cot 8]

- Tấm hình phải xếp dáng thành lưới đều (mặc định 1 hàng x 8 cột), nhân vật quay mặt sang PHẢI,
  chân chạm một đường đất chung.
- --cao: chiều cao (px) của nhân vật khi đứng thẳng sau khi thu nhỏ. Mọi khung trong tấm dùng CHUNG
  một tỉ lệ (tính từ ô cao nhất) để nhân vật không phình xẹp giữa các khung.
- Màu được ép về bảng màu lấy từ các khung đã có (prototype/dieu-khien/frames) cho đồng bộ.
- Ra file <ten>1.png, <ten>2.png... và <ten>_xem.png (phóng to, có lưới 10px) để chấm điểm neo hông.
"""
import sys, glob, os, argparse
import numpy as np
from PIL import Image, ImageDraw

ap = argparse.ArgumentParser()
ap.add_argument('sheet'); ap.add_argument('outdir'); ap.add_argument('name')
ap.add_argument('--cao', type=int, default=84); ap.add_argument('--hang', type=int, default=1)
ap.add_argument('--cot', type=int, default=8)
ap.add_argument('--theo', type=int, default=0, help='lấy chiều cao của khung số K (đứng thẳng, không giơ đao) để tính tỉ lệ; 0 = khung cao nhất')
ap.add_argument('--tile', type=float, default=0, help='tỉ lệ thu nhỏ đặt thẳng (vd 0.19), bỏ qua --cao/--theo')
ap.add_argument('--rieng', action='store_true', help='mỗi khung tự đặt chân chạm đáy (dùng cho dáng nhảy ở các độ cao khác nhau)')
ap.add_argument('--nen', default='xanh', choices=['xanh', 'hong'], help='màu nền tách: xanh = #00FF00, hong = magenta #FF00FF (nhân vật màu xanh như Thanh Vũ)')
ap.add_argument('--tach', action='store_true', help='tách theo hình dáng (khi các dáng chạm/lấn nhau) thay vì chia lưới đều')
a = ap.parse_args()
FW, FH = 150, 110
here = os.path.dirname(os.path.abspath(__file__))
REF = os.path.join(here, '..', 'prototype', 'dieu-khien', 'frames')

def key_green(im):
    arr = np.array(im.convert('RGBA')).astype(int)
    r, g, b = arr[..., 0], arr[..., 1], arr[..., 2]
    if a.nen == 'hong':
        green = (r > 140) & (b > 140) & (r > g * 1.6) & (b > g * 1.6)
        green |= (r > 170) & (b > 170) & (g < r - 40) & (g < b - 40)   # viền hồng nhạt
    else:
        green = (g > 140) & (g > r * 1.35) & (g > b * 1.35)
        green |= (g > 150) & (g >= r + 12) & (g >= b + 12)   # ngôi sao mờ của Gemini và viền xanh nhạt
    arr[..., 3] = np.where(green, 0, 255)
    # bỏ viền xanh lem: kéo kênh G xuống nếu còn ám xanh
    if a.nen == 'hong':
        spill = (r > g + 30) & (b > g + 30) & ~green           # bỏ ám hồng ở viền
        arr[..., 0] = np.where(spill, np.minimum(r, g + 30), r); arr[..., 2] = np.where(spill, np.minimum(b, g + 60), b)
    else:
        spill = (g > r) & (g > b) & ~green
        arr[..., 1] = np.where(spill, np.maximum(r, b), g)
    return Image.fromarray(arr.astype('uint8'), 'RGBA')

def palette():
    imgs = [Image.open(f).convert('RGBA') for f in sorted(glob.glob(os.path.join(REF, '*.png')))]
    px = np.concatenate([np.array(i).reshape(-1, 4) for i in imgs])
    px = px[px[:, 3] > 128][:, :3]
    pal = Image.fromarray(px.reshape(1, -1, 3).astype('uint8'), 'RGB').quantize(48, method=Image.Quantize.MEDIANCUT)
    return pal

def clean(c):
    # bỏ mảnh lấn từ ô bên cạnh: mảnh chạm mép ô mà nhỏ hơn 15% mảnh lớn nhất
    from scipy import ndimage
    arr = np.array(c); m = arr[..., 3] > 128
    lab, n = ndimage.label(m, structure=np.ones((3, 3)))
    if n == 0: return c
    sizes = ndimage.sum(m, lab, range(1, n + 1)); big = sizes.max()
    edge = set(np.unique(np.concatenate([lab[0], lab[-1], lab[:, 0], lab[:, -1]]))) - {0}
    for k in range(1, n + 1):
        if (sizes[k - 1] < big * 0.15 and k in edge) or sizes[k - 1] < big * 0.01:
            arr[lab == k, 3] = 0
    return Image.fromarray(arr, 'RGBA')

sheet = key_green(Image.open(a.sheet))
W, H = sheet.size
cells = []
if a.tach:
    # co mặt nạ vài px cho các dáng dính nhau tách ra, lấy các mảng lớn làm "hạt",
    # rồi gán mỗi điểm ảnh về hạt gần nhất -> mỗi dáng một hình riêng
    from scipy import ndimage
    arr = np.array(sheet); m = arr[..., 3] > 128
    for it in range(2, 30):
        er = ndimage.binary_erosion(m, iterations=it)
        lab, n = ndimage.label(er, structure=np.ones((3, 3)))
        sizes = ndimage.sum(er, lab, range(1, n + 1))
        seeds = [k + 1 for k in range(n) if sizes[k] > sizes.max() * 0.25]
        if len(seeds) >= a.hang * a.cot: break
    seedmap = np.zeros_like(lab)
    for i, k in enumerate(seeds, 1): seedmap[lab == k] = i
    _, (iy, ix) = ndimage.distance_transform_edt(seedmap == 0, return_indices=True)
    owner = seedmap[iy, ix] * m
    order = sorted(range(1, len(seeds) + 1), key=lambda i: np.nonzero(seedmap == i)[1].mean())
    for i in order:
        one = arr.copy(); one[..., 3] = np.where(owner == i, arr[..., 3], 0)
        c = clean(Image.fromarray(one, 'RGBA'))
        bb = c.getchannel('A').point(lambda v: 255 if v > 128 else 0).getbbox()
        cells.append((c, bb))
else:
    cw, ch = W // a.cot, H // a.hang
    for j in range(a.hang):
        for i in range(a.cot):
            c = clean(sheet.crop((i * cw, j * ch, (i + 1) * cw, (j + 1) * ch)))
            bb = c.getchannel('A').point(lambda v: 255 if v > 128 else 0).getbbox()
            if bb: cells.append((c, bb))
tallest = (cells[a.theo - 1][1][3] - cells[a.theo - 1][1][1]) if a.theo else max(bb[3] - bb[1] for _, bb in cells)
ground = max(bb[3] for _, bb in cells)            # đường đất chung của cả tấm
scale = a.tile or a.cao / tallest
pal = palette()
os.makedirs(a.outdir, exist_ok=True)
outs = []
for k, (c, bb) in enumerate(cells, 1):
    x0, x1 = bb[0], bb[2]
    part = c.crop((x0, bb[1] if a.rieng else 0, x1, bb[3] if a.rieng else ground))
    nw, nh = max(1, round(part.width * scale)), max(1, round(part.height * scale))
    small = part.resize((nw, nh), Image.LANCZOS)
    alpha = small.getchannel('A').point(lambda v: 255 if v > 110 else 0)
    rgb = small.convert('RGB').quantize(palette=pal, dither=Image.Dither.NONE).convert('RGB')
    rgb.putalpha(alpha)
    fr = Image.new('RGBA', (FW, FH))
    fr.alpha_composite(rgb, ((FW - nw) // 2, FH - 2 - nh))
    p = os.path.join(a.outdir, f'{a.name}{k}.png'); fr.save(p); outs.append(fr)
    print(p)
s = 4
view = Image.new('RGBA', (FW * s * len(outs), FH * s), (40, 40, 60, 255)); d = ImageDraw.Draw(view)
for k, fr in enumerate(outs):
    view.alpha_composite(fr.resize((FW * s, FH * s), Image.NEAREST), (k * FW * s, 0))
    for x in range(0, FW + 1, 10): d.line([(k * FW * s + x * s, 0), (k * FW * s + x * s, FH * s)], fill=(255, 255, 255, 50))
    for y in range(0, FH + 1, 10): d.line([(k * FW * s, y * s), ((k + 1) * FW * s, y * s)], fill=(255, 255, 255, 50))
view.save(os.path.join(a.outdir, f'{a.name}_xem.png'))
