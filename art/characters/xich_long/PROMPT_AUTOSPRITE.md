# Xích Long (赤龍) — bộ prompt Sorceress AutoSprite (vẽ lại model + đủ động tác)

Làm theo thứ tự: **Bước 1 vẽ model gốc** (GPT Image 2, khung 1:1, bật GREEN SCREEN) → ưng rồi bấm **ANIMATE** trên ảnh đó cho từng động tác ở Bước 2.
Số khung mỗi động tác khớp đúng với code game (`Resources/Data/frames.json`), nên làm đúng số khung thì em ghép vào là chạy.

## Lưu ý thiết kế (đọc trước khi vẽ)
Model cũ có **tóc vàng dài buộc sau lưng**. Đây là nét nhận diện của Zero (Mega Man X), file cốt truyện đã ghi phải tránh, vì để vậy dễ bị coi là nhái.
Model mới thay bằng **mũ trụ sừng rồng + dải lụa đỏ dài bay sau lưng**: vẫn có "đuôi" bay phấp phới khi chạy và chém như Zero, nhưng là của riêng Xích Long.

---

## Bước 1 — Model gốc
```
Original character design for a 2D side-scrolling action game: XICH LONG, a crimson dragon-armored cyber samurai from futuristic Vietnam (year 2189). Full body, side view facing RIGHT, standing in a ready stance, katana held low in the right hand. Tall and lean, upright warrior posture. Helmet shaped like a war helm with two small dragon horns swept backward, a mask covering the lower half of the face, glowing golden eyes. Vermilion red armor (#B3261E) with bronze-gold trims (#C9A227); Ly-dynasty Vietnamese dragon scale pattern (sinuous dragon body) running from the shoulders down the arms. One very long red silk sash flowing from the back of the waist. Weapon: Long Nha Dao, a single-edged curved blade with engraved scales on the spine and a glowing orange-red edge. No long hair, no ponytail, no gem on the forehead. 16-bit SNES-era pixel art, upgraded richer palette, crisp clean pixels, 1px dark colored outline, 3-tone cel shading, light from top-left. Pure flat green background #00FF00, single character, no shadow, no text, no watermark.
```

---

## Bước 2 — Động tác (bấm ANIMATE trên model gốc, dán từng prompt)

**Câu chung**: đã gắn sẵn ở cuối mỗi prompt, đừng xóa.
> Same character as the reference, side view facing right, same size and scale in every frame, feet on the same ground line, frames evenly spaced in one row. Pure flat green background #00FF00, no shadow, no text.

### 1. Đứng thở — 4 khung (lặp)
```
Idle breathing loop, 4 frames. Ready stance, knees slightly bent, katana held low pointing forward-down, blade edge glowing softly. Subtle chest rise and fall, red sash sways gently. Same character as the reference, side view facing right, same size and scale in every frame, feet on the same ground line, frames evenly spaced in one row. Pure flat green background #00FF00, no shadow, no text.
```

### 2. Chạy ninja — 8 khung (lặp)
```
Ninja sprint loop, 8 frames. Low aggressive run: torso leaning strongly forward about 40 degrees, head low, knees deeply bent, long strides close to the ground. Right hand holds the katana reversed, blade pointing backward along the arm; left arm swings slightly behind. Red sash streams straight back from speed. Left and right legs clearly alternate with an even rhythm, no limping, no upright posture. Same character as the reference, side view facing right, same size and scale in every frame, feet on the same ground line, frames evenly spaced in one row. Pure flat green background #00FF00, no shadow, no text.
```

### 3. Nhảy — 5 khung
```
Jump sequence, 5 frames: 1 crouch to take off, 2 rising with knees tucked and katana held back, 3 apex with body compact, 4 falling with legs extending down, 5 landing crouch absorbing impact. Red sash follows the motion. Same character as the reference, side view facing right, same size and scale in every frame, frames evenly spaced in one row. Pure flat green background #00FF00, no shadow, no text.
```

