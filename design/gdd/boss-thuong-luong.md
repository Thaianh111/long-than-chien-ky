# Trùm phụ — THUỒNG LUỒNG (Bến Bạch Hạc)

**Màn:** 1 Hồng Thủy, giữa màn (sau đoạn Mái nhà, trước Chân núi Tản Viên)
**Số liệu:** `game/src/data/boss-thuong-luong.json` (bản Unity: `unity/Assets/Resources/Data/boss-thuong-luong.json`)
**Hình:** `art/bosses/thuong_luong/` (AutoSprite, 16 khung bơi `tl_swim1..16`, đầu quay TRÁI, 435×121 px)

## 1. Tổng quan
Thuồng luồng máy là con quái sông do Thủy Tinh thả ra để giữ bến. Nó bơi dưới nước, lâu lâu trồi lên.
Đây là bài kiểm tra thứ hai của màn sau giếng nước dâng: người chơi phải **đọc báo hiệu dưới nước**
(bọt, gợn sóng) và **né bằng nhảy**, rồi tận dụng **lúc nó nổi lên** để đánh vào đầu.

## 2. Cảm giác người chơi
Đứng trên mấy cọc gỗ giữa dòng nước đục. Dưới chân là bóng đen dài lượn qua lượn lại. Bọt sủi một bên
bờ: nó sắp phóng ngang mặt nước. Khi nó nằm nổi, người chơi **nhảy lên lưng con quái** chạy tới đầu
mà chém. Người giỏi đánh trúng nhiều nhát trong một lần nó nổi nên hạ nhanh; người mới phải chờ nhiều vòng.

## 3. Luật chơi
- **Đấu trường:** cột 155–178. Cổng trái đóng khi người chơi vào qua x = 157. Nước sâu (mặt nước ở hàng 13),
  4 bệ đứng: bờ trái, 3 cọc gỗ, bờ phải. Rơi xuống nước thì chậm và mất máu từ từ như cả màn.
- **Máu:** 24. Còn ≤ 12 → pha 2 (nhanh hơn, phun nhiều hơn, có thể lao 2 lượt liền).
- **Chỉ trúng đòn ở ĐẦU** và chỉ khi đầu ở trên mặt nước. Kiếm và tên đều trúng. Thân không ăn đòn.
- **Vòng lặp:** LẶN → chọn chiêu → LẶN …
  1. **Lặn** (1,6 s / pha 2: 1,1 s): chỉ thấy bóng tối dưới nước, bơi theo người chơi. Không ai đánh ai.
  2. **Lao mặt nước** (`surge`): bọt sủi ở mép bên nó sắp lao ra (0,9 s) → phóng ngang qua cả đấu trường,
     đầu và lưng nhô cao hơn mặt cọc. Chạm là mất máu. Né: **nhảy lên đúng lúc** (hoặc nhảy 2 bậc).
     Pha 2: 40% lao tiếp lượt thứ hai theo chiều ngược lại.
  3. **Nổi lưng** (`float`): bọt ở giữa (0,8 s) → nổi ngang mặt nước 3,2 s (pha 2: 2,4 s). **Lưng thành bệ
     đứng được**; đầu cúi thấp, không gây sát thương — đây là lúc đánh. Trước khi lặn rung mình 0,7 s.
  4. **Phun nước** (`spit`): đầu trồi ở một mép (bên xa người chơi), phun 3 (pha 2: 5) cục nước bay vòng
     về phía người chơi. **Chém trúng cục nước thì nó bay ngược về đầu** (mất 2 máu). Đầu ăn đòn suốt lúc phun.
- **Chọn chiêu:** đòn đầu luôn là Lao. Không cùng chiêu 3 lần liền. Không Nổi 2 lần liền.
  Người chơi đang đứng trong nước → hay Phun hơn.
- **Hạ xong:** chìm xuống, cổng phải mở (hàng 9–11), điểm lưu ngay sau đấu trường.
- **Người chơi chết:** về điểm lưu trước bến, trùm phụ hồi đầy máu, cổng mở lại.

## 4. Công thức
- Thời gian một vòng ≈ LẶN + W + A + R ≈ 1,6 + 0,9 + 1,6 + 0 = 4,1 s (Lao) … 1,6 + 0,8 + 3,6 + 0,7 ≈ 6,7 s (Nổi).
- Sát thương người chơi ra đòn: Xích Long 1–2/nhát, Thanh Vũ 1 / 2 / 4 (nạp 0/1/2). Cục nước dội ngược: 2.
- Thời gian hạ ước tính: người giỏi (3–4 nhát mỗi lần nổi + dội nước) **≈ 35–50 s**; người mới **≈ 90–120 s**.

## 5. Tình huống đặc biệt
- Đang đứng trên lưng mà nó lặn: rơi xuống nước (đã báo bằng rung 0,7 s).
- Ô lưng là ô ẩn (giá trị 2): chỉ xoá ô 2 khi lặn, không đụng cọc gỗ.
- Hình rắn dài hơn đấu trường: vẽ cắt theo mép đấu trường, phần ngoài không hiện.
- Tên nạp đầy xuyên qua đầu, mỗi mũi chỉ trúng 1 lần.

## 6. Phụ thuộc
Nước (`world/water`), ô gạch, người chơi (`hurtPlayer`, `slashBox`), tên nỏ, camera (khóa giữa đấu trường).

## 7. Núm chỉnh
`HP, P2, UNDER[2], SURGE.W/SPD[2]/DOUBLE, FLOAT.W/HOLD[2]/SHAKE, SPIT.W/N[2]/GAP/SPD/REFL, DMG.*` trong JSON.

## 8. Tiêu chí nghiệm thu
- Đòn đầu luôn là Lao; không chiêu lặp 3 lần; không Nổi 2 lần liền (test 1.000 lần chọn).
- Lúc Nổi có ô đứng được dưới lưng, lặn xong thì hết ô (test).
- Đầu dưới nước không ăn đòn (test).
- Hạ xong thì cổng phải mở (test).
