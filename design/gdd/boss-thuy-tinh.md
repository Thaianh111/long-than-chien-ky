# Boss: Thủy Tinh — Thủy Thần Tâm Cơ (bản prototype)

> Game: Long Thần Chiến Ký · Trạng thái: Draft v1 (prototype) · Ngày: 2026-11-17
> Tác giả: Võ Thái Anh (VTA Games) · Thể loại: 2D action platformer kiểu Mega Man X

## 1. Overview

Thủy Tinh là một Tâm Cơ (robot có ý thức) bị Ma Chủng nhiễm, nắm quyền điều khiển trận
đại hồng thủy mà AI Thiên Lý thả xuống Việt Nam năm 2189. Trận đấu diễn ra trong một
phòng kín rộng đúng một màn hình (640×360, nền phẳng 20 ô × 32px, tường hai bên bám/nhảy
được). Boss có 5 đòn ở Pha 1, thêm 1 đòn và nước ngập 2 ô ở Pha 2 (HP ≤ 50%). Mỗi đòn có
báo hiệu rõ ràng và một "cửa phạt" sau đó; một đòn thưởng cho việc chém đạn bằng Xích Long,
một đòn thưởng cho phát bắn nạp đầy của Thanh Vũ. Mục tiêu: người chơi giỏi thắng lần đầu,
trận dài 60–90 giây.

## 2. Player Fantasy

"Một mình đứng giữa cơn lũ, đọc được từng con sóng rồi chém ngược nó về phía thần nước."
- Thẩm mỹ MDA chính: **Challenge** (đọc báo hiệu → né → phạt), phụ: **Fantasy** (tái hiện
  Sơn Tinh – Thủy Tinh, người chơi đứng ở phía "giữ sơn hà").
- SDT – Competence: mọi cú trúng đòn đều có lý do nhìn thấy được (báo hiệu ≥ 0,4 giây).
- SDT – Autonomy: mỗi đòn có ít nhất 2 cách xử lý (né bằng nhảy hoặc bám tường; phạt bằng
  kiếm hoặc nỏ).

**Thoại (hiện ở hộp thoại, ~2,5 giây mỗi câu):**
- Mở màn: *"Thủy dâng vạn trượng — ấy là lẽ trời. Phàm nhân, ngươi lấy gì giữ sơn hà?"*
- Bị hạ: *"Nước… rồi cũng phải rút… Lẽ trời chẳng ở nơi Thiên Lý… Giữ lấy sơn hà, hậu duệ
  Long Quân."*

**Hình ảnh (gợi ý cho art, không phải quyết định art):** chiến binh cao ~128px, giáp xanh biển
+ ngọc lam, hoa văn sóng/vảy, cầm đinh ba, lõi cầu nước ở ngực; vết nứt tím của Ma Chủng —
Pha 2 vết nứt lan rộng và phát sáng.

## 3. Detailed Design

### 3.1 Chỉ số cơ bản

| Thông số | Giá trị | Ghi chú |
|---|---|---|
| HP | **32 vạch** | Kiểu Mega Man X; Pha 2 bắt đầu khi HP ≤ 16 |
| Hurtbox thân | 56×112 px | Neo giữa chân; sprite cao 128px |
| Lõi cầu nước (ngực) | 24×24 px, cách sàn 76px | Chỉ có ý nghĩa ở đòn Thủy Kính |
| Sát thương va chạm | **3** | Chạm thân boss bất kỳ lúc nào (trừ lúc chết/intro) |
| Bất tử sau khi trúng đòn | **0,20 giây** | Nhấp nháy trắng; đủ ngắn để combo kiếm 3 nhát đều ăn |
| Tốc độ đi bộ | 90 px/s (P2: 120) | Chỉ dùng khi Idle để giữ khoảng cách |
| Đứng chờ (Idle) | 0,80 giây (P2: 0,50) | Thời gian giữa 2 đòn |

Sát thương người chơi gây ra: kiếm 1/1/2 (combo), chém xoay trên không 1, nỏ lv0 = 1,
lv1 = 2, lv2 = 4 (xuyên). Boss **không** kháng loại nào trừ khi đang bật Thủy Kính.

### 3.2 Các đòn đánh (Pha 1)

Quy ước: **W** = báo hiệu (telegraph), **A** = thời gian hoạt động, **R** = hồi chiêu (boss
đứng yên, ăn đòn được). Tọa độ y tính từ mặt sàn lên.

