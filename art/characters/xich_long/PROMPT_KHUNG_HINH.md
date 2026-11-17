# Prompt vẽ thêm khung hình cho Xích Long

Bản thử hiện mới có **8 dáng**, mỗi hành động chỉ có 1 hình. Muốn chạy, nhảy và chém mượt như Mega Man X thì mỗi hành động cần nhiều khung hình liên tiếp. Bảng dưới là các tấm hình cần vẽ, xếp theo thứ tự ưu tiên.

**Cách làm:** mở Gemini, đính kèm ảnh mẫu `xich_long_ngang_dao_laze_goc.png`, dán prompt của tấm cần vẽ rồi gửi hình nhận được cho Claude. Claude cắt bằng `tools/cat_khung.py` (thêm `--tach` khi các dáng dính nhau) rồi ghép vào game.

## Phần dán chung (để ĐẦU mọi prompt)
```
Use the attached character as the exact reference: same red-and-gold Vietnamese dragon armor, horned helmet, black face mask, red coat tails, glowing orange laser saber with a gold dragon hilt held in the right hand. Keep identical proportions, colors and outfit in every frame. Sprite sheet on a pure #00FF00 green background, frames evenly spaced in ONE horizontal row, every frame the same size, character facing RIGHT, full body visible, all feet on the same ground line, no overlap between frames, no shadows on the background.
```

## Các tấm cần vẽ (ưu tiên từ trên xuống)
| # | Tấm | Prompt riêng (dán sau phần chung) |
|---|---|---|
| 1 | ✅ Chạy, 8 khung (đã có, `sheets/chay_8_khung_gemini.png`) | `8-frame side-view run cycle loop: contact, down, passing, up, contact (other leg), down, passing, up. Saber held low behind, coat tails flowing back. Smooth, evenly spaced motion.` |
| 2 | ✅ Nhảy, 5 khung (đã có, `sheets/nhay_5_khung_gemini.png`) | `5 frames: crouch before jump, rising (legs tucked), apex, falling (legs reaching down), landing crouch.` |
| 3 | ✅ Lướt, 3 khung (đã có, `sheets/luot_3_khung_gemini.png`) | `3 frames of a ground dash: push-off lean, full forward dash low to the ground with coat streaming back, braking stop with heels skidding.` |
| 4 | ✅ Chém nhát 1, 5 khung (đã có, `sheets/chem1_5_khung_gemini.png`) | `5-frame downward diagonal saber slash: anticipation (saber raised behind head), start of swing, mid-swing with motion smear, saber low in front, recovery.` |
| 5 | ✅ Chém nhát 2, 5 khung (đã có, `sheets/chem2_5_khung_gemini.png`) | `5-frame upward saber slash: saber low in front, start rising, mid-swing with motion smear, saber high above head, recovery.` |
| 6 | ✅ Chém nhát 3, 6 khung (đã có, `sheets/chem3_6_khung_gemini.png`) | `6-frame powerful finishing slash with a forward lunge: deep crouch anticipation, explosive step forward, wide horizontal swing with big smear, follow-through, hold, recovery.` |
| 7 | ✅ Bám tường + đạp tường, 3 khung (đã có, `sheets/bam_tuong_3_khung_gemini.png`; Gemini vẽ kèm tường nên đã xóa tường) | `3 frames: sliding down a wall on the RIGHT side (back to the wall, one hand and foot against it, sparks under the foot), kicking off the wall, jumping away.` |
| 8 | ✅ Đứng thở, 4 khung (đã có, `sheets/dung_tho_4_khung_gemini.png`) | `4-frame idle breathing loop, subtle chest and coat-tail movement, saber resting low.` |
| 9 | ✅ Chém trên không, 5 khung (đã có, `sheets/chem_tren_khong_5_khung_gemini.png`) | `5-frame airborne spinning saber slash, body tucked, full 360 degree swing with circular smear.` |

## Câu khóa phong cách (để CUỐI mọi prompt, lấy từ Style Bible)
```
16-bit SNES-era pixel art, upgraded with a richer palette, crisp clean pixels, 1px dark colored outline, 3-tone cel shading, light from top-left, side view, Vietnamese historical motifs fused with futuristic armor, original character design, no text, no watermark
```

## Ghi chú
- Gemini hay vẽ mỗi khung một cỡ khác nhau. Gặp vậy cứ gửi cho Claude: công cụ cắt sẽ thu mọi khung về cùng một tỉ lệ, rồi Claude chấm lại điểm neo hông cho từng khung.
- Tấm nào ra hỏng (lệch đồ, mất đao) thì vẽ lại riêng tấm đó, không cần vẽ lại cả bộ.
