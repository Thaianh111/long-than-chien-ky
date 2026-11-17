"""Cắt một tấm hình nhiều dáng (nền xanh lá #00FF00) thành các khung pixel 150x110 cho Long Thần Chiến Ký.

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
a = ap.parse_args()
FW, FH = 150, 110
here = os.path.dirname(os.path.abspath(__file__))
REF = os.path.join(here, '..', 'prototype', 'dieu-khien', 'frames')

def key_green(im):
    arr = np.array(im.convert('RGBA')).astype(int)
    r, g, b = arr[..., 0], arr[..., 1], arr[..., 2]
    green = (g > 140) & (g > r * 1.35) & (g > b * 1.35)
    arr[..., 3] = np.where(green, 0, 255)
    # bỏ viền xanh lem: kéo kênh G xuống nếu còn ám xanh
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
        if sizes[k - 1] < big * 0.15 and (k in edge or sizes[k - 1] < 6):
            arr[lab == k, 3] = 0
    return Image.fromarray(arr, 'RGBA')

sheet = key_green(Image.open(a.sheet))
W, H = sheet.size; cw, ch = W // a.cot, H // a.hang
cells = []
for j in range(a.hang):
    for i in range(a.cot):
        c = clean(sheet.crop((i * cw, j * ch, (i + 1) * cw, (j + 1) * ch)))
        bb = c.getchannel('A').point(lambda v: 255 if v > 128 else 0).getbbox()
        if bb: cells.append((c, bb))
tallest = max(bb[3] - bb[1] for _, bb in cells)
ground = max(bb[3] for _, bb in cells)            # đường đất chung của cả tấm
scale = a.cao / tallest
pal = palette()
os.makedirs(a.outdir, exist_ok=True)
outs = []
for k, (c, bb) in enumerate(cells, 1):
    x0, x1 = bb[0], bb[2]
    part = c.crop((x0, 0, x1, ground))
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