**Đòn 1 — Sóng Thần (sóng nền)** · `wave`
- W 0,60s: giơ đinh ba lên cao, nước xoáy quanh mũi; âm "ầm" trầm.
- A: đập đinh ba xuống, tạo 1 con sóng chạy dọc sàn về phía người chơi, 280 px/s, hộp
  sát thương 32 rộng × **48 cao**, tan khi chạm tường. Sát thương **3**.
- R 0,70s sau khi đập.
- Né: nhảy qua (nhảy đơn đủ). Phạt: lao tới chém combo trong R.

**Đòn 2 — Đinh Ba Lao (lướt đâm)** · `dash`
- W 0,50s: lùi nửa bước, mũi đinh ba lóe sáng + vệt nước mảnh trên sàn chỉ hướng lao.
- A: lao ngang 560 px/s tới tường đối diện; hộp sát thương 96×40, đặt cao 20–60px.
  Sát thương **4**.
- R **1,10s**: đinh ba cắm vào tường, boss kẹt — cửa phạt lớn nhất trận.
- Né: bám lên tường (≥ 64px) hoặc nhảy đôi qua đầu. Phạt: combo kiếm 1/1/2 đủ 3 nhát.

**Đòn 3 — Thủy Cầu Phao (bong bóng cong)** · `bubbles` — *thưởng Xích Long*
- W 0,45s: lõi ngực phình sáng, 3 bong bóng hiện quanh tay trái.
- A: ném lần lượt 3 bong bóng, cách nhau 0,25s, đường parabol (vx = 160/220/280 px/s về
  phía người chơi, vy0 = 380 px/s, trọng lực 900 px/s²). Đường kính **24px**, sát thương
  **2**, vỡ khi chạm sàn/tường.
- R 0,60s.
- **Chém phản**: nhát kiếm bất kỳ trúng bong bóng → bong bóng đổi màu đỏ, bay thẳng về
  lõi boss 420 px/s, trúng gây **2** (không bị bất tử 0,20s chặn — mỗi bong bóng tính
  riêng). Chém đủ 3 = 6 sát thương, nhiều hơn phạt bằng combo thường.
- Né: đứng dưới điểm rơi giữa 2 quả, hoặc lướt qua dưới.

**Đòn 4 — Hộ Tâm Thủy Kính (khiên nước)** · `shield` — *thưởng Thanh Vũ*
- W 0,40s: cắm đinh ba xuống, màn nước dựng lên phía trước.
- A: khiên nước 24 rộng × 140 cao, cách thân 40px về phía người chơi, giữ **3,0s**. Khiên
  **hấp thụ** kiếm và nỏ lv0/lv1 (không sát thương, tia nước bắn lên báo "vô hiệu").
  Trong lúc giữ khiên, lõi ngực phát sáng và mỗi 1,0s phun 1 giọt 12px thẳng ngang
  300 px/s (sát thương 2) — 3 giọt tổng.
- **Phá khiên**: nỏ lv2 trúng khiên → khiên vỡ, phát bắn **xuyên qua và trúng lõi** gây 4
  + boss **choáng 1,50s** (nhận đủ sát thương, có thể combo thêm).
- Nếu hết 3,0s không bị phá: khiên nổ thành 8 giọt tỏa tròn (12px, 240 px/s, sát thương 2),
  rồi R 0,50s.
- Né giọt: nhảy (giọt bay ở y = 76px nên đứng sát/cúi thấp không có; phải nhảy qua hoặc
  bám tường cao).

**Đòn 5 — Vũ Thương (mưa giáo nước)** · `spears`
- W 0,80s: boss chĩa đinh ba lên trời; trên sàn hiện **6 vòng gợn nước** tại các vị trí
  giáo sẽ rơi (cách đều 96px, lệch ngẫu nhiên ±16px, luôn chừa ít nhất 2 khe trống ≥ 64px).
- A: 6 giáo 16×48 px rơi từ trần cùng lúc, 480 px/s; cắm sàn rồi tan 0,3s. Sát thương **2**.
- R 0,60s (boss vẫn giơ tay — đánh được ngay khi giáo rơi xong).
- Né: đứng vào khe không có gợn.

### 3.3 Pha 2 (HP ≤ 16)

**Chuyển pha (2,5s, boss bất tử, xóa sạch đạn trên màn):**
1. 0,0s: boss khựng, quỳ một gối; vết nứt tím lan khắp giáp.
2. 0,3s: gầm (roar) 1,2s + màn hình chớp tím 0,15s + rung màn 0,4s.
3. 0,8–2,3s: nước dâng từ 0 → **64px (2 ô)**, đổi màu xanh pha tím.
4. 2,5s: vào Idle Pha 2.

