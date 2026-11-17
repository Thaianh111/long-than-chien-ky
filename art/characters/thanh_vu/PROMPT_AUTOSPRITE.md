# Thanh Vũ (青羽) — bộ prompt Sorceress AutoSprite (model mới + động tác)

Rút kinh nghiệm từ bộ Xích Long: các clip **nhảy, chém, chém xoay, đáp đất** bị máy quay nhân vật ra **nhìn thẳng màn hình**, nên phải ghép tạm bằng khung khác.
Vì vậy prompt nào cũng mở đầu bằng câu **khóa góc nghiêng**, và mỗi clip chỉ tả **một** động tác ngắn.

## Bước 1 — Model gốc (GPT Image 2, khung 1:1, GREEN SCREEN)
```
Original character design for a 2D side-scrolling action game: THANH VU, a young cyber archer-soldier from futuristic Vietnam (year 2189). Full body, strict side view facing RIGHT, standing ready, holding a large two-handed crossbow-rifle (Than No) level at the hip, pointing right. Shorter and younger than a knight, kind face without a mask, short dark hair under a light visor. Armor in jade teal (#1F6F8B) and ivory white (#F2EBDD) with bronze-gold accents; both shoulder guards shaped like Lac bird feathers from Dong Son bronze drums; a 12-ray sun emblem from the Dong Son drum in the center of the chest. The crossbow has a glowing cyan energy string and a bronze bird-head front. Anime action game sprite style, crisp clean lineart, cel shading, light from top-left. Character fills about 55% of the image height, centered, feet near the bottom with empty space around. Pure flat green background #00FF00, single character, no shadow, no text, no watermark.
```
> Chú ý câu **"fills about 55% of the image height … empty space around"**: chừa chỗ trống để lúc nhảy và bắn không bị cắt khung, khỏi phải bấm CALIBRATE CANVAS.

## Bước 2 — Animate Studio
Cài đặt: Imagine 1.5 (thử) hoặc Kling 3.0 (đẹp) · 720p · Chroma Key · Cloud · Frames Every 2 · Size 512 · CLIPS 1.
**Câu đầu** (dán ở đầu MỌI prompt):
`Strict side profile view facing right for the entire clip, never turn toward the camera, never show the front of the body.`
**Câu cuối** (dán ở cuối MỌI prompt):
`Camera fixed, character stays centered in place, same size, green background unchanged, no dust, no smoke, no other objects.`

| # | Động tác | Giây | Phần giữa của prompt |
|---|---|---|---|
| 1 | Đứng thở | 2s | Standing ready with the crossbow held level at hip height pointing right, breathing slowly, feather shoulder guards and cloth sway gently, crossbow string glows softly. Loopable. |
| 2 | Chạy | 2s | Running to the right in place, athletic soldier run leaning forward 20 degrees, both hands keep the crossbow level and steady at hip height pointing forward, legs clearly alternate with an even rhythm, no limping. Loopable. |
| 3 | Chạy vừa bắn | 2s | Running to the right in place exactly like a soldier run, while the crossbow is raised to shoulder height and aimed straight forward, firing small cyan energy bolts to the right with a short recoil each time. Loopable. |
| 4 | Đứng bắn | 2s | Standing, raises the crossbow to the shoulder and aims straight right, charges energy so the string glows brighter, fires one cyan energy bolt with a strong recoil pushing the shoulders back, then lowers the crossbow to ready. |
| 5 | Nhảy | 2s | Crouches, jumps straight up with knees tucked holding the crossbow close to the chest, reaches the top, falls with legs extending, lands in a crouch, returns to ready. Body stays in side profile the whole time, especially in the air. |
| 6 | Nhảy bắn | 2s | In mid-air with knees tucked, aims the crossbow straight right and fires one cyan energy bolt, small recoil, then falls and lands. Body stays in side profile in the air. |
| 7 | Lướt | 1.5s | Bursts into a fast low ground dash to the right in place, body leaning far forward, crossbow tucked under the arm, then brakes by sliding on the front heel and leaning back. |
| 8 | Bám + đạp tường | 2s | An invisible wall is on the LEFT. Back pressed to the wall, sliding down slowly, crossbow aimed to the right away from the wall, then kicks off the wall and leaps to the right. Do not draw the wall. |
| 9 | Trúng đòn | 1.5s | Gets hit from the front: recoils backward, arms flung, armor flashes white, staggers, then recovers to ready. |
| 10 | Xuất hiện / chiến thắng | 2s | Lands from above into a low crouch with one hand on the ground, stands up, spins the crossbow once and holds it proudly on the shoulder. |

Ví dụ prompt đầy đủ (Đứng thở):
```
Strict side profile view facing right for the entire clip, never turn toward the camera, never show the front of the body. Standing ready with the crossbow held level at hip height pointing right, breathing slowly, feather shoulder guards and cloth sway gently, crossbow string glows softly. Loopable. Camera fixed, character stays centered in place, same size, green background unchanged, no dust, no smoke, no other objects.
```

**Ưu tiên làm trước:** 1, 2, 4, 5 (đủ chơi được). Sau đó làm 3, 6, 7, 8. Hai động tác 9, 10 để sau cùng.
Làm xong anh tải **sprite pack (zip)** gửi em như lần Xích Long, em tự cắt và ghép.
