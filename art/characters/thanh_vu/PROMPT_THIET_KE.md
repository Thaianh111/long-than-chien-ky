# Thanh Vũ — thiết kế và prompt hình mẫu

## Thiết kế (bám Style Bible)
- Xạ thủ trẻ của Bạch Đằng Vệ, đồng đội của Xích Long. Dáng nhanh nhẹn, gọn hơn Xích Long; tỉ lệ 5–6 đầu, không chibi.
- Màu: xanh ngọc #1F6F8B (giáp), trắng ngà #F2EBDD (áo lót, khăn), tên năng lượng xanh băng #7FE3FF.
- Họa tiết: lông vũ chim Lạc trên mũ và vai; mặt trời 12 tia trống đồng Đông Sơn trên ngực và trên thân nỏ.
- Vũ khí: **Thần Nỏ** — nỏ thần Cổ Loa phiên bản tương lai, cầm bằng hai tay (KHÔNG gắn vào cánh tay), cò nỏ hình móng Rùa Vàng, cánh nỏ cong như cánh chim Lạc, bắn tên năng lượng.
- Không được giống Mega Man X: không súng gắn tay, không giáp xanh dương tròn kiểu X, không mũ có ngọc giữa trán.
- Nền tách: magenta #FF00FF (vì nhân vật màu xanh).

## Prompt hình mẫu (Gemini) — 1 dáng đứng ngang
```
Original character design for a side-scrolling action game: THANH VU, a young Vietnamese archer-warrior of a future army, agile and slim (5-6 heads tall, not chibi). Full body, side view, standing calm, facing RIGHT, holding a large futuristic crossbow with BOTH hands in front of the chest, pointing forward. Armor in teal jade green (#1F6F8B) with ivory white cloth (#F2EBDD) underneath, light layered plates, a short ivory scarf, a sleek helmet with a dark visor and a crest of Lac bird feathers, Lac bird feather motifs on the shoulder plates, a Dong Son bronze drum 12-ray sun emblem on the chest. The crossbow "Than No": bronze and teal, limbs curved like Lac bird wings, a golden turtle-claw trigger, a 12-ray sun disc on the stock, glowing icy cyan (#7FE3FF) energy string and a loaded cyan energy bolt. The crossbow is held in the hands, NOT attached to the arm. Pure solid magenta #FF00FF background, single character, no shadow, no props.
16-bit SNES-era pixel art, upgraded with a richer palette, crisp clean pixels, 1px dark colored outline, 3-tone cel shading, light from top-left, side view, Vietnamese historical motifs fused with futuristic armor, original character design, no text, no watermark
```

## Hình mẫu đã chốt (2026-11-17)
- `thanh_vu_ngang_goc.png` — đứng nghiêng, nỏ cầm thả xuôi bên hông. Dùng làm hình đính kèm cho MỌI prompt hoạt ảnh (gửi kèm tên `thanh_vu_mau.png`).
- Cắt khung Thanh Vũ: `python3 tools/cat_khung.py <tấm> <ra> <tên> --nen hong --bang art/characters/thanh_vu/frames ...` (bảng màu xanh ngọc riêng, không dùng bảng màu đỏ của Xích Long).

## Các tấm hoạt ảnh cần vẽ (ưu tiên từ trên xuống)
| # | Tấm | Ghi chú |
|---|---|---|
| 1 | ✅ Bắn đứng, 4 khung (`sheets/ban_dung_4_khung_gemini.png`) | giương nỏ → nạp lực → bắn giật → hạ nỏ |
| 2 | Đứng thở, 4 khung | |
| 3 | Chạy 4 khung + vẽ lại đổi chân 4 khung | làm 2 bước như Xích Long cho khỏi thọt |
| 4 | Chạy vừa bắn | |
| 5 | Nhảy 5 khung + bắn trên không | |
| 6 | Lướt 3 khung | |
| 7 | Bám tường + đạp tường + bắn khi bám | |
