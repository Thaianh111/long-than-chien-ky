# Thuồng Luồng máy — trùm phụ màn Thủy Tinh · prompt Sorceress AutoSprite

**Vai trò:** trùm phụ giữa màn (khoảng phút thứ 2). Con rắn nước trong truyện dân gian, bị Thủy Tinh nhiễm Ma Chủng rồi bọc giáp máy.
Nó đuổi người chơi qua đường hầm ngập, rồi trồi lên đánh nhau ở hồ dưới chân núi Tản Viên.

**Cách làm hình:** rắn rất dài nên **không vẽ cả con một lần**. Mình vẽ riêng **đầu**, **một đốt thân** và **đuôi**; code nối các đốt thành con rắn uốn lượn (kiểu rắn trong Mega Man).
Chỉ phần **đầu** cần làm hoạt ảnh.
Nền **cam #FF8C00** (con rắn màu xanh lam, xanh ngọc, đồng, tím nên không dùng nền xanh lá được).

## Thiết kế
- Đầu dài kiểu rồng thời Lý pha cá sấu. Hàm răng kim loại, hai râu dài như dây cáp, mắt đỏ tím.
- Giáp vảy xanh rêu thẫm #1E4D5C, viền đồng xanh #8FA36B, bụng ngà #D9CFA8. Vết nhiễm Ma Chủng là các đường nứt tím phát sáng #B57CFF.
- Trên đầu có mào vây gai. Cổ có đèn lõi xanh ngọc #3FB8C4: đây là **điểm yếu**, sáng lên khi nó há miệng.

## Bước 1 — Đầu (model gốc, GPT Image 2, khung 1:1)
```
Original boss design for a 2D side-scrolling action game: THUONG LUONG, a giant mechanical river serpent from Vietnamese folklore, corrupted by a purple virus. Only the HEAD and the start of the neck, strict side view facing LEFT. Long dragon-crocodile head inspired by Ly-dynasty Vietnamese dragons, metal fangs, two long cable-like whiskers, a spiked fin crest on top, glowing red-violet eyes. Armored scales in deep sea-moss teal (#1E4D5C) with bronze-green trims (#8FA36B), ivory belly plates (#D9CFA8), glowing purple cracks (#B57CFF) across the armor, a glowing aqua core light (#3FB8C4) in the throat. Mouth closed. Anime action game sprite style, crisp clean lineart, cel shading, light from top-left. The head fills about 60% of the image, centered with empty space around. Pure flat orange background #FF8C00, no shadow, no water, no text.
```

## Bước 2 — Đốt thân và đuôi (GPT Image 2, gửi kèm hình đầu làm mẫu)
```
Same creature as the reference: ONE single body segment of the mechanical serpent, strict side view, a short round armored ring segment with teal scales, bronze trims, ivory belly plate on the bottom, one glowing purple crack, small fin on top. Seamless left and right edges so many segments can be chained. Centered, empty space around. Pure flat orange background #FF8C00, no shadow, no text.
```
```
Same creature as the reference: the TAIL end only, strict side view pointing RIGHT, tapering armored tail ending in a fan-shaped metal fin like a carp tail, teal scales, bronze trims, purple glowing cracks. Centered, empty space around. Pure flat orange background #FF8C00, no shadow, no text.
```

## Bước 3 — Hoạt ảnh đầu (ANIMATE trên hình đầu)
Cài đặt: Imagine 1.5 hoặc Kling 3.0 · 720p · Chroma Key (cam) · Frames Every 2 · Size 512.
Câu đầu mọi prompt: `Strict side profile view facing left for the entire clip, never turn toward the camera.`
Câu cuối mọi prompt: `Camera fixed, the head stays centered in place, same size, orange background unchanged, no water, no splash, no other objects.`

| # | Động tác | Giây | Phần giữa của prompt |
|---|---|---|---|
| 1 | Bơi / thở | 2s | The serpent head sways gently up and down as if swimming, whiskers float, crest fins ripple, eyes glow and dim slowly. Loopable. |
| 2 | Há miệng cắn | 1.5s | The head pulls back, then lunges forward to the left and snaps the jaws open wide and shut hard, the throat core flashes bright aqua when the mouth is open. |
| 3 | Phun nước | 2s | The head rears back, the throat core glows brighter, then opens the mouth wide and holds it open as if spraying a powerful jet to the left, the head shakes from the force, then closes the mouth. Do not draw the water jet. |
| 4 | Gầm | 2s | The head raises up and roars with the mouth wide open, whiskers and crest fins flare out, purple cracks glow intensely and pulse. |
| 5 | Trúng đòn | 1.5s | The head recoils backward from a hit, armor flashes white, eyes squint, then recovers to the swimming pose. |
| 6 | Gục | 2s | The head jerks, purple cracks crack wider and leak purple light, the eyes fade out, the jaw drops open and the head slowly sinks down and goes limp. |

Ví dụ prompt đầy đủ (Há miệng cắn):
```
Strict side profile view facing left for the entire clip, never turn toward the camera. The head pulls back, then lunges forward to the left and snaps the jaws open wide and shut hard, the throat core flashes bright aqua when the mouth is open. Camera fixed, the head stays centered in place, same size, orange background unchanged, no water, no splash, no other objects.
```

**Tia nước, sóng và bọt** em vẽ bằng code, anh không cần vẽ.
Làm xong anh gửi gói zip (đầu, đốt thân, đuôi, các clip đầu).
