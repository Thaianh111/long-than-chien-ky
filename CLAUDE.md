# Long Thần Chiến Ký — ghi chú cho Claude

Game hành động 2D kiểu Mega Man X của **Võ Thái Anh** (studio **VTA Games**). Việt Nam năm 2189, lịch sử + truyền thuyết Việt.
Đọc file này trước khi làm gì trong repo. Giao tiếp, chữ trong game và tài liệu bằng **tiếng Việt**; credit ghi "Võ Thái Anh".

## Bản đang làm chính: Unity (`unity/`)
- Unity 6. Luật chơi C# thuần ở `unity/Assets/Scripts/Core` (không UnityEngine), phần Unity ở `Assets/Scripts/Game`, số chỉnh tay ở `Assets/Resources/Data/*.json`.
- Chạy test ngoài Unity: `cd unity/Tools/CoreCheck && dotnet run` (19 test). Kiểm tra biên dịch phần Unity: `cd unity/Tools/UnityStubCheck && dotnet build`.
- Trong Unity: Play ở cảnh bất kỳ là chạy (GameRunner tự khởi động). Test: Test Runner → EditMode.
- Vẽ 2D bằng `Painter` (GL tức thời, tọa độ pixel logic cao 360, y hướng xuống) — cùng cách vẽ với bản web nên port hiệu ứng dễ.

## Bản web (`game/`) và bản thử (`prototype/`)
- `game/`: cùng luật chơi bằng TypeScript + Vite (npm test / npm run build). Là bản chơi thử trên trình duyệt/điện thoại; logic hai bản phải giống nhau.
- `prototype/dieu-khien/`: bản thử đầu tiên, không sửa nữa.

## Nội dung hiện có (Phần 1: Hồng Thủy)
- 2 nhân vật: **Xích Long** (đao laze, combo 3 nhát kiểu Zero, chém xoay trên không, chém tan đạn) và **Thanh Vũ** (Thần Nỏ, nạp 3 cấp, cấp 2 xuyên quái).
- Chạy, nhảy 2 bậc, lướt, bám/đạp tường, xuất hiện kiểu tia năng lượng (Mega Man X8).
- Màn 1 Hồng Thủy (`design/levels/hong-thuy-man1.md`), trùm Thủy Tinh 6 chiêu + pha 2 (`design/gdd/boss-thuy-tinh.md`).
- Hình nhân vật/trùm do anh Thái Anh vẽ bằng Gemini theo prompt Claude viết (`art/`), Claude cắt khung bằng `tools/`.
- Cốt truyện bản chốt: https://claude.ai/artifact/Jy8bSzPmuMKpGM59HQHiAs

## Việc tiếp theo (theo game brief)
1. Màn mở đầu VTA Games (logo chó mực) + lưu game.
2. Màn mở đầu chơi Xích Long → đấu Hắc Hổ → cảnh Xích Long hy sinh.
3. Trùm thứ 2 Thiết Mã, vũ khí lấy từ trùm (Thủy Tiễn).
