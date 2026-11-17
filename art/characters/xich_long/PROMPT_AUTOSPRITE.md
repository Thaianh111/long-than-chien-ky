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

---

## Animate Studio (làm video rồi cắt khung) — prompt chuyển động + số giây
Cài đặt: Imagine 1.5 (thử) / Kling 3.0 (bản đẹp) · 720p · Chroma Key, Cloud · Frames Every 2 · Size 512 · CLIPS 1.
Trước khi làm: CALIBRATE CANVAS cho nhân vật cao khoảng 55–60% khung, đặt giữa, chân cách đáy khoảng 10%.
Mọi prompt đều kết thúc bằng câu khóa camera:
`Camera fixed, side view, character stays centered in place, same size, green background unchanged, no other objects.`

| # | Động tác | Giây | Prompt |
|---|---|---|---|
| 1 | Đứng thở | 2s | Character stands in a ready stance facing right, katana held low, breathing slowly, chest rises and falls, sash sways gently, blade edge glows softly. Loopable. |
| 2 | Chạy ninja | 2s | Character runs to the right in place, low ninja sprint like Zero: torso leaning forward 40 degrees, head low, knees deeply bent, long fast strides, katana held reversed pointing backward along the arm, left arm swinging behind, sash streaming straight back. Even rhythm, legs clearly alternate, no limping. Loopable. |
| 3 | Nhảy | 2s | Character crouches, jumps straight up high with knees tucked and katana held back, reaches the top, falls back down with legs extending, lands in a crouch absorbing impact, returns to ready stance. |
| 4 | Lướt | 1.5s | Character bursts into a ground dash to the right in place: explosive push-off, body almost horizontal and very low, katana held back, sash stretched straight behind, then brakes hard sliding on the heel and leans back. |
| 5 | Chém nhát 1 | 2s | Character pulls the katana back then performs one fast horizontal slash forward at chest height, leaving a wide glowing orange half-moon trail, follows through and returns to ready stance. |
| 6 | Chém nhát 2 | 2s | Character steps forward and swings the katana from low behind up to high overhead in one rising slash, leaving a big glowing orange crescent arc, then returns to ready stance. |
| 7 | Chém nhát 3 | 2s | Character raises the katana high with both hands, leaps forward slightly and slashes straight down with full power, a huge glowing orange half-moon trail, blade stops at ground level with sparks, holds a low crouch, then stands. |
| 8 | Chém xoay trên không | 2s | Character jumps into the air and spins forward like a wheel twice, katana extended outward drawing a full glowing orange circle around the body, sash wrapping with the spin, then lands. |
| 9 | Bám + đạp tường | 2s | An invisible wall is on the right. Character clings to it with hand and foot, slides down slowly with sparks at the foot, then crouches and kicks off the wall, leaping away to the left. Do not draw the wall. |
| 10 | Trúng đòn | 1.5s | Character gets hit from the front: recoils backward, head snaps back, arms flung out, armor flashes white, staggers with one knee bending, then recovers to ready stance. |
| 11 | Xuất hiện / chiến thắng | 2s | Character lands from above into a low crouch with one hand on the ground, rises up, swings the katana once to the side and stands proud, sash flowing. |
