# CẨM NANG THIẾT LẬP 3D TRIPLE-AAA CHO UNITY 6 (6.0.3)
## Game: The Forest Between Us (Chiến Dịch 30 Ngày & 5 Đa Kết Thúc)

Cẩm nang này hướng dẫn chi tiết từng cú nhấp chuột, cài đặt Đồ họa, Animation, Enemy AI, Âm thanh và VFX chuẩn **AAA** để kết nối hoàn hảo với bộ mã nguồn C# đã viết sẵn.

---

## 🎨 PHẦN 1: CÀI ĐẶT ĐỒ HỌA & KHÔNG GIAN SƯƠNG MÙ (GRAPHICS & ATMOSPHERE)

Unity 6 (6.0.3) nâng cấp mạnh mẽ về hệ thống **Volumetric Fog** và **Adaptive Probe Volumes**.

### 1.1. Cấu Hình Sương Mù Độc Hại & Ánh Sáng (Sun Light)
1. Trong cửa sổ **Hierarchy**, chọn ngọn đèn Mặt Trời (`Directional Light`).
2. Ở bảng **Inspector**:
   - **Mode**: Chọn `Realtime`.
   - **Shadow Type**: Chọn `Soft Shadows`.
   - **Resolution**: Chọn `High` hoặc `Very High`.
3. Chọn GameObject `GameManagers` -> Tại component `DayManager`:
   - Kéo chiếc đèn `Directional Light` vào ô `directionalLight`.
   - Đặt `Day Fog Density` = `0.01`, `Night Fog Density` = `0.05`.
   - `Day Fog Color`: Chọn màu xám sáng nhạt `(R:200, G:200, B:200)`.
   - `Night Fog Color`: Chọn màu xanh đen huyền bí `(R:10, G:15, B:30)`.

### 1.2. Thêm Hậu Kỳ Đồ Họa (Post-Processing Volume AAA)
1. Trong Hierarchy: Chuột phải -> **Volume > Global Volume**.
2. Tạo Volume Profile mới và thêm các hiệu ứng sau:
   - **Bloom**: Threshold = `0.9`, Intensity = `1.5` (Giúp hạt dưa hấu Hạt Đen và đuốc phát quang lộng lẫy).
   - **Vignette**: Intensity = `0.35`, Smoothness = `0.4` (Tạo góc nhìn viền tối điện ảnh).
   - **Color Adjustments**: Post Exposure = `0.2`, Contrast = `15`, Saturation = `10` (Cho màu rừng rực rỡ nhưng ma mị).
   - **Motion Blur**: Intensity = `0.2` (Tạo độ mượt khi xoay camera).

---

## 🏃 PHẦN 2: THIẾT LẬP NHÂN VẬT 3D & ANIMATION RIGGING

### 2.1. Cấu Hình 3D Model Nhân Vật (Humanoid Rig)
1. Chọn file 3D Model nhân vật (FBX) trong cửa sổ **Project**.
2. Tại bảng **Inspector** -> Chọn Tab **Rig**:
   - **Animation Type**: Chọn `Humanoid`.
   - Bấm **Apply**.

### 2.2. Cấu Hình Animator Controller 3 Tư Thế (Stand / Crouch / Prone)
1. Mở cửa sổ **Animator Window** (`Window > Animation > Animator`).
2. Thêm các tham số (Parameters):
   - `IsCrouching` (Bool)
   - `IsProne` (Bool)
   - `Speed` (Float)
3. Trong Animator:
   - Tạo **Blend Tree** cho di chuyển Đứng: Đi dạo -> Chạy nhanh dựa trên `Speed`.
   - Tạo State **Crouch_BlendTree** nối với Đứng bằng điều kiện `IsCrouching = true`.
   - Tạo State **Prone_BlendTree** nối bằng điều kiện `IsProne = true`.
4. Gắn Animator Controller này vào `Animator` component trên GameObject Player.
   👉 Component `PlayerPostureController.cs` đã viết sẵn sẽ tự động kích hoạt các tư thế này khi bấm **`C`** và **`X`**!

---

## 🧟‍♂️ PHẦN 3: KẺ THÙ SHADOW SWARM AI & BẢN ĐỒ NAVMESH (UNITY 6)

### 3.1. Bake Bản Đồ Di Chuyển (NavMesh Surface Trong Unity 6)
1. Mở menu `Window > AI > Navigation (Obsolete)` hoặc dùng package **AI Navigation** mới của Unity 6.
2. Chọn địa hình Rừng (Terrain / Ground Mesh) -> Đánh dấu là **Navigation Static**.
3. Bấm nút **Bake** để Unity tạo vùng di chuyển màu xanh dương.

