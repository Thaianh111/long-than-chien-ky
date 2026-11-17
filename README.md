# Long Thần Chiến Ký (龍神戰記)

Game hành động platformer 2D lấy cảm hứng từ dòng Mega Man X, đặt trong một Việt Nam tương lai (năm 2189) pha trộn lịch sử và truyền thuyết Việt.

© Võ Thái Anh · VTA Games

## Cốt truyện

Bản chốt cốt truyện, nhân vật và game brief Phần 1 nằm trong tài liệu:
https://claude.ai/artifact/Jy8bSzPmuMKpGM59HQHiAs

Tóm tắt: AI điều hành quốc gia **Thiên Lý** muốn "giữ sơn hà" bằng cách tỉa bớt con người. Kiếm sĩ giáp đỏ **Xích Long** và xạ thủ **Thanh Vũ** của Bạch Đằng Vệ chống lại nó qua 8 phần — trong khi bí mật lớn nhất nằm trong chính Xích Long: hạt giống **Ma Chủng** gây ra cơn loạn của người máy.

## Phần 1: Hồng Thủy — bản đầu tiên (MVP)

1. Điều khiển chuẩn trên PC và nút ảo trên điện thoại
2. Xích Long chém combo 3 nhát
3. Thanh Vũ bắn Thần Nỏ có nạp lực
4. Màn mở đầu và cảnh Xích Long hy sinh
5. 2 vùng, 2 trùm (Thủy Tinh, Thiết Mã), hạ xong lấy được vũ khí
6. Trùm Thiên Lý bản rút gọn
7. Màn mở đầu VTA Games và lưu game

Engine: Unity, PC + điện thoại.

## Thư mục

- `prototype/dieu-khien/` — bản thử điều khiển Xích Long (HTML, vứt đi sau khi thử; không đưa code này vào game thật). Chỉnh cảm giác tay ở khối `T` đầu file.
  - Hoạt ảnh: mỗi dáng có điểm neo hông riêng (`FA`) nên đổi dáng không bị giật ngang; nhân vật vẽ nội suy giữa các bước vật lý (mượt trên màn 60/90/120Hz); có co giãn khi nhảy, đáp, lướt, quay đầu; nhún theo bước chạy; bóng mờ khi lướt; giữ dáng chém thêm một nhịp khi dứt combo.
  - Chém kiểu Zero: vệt bán nguyệt (`drawSwing`/`crescent`, chỉnh `ARC_*`), trúng khi đao quét qua `HIT_AT`, khựng `HITSTOP` (nhát cuối x`FINISH_STOP`), vết cắt + vòng nổ + chớp màn hình nhát cuối, tiếng vút/tiếng trúng bằng WebAudio. Nhảy 2 bậc: `AIR_JUMPS`, `AIR_JUMP_VEL`.
  - Đổi nhân vật: màn chọn nhân vật lúc vào, nút ⇄ trên cùng / phím Q. Thanh Vũ bắn Thần Nỏ: bấm = tên thường, giữ 0,35 giây = nạp cấp 1, giữ 1 giây = nạp đầy (tên to xuyên quái); bắn được khi đứng, chạy, nhảy, bám tường (quay lưng vào tường). Thông số ở `T.TV`, đầu nỏ theo dáng ở `MZ`.
  - Thanh Vũ đã đủ bộ khung (Gemini, 2026-11-17): đứng thở 4, bắn đứng 4, chạy 4, chạy bắn 4, nhảy 5, nhảy bắn 5, lướt 3, bám tường 3. Tấm gốc ở `art/characters/thanh_vu/sheets/`.
  - Xuất hiện kiểu Mega Man X8: tia năng lượng rơi xuống rồi hiện người từ chân lên (`warpIn`, `WARP_*`), khi chọn/đổi nhân vật và khi hồi sinh. Tiếng hô tổng hợp bằng WebAudio (`shout`, bảng `CRIES`, âm lượng `VOICE`) khi chém từng nhát và khi bắn theo cấp nạp.
  - Thêm khung mới: bỏ file vào `frames/`, ghi tên vào `ANIM` (hoặc `ATKF` cho nhát chém) và điểm neo hông vào `FA` (`FY` nếu cần dịch dọc).
  - Xích Long đã đủ bộ khung (Gemini, 2026-11-17): chạy 8, nhảy 5, lướt 3, chém nhát 1/2/3 = 5/5/6, bám + đạp tường 3, đứng thở 4, chém trên không 5. Tấm gốc ở `art/characters/xich_long/sheets/`.
- **Màn 1 Hồng Thủy** (bản chơi thử): phố ngập, quảng trường có hố nước, giếng nước dâng đuổi (cửa sập sau lưng), mái nhà, đấu trường trùm. Bố cục: `design/levels/hong-thuy-man1.md`. Nước làm chậm và rút máu từ từ (`WATER_K`), không chết ngay.
  - Trùm **Thủy Tinh** (`BT`, `updateBoss`): 32 máu, 5 chiêu Sóng Thần / Đinh Ba Lao / Thủy Cầu Phao (chém bong bóng bay ngược lại) / Hộ Tâm Thủy Kính (chỉ tên nạp đầy phá được, trùm choáng) / Vũ Thương; dưới nửa máu gầm, nước dâng 2 ô, thêm Đại Hồng Thủy (nước tím, phải bám tường). Thiết kế: `design/gdd/boss-thuy-tinh.md`. Hình trùm đang vẽ tạm bằng code, chờ tấm Gemini (`art/bosses/thuy_tinh/PROMPT_THIET_KE.md`).
- `tools/cat_khung.py` — cắt tấm hình nhiều dáng nền xanh lá thành các khung 150×110, cùng tỉ lệ, ép về bảng màu chung.
- `art/characters/xich_long/PROMPT_KHUNG_HINH.md` — prompt vẽ thêm khung chạy, nhảy, lướt, chém.

## Ghi công (credit)
- Tiếng hô nhân vật: "Male Grunt/Yelling sounds" — HaelDB, OpenGameArt (CC0 / OGA-BY 3.0).
- Quái tạm (drone, tháp súng, nổ, đạn) và nền thành phố: "Warped City" — Ansimuz, OpenGameArt (CC0). Đã đổi màu tím Ma Chủng.
