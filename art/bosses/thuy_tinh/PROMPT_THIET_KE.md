# Thủy Tinh (水精) — trùm màn 1 Hồng Thủy · thiết kế và prompt Gemini

## Thiết kế (bám Style Bible)
- Tâm Cơ chiến binh nước bị Ma Chủng chiếm lõi, chỉ huy trận lũ Thiên Lý gây ra. To cao hơn Xích Long khoảng 1,6 lần (~128 px), dáng thủy thần oai vệ.
- Màu: xanh biển sâu #1B4F8A (giáp), xanh ngọc nước #3FB8C4 (viền, sóng), trắng bọt #E8F6F8 (bờm sóng), lõi nước ở ngực xanh sáng #8FF0FF. Vết nhiễm Ma Chủng: nứt tím #6B2FA0 / #B57CFF.
- Họa tiết: vảy cá chép hóa rồng, sóng nước kiểu hoa văn cổ Việt trên vai và váy giáp; mũ có mào hình con sóng cuộn; râu/tóc dài như bọt sóng bay ngược ra sau.
- Vũ khí: **đinh ba** (cây chĩa 3 mũi) dài hơn người, mũi bằng đồng xanh, cán quấn rong biển phát sáng.
- Không giống trùm Mega Man nào; không dùng chữ, không logo.
- Nền tách: **cam đặc #FF8C00** (nhân vật không có màu cam).

## Bước 1 — hình mẫu đứng (gửi Gemini trước, chốt rồi mới làm hoạt ảnh)
```
Original boss character design for a side-scrolling action game: THUY TINH, a towering water-god warrior robot from a Vietnamese legend, reimagined in the year 2189, corrupted by a purple virus. Full body, side view, standing in a powerful stance, facing LEFT, holding a long bronze-green trident (three prongs) upright in the right hand. Heavy armor in deep sea blue (#1B4F8A) with aqua teal trims (#3FB8C4), carp-dragon scale patterns and ancient Vietnamese wave motifs on the shoulders and armored skirt, a helmet crest shaped like a curling wave, long foam-white hair and beard flowing backward like sea spray (#E8F6F8), a glowing round water-orb core in the chest (#8FF0FF). Glowing purple cracks (#6B2FA0, #B57CFF) spread from the chest core across the armor. Imposing, about 1.6 times taller than a normal hero. Pure solid flat orange #FF8C00 background, single character, no shadow, no water splash, no props.
16-bit SNES-era pixel art, upgraded with a richer palette, crisp clean pixels, 1px dark colored outline, 3-tone cel shading, light from top-left, side view, Vietnamese mythological motifs fused with futuristic armor, original character design, no text, no watermark
```

## Bước 2 — tấm hoạt ảnh (đính kèm hình mẫu đã chốt, tên `thuy_tinh_mau.png`)
Mỗi tấm: cùng nhân vật y hệt hình mẫu, quay mặt sang TRÁI, xếp 1 hàng ngang, cách đều, cùng cỡ, nền cam đặc #FF8C00, không bóng, không chữ.

| # | Tấm | Prompt ngắn (thêm vào đầu, giữ nguyên dòng style ở trên) |
|---|---|---|
| 1 | Đứng thở 4 khung | `4-frame idle breathing animation sprite sheet of the attached character, trident upright, hair flowing, chest core pulsing` |
| 2 | Lao đinh ba 4 khung | `4-frame attack sprite sheet: crouch back with trident (wind-up), lunge forward, full thrust with trident pointing LEFT, recover` |
| 3 | Gọi sóng 4 khung | `4-frame spell sprite sheet: raise trident high over head, glow, slam trident butt down to the ground, hold pose` |
| 4 | Nhảy cao 3 khung | `3-frame jump sprite sheet: crouch, leap upward with trident held overhead, fall pose pointing trident down` |
| 5 | Bị đánh + gầm 2 khung | `2-frame sprite sheet: recoil hurt pose, then roaring with arms wide and purple cracks glowing brighter` |
| 6 | Gục ngã 3 khung | `3-frame defeat sprite sheet: staggering, kneeling with trident stuck in the ground, collapsing with armor cracking purple light` |

Hiệu ứng nước (sóng, bong bóng, mưa giáo nước) em vẽ bằng code nên anh không cần vẽ.
