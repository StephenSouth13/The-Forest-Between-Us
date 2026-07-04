# Development Roadmap

## Mục Tiêu Gần Nhất

Hoàn thiện tutorial thành một vòng chơi đầy đủ: spawn nhân vật, học phím, đi tới radio, nhặt radio, nhận tín hiệu Mai An Tiêm, mở nhiệm vụ ngày 1.

## Checklist Code

1. Player
   - Dùng `StarterAssets/ThirdPersonController` làm nền.
   - Gắn tag `Player`.
   - Kiểm tra `TutorialManager.playerTransform` đang trỏ đúng player.
   - Thêm `PlayerInteraction` để raycast từ camera hoặc trigger vùng gần.

2. Interaction
   - Chuẩn hóa interface `Interactable`.
   - `ItemObject` dùng cho vật phẩm thường.
   - Tạo `RadioInteractable` riêng cho radio để vừa nhặt vừa gọi quest/story.
   - Phím chính: `F`.

3. Quest
   - Tutorial nên có 3 bước:
     - `Movement`: master controls.
     - `ReachTarget`: reach first radio signal.
     - `Interaction`: pick up the radio.
   - `QuestTrigger` chỉ dùng cho vùng đến nơi.
   - Collect/item interaction nên gọi quest từ object được nhặt.

4. Radio
   - Radio là "la bàn truyện", không chỉ là item.
   - Radio phát static khi gần objective.
   - Voice line của Mai An Tiêm chỉ bật khi quest đổi step.
   - Về sau radio có thể rung, đổi tần số, hoặc sai lệch khi có enemy.

5. Survival
   - Ngày 1: stamina + crouch/prone để né nguy hiểm.
   - Ngày 2: inventory + craft thô sơ.
   - Ngày 3: puzzle + ending choice.

## Gợi Ý Góc Nhìn Nhiệm Vụ

Nên giữ góc nhìn thứ ba, camera thấp và hơi lệch vai khi đi trong rừng. Khi radio phát tín hiệu, camera có thể siết FOV nhẹ hoặc thêm camera rumble rất nhỏ để tạo cảm giác tín hiệu đang kéo player đi.

UI nhiệm vụ nên ít chữ. Hãy để radio, ánh sáng emission trên dưa hấu và fog dẫn đường. Quest text chỉ nên là nhắc ngắn như:

- Reach the radio signal.
- Follow the black seeds.
- Hide until the swarm passes.
- Keep the fire alive.
- Restore the relay circuit.

## Rủi Ro Cần Xử Lý

- `Misson_Manager` đang sai chính tả tên class/file; nên đổi thành `MissionManager` khi chưa có nhiều scene/prefab phụ thuộc.
- Singleton hiện đủ dùng cho prototype, nhưng về sau nên có `GameSession` hoặc `ServiceLocator` nhẹ.
- Inventory hiện chưa xử lý phần dư khi nhặt số lượng vượt `maxStackSize`.
- Scene `Tutorial.unity` rất lớn, nên tránh sửa thủ công bằng text nếu không cần.
- Build settings đang trỏ tới scene sample cũ, cần cập nhật trước khi build demo.