**Nước ngập 2 ô (vĩnh viễn trong Pha 2):** người chơi chạm nước → tốc chạy ×0,80, trọng lực
×0,70, tốc rơi tối đa ×0,60 (nhảy cao/bay lâu hơn, như dưới nước trong MMX). Nước tĩnh
không gây sát thương. Thân boss không bị ảnh hưởng.

**Thay đổi đòn cũ ở Pha 2:**
| Đòn | Thay đổi |
|---|---|
| Sóng Thần | 2 con sóng cách nhau 0,45s; sóng cưỡi mặt nước: hộp cao 64→**112px** tính từ sàn (cần nhảy dưới nước, cao hơn bình thường) |
| Đinh Ba Lao | W 0,40s; 640 px/s; R giữ 1,10s (giữ cửa phạt công bằng) |
| Thủy Cầu Phao | 4 bong bóng, cách nhau 0,20s |
| Hộ Tâm Thủy Kính | Giữ 2,5s; phun giọt mỗi 0,8s |
| Vũ Thương | 8 giáo, cách đều 72px, vẫn đảm bảo 2 khe ≥ 64px; W 0,70s |

**Đòn mới — Đại Hồng Thủy (nước dâng tím)** · `flood` (chỉ Pha 2)
- W 1,00s: boss bay lên giữa phòng (đáy chân y = 200px), nước sủi bọt tím, cảnh báo "!!"
  hai bên tường.
- A 3,0s: nước dâng 64 → **176px** trong 0,6s, giữ 1,8s, rút về 64px trong 0,6s. Phần nước
  **trên 64px** là nước Ma Chủng: chạm vào mất **2** mỗi 0,5s (không đẩy lùi).
- Né: bám tường, giữ vị trí trên 176px (wall-cling trượt chậm; nhảy tường lặp lại để không tụt).
- **Cửa phạt trong lúc né**: boss lơ lửng, lõi ngực hướng thẳng ra — bắn nỏ từ tường (mọi
  cấp đều trúng). Lv2 trúng lõi → rơi xuống sàn sớm và choáng 1,0s.
- R 0,80s sau khi chạm đất. Thời gian hồi riêng: không dùng lại trong 12s.

### 3.4 Logic AI (máy trạng thái)

```
state INTRO:      khóa input người chơi, thoại mở màn 2.5s, thanh HP boss nạp đầy 1.0s → IDLE
state IDLE:       đi về phía giữ khoảng cách 160–260px với người chơi; sau idle_time → CHOOSE
state CHOOSE:
    d = |boss.x - player.x|
    w = base_weight[phase]               # bảng dưới
    if d < 120:  w.wave += 2; w.bubbles -= 1
    if d > 300:  w.dash += 3
    if player.on_wall: w.spears += 2; w.dash -= 2
    if phase == 2 and flood_cd <= 0 and hp_lost_since_last_flood >= 6: w.flood += 6
    if last[0] == last[1]: w[last[0]] = 0          # cấm lặp lại lần thứ 3
    if last[0] == "shield": w.shield = 0           # không khiên 2 lần liền
    clamp mọi w >= 0
    move = weighted_random(w)  → push vào last (giữ 2 phần tử)
    → ATTACK(move)
state ATTACK(move): chạy W → A → R theo bảng §3.2/3.3 → IDLE
on hp <= 16 and phase == 1 (kiểm tra cuối mỗi ATTACK hoặc ngay nếu đang IDLE):
    hủy đòn hiện tại → PHASE_SHIFT (2.5s bất tử) → IDLE, phase = 2
on hp <= 0: → DEFEAT (thoại bị hạ, nổ dây chuyền 2.0s, nước rút, mở cửa)
```

| Trọng số gốc | wave | dash | bubbles | shield | spears | flood |
|---|---|---|---|---|---|---|
| Pha 1 | 3 | 3 | 3 | 2 | 2 | 0 |
| Pha 2 | 3 | 3 | 3 | 2 | 3 | 0 (+6 khi đủ điều kiện) |

Đòn đầu tiên của trận luôn là **Sóng Thần** (dạy đọc báo hiệu bằng đòn dễ nhất).
Random dùng seed riêng của trận (`rng_seed` truyền vào) để test được tất định.

### 3.5 Phần thưởng sau khi hạ