### 4. Lướt (dash) — 3 khung
```
Ground dash, 3 frames: 1 explosive push-off, body low and leaning far forward, back foot kicking off; 2 full-speed dash, body almost horizontal, katana held back, red sash stretched straight behind; 3 braking stop, heel sliding forward, body leaning back. Same character as the reference, side view facing right, same size and scale in every frame, feet on the same ground line, frames evenly spaced in one row. Pure flat green background #00FF00, no shadow, no text.
```

### 5. Chém nhát 1 — 5 khung
```
Sword attack 1, fast horizontal slash, 5 frames: 1 wind-up with blade pulled back, 2 slash starting, 3 blade sweeping forward at chest height with a wide glowing orange half-moon trail, 4 follow-through, 5 recover to ready stance. Same character as the reference, side view facing right, same size and scale in every frame, feet on the same ground line, frames evenly spaced in one row. Pure flat green background #00FF00, no shadow, no text.
```

### 6. Chém nhát 2 — 5 khung
```
Sword attack 2, rising upward slash, 5 frames: 1 blade low behind, 2 stepping forward, 3 blade swinging from low to high in a big glowing orange crescent arc, 4 blade overhead, 5 recover. Same character as the reference, side view facing right, same size and scale in every frame, feet on the same ground line, frames evenly spaced in one row. Pure flat green background #00FF00, no shadow, no text.
```

### 7. Chém nhát 3 (kết liễu) — 6 khung
```
Sword attack 3, heavy finishing overhead slash with a lunge, 6 frames: 1 blade raised high with both hands, 2 leaping forward, 3 slashing down hard with a huge glowing orange half-moon trail, 4 blade hits the ground level with sparks, 5 low crouch hold, 6 recover. Same character as the reference, side view facing right, same size and scale in every frame, feet on the same ground line, frames evenly spaced in one row. Pure flat green background #00FF00, no shadow, no text.
```

### 8. Chém xoay trên không — 5 khung
```
Aerial spinning sword attack, 5 frames: the character is mid-air, body curled and spinning forward like a wheel, katana extended outward drawing a full glowing orange circle around the body; show the spin at different angles in each frame. Red sash wraps around with the spin. Same character as the reference, side view facing right, same size and scale in every frame, frames evenly spaced in one row. Pure flat green background #00FF00, no shadow, no text.
```

### 9. Bám tường + đạp tường — 3 khung
```
Wall moves, 3 frames, a wall is on the RIGHT side but do NOT draw the wall: 1 sliding down the wall, right hand and right foot pressed against it, body facing the wall, sparks at the foot; 2 crouching against the wall ready to kick off; 3 kicking off the wall, leaping away to the left with legs extended. Same character as the reference, side view, same size and scale in every frame, frames evenly spaced in one row. Pure flat green background #00FF00, no shadow, no text, no wall.
```

### 10. Bị trúng đòn — 2 khung (mới)
```
Hurt reaction, 2 frames: 1 recoiling backward from a hit, head snapped back, arms flung, armor flashing; 2 staggering, one knee bending, katana still in hand. Same character as the reference, side view facing right, same size and scale in every frame, feet on the same ground line, frames evenly spaced in one row. Pure flat green background #00FF00, no shadow, no text.
```

### 11. Xuất hiện / chiến thắng — 3 khung (mới)
```
Teleport-in and victory pose, 3 frames: 1 landing in a low crouch with one hand on the ground, katana held back; 2 rising up; 3 standing proud, swinging the katana once to the side, red sash flowing. Same character as the reference, side view facing right, same size and scale in every frame, feet on the same ground line, frames evenly spaced in one row. Pure flat green background #00FF00, no shadow, no text.
```

---

## Mẹo
- Động tác nào ra sai thì làm lại riêng động tác đó, không cần làm lại cả bộ.
- Thân người vẫn thẳng quá thì đổi "40 degrees" thành "50 degrees".
- Các khung lệch cỡ nhau thì thêm câu: `keep exactly the same pixel size as the reference image`.
- Làm xong gửi em **tấm sprite sheet PNG** (hoặc GIF). Em sẽ cắt khung, canh chân rồi thay vào game Unity và bản web.
