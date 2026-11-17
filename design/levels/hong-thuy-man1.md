# Màn 1 — HỒNG THỦY (洪水)

**Game:** Long Thần Chiến Ký · **Thời lượng:** khoảng 4 phút tới trùm Thủy Tinh (người mới 5–6 phút)
**Bản đồ (bản 2, 2026-11-17):** MW = 264 ô × MH = 17 ô (TS = 32px → 8448 × 544 px). Số liệu thật: `game/src/levels/hongThuy.ts`.

## Bản 2 — thêm Bến Bạch Hạc (Thuồng Luồng) và Chân núi Tản Viên

Đoạn 1–4 (ô 0–143) giữ nguyên như bản 1 bên dưới. Từ ô 144 trở đi làm lại:

| # | Đoạn | Ô x | Thời gian | Cường độ | Dạy / thử thách |
|---|------|-----|-----------|----------|-----------------|
| 1–4 | Phố ngập → Mái nhà | 0–143 | ~2 phút | thấp → cao | như bản 1 |
| 5 | **Bến Bạch Hạc** — trùm phụ **Thuồng Luồng** | 144–178 | ~40 s – 2 phút | **đỉnh 2** | Đọc bọt nước để né cú lao, nhảy lên lưng rắn đánh đầu. Thiết kế: `design/gdd/boss-thuong-luong.md` |
| 6 | **Chân núi Tản Viên** | 179–241 | ~50 s | vừa → cao | **Cột đá Sơn Tinh:** chém/bắn vào cột thì cột dựng lên và nâng người chơi theo. Vách A, sông có 2 cột, vách B, bậc xuống |
| 7 | **Cổng Thủy Tinh** | 236–263 | nghỉ + trùm | **đỉnh 3** | Điểm lưu ô 238, đấu trường kín 243–263 |

```
Cường độ
 cao |              ███                  ▓▓▓ TL              ▓▓▓▓ THỦY TINH
 vừa |      ██  ██  ███  ██ ██ ██        ▓▓▓       ██  ██    ▓▓▓▓
thấp |██ ███████████████████████████ ░░  ▓▓▓  ░░ ██████████ ░ ▓▓▓▓
     +-------------------------------------------------------------> x
      Phố  Quảng trường Giếng Mái nhà   Bến Bạch Hạc  Tản Viên  Cổng
```

**Ý đồ:** người chơi gặp cái mới mỗi ~40 giây. Giếng nước dâng dạy "nước đuổi thì leo".
Thuồng Luồng là bài thi giữa màn về đọc báo hiệu dưới nước. Cột Sơn Tinh mở đầu cho chủ đề Sơn Tinh – Thủy Tinh:
núi dựng lên chống nước, rồi mới gặp Thủy Tinh.

**Bến Bạch Hạc (ô 150–178):** bờ kè 150–154 (cao 3 ô), tường 155 có cổng hàng 9–11 (đóng khi vào trận),
bờ trái 156–157, sông sâu 158–175 (mặt nước hàng 13), cọc gỗ 161, 166, 171 (đỉnh hàng 11), bờ phải 176–177,
cổng ra 178 hàng 9–11 (mở khi hạ trùm). Lưng rắn khi nổi là ô ẩn ở hàng 12.

**Chân núi Tản Viên (ô 179–241):** điểm lưu 182 · cột Sơn Tinh 186 (2 → 6 ô) lên vách A (188–197, đỉnh hàng 7, có tháp súng)
· sông 198–207 với cột 200 và 204 (2 → 7 ô, phải xuống nước đánh cột rồi đứng lên cho cột nâng) · vách B 208–217
(tháp súng) · bậc xuống 218–226 · hành lang 227–241 có drone và tháp súng. Người giỏi có thể đạp tường leo vách A,
bỏ qua cột đầu.

**Điểm lưu:** ô 3, 51, 85, 119, 147 (trước bến), 182 (sau Thuồng Luồng), 238 (trước Thủy Tinh).

---

# Bản 1 (lưu lại để tham khảo đoạn 1–4)


**Game:** Long Thần Chiến Ký · **Loại:** màn thử (prototype) · **Thời lượng dự kiến:** 2–3 phút tới boss
**Bản đồ:** MW = 180 ô × MH = 17 ô (TS = 32px → 5760 × 544 px)