- **Thủy Tiễn** (vũ khí phụ của Thanh Vũ): mũi tên nước lướt sát sàn và leo theo tường; nạp
  đầy thì thành con sóng nhỏ cao 48px quét ngang, chém tan đạn yếu.
- Theo luật MMX, Thủy Tiễn sẽ là điểm yếu của một boss hệ Hỏa/Lôi về sau (gây ×2 + choáng).

## 4. Formulas

- **Thời gian một chu kỳ** `T_cycle = idle_time + W + A + R`.
  Pha 1 trung bình ≈ 0,8 + (3,1 + 3,1 + 2,1 + 3,9 + 2,1)/5 ≈ **3,7s**; Pha 2 ≈ **3,4s**.
- **Sát thương người chơi gây ra mỗi chu kỳ** (ước lượng):
  - Giỏi: Sóng 4, Lao 4, Bong bóng 4–6, Khiên 4–8, Mưa 2–4 → TB ≈ **4,0**.
  - Lần đầu, chưa thuộc: TB ≈ **2,0**.
- **Thời lượng trận** `T = HP/dmg_cycle × T_cycle + intro 3,5 + chuyển pha 2,5`.
  - Giỏi: 16/4×3,7 + 16/4×3,4 + 6 ≈ **34s** (tối ưu tuyệt đối, hiếm đạt).
  - Thực tế giỏi (trượt ~40% cửa phạt): ≈ **55–65s**. Lần đầu: 16/2×3,7 + 16/2×3,4 + 6 ≈ **63s**
    + thời gian hồi phục sau trúng đòn → **75–90s**. Nằm trong mục tiêu 60–90s.
- **Áp lực lên người chơi**: 16 vạch HP; đòn TB ≈ 2,7 sát thương → chịu được **~6 lần trúng**.
  Người chơi giỏi trúng 2–4 lần/trận → thắng lần đầu với 4–10 vạch còn lại.

## 5. Edge Cases

- **Đứng dính sát boss chém liên tục**: va chạm gây 3 → bị phạt tự nhiên; boss không có
  "đẩy lùi" để tránh gây cảm giác bất công.
- **Spam nỏ lv0 từ xa**: được phép nhưng chậm (1/phát) và bị khiên chặn → không thống trị.
- **Trốn trên tường cả trận**: trọng số `spears` +2 khi người chơi bám tường; giáo rơi
  phủ cả sát tường (vị trí ngoài cùng cách tường 16px).
- **Phá khiên ngay frame khiên vừa hiện**: hợp lệ, tính như phá bình thường.
- **Bong bóng bị chém khi boss đang bất tử (chuyển pha)**: bong bóng tan, không gây sát thương.
- **HP về ≤16 giữa lúc Đinh Ba Lao**: hoàn tất đòn hiện tại rồi mới chuyển pha (tránh boss
  biến mất giữa không trung); ngoại lệ: Đại Hồng Thủy không có ở Pha 1 nên không xung đột.
- **Người chơi chết trong lúc nước tím dâng**: reset nước về 0 khi hồi sinh, boss về đủ HP
  (kiểu MMX, không giữ HP boss).
- **Sát thương thừa**: phát lv2 khi boss còn 2 HP → HP = 0, không âm.
- **Bị đẩy vào tường khi Lao**: hộp Lao dừng cách tường 0px; người chơi kẹt giữa
  boss–tường nhận sát thương 1 lần rồi được i-frame, không bị kẹt hình.
- **Chiến thuật thoái hóa đáng ngờ**: đứng góc + chỉ chờ Đinh Ba Lao để phạt. Cấm lặp 3 lần
  + trọng số ngẫu nhiên khiến chiến thuật này chỉ chiếm ~27% số đòn → không tối ưu.

## 6. Dependencies

| Hệ thống | Hướng | Hợp đồng |
|---|---|---|
| Điều khiển người chơi (Xích Long / Thanh Vũ) | Boss ← | Cần: hitbox kiếm có cờ `can_reflect`, cấp nạp nỏ 0/1/2, cờ `piercing`, `player.on_wall` |
| Vật lý nước | Boss → | Cung cấp `water_level_px`, `water_is_hazard_above_px`; hệ người chơi tự áp hệ số ×0,80/×0,70/×0,60 |
| Hệ đạn | Hai chiều | Đạn boss có `reflectable` (chỉ bong bóng) và `team` đổi khi bị chém |
| Camera/HUD | Boss → | Thanh HP boss 32 vạch, rung màn, chớp màn |
| Hội thoại | Boss → | 2 câu thoại (intro/defeat), chữ tiếng Việt |
| Vũ khí phụ / Lưu game | Boss → | Sự kiện `boss_defeated("thuy_tinh")` mở Thủy Tiễn |
| Giả định chưa kiểm chứng | — | Nhảy đơn ≥ 56px, nhảy đôi ≥ 120px, i-frame người chơi ~1,0s. Nếu khác, chỉnh chiều cao Sóng Thần |

