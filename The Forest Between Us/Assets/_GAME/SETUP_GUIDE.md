# 🌲 THE FOREST BETWEEN US — HƯỚNG DẪN SETUP HOÀN CHỈNH

> **Phiên bản:** v1.0  
> **Cập nhật lần cuối:** 06/08/2026  
> **Engine:** Unity (URP)  
> **Thể loại:** Survival Open-World, 30 Ngày Sinh Tồn  

---

## 📋 MỤC LỤC

1. [Tổng Quan Kiến Trúc Game](#-1-tổng-quan-kiến-trúc-game)
2. [Bước 0: Tạo Developer Control Center (Brain Scene)](#-bước-0-tạo-developer-control-center)
3. [Bước 1: Setup Scene Home (Menu Chính)](#-bước-1-setup-scene-home)
4. [Bước 2: Setup Scene GamePlay](#-bước-2-setup-scene-gameplay)
5. [Bước 3: Sinh Vật Phẩm & Công Thức](#-bước-3-sinh-vật-phẩm--công-thức)
6. [Bước 4: Gắn Tài Nguyên Thu Hoạch](#-bước-4-gắn-tài-nguyên-thu-hoạch)
7. [Bước 5: Setup NPC & Thương Nhân](#-bước-5-setup-npc--thương-nhân)
8. [Bước 6: Setup Quái Vật & Chiến Đấu](#-bước-6-setup-quái-vật--chiến-đấu)
9. [Bước 7: Setup Hệ Thống Crafting](#-bước-7-setup-hệ-thống-crafting)
10. [Bước 8: Setup Save Point & Respawn](#-bước-8-setup-save-point--respawn)
11. [Bước 9: Kết Nối Build Settings](#-bước-9-kết-nối-build-settings)
12. [Phím Điều Khiển](#-phím-điều-khiển)
13. [Danh Sách Toàn Bộ Script](#-danh-sách-toàn-bộ-script)
14. [Checklist Trước Khi Build](#-checklist-trước-khi-build)

---

## 🏗 1. TỔNG QUAN KIẾN TRÚC GAME

```
Developer_Control_Center.unity (Brain Scene - Chỉ Dev mở)
   └── GAME_DIRECTOR_BRAIN (GameDirector.cs)
         ├── Cấu hình World: Đói/Khát/Ngày Đêm
         ├── Cấu hình Player: Tốc độ/Nhảy
         ├── Cấu hình Death: Hồi sinh/Rớt đồ/Karma
         └── Cấu hình Economy: Cửa hàng Vỏ Sò

Home.unity (Menu Chính)
   ├── MainMenuUI (SceneTransitionManager.cs)
   └── Nút Play / Settings / Quit

GamePlay.unity (Màn Chơi Chính)
   ├── PLAYER (FPSController + PlayerStatsManager + PlayerEquipmentManager + ...)
   ├── MANAGERS (DayManager + InventoryManager + CraftingManager + ...)
   ├── UI_CANVAS (CircularSurvivalHUD + BackpackUI + CraftingUI + ...)
   ├── ENVIRONMENT (Cây/Đá/Thú/NPC/Campfire/...)
   └── ENEMIES (EnemyAI + SwarmController)
```

---

## 🧠 BƯỚC 0: TẠO DEVELOPER CONTROL CENTER

Đây là Scene "Đầu Não" dành **riêng cho nhà phát triển** (bạn). Không cần thêm vào Build.

### Cách tạo:
1. Mở Unity Editor
2. Menu: **`Tools > Forest Between Us > Create Developer Dashboard Scene`**
3. Unity sẽ tự tạo file `Assets/_GAME/Scenes/Developer_Control_Center.unity`
4. Mở Scene đó lên → Click vào GameObject **`GAME_DIRECTOR_BRAIN`**
5. Trong Inspector, bạn sẽ thấy **Custom Dashboard** với các Tab:

| Tab | Chức Năng |
|-----|-----------|
| 🌍 WORLD | Chỉnh tốc độ tụt Đói/Khát/Thể Lực, tốc độ Ngày Đêm |
| 🧍 PLAYER | Chỉnh tốc độ Đi/Chạy/Nhảy |
| 💀 DEATH & COMBAT | Chỉnh % máu hồi sinh, rớt đồ khi chết, hệ số sát thương quái |
| 🛒 ECONOMY | Cài đặt Tiền tệ (Vỏ Sò), danh sách hàng bán |
| 🎬 SCENES | Nút chuyển nhanh giữa các Scene |

> **QUAN TRỌNG:** Bạn phải kéo Prefab **GAME_DIRECTOR_BRAIN** vào Scene **GamePlay** để các cấu hình có hiệu lực khi chơi. Hoặc gắn `GameDirector.cs` vào một GameObject trong Scene GamePlay.

---

## 🏠 BƯỚC 1: SETUP SCENE HOME

### Tạo tự động:
Menu: **`Tools > Forest Between Us > Setup Main Menu UI`**

### Hoặc Setup thủ công:
1. Tạo Scene mới tên `Home`
2. Tạo Canvas → Gắn các nút: **Chơi Mới**, **Tiếp Tục**, **Cài Đặt**, **Thoát**
3. Gắn Script `SceneTransitionManager.cs` vào một GameObject rỗng
4. Nút "Chơi Mới" gọi hàm `SceneTransitionManager.instance.LoadScene("GamePlay")`

---

## 🎮 BƯỚC 2: SETUP SCENE GAMEPLAY

Đây là Scene chính của game. Bạn cần tạo các **GameObject rỗng** và gắn Script vào theo thứ tự sau:

### 2.1 PLAYER (Nhân Vật)
Đã có sẵn `FPSController` từ package Toby Fredson. Bạn cần thêm các Script sau vào Player:

| Script | Gắn Vào | Chức Năng |
|--------|---------|-----------|
| `PlayerStatsManager.cs` | Player | Quản lý Máu/Đói/Khát/Thể Lực/XP/Level |
| `PlayerEquipmentManager.cs` | Player | Quản lý trang bị đang mặc (Giáp/Giày/Vũ khí) |
| `PlayerInteraction.cs` | Player | Phát Raycast tương tác đối tượng bằng phím E |
| `PlayerPostureController.cs` | Player | Cúi/Đứng |
| `PlayerDiseaseManager.cs` | Player | Quản lý bệnh tật (Sốt rét, Cảm cúm) |

### 2.2 MANAGERS (Các Quản Lý)
Tạo một GameObject rỗng tên `_MANAGERS` và gắn **tất cả** các Script quản lý:

| Script | Chức Năng |
|--------|-----------|
| `GameDirector.cs` | **Bộ não trung tâm** - Cấu hình toàn game |
| `DayManager.cs` | Chu kỳ Ngày/Đêm 30 ngày |
| `InventoryManager.cs` | Quản lý Balo (nhặt/thả/dùng đồ) |
| `CraftingManager.cs` | Quản lý chế tạo & danh sách công thức |
| `PlayerRespawnManager.cs` | Xử lý chết & hồi sinh |
| `MissionManager.cs` | Nhiệm vụ hàng ngày |
| `QuestManager.cs` | Nhiệm vụ chính (Story Quest) |
| `Campaign30DaysManager.cs` | Sự kiện 30 ngày sinh tồn |
| `FoodSpoilageManager.cs` | Thức ăn thối hỏng theo thời gian |
| `SaveSystem.cs` | Lưu/Tải game bằng PlayerPrefs |
| `EndingManager.cs` | Quản lý kết thúc game (Ngày 30) |
| `TutorialManager.cs` | Hướng dẫn tân thủ |
| `CombatManager.cs` | Quản lý chiến đấu (đánh quái) |

### 2.3 UI CANVAS
Tạo một Canvas → Gắn các UI Controller:

| Script | Chức Năng | Phím Tắt |
|--------|-----------|----------|
| `CircularSurvivalHUD.cs` | Thanh Máu/Đói/Khát/Thể Lực dạng tròn | Luôn hiển thị |
| `PlayerStatusUI.cs` | Bảng thông tin chi tiết nhân vật | Tab |
| `BackpackUIController.cs` | Giao diện Balo (mở/đóng) | I |
| `CraftingUIController.cs` | Sổ Tay Công Thức chế tạo | K |
| `FullMapUIController.cs` | Bản đồ toàn cảnh | M |
| `PauseSettingsUIController.cs` | Menu Tạm Dừng & Cài Đặt | Esc |
| `DialogueChoiceUIController.cs` | Hội thoại phân nhánh NPC | Tự động |
| `TradingUIController.cs` | Giao diện cửa hàng Thương Nhân | Tự động |
| `RadioDialogueUIController.cs` | Thoại qua Radio | Tự động |
| `ObjectiveWaypointArrow.cs` | Mũi tên chỉ mục tiêu | Luôn hiển thị |
| `EndingUIController.cs` | UI kết thúc game | Tự động |
| `RecipeBookUIController.cs` | Sổ tay công thức nâng cao | Tự động |

---

## 📦 BƯỚC 3: SINH VẬT PHẨM & CÔNG THỨC

### Tạo tự động (Khuyến nghị):
Menu: **`Tools > Forest Between Us > Generate Items & Recipes`**

Tool sẽ tự tạo ra tất cả file `.asset` trong `Assets/_GAME/Data/`:

**Vật phẩm Thức ăn:**
| Tên | Loại | Ghi Chú |
|-----|------|---------|
| Raw_Meat | Food | Thịt sống (ăn sống bị ngộ độc -25 HP) |
| Cooked_Meat | Food | Thịt nướng (+40 Đói) |
| Wild_Vegetable | Food | Rau rừng |
| Grilled_Vegetable | Food | Rau xào (+30 Đói) |
| Wild_Berry | Food | Quả mọng dại |

**Vật phẩm Tiêu thụ:**
| Tên | Loại | Ghi Chú |
|-----|------|---------|
| Water_Bottle | Consumable | Bình nước suối (+50 Khát) |
| Super_Health_Potion | Consumable | Thuốc hồi huyết (+100 HP) |
| Super_Energy_Drink | Consumable | Nước tăng lực (+100 Thể lực) |

**Trang bị:**
| Tên | Loại | Giáp | Tốc Độ |
|-----|------|------|--------|
| Iron_Sword | Weapon | 0 | 0 |
| Leather_Armor | Equipment (Chest) | +15 | 0 |
| Iron_Armor | Equipment (Chest) | +40 | -0.5 |
| Leather_Boots | Equipment (Boots) | +2 | +1.5 |

**Tiền tệ:**
| Tên | Loại | Ghi Chú |
|-----|------|---------|
| Item_Seashell | Resource | Vỏ Sò Biển - Đơn vị tiền tệ trao đổi |

**Công thức Chế tạo:**
| Tên Công Thức | Nguyên Liệu | Kết Quả |
|---------------|--------------|---------|
| Thịt Nướng | 1x Thịt Sống | 1x Thịt Nướng |
| Rau Xào | 1x Rau Rừng | 1x Rau Xào |
| Thuốc Hồi Huyết | 1x Rau + 1x Nước | 1x Thuốc Hồi Máu |
| Chế Giáp Da | 2x Thịt Sống | 1x Giáp Da (+15 Giáp) |
| Chế Giày Da | 1x Thịt + 2x Rau | 1x Giày Da (+1.5 Tốc độ) |

> **GHI CHÚ:** Sau khi chạy Tool, bạn có thể mở từng file `.asset` trong `Assets/_GAME/Data/Items/` để kéo thả icon, model 3D, và âm thanh cho từng vật phẩm.

---

## 🌳 BƯỚC 4: GẮN TÀI NGUYÊN THU HOẠCH

### Gắn hàng loạt bằng Tool:
Menu: **`Tools > Forest Between Us > Auto-Assign Trees as Resource Nodes`**

Tool này sẽ tự động quét toàn bộ Scene và gắn `ResourceNode.cs` vào:
- **Cây** (có chữ "tree" trong tên) → Chặt ra Gỗ
- **Đá** (có chữ "rock/stone") → Đập ra Đá
- **Bụi cây** (có chữ "berry/bush") → Hái ra Quả Mọng

### Gắn thủ công:
1. Chọn bất kỳ Object nào trong Scene (cây, đá...)
2. Add Component → `ResourceNode`
3. Thiết lập:
   - `Resource Item`: Kéo thả file `.asset` vật phẩm (VD: `Raw_Wood.asset`)
   - `Amount Per Harvest`: Số lượng nhặt được mỗi lần
   - `Max Harvests`: Bao nhiêu lần trước khi cạn kiệt
   - `Tool Required`: Có cần công cụ không (VD: Rìu)

---

## 🧑‍🤝‍🧑 BƯỚC 5: SETUP NPC & THƯƠNG NHÂN

### NPC Dân Bản Địa:
1. Đặt một Model 3D nhân vật vào Scene
2. Add Component → `NavMeshAgent`
3. Add Component → `NativeNPC`
4. Thiết lập trong Inspector:
   - `NPC Name`: Tên NPC
   - `Dialogue Lines`: Các câu thoại
   - `Patrol Points`: Kéo thả các Transform để NPC đi tuần
   - `Attack Damage` / `Attack Range`: Chỉ dùng khi NPC bị Hắc Hóa ban đêm

### NPC Thương Nhân (Cửa Hàng):
1. Đặt Model 3D → Add Component → `MerchantNPC`
2. Thiết lập:
   - `Merchant Name`: "Thương Nhân Lữ Hành"
   - `Greeting Text`: Lời chào khi người chơi đến gần
3. Danh sách hàng bán được cấu hình từ **GameDirector** (Tab 🛒 ECONOMY):
   - Kéo thả vật phẩm vào `Merchant Inventory`
   - Đặt giá bằng số lượng **Vỏ Sò**

---

## 👹 BƯỚC 6: SETUP QUÁI VẬT & CHIẾN ĐẤU

### Setup EnemyAI:
1. Đặt Model 3D quái vật vào Scene
2. Add Component → `NavMeshAgent`
3. Add Component → `EnemyAI`
4. Thiết lập:
   - `Max Health`: Máu quái
   - `Attack Damage`: Sát thương đánh người chơi
   - `Detection Range`: Phạm vi phát hiện
   - `Patrol Points`: Các điểm tuần tra

### Setup SwarmController (Bầy đàn):
1. Tạo GameObject rỗng → Add Component → `SwarmController`
2. Kéo thả nhiều `EnemyAI` vào danh sách `Swarm Members`
3. Khi 1 con phát hiện người chơi, cả bầy sẽ tấn công

### Cơ chế Giáp (Armor System):
- Sát thương quái = `baseDamage × enemyDamageMultiplier (GameDirector) - totalArmor (Equipment)`
- VD: Quái đánh 20, hệ số x1.5 = 30, mặc Giáp Da (15 Giáp) → Chỉ mất 15 HP

---

## 🔨 BƯỚC 7: SETUP HỆ THỐNG CRAFTING

### Tạo tự động:
Menu: **`Tools > Forest Between Us > Setup Crafting System`**

### Thêm Công thức mới thủ công:
1. Vào `Assets/_GAME/Data/Recipes/`
2. Chuột phải → Create → Inventory → Recipe
3. Điền:
   - `Recipe Name`: Tên công thức
   - `Description`: Mô tả
   - `Result Item`: Vật phẩm tạo ra
   - `Ingredients`: Danh sách nguyên liệu (Item + Số lượng)
   - `Crafting Time`: Thời gian chế tạo (giây)

### Setup các Trạm Môi Trường:
| Script | GameObject | Chức Năng |
|--------|-----------|-----------|
| `Campfire.cs` | Lửa trại | Nướng thịt, sưởi ấm, lưu game |
| `CookingPot.cs` | Nồi nấu | Nấu thức ăn nâng cao |
| `FarmingPlot.cs` | Luống đất | Trồng hạt giống, thu hoạch |
| `WaterCollector.cs` | Bẫy sương | Tự động thu nước mưa |
| `RepairStation.cs` | Bàn sửa chữa | Sửa vũ khí/công cụ bị hỏng |
| `SalvageStation.cs` | Bàn tháo dỡ | Phá đồ cũ lấy nguyên liệu |
| `UpgradeStation.cs` | Bàn nâng cấp | Nâng cấp trang bị lên cấp cao hơn |
| `AnimalTrap.cs` | Bẫy thú | Đặt bẫy bắt động vật |

---

## 💾 BƯỚC 8: SETUP SAVE POINT & RESPAWN

### Tạo Điểm Lưu (Campfire Checkpoint):
1. Đặt một Campfire Model vào Scene
2. Add Component → `SavePointCheckpoint`
3. Add Component → `Campfire` (tùy chọn, để nấu ăn)
4. Thiết lập:
   - `Checkpoint Name`: "Lửa Trại Bìa Rừng"
   - `Prompt`: Tự động hiển thị `"Kích Hoạt Điểm Lưu Campfire [E]"`

### Cơ chế khi chết (Cấu hình trong GameDirector):
| Thông Số | Mặc Định | Ghi Chú |
|----------|----------|---------|
| Respawn Health % | 100% | Hồi sinh với bao nhiêu % máu |
| Drop Items On Death | true | Mất hết đồ trong Balo khi chết |
| Karma Penalty | 10 | Trừ điểm Karma mỗi lần chết |

---

## 🔧 BƯỚC 9: KẾT NỐI BUILD SETTINGS

1. Mở `File > Build Settings`
2. Kéo thả các Scene theo thứ tự:
   - `0` — `Home.unity`
   - `1` — `GamePlay.unity`
3. **KHÔNG** thêm `Developer_Control_Center.unity` (Scene này chỉ dành cho Dev)
4. Bấm `Build And Run`

---

## ⌨ PHÍM ĐIỀU KHIỂN

| Phím | Chức Năng |
|------|-----------|
| `WASD` | Di chuyển |
| `Shift` | Chạy nhanh (tốn Thể lực) |
| `Space` | Nhảy |
| `E` | Tương tác (nhặt đồ, nói chuyện, mở cửa) |
| `I` | Mở/Đóng Balo |
| `K` | Mở/Đóng Sổ Tay Chế Tạo |
| `M` | Mở/Đóng Bản Đồ |
| `Tab` | Xem thông tin nhân vật |
| `Esc` | Menu Tạm Dừng |
| `C` | Cúi người |
| `Chuột Trái` | Tấn công / Sử dụng công cụ |

---

## 📁 DANH SÁCH TOÀN BỘ SCRIPT

### 🧠 Manager (Quản lý)
| File | Vai Trò |
|------|---------|
| `GameDirector.cs` | Bộ não trung tâm cấu hình toàn game |
| `PlayerStatsManager.cs` | HP/Đói/Khát/XP/Level |
| `PlayerEquipmentManager.cs` | Trang bị đang mặc & tính Giáp/Tốc độ |
| `PlayerRespawnManager.cs` | Chết & Hồi sinh |
| `PlayerDiseaseManager.cs` | Bệnh tật (Muỗi, Cảm cúm) |
| `InventoryManager.cs` | Balo (nhặt/dùng/thả) |
| `DayManager.cs` | Chu kỳ Ngày/Đêm |
| `CraftingManager.cs` | Chế tạo đồ |
| `Campaign30DaysManager.cs` | Sự kiện 30 ngày |
| `EndingManager.cs` | Kết thúc game |
| `MissionManager.cs` | Nhiệm vụ hàng ngày |
| `QuestManager.cs` | Nhiệm vụ chính |
| `FoodSpoilageManager.cs` | Thức ăn thối hỏng |
| `SaveSystem.cs` | Lưu/Tải PlayerPrefs |
| `TutorialManager.cs` | Hướng dẫn tân thủ |
| `SceneTransitionManager.cs` | Chuyển cảnh |

### 🎨 UI (Giao diện)
| File | Vai Trò |
|------|---------|
| `CircularSurvivalHUD.cs` | HUD sinh tồn dạng tròn |
| `PlayerStatusUI.cs` | Bảng thông tin nhân vật |
| `BackpackUIController.cs` | Giao diện Balo |
| `CraftingUIController.cs` | Sổ tay Chế tạo (2 trang) |
| `FullMapUIController.cs` | Bản đồ |
| `PauseSettingsUIController.cs` | Menu tạm dừng |
| `DialogueChoiceUIController.cs` | Hội thoại phân nhánh |
| `TradingUIController.cs` | Giao diện cửa hàng |
| `RadioDialogueUIController.cs` | Thoại Radio |
| `ObjectiveWaypointArrow.cs` | Mũi tên mục tiêu |
| `EndingUIController.cs` | UI kết thúc |
| `RecipeBookUIController.cs` | Sổ công thức nâng cao |

### 🤖 AI & NPC
| File | Vai Trò |
|------|---------|
| `NativeNPC.cs` | NPC Dân bản địa (tương tác + Hắc hóa ban đêm) |
| `MerchantNPC.cs` | NPC Thương nhân (cửa hàng trao đổi) |
| `AnimalAI.cs` | Động vật hoang dã |
| `AnimalTrap.cs` | Bẫy bắt thú |
| `EnemyAI.cs` | Quái vật tấn công |
| `SwarmController.cs` | Bầy đàn quái |

### 🌍 Environments (Môi trường)
| File | Vai Trò |
|------|---------|
| `ResourceNode.cs` | Nút tài nguyên (Cây/Đá/Bụi) |
| `Campfire.cs` | Lửa trại (nấu ăn, sưởi ấm) |
| `CookingPot.cs` | Nồi nấu thức ăn |
| `FarmingPlot.cs` | Luống trồng trọt |
| `WaterCollector.cs` | Bẫy hứng nước |
| `RepairStation.cs` | Sửa chữa đồ |
| `SalvageStation.cs` | Tháo dỡ lấy nguyên liệu |
| `UpgradeStation.cs` | Nâng cấp trang bị |
| `MosquitoZone.cs` | Vùng muỗi (gây bệnh) |

### 🛠 Editor Tools (Chỉ hoạt động trong Unity Editor)
| File | Menu Path |
|------|-----------|
| `DevSceneSetupTool.cs` | Tools > Create Developer Dashboard Scene |
| `AutoAssignTreesTool.cs` | Tools > Auto-Assign Trees/Rocks/Bushes |
| `ItemAndRecipeGeneratorTool.cs` | Tools > Generate Items & Recipes |
| `CraftingSystemSetupTool.cs` | Tools > Setup Crafting System |
| `InventorySetupTool.cs` | Tools > Setup Inventory |
| `HarvestSystemSetupTool.cs` | Tools > Setup Harvest System |
| `AISystemSetupTool.cs` | Tools > Setup AI System |
| `MainMenuSetupTool.cs` | Tools > Setup Main Menu UI |
| `TutorialSetupTool.cs` | Tools > Setup Tutorial |
| `SceneTransitionSetupTool.cs` | Tools > Setup Scene Transition |
| `Campaign30DaysSetupTool.cs` | Tools > Setup 30-Day Campaign |
| `GameDirectorEditor.cs` | Custom Inspector cho GameDirector |

---

## ✅ CHECKLIST TRƯỚC KHI BUILD

```
[ ] 1. Đã chạy Tools > Generate Items & Recipes (tạo vật phẩm)
[ ] 2. Đã chạy Tools > Auto-Assign Trees (gắn tài nguyên vào cây/đá)
[ ] 3. Đã gắn GameDirector vào Scene GamePlay
[ ] 4. Đã gắn PlayerStatsManager + PlayerEquipmentManager vào Player
[ ] 5. Đã gắn PlayerInteraction vào Player
[ ] 6. Đã gắn InventoryManager + CraftingManager vào _MANAGERS
[ ] 7. Đã gắn DayManager + Directional Light vào DayManager
[ ] 8. Đã gắn PlayerRespawnManager vào _MANAGERS
[ ] 9. Đã đặt ít nhất 1 Campfire + SavePointCheckpoint
[ ] 10. Đã kéo icon vào các file .asset (vật phẩm) 
[ ] 11. Đã Bake NavMesh cho Scene GamePlay
[ ] 12. Đã thêm Home + GamePlay vào Build Settings
[ ] 13. Đã test: Nhặt đồ, Chế tạo, Chiến đấu, Chết/Hồi sinh
[ ] 14. Đã test: Mở Balo (I), Sổ tay (K), Bản đồ (M)
```

---

> **💡 MẸO:** Mọi thay đổi về chỉ số cân bằng (độ khó, tốc độ, giáp...) đều có thể điều chỉnh từ **GameDirector** mà không cần sửa code. Chỉ cần mở Scene `Developer_Control_Center`, chỉnh số, rồi chuyển sang Scene GamePlay để test!