---

## 1. Tổng quan

Năm 2189, AI Thiên Lý mở toang mọi con đập, nước biển và nước lũ tràn vào thành phố ven biển. Người chơi đi từ con phố ngập mắt cá, qua quảng trường đã thành hồ, leo lên một tòa tháp khi nước dâng đuổi theo phía dưới, chạy trên mái nhà rồi vào cổng đối đầu Thủy Tinh. Màn này có vai trò dạy nước: lội nước làm chậm và mất máu từ từ, không chết ngay, nên người chơi được phép sai mà không bị phạt quá nặng. Toàn màn không có hố rơi mất mạng; rơi xuống chỉ là rơi vào nước, rồi phải leo lên lại.

### Nhịp màn (5 đoạn)

| # | Đoạn | Ô x | Thời gian | Cường độ | Dạy / thử thách |
|---|------|-----|-----------|----------|-----------------|
| 1 | **Phố ngập** | 1–34 | ~25 s | Thấp → vừa | Nhảy qua xe hỏng, vũng nước nông (chỉ làm chậm), drone đầu tiên, tháp súng đầu tiên |
| 2 | **Quảng trường thành hồ** | 35–83 | ~40 s | Vừa | Nhảy giữa các bệ cột trên mặt nước sâu, tháp súng bắn ngang qua hố, khe 6 ô dạy lướt + nhảy |
| 3 | **Giếng nước dâng** | 84–93 | ~10–15 s | **Cao (đỉnh 1)** | Cửa sập sau lưng, nước dâng 48 px/s, leo bằng bám tường hoặc gờ |
| 4 | **Mái nhà** | 94–143 | ~40 s | Vừa → cao | Nhảy mái, khe 6 ô, leo tường lên mái cao hơn, tháp súng bắn chặn |
| 5 | **Cổng Thủy Tinh** | 144–178 | ~10 s nghỉ + boss | Nghỉ → **đỉnh 2** | Hành lang khô, yên tĩnh, điểm lưu, rồi đấu trường kín |

```
Cường độ
 cao |                      ███                    ▓▓▓▓ BOSS
     |                      ███          ██        ▓▓▓▓
 vừa |          ██   ██  ██ ███    ██ ██ ██        ▓▓▓▓
     |     ██   ██   ██  ██ ███ ██ ██ ██ ██        ▓▓▓▓
thấp |██ ████ ███████████████████████████████ ░░░░ ▓▓▓▓
     +------------------------------------------------------> x
      Phố ngập   Quảng trường   Giếng   Mái nhà    Cổng  Boss
```

---

## 2. Sơ đồ (nhìn ngang, giản lược, 1 ký tự ≈ 2 ô)

```
row 0 ................................................................................#########
row 4 ............................................ |   AAAA                         #       #
row 6 ............................................ |   AAAA BBBB   CCC          EE  #       #
row 8 ................................pp.......... | ‖ AAAA BBBB   CCC  DDDD    EE  #       #
row11 ................  AAAA BBB CCC  .. DDD       |‖  AAAA BBBB ‖ CCC  DDDD    EE  #       #
row12 ....  ..          AAAA~BBB~CCC~~~~DDD         ~~ AAAA~BBBB~~CCC~DDDD~~EE  G       #
row15 ##xx##~~##ttt## ##AAAA~BBB~CCC~~~~DDD##  ##  ~~##AAAA~BBBB~~CCC~DDDD~~EE#######G#########
       Phố ngập         Quảng trường           Giếng       Mái nhà            Cổng   Đấu trường
       (~ = nước, ‖ = gờ trong giếng, G = cổng boss, pp = biển hiệu nổi)
```

---

## 3. Bản đồ JavaScript (dán thẳng vào prototype)

Cách dán vào `prototype/dieu-khien/index.html`:
- Đổi dòng `const TS = 32, MW = 110, MH = 17;` thành `const TS = 32, MH = 17;` và đặt `const MW = 180;` (khối dưới) **trước** dòng tạo `map`.
- Thay toàn bộ các lệnh `rect(...)` cũ và `const checkpoints` cũ bằng khối dưới.
- `addFoe()`, `WATER`, `BOSS_ARENA` cần đặt sau khi hàm `addFoe` đã được khai báo (hoặc gom vào một hàm `buildLevel()` gọi sau).

