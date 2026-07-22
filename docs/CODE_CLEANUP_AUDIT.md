# Code Cleanup Audit

Ngày kiểm tra: 2026-07-22

## Kết luận

Folder code tự viết trong `Assets/_GAME/Scripts` đã đi đúng hướng, nhưng mới ổn cho prototype. Sau lần cleanup này, các lỗi dễ gây kẹt tutorial đã được giảm bớt, còn phần phải nối trong Unity Inspector trước khi gọi là ổn định.

## Đã chỉnh

- Đổi `Misson_Manager` thành `MissionManager`.
- Giữ nguyên GUID `.meta` khi rename manager để Unity có thể giữ reference nếu có.
- Cập nhật `TutorialManager` gọi `MissionManager.instance`.
- Thêm step `Interaction` vào `Tutorial_Quest.asset` để radio pickup có thể advance quest.
- Sửa `InventoryManager.PickUpItem()` để không làm mất số lượng dư khi nhặt item stack vượt `maxStackSize`.
- Thêm null guard cho:
  - `ItemObject` khi thiếu `InventoryManager`.
  - `QuestTrigger` khi thiếu `QuestManager`.
  - `InventorySlot` khi thiếu `iconDisplay` hoặc `countText`.
- Cập nhật Build Settings về:
  - `Assets/Scenes/Home.unity`
  - `Assets/Scenes/Tutorial.unity`

## Vẫn cần chỉnh trong Unity Editor

- Gắn `MissionManager` vào scene `Tutorial.unity`.
- Trong `MissionManager`:
  - `allQuests[0]` = `Tutorial_Quest.asset`
  - `radioObject` = object radio trong scene.
- Gắn `RadioInteractable` vào object radio.
- Nối `PlayerInteraction.promptPanel` và `PlayerInteraction.promptText`.
- Kiểm tra chỉ có một player/controller active trong tutorial scene.
- Kiểm tra `PlayerInteraction.playerCamera` hoặc `rayOrigin`; hiện scene đang để null nên raycast sẽ dùng fallback từ transform player.

## Chưa nên clean mạnh lúc này

- Chưa nên di chuyển asset package/demo trong `Assets/Scripts/Packages`, `StarterAssets`, `Devion Games`, `Toby Fredson`, vì dễ mất dependency.
- Chưa nên sửa trực tiếp scene `Tutorial.unity` bằng text nếu không cần, vì file scene rất lớn.
- Chưa nên tách singleton thành service architecture ngay; prototype hiện vẫn đủ cho tutorial. Nên làm sau khi Day 1 có loop rõ.

## Nên làm tiếp

1. Hoàn thiện scene wiring cho tutorial.
2. Tách `RadioController` để radio đảm nhiệm static, voice line, dẫn hướng và story beat.
3. Tạo `PlayerStats` thật cho stamina, thay `PlayerStatusUI` mock.
4. Tách folder `Manager` thành các nhóm rõ hơn khi hệ thống lớn:
   - `Systems/Quest`
   - `Systems/Inventory`
   - `Systems/Tutorial`
   - `Systems/Radio`
