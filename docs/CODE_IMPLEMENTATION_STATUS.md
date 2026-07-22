# Code Implementation Status

Ngày kiểm tra: 2026-07-22

## Tóm tắt nhanh

Prototype hiện đã có nền tảng tutorial khoảng 70-75% ở mức code và scene wiring cơ bản.

Đã có:
- Menu scene `Home.unity` và gameplay scene `Tutorial.unity`.
- Quest tuyến tính bằng `QuestData` + `QuestManager`.
- Tutorial học phím, sau đó chuyển sang mục tiêu tìm radio.
- Raycast interaction bằng phím `F`.
- `RadioInteractable` để nhặt radio và advance quest step `Interaction`.
- Inventory cơ bản gồm data item, item pickup, slot UI, filter theo category.
- Crouch/prone controller đổi chiều cao `CharacterController`.
- Player tag đã có trong `TagManager.asset`.

Chưa hoàn chỉnh:
- Quest data tutorial đã có 3 bước `Movement`, `ReachTarget`, `Interaction`, nhưng cần verify trong Unity Inspector sau khi mở project.
- `RadioInteractable` tồn tại ở code nhưng chưa thấy được gắn vào object trong `Tutorial.unity`.
- `MissionManager` tồn tại ở code nhưng chưa thấy được gắn trong `Tutorial.unity`, nên luồng `ActivateRadio()` có thể chưa chạy thật trong scene.
- `InventoryManager` chưa thấy được gắn trong scene tutorial.
- Prompt interaction trong `PlayerInteraction` chưa được nối UI trong scene (`promptPanel` và `promptText` đang null).
- `PlayerPostureController` có code nhưng animator chưa được nối, chỉ đổi capsule height.
- Stamina/survival hiện mới là UI mock, chưa có `PlayerStats` gameplay thật.
- Radio chưa có controller riêng cho static audio, voice line, frequency, rumble, dẫn hướng.
- Build settings đã được chỉnh về `Home` và `Tutorial`, cần verify lại trong Unity Editor.

## Đối chiếu với roadmap

### Player

Trạng thái: gần xong phần nền.

- Có Starter Assets ThirdPersonController trong project.
- Scene có player tag.
- `TutorialManager.playerTransform` đã được gắn trong `Tutorial.unity`.
- Đã có `PlayerInteraction`.
- Còn cần kiểm tra trong Unity: chỉ giữ player active đúng, vì scene đang có ít nhất 2 component `PlayerInteraction`, trong đó một cụm bị disable và một cụm enabled.

### Interaction

Trạng thái: code đã viết, scene chưa nối đủ.

- Có interface `Interactable`.
- `ItemObject` implement `Interactable`.
- Có `RadioInteractable` riêng cho radio.
- Phím chính đang là `F`.
- Cần gắn `RadioInteractable` vào radio prefab/object trong scene.
- Cần gắn prompt UI vào `PlayerInteraction`.

### Quest

Trạng thái: core chạy được, data thiếu một bước quan trọng.

- `QuestManager` quản lý quest theo step.
- `Tutorial_Quest.asset` hiện có:
  - `Movement`: Master controls.
  - `ReachTarget`: Reach the first radio signal.
  - `Interaction`: Pick up the radio.
- Roadmap yêu cầu 3 bước:
  - `Movement`
  - `ReachTarget`
  - `Interaction`
- Cần verify step `Interaction` trong Inspector để đảm bảo `RadioInteractable.AdvanceStep(StepType.Interaction, 1)` chạy đúng sau khi đã hoàn tất `ReachTarget`.

### Radio

Trạng thái: mới là pickup object, chưa là "la bàn truyện".

- Có model/audio radio trong assets.
- Có `RadioInteractable` để nhặt radio.
- Chưa có:
  - `RadioController`
  - static theo khoảng cách objective
  - voice line Mai An Tiêm theo quest step
  - rung/đổi tần số/sai lệch khi có enemy
  - mapping radio với cốt truyện/ngày chơi

### Survival

Trạng thái: mới có mầm hệ thống.

- Có crouch/prone.
- Có `PlayerStatusUI` hiển thị health/stamina/hunger/thirst dạng mock.
- Chưa có gameplay stamina drain/regen.
- Chưa có enemy avoidance, inventory crafting, puzzle, day-night, relay/ending choice.

## Ưu tiên tiếp theo

1. Trong `Tutorial_Quest.asset`, verify step thứ ba:
   - description: `Pick up the radio`
   - type: `Interaction`
   - targetAmount: `1`

2. Trong `Tutorial.unity`, gắn `RadioInteractable` vào radio object:
   - prompt: `Pick up Radio`
   - objectToHide: radio object hoặc để mặc định tự hide gameObject.

3. Gắn hoặc tạo `MissionManager` trong scene:
   - allQuests[0] = `Tutorial_Quest.asset`
   - radioObject = radio object đang cần bật sau khi học phím.

4. Nối UI prompt cho `PlayerInteraction`:
   - `promptPanel`
   - `promptText`

5. Sau khi nhặt radio, thay vì kết thúc bằng text, nên gọi bước mở Day 1:
   - tắt tutorial UI
   - bật radio UI/audio
   - chuyển objective sang nhiệm vụ ngày 1
   - hoặc trigger cutscene ngắn.

6. Verify Build Settings:
   - `Assets/Scenes/Home.unity`
   - `Assets/Scenes/Tutorial.unity`

## Ghi chú cho cốt truyện

Code hiện phù hợp để bạn viết cốt truyện theo cấu trúc:

- Tutorial: Stephen tỉnh dậy, học điều khiển, lần theo tín hiệu đầu tiên.
- Radio pickup: khoảnh khắc Mai An Tiêm bắt đầu "nói" qua tín hiệu.
- Day 1: radio dẫn Stephen tới dấu vết dưa hấu đen.
- Day 2: sinh tồn/craft mở rộng.
- Day 3: relay và lựa chọn kết thúc.

Điểm cần chốt về narrative trước khi code sâu hơn: radio là nhân vật kể chuyện, không chỉ là item. Vì vậy bước code tiếp theo nên là `RadioController` thay vì chỉ thêm nhiều quest text.