```js
/* ================= MÀN 1 — HỒNG THỦY ================= */
const MW = 180;                                   // rộng 180 ô = 5760px
// (const map = ... và const rect = ... giữ nguyên như cũ, đặt sau dòng này)

/* ---- khung ---- */
rect(0, 15, MW, 2);                               // nền toàn màn (hàng 15–16, mặt nền y = 480px)
rect(0, 0, 1, MH);                                // tường trái
rect(MW - 1, 0, 1, MH);                           // tường phải (cũng là tường phải đấu trường)

/* ---- ĐOẠN 1: PHỐ NGẬP (x 1–34) ---- */
rect(8, 13, 3, 2);                                // xe hơi chìm: cao 2 ô, nhảy thường qua
rect(14, 15, 5, 1, 0);                            // vũng nước nông x14–18 (khoét 1 ô, hàng 16 vẫn là đáy)
rect(28, 13, 4, 2);                               // trạm xe buýt đổ: bệ cao 2 ô, có tháp súng trên nóc

/* ---- ĐOẠN 2: QUẢNG TRƯỜNG THÀNH HỒ (x 35–83) ---- */
rect(35, 13, 3, 2);                               // bậc lên quảng trường (mặt y = 416)
rect(38, 11, 8, 4);                               // bệ cột A x38–45 (mặt y = 352)
                                                  // hố nước 1: x46–49 (4 ô) — nhảy thường
rect(50, 11, 6, 4);                               // bệ cột B x50–55 (tháp súng)
                                                  // hố nước 2: x56–61 (6 ô) — lướt+nhảy hoặc nhảy đôi
rect(58, 13, 2, 2);                               //   bậc thoát dưới nước trong hố 2 (mặt y = 416)
rect(62, 11, 6, 4);                               // bệ cột C x62–67
                                                  // hố nước 3: x68–75 (8 ô) — PHẢI đi qua biển hiệu nổi
rect(70, 8, 4, 1);                                //   biển hiệu nổi x70–73 (mặt y = 256)
rect(72, 13, 2, 2);                               //   bậc thoát dưới nước trong hố 3
rect(76, 11, 5, 4);                               // bệ cột D x76–80 (tháp súng)
rect(81, 13, 3, 2);                               // bậc xuống phố x81–83

/* ---- ĐOẠN 3: GIẾNG NƯỚC DÂNG (x 84–93) ---- */
                                                  // phố khô x84–87 (điểm lưu 3)
rect(88, 2, 1, 10);                               // tường trái tháp x88, hàng 2–11; cửa vào hàng 12–14 (cao 3 ô)
                                                  // lòng giếng x89–93 = rộng 5 ô; tường phải = mặt trái mái A (x94)
rect(92, 11, 2, 1);                               // gờ 1 bám tường phải (mặt y = 352)
rect(89, 8, 2, 1);                                // gờ 2 bám tường trái (mặt y = 256)
rect(92, 6, 2, 1);                                // gờ 3 bám tường phải (mặt y = 192)
// Khi nước dâng kích hoạt: rect(88, 12, 1, 3) — cửa sập đóng sau lưng (xem WATER.rise.lockTiles)

/* ---- ĐOẠN 4: MÁI NHÀ (x 94–143) ---- */
rect(94, 4, 8, 11);                               // nhà A x94–101, mái y = 128 (lối ra của giếng)
                                                  // khe 1: x102–104 (3 ô), mái B thấp hơn 2 ô
rect(105, 6, 7, 9);                               // nhà B x105–111, mái y = 192 (tháp súng)
                                                  // khe 2: x112–117 (6 ô), cùng độ cao — lướt+nhảy
rect(114, 11, 2, 1);                              //   gờ ban công thoát nước trong khe 2 (mặt y = 352)
rect(118, 6, 6, 9);                               // nhà C x118–123, mái y = 192 (điểm lưu 4)
                                                  // khe 3: x124–126 (3 ô), xuống 2 ô
rect(127, 8, 8, 7);                               // nhà D x127–134, mái y = 256
                                                  // khe 4: x135–138 (4 ô), LÊN 3 ô — nhảy đôi hoặc bám tường E
rect(139, 5, 5, 10);                              // nhà E x139–143, mái y = 160 (tháp súng), mặt trái leo tường được

/* ---- ĐOẠN 5: CỔNG THỦY TINH (x 144–178) ---- */
                                                  // hành lang khô x144–157 (rơi từ mái E xuống, điểm lưu 5)
rect(158, 1, 1, 11);                              // tường trái đấu trường x158, hàng 1–11 (cao 11 ô, đạp tường được)
                                                  // cổng mở ở hàng 12–14; đóng lại = rect(158, 12, 1, 3)
rect(158, 0, MW - 158, 1);                        // trần đấu trường hàng 0 (không trèo qua được)
                                                  // sàn đấu trường x159–178 phẳng (nền chung hàng 15)

/* ---- QUÁI ---- */
addFoe('drone',  24,  9);                         // drone đầu tiên, trên phố trống — dạy chém trên không
addFoe('turret', 30, 12);                         // trên nóc trạm xe buýt (mặt hàng 13)
addFoe('turret', 54, 10);                         // bệ B, bắn ngang qua hố 1 về phía bệ A
addFoe('drone',  60,  6);                         // trên hố 2 — quấy khi người chơi lấy đà lướt
addFoe('turret', 79, 10);                         // bệ D, bắn về phía biển hiệu nổi
addFoe('drone', 100,  1);                         // chào người chơi khi vừa thoát giếng lên mái A
addFoe('turret',110,  5);                         // mái B, bắn ngang mái
addFoe('drone', 130,  4);                         // trên mái D
addFoe('turret',142,  4);                         // mái E, bắn qua khe 4 khi người chơi leo lên

/* ---- ĐIỂM LƯU (y = mặt đất chỗ đứng) ---- */
const checkpoints = [
  { x:   3 * TS, y: 15 * TS },                    // 1 đầu màn
  { x:  51 * TS, y: 11 * TS },                    // 2 bệ cột B (giữa quảng trường)
  { x:  85 * TS, y: 15 * TS },                    // 3 trước giếng nước dâng
  { x: 119 * TS, y:  6 * TS },                    // 4 mái nhà C
  { x: 148 * TS, y: 15 * TS },                    // 5 trước cổng boss
];

/* ---- NƯỚC ---- */
// Quy tắc: thân người chơi chạm vùng nước (x0 <= x < x1, y > mặt nước)
//   - chân dưới mặt nước  -> tốc độ chạy/lướt x0.5 (nhảy giữ nguyên để không bị kẹt)
//   - nửa thân dưới mặt nước và deep !== false -> mất 1/8 máu tối đa mỗi giây (tích theo dt)
//   - KHÔNG chết ngay; hết máu thì chết bình thường, hồi sinh ở điểm lưu
const WATER = {
  zones: [
    { x0:  14 * TS, x1:  19 * TS, y: 15 * TS + 8, deep: false }, // vũng nông phố ngập: chỉ chậm
    { x0:  46 * TS, x1:  50 * TS, y: 12 * TS },  // hố nước 1 quảng trường (y = 384)
    { x0:  56 * TS, x1:  62 * TS, y: 12 * TS },  // hố nước 2
    { x0:  68 * TS, x1:  76 * TS, y: 12 * TS },  // hố nước 3 (dưới biển hiệu nổi)
    { x0: 102 * TS, x1: 105 * TS, y: 12 * TS },  // khe mái 1
    { x0: 112 * TS, x1: 118 * TS, y: 12 * TS },  // khe mái 2
    { x0: 124 * TS, x1: 127 * TS, y: 12 * TS },  // khe mái 3
    { x0: 135 * TS, x1: 139 * TS, y: 12 * TS },  // khe mái 4
  ],
  rise: {
    triggerX: 90 * TS,        // 2880px: người chơi đã vào hẳn lòng giếng
    x0: 88 * TS,              // 2816px (gồm cả cột cửa vào)
    x1: 94 * TS,              // 3008px (đến mặt tường nhà A)
    startY: 16 * TS,          // 512px: dưới mặt nền 1 ô, nước trào lên từ cống
    endY: 5 * TS,             // 160px: dừng dưới mái A 1 ô, mái luôn an toàn
    speed: 48,                // px/s
    delay: 1.0,               // giây báo động (còi + rung màn) trước khi nước bắt đầu dâng
    lockTiles: { x: 88, y: 12, w: 1, h: 3 },     // cửa sập đóng sau lưng lúc kích hoạt
    // Chết / hồi sinh ở điểm lưu 3: đặt lại mặt nước = startY, mở lại lockTiles (rect(..., 0)), cho kích hoạt lại.
  },
};

/* ---- ĐẤU TRƯỜNG BOSS ---- */
const BOSS_ARENA = {
  x0: 159 * TS,               // 5088px: mép trong tường trái
  x1: 179 * TS,               // 5728px: mép trong tường phải (rộng 20 ô)
  gateX: 158 * TS,            // 5056px: cột cổng
  closeAtX: 161 * TS,         // người chơi vượt qua đây thì cổng đóng: rect(158, 12, 1, 3)
  floorY: 15 * TS,            // sàn phẳng y = 480; tường cao 14 ô (hàng 1–14) để đạp tường
};
```

