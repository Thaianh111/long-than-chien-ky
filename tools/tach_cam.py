# Tách nền cam #FF8C00 từ các khung Gemini, cắt chung khung, thu nhỏ về chiều cao H, lưu PNG
import sys, numpy as np
from PIL import Image
H = int(sys.argv[1]); out = sys.argv[2]; ins = sys.argv[3:]
ims = []
for f in ins:
    a = np.asarray(Image.open(f).convert('RGB')).astype(int)
    r, g, b = a[..., 0], a[..., 1], a[..., 2]
    bg = (r > 200) & (g > 100) & (g < 175) & (b < 80)          # cam
    edge = ((r > 170) & (g > 80) & (b < 110) & (r - b > 120)) | ((r - g > 55) & (r - b > 110))     # viền cam lem
    al = np.where(bg | edge, 0, 255).astype(np.uint8)
    a[al == 0] = (14, 26, 51)                                   # nền -> xanh đêm để viền thu nhỏ không lem cam
    ims.append(np.dstack([a.astype(np.uint8), al]))
ys, xs = np.where(np.max([m[..., 3] for m in ims], axis=0) > 0)
y0, y1, x0, x1 = ys.min(), ys.max() + 1, xs.min(), xs.max() + 1
k = H / (y1 - y0)
for i, m in enumerate(ims):
    im = Image.fromarray(m[y0:y1, x0:x1], 'RGBA')
    W = round(im.width * k)
    rgb = im.convert('RGB').resize((W, H), Image.LANCZOS)
    al = im.getchannel('A').resize((W, H), Image.BOX).point(lambda v: 255 if v > 140 else 0)
    q = rgb.quantize(64, method=Image.Quantize.MEDIANCUT).convert('RGB')
    q.putalpha(al); q.save(f'{out}{i+1}.png')
print(W, H, 'scale', round(k, 3))
