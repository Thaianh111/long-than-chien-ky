# Cắt các tấm Gemini của trùm (nền cam hoặc nền trong suốt) thành từng khung RGBA cỡ gốc
import sys, numpy as np
from PIL import Image
from scipy import ndimage
def key(img):
    a = np.asarray(img.convert('RGBA')).astype(int)
    r, g, b, al = a[..., 0], a[..., 1], a[..., 2], a[..., 3]
    orange = (r > 200) & (g > 95) & (g < 185) & (b < 90)
    glow = (r - b > 90) & (r > 180) & (r - g > 35)            # quầng sáng pha cam
    dark_line = (r < 60) & (g < 50) & (b < 45)                # đường kẻ chia ô
    m = (al > 128) & ~orange & ~glow
    return a.astype(np.uint8), m, dark_line
def frames(path, boxes):
    im = Image.open(path); out = []
    for (x0, y0, x1, y1) in boxes:
        a, m, _ = key(im.crop((x0, y0, x1, y1)))
        m = ndimage.binary_opening(m, iterations=1)
        lab, n = ndimage.label(m); sizes = ndimage.sum(m, lab, range(1, n + 1))
        keep = np.isin(lab, [i + 1 for i, s in enumerate(sizes) if s > 400])   # bỏ hạt vụn
        ys, xs = np.where(keep); a = a.copy(); a[..., 3] = np.where(keep, 255, 0)
        a[~keep, :3] = (14, 26, 51)
        out.append(Image.fromarray(a[ys.min():ys.max() + 1, xs.min():xs.max() + 1], 'RGBA'))
    return out
def save(fr, scale, name, foot=None):
    for i, f in enumerate(fr, 1):
        W, H = max(1, round(f.width * scale)), max(1, round(f.height * scale))
        rgb = f.convert('RGB').resize((W, H), Image.LANCZOS)
        al = f.getchannel('A').resize((W, H), Image.BOX).point(lambda v: 255 if v > 140 else 0)
        q = rgb.quantize(64, method=Image.Quantize.MEDIANCUT).convert('RGB'); q.putalpha(al); q.save(f'{name}{i}.png')
def by_components(path, n, minsz=300):
    im = Image.open(path); a, m, _ = key(im); m = ndimage.binary_opening(m)
    lab, k = ndimage.label(m); sl = ndimage.find_objects(lab); sz = ndimage.sum(m, lab, range(1, k + 1))
    mains = sorted(list(np.argsort(sz)[::-1][:n]), key=lambda i: sl[i][1].start)
    groups = {i: [i] for i in mains}
    for j in range(k):
        if j in mains or sz[j] < minsz: continue
        cxj = (sl[j][1].start + sl[j][1].stop) / 2
        best = min(mains, key=lambda i: abs((sl[i][1].start + sl[i][1].stop) / 2 - cxj))
        groups[best].append(j)
    out = []
    for i in mains:
        keep = np.isin(lab, [g + 1 for g in groups[i]]); ys, xs = np.where(keep)
        b = a.copy(); b[..., 3] = np.where(keep, 255, 0); b[~keep, :3] = (14, 26, 51)
        out.append(Image.fromarray(b[ys.min():ys.max() + 1, xs.min():xs.max() + 1], 'RGBA'))
    return out
def core(f):
    a = np.asarray(f).astype(int); r, g, b, al = a[..., 0], a[..., 1], a[..., 2], a[..., 3]
    c = (al > 0) & (b > 235) & (g > 225) & (r > 150) & (r < 235)
    lab, k = ndimage.label(c)
    if not k: return None
    sz = ndimage.sum(c, lab, range(1, k + 1)); i = int(np.argmax(sz)) + 1
    y, x = ndimage.center_of_mass(lab == i); return round(x), round(y), int(sz[i - 1])