---

## 4. Kiểm tra từng cú nhảy (theo số của prototype)

Số dùng: nhảy 3,5 ô · nhảy đôi 6 ô · lướt+nhảy vượt 7 ô · bám/đạp tường được · người cao 2,4 ô.

| Chỗ | Khoảng ngang | Chênh cao | Cách qua | Kết quả |
|-----|-------------|-----------|----------|---------|
| Xe chìm x8–10 | — | lên 2 | nhảy thường | OK |
| Vũng nông x14–18 | 5 | sâu 1 | nhảy hoặc lội (chỉ chậm) | OK |
| Bệ trạm xe x28–31 | — | lên 2 | nhảy thường | OK |
| Bậc 35 → bệ A | — | 2 + 2 | hai nhảy thường | OK |
| Hố 1 (A→B) | 4 | 0 | nhảy chạy | OK |
| Hố 2 (B→C) | 6 | 0 | lướt+nhảy (7) hoặc nhảy đôi | OK, dư 1 ô |
| Bệ C → biển hiệu | 2 | lên 3 | nhảy thường (3,5) hoặc nhảy đôi | OK |
| Biển hiệu → bệ D | 2 | xuống 3 | rơi | OK |
| Hố 3 đi thẳng C→D | 8 | 0 | **không** nhảy thẳng được (>7) — bắt buộc qua biển hiệu, hoặc lội nước | Có chủ ý |
| Thoát hố nước quảng trường | — | đáy 480 → bậc 416 → bệ 352 | hai nhảy thường; hố 1 không có bậc: nhảy đôi (4 ô) | OK |
| Giếng: nền → gờ 1 | — | lên 4 | nhảy đôi hoặc đạp tường | OK |
| Gờ 1 → gờ 2 → gờ 3 → mái A | 1–3 | lên 3 / 2 / 2 | nhảy thường | OK |
| Giếng chỉ bằng đạp tường | rộng 5 ô | 11 ô | đạp tường trái x88 (hàng 2–11) / phải x94 (hàng 4–14) | OK |
| Khe 1 (A→B) | 3 | xuống 2 | nhảy thường | OK |
| Khe 2 (B→C) | 6 | 0 | lướt+nhảy hoặc nhảy đôi | OK |
| Khe 3 (C→D) | 3 | xuống 2 | nhảy thường | OK |
| Khe 4 (D→E) | 4 | lên 3 | nhảy đôi, hoặc bám mặt tường E rồi đạp lên | OK |
| Rơi xuống khe mái 2 | — | đáy 480 → gờ 352 → mái 192 | nhảy đôi (4) rồi nhảy đôi (5) | OK |
| Rơi xuống khe 1/3/4 | rộng 3–4 | 8–10 ô | đạp tường qua lại hai mặt nhà | OK |
| Mái E → hành lang | — | xuống 10 | rơi | OK |
| Đấu trường | sàn 20 ô | tường 14 ô | đạp tường hai bên | OK |