## 7. Tuning Knobs

Tất cả để trong `assets/data/boss_thuy_tinh.json`, không hardcode.

| Knob | Mặc định | Khoảng | Loại | Lý do |
|---|---|---|---|---|
| `hp_max` | 32 | 28–32 | Curve | Chuẩn MMX; 28 nếu trận > 90s |
| `phase2_hp` | 16 | 12–18 | Gate | 50% như brief |
| `contact_damage` | 3 | 2–4 | Curve | Phạt đứng dính thân |
| `hit_invuln_s` | 0,20 | 0,15–0,40 | Feel | Phải < khoảng cách 2 nhát combo |
| `idle_time_s` P1/P2 | 0,80/0,50 | 0,4–1,2 | Feel | Nhịp thở giữa đòn |
| `wave_speed` / `wave_h` | 280 / 48 | 220–340 / 40–64 | Feel | Nhảy đơn qua được |
| `dash_speed` / `dash_recover_s` | 560 / 1,10 | 480–700 / 0,8–1,4 | Feel | Cửa phạt chính |
| `bubble_count` P1/P2 | 3 / 4 | 2–5 | Curve | Thưởng chém phản |
| `reflect_damage` | 2 | 1–3 | Curve | Phải > nhát kiếm thường |
| `shield_hold_s` P1/P2 | 3,0 / 2,5 | 2,0–4,0 | Gate | Đủ thời gian nạp lv2 |
| `shield_break_stun_s` | 1,50 | 1,0–2,0 | Feel | Thưởng Thanh Vũ |
| `spear_count` P1/P2 | 6 / 8 | 5–9 | Curve | Luôn chừa ≥ 2 khe 64px |
| `spear_warn_s` | 0,80 | 0,6–1,0 | Feel | Đọc được vị trí gợn |
| `water_p2_px` | 64 | 32–96 | Gate | 2 ô |
| `flood_peak_px` / `flood_hold_s` | 176 / 1,8 | 144–224 / 1,2–2,5 | Gate | Ép bám tường |
| `flood_tick_dmg` | 2 / 0,5s | 1–3 | Curve | |
| `flood_cooldown_s` | 12 | 8–20 | Gate | Không ngập liên tục |
| `move_weights` | bảng §3.4 | 0–6 | Curve | |

## 8. Acceptance Criteria

**Chức năng (test tự động được, seed cố định):**
- [ ] HP 32; trúng combo kiếm 1/1/2 trừ đúng 4; nỏ lv2 trừ 4 và xuyên.
- [ ] Không có chuỗi 3 đòn giống nhau liên tiếp trong 1.000 lần chọn đòn mô phỏng.
- [ ] Đòn đầu tiên luôn là Sóng Thần.
- [ ] HP ≤ 16 → chuyển pha đúng 1 lần, bất tử 2,5s, nước lên 64px, xóa hết đạn.
- [ ] Bong bóng bị chém bay về boss và trừ 2; khiên chặn kiếm và nỏ lv0/1, vỡ bởi lv2 và choáng 1,5s.
- [ ] Mưa giáo luôn có ≥ 2 khe trống ≥ 64px (kiểm 1.000 seed).
- [ ] Đại Hồng Thủy chỉ xuất hiện ở Pha 2, cách nhau ≥ 12s; vị trí trên 176px ở tường an toàn.
- [ ] Mọi đòn có báo hiệu ≥ 0,40s.

**Trải nghiệm (playtest):**
- [ ] Người chơi quen thể loại thắng lần thử đầu tiên với ≥ 3/5 người test.
- [ ] Trận kéo dài 60–90s (trung vị) với người chơi lần đầu; người chơi giỏi ≤ 65s.
- [ ] Sau trận, người test nói được ít nhất 1 lý do vì sao mình bị trúng đòn (báo hiệu đọc được).
- [ ] ≥ 50% người test tự phát hiện chém phản bong bóng hoặc phá khiên bằng nỏ nạp đầy mà không cần hướng dẫn.
- [ ] Chuyển pha tạo cảm giác "boss nổi giận" (hỏi sau test: có nhớ khoảnh khắc nước dâng không).
