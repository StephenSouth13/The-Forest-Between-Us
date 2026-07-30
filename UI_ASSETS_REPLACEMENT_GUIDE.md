# 🎨 Hướng Dẫn Kéo Thả Asset Ảnh UI Trong Unity Inspector

Tài liệu này tổng hợp toàn bộ các **Custom UI Asset Slots (Lỗ Đục UI)** đã được tạo sẵn trong Unity Inspector. Bạn chỉ cần chọn đối tượng UI trong `Hierarchy` và kéo thả file ảnh (`Sprite`) của bạn vào ô tương ứng trong cửa sổ `Inspector`.

---

## 1. 🌀 `CircularSurvivalHUD` (HUD Sinh Tồn Góc Phải - Bottom Right)
Vị trí trong Hierarchy: `Canvas > SonsOfForest_HUD_Container > CircularSurvivalHUD`

| Tên Slot Trong Inspector | Loại File | Mô Tả & Vị Trí Asset |
| :--- | :--- | :--- |
| **`Hud Frame Sprite`** | Sprite | Hình ảnh khung viền đĩa mờ hình tròn của HUD. |
| **`Health Arc Sprite`** | Sprite | Vòng cung thanh Máu màu Đỏ. |
| **`Stamina Arc Sprite`** | Sprite | Vòng cung thanh Thể Lực màu Cyan/Xanh Dương. |
| **`Badge Health Sprite`** | Sprite | Icon biểu tượng Sức khỏe (💪). |
| **`Badge Thirst Sprite`** | Sprite | Icon biểu tượng Giọt Nước (💧). |
| **`Badge Hunger Sprite`** | Sprite | Icon biểu tượng Thức Ăn (🍗). |
| **`Badge Sleep Sprite`** | Sprite | Icon biểu tượng Giấc Ngủ (🌙). |
| **`Compass Pointer Sprite`** | Sprite | Icon kim chỉ nam la bàn center (▲). |
| **`Quest Panel Background Sprite`** | Sprite | Khung hình nền bảng Nhiệm vụ (Top-Left). |

---

## 2. 📊 `PlayerStatusUI` (Bảng Trạng Thái Nhân Vật - Phím TAB)
Vị trí trong Hierarchy: `Canvas > PlayerStatusUI`

| Tên Slot Trong Inspector | Loại File | Mô Tả & Vị Trí Asset |
| :--- | :--- | :--- |
| **`Status Panel Background Sprite`** | Sprite | Hình nền khung bảng trạng thái nhân vật. |
| **`Status Header Icon Sprite`** | Sprite | Icon vương miện/tiêu đề bảng TAB. |
| **`Stat Icon Health`** | Sprite | Icon chỉ số Máu. |
| **`Stat Icon Stamina`** | Sprite | Icon chỉ số Thể lực. |
| **`Stat Icon Hunger`** | Sprite | Icon chỉ số Cơn đói. |
| **`Stat Icon Thirst`** | Sprite | Icon chỉ số Cơn khát. |
| **`Slider Fill Sprite`** | Sprite | Hình thanh trượt/thanh đếm chỉ số. |

---

## 3. 🎒 `BackpackUIController` (Balô & Ô Đồ - Phím B)
Vị trí trong Hierarchy: `Canvas > Backpack Panel`

| Tên Slot Trong Inspector | Loại File | Mô Tả & Vị Trí Asset |
| :--- | :--- | :--- |
| **`Backpack Panel Background Sprite`** | Sprite | Khung hình nền chiếc Balô. |
| **`Slot Background Sprite`** | Sprite | Khung ô chứa đồ (24 ô túi đồ). |

---

## 4. 🗺️ `FullMapUIController` (Bản Đồ Chiến Thuật - Phím M)
Vị trí trong Hierarchy: `Canvas > FullMap_Panel (Runtime)`

| Tên Slot Trong Inspector | Loại File | Mô Tả & Vị Trí Asset |
| :--- | :--- | :--- |
| **`Map Background Sprite`** | Sprite | Texture ảnh bản đồ địa hình 2D. |
| **`Player Marker Sprite`** | Sprite | Icon định vị người chơi trên bản đồ. |
| **`Radio Marker Sprite`** | Sprite | Icon định vị vị trí đài Radio. |

---

## 5. ⚙️ `PauseSettingsUIController` (Menu Pause - Phím ESC)
Vị trí trong Hierarchy: `Canvas > PauseSettings_Panel (Runtime)`

| Tên Slot Trong Inspector | Loại File | Mô Tả & Vị Trí Asset |
| :--- | :--- | :--- |
| **`Menu Background Sprite`** | Sprite | Khung hình nền menu Pause / Cài đặt. |
| **`Button Background Sprite`** | Sprite | Hình dáng nút bấm Resume / Quit Game. |
| **`Slider Fill Sprite`** | Sprite | Thanh trượt âm lượng tổng. |

---

## 6. 📖 `RecipeBookUIController` (Cuốn Sách Hướng Dẫn & Thư Viện - Phím L)
Vị trí trong Hierarchy: `Canvas > RecipeBook_Panel (Runtime)`

| Tên Slot Trong Inspector | Loại File | Mô Tả & Vị Trí Asset |
| :--- | :--- | :--- |
| **`Book Background Sprite`** | Sprite | Bìa/trang sách hướng dẫn chế đồ. |
| **`Craft Button Sprite`** | Sprite | Nút bấm "Chế Tạo" (Craft). |

---

## 📖 `RadioDialogueUIController` (Hộp Thoại Subtitles)
Vị trí trong Hierarchy: `Canvas > RadioDialogue_Panel (Runtime)`

| Tên Slot Trong Inspector | Loại File | Mô Tả & Vị Trí Asset |
| :--- | :--- | :--- |
| **`Dialogue Panel Background Sprite`** | Sprite | Khung nền hộp thoại Subtitles bên dưới. |
| **`Speaker Badge Icon Sprite`** | Sprite | Icon avatar người nói. |