- Không có hố đáy trống: hàng 16 liền suốt màn, nên không có cái chết do rơi.
- Không khe nào cần hơn lướt+nhảy (7 ô); khe rộng nhất là 6 ô (hai chỗ: hố 2, khe mái 2). Hố 3 rộng 8 ô có biển hiệu chia đôi.
- Cửa giếng cao 3 ô (96px) > người 78px.

### Thời gian giếng nước dâng (speed 48 px/s, delay 1 s)

| Mặt nước tới | y | Thời điểm |
|---|---|---|
| Chân người trên nền | 480 | 1,7 s |
| Gờ 1 | 352 | 4,3 s |
| Gờ 2 | 256 | 6,3 s |
| Gờ 3 | 192 | 7,7 s |
| Dừng hẳn | 160 | 8,3 s |

Người chơi leo bằng gờ mất khoảng 4–5 s, leo bằng đạp tường khoảng 3 s → luôn có khoảng trống an toàn 2–4 s, nhưng đứng ngập ngừng ở một gờ quá 2 s là bị nước bắt kịp (mất máu, không chết). Muốn khó hơn: tăng `speed` lên 60.

---

## 5. Danh sách chạm trán

| # | Quái | Ô | Đoạn | Độ khó | Mục đích |
|---|------|---|------|--------|----------|
| 1 | drone | (24, 9) | Phố ngập | 1/5 | Kẻ địch đầu tiên, không gian trống, dạy chém |
| 2 | turret | (30, 12) | Phố ngập | 2/5 | Dạy né đạn ngang bằng nhảy |
| 3 | turret | (54, 10) | Quảng trường | 2/5 | Đạn bay qua hố 1 — né trong lúc nhảy |
| 4 | drone | (60, 6) | Quảng trường | 3/5 | Quấy lúc lấy đà lướt qua hố 2 |
| 5 | turret | (79, 10) | Quảng trường | 3/5 | Bắn về phía biển hiệu nổi |
| 6 | drone | (100, 1) | Mái A | 2/5 | Thưởng nhẹ sau giếng (thả lỏng) |
| 7 | turret | (110, 5) | Mái B | 3/5 | Chặn đà trước khe 2 |
| 8 | drone | (130, 4) | Mái D | 3/5 | Quấy trước khe leo |
| 9 | turret | (142, 4) | Mái E | 4/5 | Bắn khi người chơi đang bám tường E |
| B | **Thủy Tinh** | đấu trường 159–178 | Cổng | boss | Thiết kế riêng (game-designer) |

