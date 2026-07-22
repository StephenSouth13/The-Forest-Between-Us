# Unity Editor Completion Guide

Ngay 2026-07-22

File nay dung de mo Unity len va lam tung buoc. Muc tieu gan nhat: hoan thien tutorial thanh mot vong choi day du: nhan vat spawn dung, hoc phim, di toi tin hieu radio, nhat radio bang `F`, radio mo dau story Mai An Tiem, sau do chuyen sang Day 1.

## 1. Mo Dung Scene

Trong Unity:

1. Mo project: `The Forest Between Us`.
2. Mo scene: `Assets/Scenes/Tutorial.unity`.
3. Kiem tra Build Settings:
   - Scene 0: `Assets/Scenes/Home.unity`
   - Scene 1: `Assets/Scenes/Tutorial.unity`

## 2. Chinh Nhan Vat Player

Chon player that trong Hierarchy.

Can dam bao:

- GameObject player co tag `Player`.
- Player co `CharacterController`.
- Player co `ThirdPersonController` tu Starter Assets.
- Player co `PlayerInteraction`.
- Player co `PlayerPostureController`.
- Chi nen co 1 player active trong scene.

Inspector cho `PlayerInteraction`:

- `Interact Key`: `F`
- `Player Camera`: gan Main Camera neu co.
- Neu khong gan camera, tao empty object truoc mat player, dat ten `InteractionRayOrigin`, gan vao `Ray Origin`.
- `Interact Distance`: `3`
- `Prompt Panel`: gan panel UI hien prompt.
- `Prompt Text`: gan TextMeshProUGUI hien chu prompt.
- `Prompt Format`: `Press F - {0}`

Inspector cho `PlayerPostureController`:

- `Crouch Key`: `C`
- `Prone Key`: `X`
- `Standing Height`: khoang `1.8`
- `Crouch Height`: khoang `1.1`
- `Prone Height`: khoang `0.55`
- `Animator`: gan Animator cua model nhan vat neu animator co bool crouch/prone.
- Neu animator chua co bool `IsCrouching` va `IsProne`, tam thoi co the de trong. Code van doi height capsule.

## 3. Tao UI Prompt Tuong Tac

Trong Canvas cua scene Tutorial:

1. Tao panel nho o gan duoi man hinh, dat ten `InteractionPromptPanel`.
2. Cho panel mac dinh inactive.
3. Ben trong panel tao TextMeshPro text, dat ten `InteractionPromptText`.
4. Keo panel vao `PlayerInteraction.promptPanel`.
5. Keo text vao `PlayerInteraction.promptText`.

Test nhanh:

- Play scene.
- Nhin vao object co `Interactable`.
- UI phai hien: `Press F - Pick up Radio` hoac ten item.

## 4. Gan MissionManager

Trong Hierarchy:

1. Tao empty GameObject: `[MISSION_MANAGER]`.
2. Add Component: `MissionManager`.
3. Trong `All Quests`, size = `1`.
4. Element 0: keo asset `Assets/_GAME/Data/Quests/Tutorial_Quest.asset`.
5. `Current Day`: `1`.
6. `Radio Object`: keo radio object trong scene vao day.

Luu y:

- Khi scene bat dau, `MissionManager` se an radio.
- Sau khi player hoc xong cac phim tutorial, `TutorialManager` goi `MissionManager.ActivateRadio()` de bat radio len.

## 5. Gan QuestManager

Chon GameObject dang co `QuestManager`.

Can dam bao:

- `Active Quest`: `Tutorial_Quest.asset`.
- `Title Text`: text UI hien ten quest.
- `Objective Text`: text UI hien objective.
- `Story Overlay`: text UI hien doan story ngan luc quest bat dau.

`Tutorial_Quest.asset` hien nen co 3 step:

1. `Movement` - Master controls.
2. `ReachTarget` - Reach the first radio signal.
3. `Interaction` - Pick up the radio.

## 6. Gan TutorialManager

Chon GameObject `[TUTORIAL_LOGIC]` hoac object dang co `TutorialManager`.

Can dam bao:

