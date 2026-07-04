# The Forest Between Us

Episode 1: The First Frequency

## Tổng Quan

`The Forest Between Us` là game 3D Unity thuộc hướng action-survival adventure pha psychological thriller, lấy cảm hứng từ truyện dân gian Mai An Tiêm và sự tích quả dưa hấu. Dự án đang được dựng trên Unity `6000.3.12f1`, dùng scene chính `Home` và `Tutorial`.

Nhân vật chính là Stephen, một kỹ sư công nghệ kiệt sức. Sau khi đọc lại truyện Mai An Tiêm lúc nửa đêm, Stephen bị kéo khỏi phòng ngủ bởi tín hiệu radio dị thường đúng `03:00 AM`. Anh tỉnh dậy trên một hòn đảo mù sương, nơi ký ức dân gian, nhiễu sóng vô tuyến và nỗi sợ sinh tồn bị trộn lại thành một vùng không gian méo mó.

## Cốt Truyện

Stephen không bị rơi máy bay hay trôi dạt theo kiểu sinh tồn quen thuộc. Anh bị kéo vào một "vùng ký ức" được kích hoạt bởi chiếc radio cũ. Trên đảo, những dấu vết của Mai An Tiêm xuất hiện qua tần số radio, hạt dưa màu đen, quả dưa hắc hóa và các cột mốc dẫn đường.

Vòng lặp chính dự kiến kéo dài 3 ngày 2 đêm:

1. Tutorial: Stephen tỉnh dậy trên bãi cát, học di chuyển, tìm tín hiệu radio đầu tiên.
2. Ngày 1: tìm dấu vết dưa hấu đen, học cách định vị trong sương mù và tránh mối nguy ban đêm.
3. Ngày 2: thu thập tài nguyên, dựng trại, chế tạo công cụ thô sơ.
4. Ngày 3: tới trạm relay/ăng-ten cũ, sửa tín hiệu và mở lựa chọn kết thúc.

Hai hướng kết thúc chính:

- Chấp nhận tín hiệu: Stephen tin vào radio và mắc kẹt trong vòng lặp.
- Phá radio: đảo tan biến, Stephen tỉnh lại, nhưng dấu vết nước biển và hạt dưa đen vẫn còn.

## Trạng Thái Hiện Tại

Dự án đã có nền tảng tutorial khoảng 70%:

- Có scene `Home.unity` cho menu.
- Có scene `Tutorial.unity` với player, UI tutorial, quest UI và mục tiêu radio.
- Có `QuestData` dạng ScriptableObject tại `Assets/_GAME/Data/Quests/Tutorial_Quest.asset`.
- Có `QuestManager` quản lý quest tuyến tính theo step.
- Có `TutorialManager` kiểm tra các phím `WASD`, `Space`, `Shift`, `C`, `X`, sau đó chuyển sang bước tìm radio.
- Có `QuestTrigger` để trigger objective khi player đi vào vùng.
- Có inventory cơ bản gồm `ItemData`, `ItemObject`, `InventorySlot`, `InventoryManager`.

Đã chỉnh lại logic tutorial để bước thứ hai là `ReachTarget` thay vì cộng nhầm `Movement`, đồng thời `finishDistance` trong scene tutorial được đưa về `3m`.

## Cấu Trúc Dự Án

```text
The Forest Between Us/
├── Assets/
│   ├── _GAME/
│   │   ├── Data/
│   │   │   └── Quests/
│   │   └── Scripts/
│   │       ├── Home/
│   │       ├── Inventory/
│   │       ├── Manager/
│   │       ├── Missions/
│   │       ├── Player/
│   │       └── Triggers/
│   ├── Art/
│   ├── Audio/
│   ├── Features/
│   ├── Models/
│   ├── Scenes/
│   ├── StarterAssets/
│   ├── Toby Fredson/
│   └── UI/
├── Packages/
├── ProjectSettings/
├── DEVELOPMENT_ROADMAP.md
└── readme.md
```

Nguyên tắc sắp xếp:

- Code gameplay tự viết đặt trong `Assets/_GAME/Scripts`.
- Data gameplay tự viết đặt trong `Assets/_GAME/Data`.
- Asset từ package, demo, foliage, starter asset giữ nguyên thư mục gốc để tránh mất dependency.
- Khi move file Unity, luôn move cả `.meta` để giữ GUID.

## Hướng Code Tiếp Theo

Ưu tiên 1 là hoàn thiện vòng tutorial:

- Gắn player thật với `StarterAssets ThirdPersonController`.
- Tạo interaction raycast/trigger để nhặt radio bằng phím `F`.
- Sau khi nhặt radio, gọi `QuestManager.AdvanceStep(StepType.Interaction, 1)`.
- Thêm UI prompt nhỏ: "Press F - Pick up Radio".
- Chốt quest tutorial bằng cutscene ngắn hoặc transition sang Day 1.

Ưu tiên 2 là tách hệ thống gameplay:

- `PlayerInteraction`: phát hiện object có `Interactable`.
- `PlayerStats`: health, stamina, hunger/thirst nếu cần.
- `RadioController`: bật/tắt nhiễu, phát voice line, định hướng nhiệm vụ.
- `DayNightManager`: điều khiển ngày, đêm, fog, enemy spawn.
- `SaveLoadManager`: lưu quest progress, inventory, vị trí player.

Ưu tiên 3 là thiết kế nhiệm vụ theo dữ liệu:

- Mỗi ngày là một `QuestData`.
- Mỗi mục tiêu dùng `StepType`: `Movement`, `ReachTarget`, `Collect`, `Interaction`, `Survival`.
- Trigger trong scene chỉ báo tiến độ, không tự chứa logic truyện.
- Radio là "người dẫn chuyện" chính, UI chỉ hỗ trợ.

## Ghi Chú Kỹ Thuật

- Unity version: `6000.3.12f1`.
- Cinemachine, Input System, URP/HDRP packages đều đã có trong manifest.
- `Assets/Scripts/Packages` hiện là code package/demo, chưa gom vào `_GAME`.
- `Assets/_Recovery` chứa scene recovery rất lớn, chỉ dùng khi cần cứu scene.
- `EditorBuildSettings.asset` hiện vẫn chứa scene mẫu cũ từ package; khi build thật nên cập nhật lại về `Home` và `Tutorial`.