---

## 6. Đường chính và đường phụ

- **Đường chính:** phố → bậc 35 → bệ A–B–C → biển hiệu → bệ D → giếng (leo) → mái A–E → hành lang → cổng boss.
- **Đường phụ (bản thử chưa có vật phẩm):** gờ ban công x114 trong khe mái 2 và bậc dưới nước x58, x72 là chỗ hợp lý để đặt hộp máu / mảnh nâng cấp sau này — phần thưởng cho người dám lội nước.

## 7. Kể chuyện bằng môi trường

- Phố ngập: xe hơi chìm, trạm xe buýt đổ, biển báo "SƠ TÁN" nhấp nháy tím (Ma Chủng đã nhiễm cả hệ thống đèn).
- Quảng trường: tượng đài ngập tới ngực, biển hiệu quảng cáo nổi lềnh bềnh làm bậc đi.
- Giếng: còi báo động của tháp, cửa sập tự đóng (Thiên Lý chủ động nhốt người chơi) — khoảnh khắc người chơi hiểu "AI đang săn mình".
- Mái nhà: thấy xa xa con đập mở toang, nước trút xuống thành phố (ảnh nền).
- Hành lang cổng: nước rút đột ngột, im lặng — nước đang bị gom về phía đấu trường, báo hiệu Thủy Tinh.

## 8. Âm thanh / nhạc

| Lúc | Âm thanh |
|-----|----------|
| Đầu màn | Nhạc màn chính, tiếng mưa + nước chảy làm nền |
| Vào nước | Âm bị bóp (lọc thấp) khi ngập nửa thân |
| x = 90 (giếng) | Còi báo động 1 s, rồi nhạc đổi sang bản dồn dập; tiếng nước sủi tăng theo mực nước |
| Lên mái A | Nhạc màn trở lại, gió lớn |
| x = 144 (hành lang) | Tắt nhạc, chỉ còn tiếng nước rỉ — khoảng nghỉ trước boss |
| Cổng đóng (x = 161) | Tiếng cửa sập nặng, nhạc boss Thủy Tinh |
