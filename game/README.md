# Long Thần Chiến Ký — bản chính thức (web)

© Võ Thái Anh · VTA Games

Viết bằng **TypeScript + Vite**, vẽ bằng Canvas 2D, không dùng thư viện game ngoài để giữ nguyên cảm giác tay đã chỉnh trong bản thử (`../prototype/dieu-khien`).

## Chạy
```
npm install
npm run dev        # mở http://localhost:5173
npm test           # chạy test (Vitest)
npm run build      # kiểm tra kiểu + đóng gói ra dist/
```

## Cấu trúc
| Thư mục | Nội dung |
|---|---|
| `src/data/` | **Mọi số chỉnh tay** (JSON): `tuning.json` (điều khiển, sát thương, quái, nước), `boss-thuy-tinh.json` (trùm), `frames.json` (điểm neo + thứ tự khung hình) |
| `src/core/` | Trạng thái lượt chơi (`game.ts`), vòng bước 120Hz (`flow.ts`), nhập liệu, âm thanh WebAudio, tải hình |
| `src/world/` | Bản đồ ô gạch + va chạm (`tilemap.ts`), nước (`water.ts`) |
| `src/levels/` | Màn chơi. `hongThuy.ts` = Màn 1 (thiết kế: `design/levels/hong-thuy-man1.md`) |
| `src/entities/` | Người chơi (Xích Long, Thanh Vũ), chọn khung hình, quái thường, đạn, tên nỏ |
| `src/boss/` | Trùm Thủy Tinh (thiết kế: `design/gdd/boss-thuy-tinh.md`) |
| `src/render/` | Vẽ: thế giới, người chơi, trùm, nước, HUD |
| `public/assets/` | Hình khung (`frames/`), nền (`bg/`), tiếng hô (`voice/`) |
| `tests/unit/` | Test logic: va chạm, trùm chọn chiêu, mưa giáo luôn có khe né, nước, người chơi |

## Quy ước
- Không biến toàn cục: mọi hệ thống nhận `Game` làm tham số (dễ test).
- Số ngẫu nhiên của logic trùm đi qua `g.rng` (có hạt giống) để test tất định.
- Thêm khung hình: bỏ `f_<tên>.png` vào `public/assets/frames/`, ghi tên + điểm neo vào `src/data/frames.json`.
- Thêm màn: tạo file trong `src/levels/` theo kiểu `LevelDef`.
