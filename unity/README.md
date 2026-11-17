# Long Thần Chiến Ký — bản Unity

© Võ Thái Anh · VTA Games

Chuyển từ bản web (`../game`, TypeScript) sang C# ngày 2026-11-17, **giữ nguyên từng con số và cảm giác tay**:
cùng một chuỗi bấm nút, nhân vật ở bản Unity và bản web đứng đúng cùng tọa độ (đã so 600 bước vật lý).

## Mở và chơi
1. Unity Hub → **Add project from disk** → chọn thư mục `unity/` (dùng Unity 6, bản 6000.x).
2. Lần đầu mở, Unity tự tạo `Library/`, `ProjectSettings/` và nhập ảnh (ảnh pixel tự đặt lọc Point nhờ `PixelArtImporter`).
   Nếu Unity hỏi bật **Input System** thì chọn **Yes** (code chạy được cả kiểu cũ lẫn kiểu mới).
3. Mở cảnh bất kỳ (cảnh trống cũng được) rồi bấm **Play**. Game tự khởi động, không cần kéo thả gì.
4. Phím: ←→ chạy · Z/K/Space nhảy · X/J chém/bắn (giữ để nạp) · C/L/Shift lướt · Q/Tab đổi nhân vật · R về điểm lưu. Có tay cầm và nút cảm ứng trên điện thoại.

## Cấu trúc
| Thư mục | Nội dung |
|---|---|
| `Assets/Scripts/Core/` | **Toàn bộ luật chơi**, C# thuần, không dùng UnityEngine (`LongThan.Core.asmdef`): người chơi, quái, trùm Thủy Tinh, nước, màn Hồng Thủy, va chạm ô gạch |
| `Assets/Scripts/Game/` | Phần Unity: `GameRunner` (vòng lặp 120Hz, điều khiển, menu, chữ), `WorldRenderer` + `Painter` (vẽ 2D tức thời bằng GL, giống Canvas bản web), `SynthSfx` (âm thanh tạo bằng code) |
| `Assets/Resources/Data/` | **Mọi số chỉnh tay** (JSON, giống `game/src/data`) |
| `Assets/Resources/Frames/`, `Bg/`, `Voice/` | Hình khung nhân vật/trùm/quái, nền, tiếng hô |
| `Assets/Tests/EditMode/` | 19 test (Window → General → Test Runner → EditMode → Run All) |
| `Tools/CoreCheck/` | Chạy chính các test đó **ngoài Unity** bằng .NET 8: `cd Tools/CoreCheck && dotnet run` |
| `Tools/UnityStubCheck/` | Biên dịch thử code phía Unity với lớp UnityEngine giả, để bắt lỗi gõ khi không mở Unity: `dotnet build` |

## Quy ước
- Luật chơi chỉ nằm trong `Core/`; `Game/` chỉ đọc trạng thái để vẽ và đưa phím bấm vào.
- Số ngẫu nhiên ảnh hưởng luật chơi đi qua `g.rng` (có hạt giống) để test tất định; hiệu ứng dùng `g.R()`.
- Thêm khung hình: bỏ `f_<tên>.png` vào `Resources/Frames/`, ghi điểm neo vào `Resources/Data/frames.json`.