### 3.2. Cấu Hình Prefab Kẻ Thù 3D (Shadow Enemy Prefab)
1. Kéo Model 3D Quái Vật vào Scene.
2. Thêm các Component:
   - `NavMeshAgent`: Set `Speed` = `3.5`, `Angular Speed` = `120`, `Stopping Distance` = `1.5`.
   - `CapsuleCollider`: Điều chỉnh vừa kích thước thân quái.
   - **[EnemyAI.cs](file:///d:/VTC_Academy/game3d/The-Forest-Between-Us/The%20Forest%20Between%20Us/Assets/_GAME/Scripts/Enemy/EnemyAI.cs)**:
     - `View Radius` = `12` (Tầm nhìn).
     - `View Angle` = `90` (Góc nhìn nón 90 độ).
     - `Target Mask` = chọn Layer **`Player`**.
     - `Obstacle Mask` = chọn Layer **`Default`** / **`Environment`**.
     - `Sound Radius Standing` = `15` (Tầm nghe khi người chơi đứng chạy).
     - `Sound Radius Crouching` = `6` (Tầm nghe khi rón rén cúi).
     - `Sound Radius Prone` = `2` (Gần như không nghe thấy khi trườn nằm).
3. Kéo GameObject Quái Vật từ Hierarchy thả vào thư mục `Assets/_GAME/Prefabs/Enemies/` để biến thành **Prefab**.
4. Kéo Prefab này vào ô `enemyPrefabs` của **`CombatManager`** trên GameObject `GameManagers`.

---

## 🎒 PHẦN 4: VẬT PHẨM 3D, HÁI ĐỒ & CHẾ TẠO (INTERACTABLES & CRAFTING)

### 4.1. Tạo Model 3D Dưa Hấu Hạt Đen / Đồ Nhặt
1. Đặt Model 3D Quả Dưa Hấu / Linh kiện vào Scene.
2. Thêm component `SphereCollider` hoặc `BoxCollider`, tích chọn **`Is Trigger`**.
3. Thêm component **[ItemObject.cs](file:///d:/VTC_Academy/game3d/The-Forest-Between-Us/The%20Forest%20Between%20Us/Assets/_GAME/Scripts/Inventory/ItemObject.cs)**:
   - Gán file `ItemData` tương ứng (ví dụ: `Item_BlackSeedWatermelon`).
   - Đặt `Amount` = `1`.
4. *(Nâng cao AAA)*: Thêm 1 **Particle System** hiệu ứng đom đóm sáng lung linh làm con của Quả Dưa Hấu.

### 4.2. Tạo Công Thức Chế Tạo 3D (Crafting Recipes)
1. Trong cửa sổ Project: Nhấp chuột phải -> **Create > Forest Between Us > Crafting Recipe**.
2. Đặt tên file (ví dụ: `Recipe_SignalFlare` hoặc `Recipe_RadioTuner`).
3. Điền thông tin trong Inspector:
   - **Recipe Name**: Đuốc Kháng Tín Hiệu
   - **Ingredients**: Gỗ x2, Nhựa Phát Quang x1
   - **Result Item**: Gán `Item_SignalFlare`
   - **Craft Time Seconds**: `2.0`
4. Kéo file công thức này vào danh sách `knownRecipes` của **`CraftingManager`**.

---

## 🔊 PHẦN 5: ÂM THANH 3D SPATIAL & TẦN SỐ RADIO

### 5.1. Cấu Hình Đài Radio 3D Phát Tiếng Rè
1. Chọn Model 3D Radio trong Scene.
2. Thêm component `AudioSource`:
   - **Spatial Blend**: Kéo hết về bên phải `= 1.0 (3D Sound)`.
   - **Min Distance**: `2`, **Max Distance**: `25`.
   - **Rolloff Mode**: Chọn `Logarithmic Rolloff`.
3. Gắn component **[RadioInteractable.cs](file:///d:/VTC_Academy/game3d/The-Forest-Between-Us/The%20Forest%20Between%20Us/Assets/_GAME/Scripts/Interaction/RadioInteractable.cs)**.

---

## 🎯 PHẦN 6: CHECKLIST HOÀN THIỆN TRONG UNITY 6 (1-CLICK EXECUTION)

Chỉ cần làm theo đúng 3 bước menu này trong Unity 6:

- [x] **Bước 1**: Chọn menu **`Tools > Forest Between Us > Setup 30-Day Campaign`**
- [x] **Bước 2**: Chọn menu **`Tools > Forest Between Us > Setup Inventory UI`**
- [x] **Bước 3**: Chọn GameObject `GameManagers` -> Kéo chiếc đèn `Directional Light` vào ô `directionalLight` của `DayManager`.

Sau đó bấm **PLAY (▶️)** để thưởng thức siêu phẩm 3D 30 Ngày với 5 Kết Thúc!