- `Keys Panel`: panel hien cac phim tutorial.
- Gan cac image key: `W`, `A`, `S`, `D`, `Space`, `Shift`, `C`, `X`.
- `Player Transform`: transform cua player active.
- `Goal Transform`: transform gan radio/tin hieu radio dau tien.
- `Finish Distance`: `3`.
- `Reach Radio Objective`: `Reach the radio signal.`

Neu muon tutorial ngan hon:

- Chi can dung `W`, `A`, `S`, `D`, `Space`, `Shift`, `C`, `X`.
- Chua can bat buoc `Tab`, `B`, `F` trong key tutorial, vi `F` se duoc dung rieng cho radio interaction.

## 7. Gan Radio

Chon object radio trong scene.

Can dam bao:

- Radio co Collider.
- Collider co the la trigger hoac non-trigger, mien la raycast cham duoc.
- Add Component: `RadioInteractable`.
- `Prompt`: `Pick up Radio`.
- `Destroy After Pickup`: false neu muon an object.
- `Object To Hide`: co the keo chinh radio object vao day, hoac de trong de script tu `SetActive(false)`.

Quan trong:

- Radio object phai nam trong tam raycast cua player/camera.
- Neu radio nam tren layer rieng, layer do phai nam trong `PlayerInteraction.interactableLayers`.

## 8. Gan Inventory Co Ban

Neu scene Tutorial co backpack/inventory UI:

1. Tao GameObject `[INVENTORY_MANAGER]`.
2. Add Component: `InventoryManager`.
3. `Slot Container`: keo parent object chua cac slot UI.
4. Moi slot UI can co `InventorySlot`.
5. Trong moi `InventorySlot`, gan:
   - `Icon Display`
   - `Count Text`

Item pickup:

- Object item can co Collider.
- Add Component: `ItemObject`.
- Gan `ItemData`.
- Set `Amount`.

## 9. Test Flow Tutorial

Play scene va test theo thu tu:

1. Player spawn dung vi tri.
2. Player di chuyen bang `WASD`.
3. `Space`, `Shift`, `C`, `X` duoc tutorial ghi nhan.
4. Sau khi hoc xong phim, keys panel tat.
5. Radio duoc bat len.
6. Objective doi sang `Reach the radio signal`.
7. Di toi radio, objective reach target hoan tat.
8. Nhin vao radio, prompt hien `Press F - Pick up Radio`.
9. Bam `F`, radio bien mat.
10. Quest step `Interaction` hoan tat.
11. Man hinh chuyen sang objective/story tiep theo cho Day 1.

Hien tai buoc 11 chua co code day du. Sau khi radio pickup, can code them `RadioController` hoac transition sang Day 1.

## 10. Folder Nen Giu

Khong nen di chuyen:

- `Assets/StarterAssets`
- `Assets/Devion Games`
- `Assets/Toby Fredson`
- `Assets/Scripts/Packages`
- `Assets/Art`
- `Assets/Audio`
- `Assets/Models`

Code tu viet nen de trong:

- `Assets/_GAME/Scripts`
- `Assets/_GAME/Data`
- `Assets/_GAME/Prefabs` neu sau nay can tao prefab rieng.
- `Assets/_GAME/ScriptableObjects` neu muon tach data lon hon.

## 11. Viec Code Nen Lam Sau Khi Scene Chay

Sau khi tutorial scene chay tron flow, moi code tiep:

1. `RadioController`
   - Phat static audio.
   - Tang/giam nhieu theo khoang cach objective.
   - Phat voice line Mai An Tiem theo quest step.
   - Dieu huong Day 1.

2. `PlayerStats`
   - Health.
   - Stamina drain khi sprint.
   - Stamina regen khi dung/y di bo.
   - Ket noi voi `PlayerStatusUI`.

3. `DayNightManager`
   - Dieu khien ngay/dem.
   - Fog.
   - Enemy spawn ve dem.

4. `SaveLoadManager`
   - Luu quest progress.
   - Luu inventory.
   - Luu vi tri player.

## Trang Thai Chot

Code hien tai du de lap tutorial prototype. Phan quan trong nhat bay gio khong phai them code moi ngay, ma la noi scene trong Unity cho dung:

- Player active dung.
- Managers co trong scene.
- Quest data du 3 step.
- Radio co `RadioInteractable`.
- Prompt UI duoc gan.
- Build Settings dung `Home` va `Tutorial`.
