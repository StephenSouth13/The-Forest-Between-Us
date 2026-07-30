<p align="center">
  <img src="https://img.shields.io/badge/Unity%206-HDRP%20%7C%20WebGL-blueviolet?style=for-the-badge&logo=unity&logoColor=white" alt="Unity 6 Badge" />
  <img src="https://img.shields.io/badge/Author-Qu%C3%A1ch%20Thanh%20Long-007ACC?style=for-the-badge&logo=wordpress&logoColor=white" alt="Author Badge" />
  <img src="https://img.shields.io/badge/GitHub-StephenSouth13-181717?style=for-the-badge&logo=github&logoColor=white" alt="GitHub Badge" />
  <img src="https://img.shields.io/badge/License-MIT-green?style=for-the-badge" alt="License Badge" />
</p>

<h1 align="center">🌲 THE FOREST BETWEEN US 🌲</h1>
<h3 align="center"><i>Bản Tình Ca Rừng Sương Mù & Cuộc Chiến Sinh Tồn 30 Ngày</i></h3>
<h4 align="center">A 30-Day AAA Survival Experience Powered by Unity 6 HDRP</h4>

<p align="center">
  🌐 <b>Official Website & Play Online:</b> <a href="https://quachthanhlong.com">quachthanhlong.com</a> <br/>
  🐙 <b>GitHub Repository:</b> <a href="https://github.com/StephenSouth13/The-Forest-Between-Us">StephenSouth13/The-Forest-Between-Us</a>
</p>

---

## 📖 GIỚI THIỆU DỰ ÁN | PROJECT OVERVIEW

### 🇻🇳 Tiếng Việt
**The Forest Between Us** là một tựa game 3D sinh tồn góc nhìn thứ nhất (FPS Survival) lấy bối cảnh tại **Vùng Đứt Gãy (Sector 01)** - một vùng rừng núi nguyên sinh huyền bí bị bao phủ bởi sương mù độc và sóng tần số vô tuyến kỳ lạ. 

Người chơi vào vai một nhà thám hiểm bị kẹt giữa rừng sâu, nhận được tín hiệu vô tuyến ngắt quãng từ **Mai An Tiêm**. Để sống sót trong **30 Ngày Sinh Tồn**, bạn phải quản lý chỉ số sinh tồn (Máu, Đói, Khát, Thể Lực), khai thác tài nguyên tuân thủ quy tắc bảo vệ môi trường, gieo trồng nông sản, nhóm lửa sưởi ấm, nấu ăn hầm súp, thuần hóa thú rừng, chế tạo vũ khí và chống lại sự càn quét của **Sinh Thể Hắc Hóa** khi màn đêm buông xuống.

### 🇬🇧 English
**The Forest Between Us** is an immersive 3D First-Person Survival Action-Adventure game set in **Sector 01 (The Rift Zone)**—a mysterious primeval forest enveloped in toxic fog and peculiar radio frequencies.

Stranded in the wilderness, players pick up distress signals from **Mai An Tiêm**. To survive the **30-Day Campaign**, players must manage vitals (Health, Hunger, Thirst, Stamina), harvest resources adhering to eco-protection rules, cultivate crops, build fires, cook culinary soups, tame wild animals, upgrade gear, and defend against **Corrupted Shadow Entities** during the night.

---

## 🔥 TÍNH NĂNG NỔI BẬT | KEY FEATURES

| 🇻🇳 Tính Năng (Vietnamese) | 🇬🇧 Feature Description (English) |
| :--- | :--- |
| **🗺️ Bản Đồ Chiến Thuật (Phím M)** | **Tactical Fullscreen Map (M-Key):** Dynamic player GPS tracking, orientation arrow, radio frequency waypoints, and coordinates. |
| **🎒 Balo Cân Nặng (30kg) & Thối Hỏng** | **Weight-Capacity Backpack & Spoilage:** 30kg weight limit, slot capacity, and perishable food decay timer into spoiled items. |
| **🪓 Chặt Cây & Bảo Vệ Môi Trường** | **Eco-Harvesting & Tree Regrowth:** Chopping trees with an environmental limit (**5 trees/day max**) and automatic cooldown tree regrowth. |
| **🌾 Nông Trại & Trồng Trọt** | **Farming & Agriculture System:** Plant seeds (`BerrySeed`), water soil plots, grow crops, and harvest fresh produce. |
| **🍼 Bể Nước & Bình Nước Uống** | **Water Storage & Bottle Refills:** Collect rainwater, drink directly, and fill empty water bottles into full water containers. |
| **🍳 Ẩm Thực & Nồi Nấu Ăn** | **Culinary Cooking & Campfires:** Group campfires for warmth, attach Cooking Pots for herbal stews/teas, and avoid raw food toxicity. |
| **🔧 3 Bàn Chế Tạo (Sửa / Nâng / Đập)** | **3 Workbenches (Repair / Upgrade / Salvage):** Repair broken tools, upgrade Level 1 to Level 2 gear, and dismantle items for **50% material refund**. |
| **🐺 AI Sinh Thể, Thuần Hóa & Bẫy** | **Living Entity AI & Taming:** Fleeing prey (rabbits), aggressive predators (boars), taming pets with food, and placing animal traps. |
| **🗿 Thổ Dân & Hắc Hóa Ban Đêm** | **Native NPCs & Night Corruption:** Friendly tribal NPCs teach recipes by day, but transform into Corrupted Shadow Warriors at night. |
| **🌙 Chu Kỳ 30 Ngày & Kết Game** | **30-Day Progression & Judgment Day:** Day/night cycle with fog density scaling, leading to Day 30 final choice endings. |
| **🌐 1-Click WebGL Deployment** | **Ready for WebGL:** Optimized for direct web browser hosting on cPanel/WordPress at `quachthanhlong.com`. |

---

## 🛠️ KIẾN TRÚC DỰ ÁN & FOLDER STRUCTURE

Tất cả mã nguồn C# được cấu trúc vô cùng gọn gàng trong thư mục `Assets/_GAME/Scripts/`:

```text
Assets/_GAME/Scripts/
├── 🤖 AI/                     # AI Động vật, Thổ dân K'Nu, Thuần hóa, Bẫy săn bắt
│   ├── AnimalAI.cs            # Thỏ Rừng (Prey), Lợn Rừng (Predator), Thuần hóa Pet
│   ├── NativeNPC.cs           # Thổ Dân K'Nu (Nói chuyện, dạy skill, Hắc Hóa ban đêm)
│   └── AnimalTrap.cs          # Bẫy lồng săn bắt động vật & quái vật
├── 🪓 Environments/          # Môi trường, Nấu ăn, Trồng trọt, Nâng cấp
│   ├── ResourceNode.cs        # Chặt cây mọc lại (Giới hạn 5 cây/ngày), Bụi quả, Hòn đá
│   ├── FarmingPlot.cs         # Ô đất gieo hạt, tưới nước, thu hoạch nông sản
│   ├── Campfire.cs            # Đống lửa trại nhóm củi, sưởi ấm đêm, nướng thịt
│   ├── CookingPot.cs          # Nồi nấu ăn hầm súp & trà thảo dược
│   ├── WaterCollector.cs      # Bể tích trữ nước suối & múc bình nước
│   ├── RepairStation.cs       # Bàn sửa chữa trang bị hỏng về 100% độ bền
│   ├── UpgradeStation.cs      # Bàn nâng cấp Rìu Gỗ Lvl 1 ➔ Rìu Thép Lvl 2
│   └── SalvageStation.cs      # Bàn tinh giản đập bỏ hoàn lại 50% nguyên liệu
├── 🎒 Inventory/             # Quản lý Balo, Vật phẩm & Slot
│   ├── ItemData.cs            # Custom Asset Slots, Cân nặng kg, Độ bền, Hạn dùng
│   ├── ItemObject.cs          # Vật thể 3D nhặt dưới mặt đất
│   └── InventorySlot.cs       # Cấu trúc Slot ô chứa túi đồ
├── ⚙️ Manager/                # Các bộ điều hành hệ thống
│   ├── InventoryManager.cs    # Quản lý Balo, kiểm tra giới hạn 30kg & tiêu thụ
│   ├── FoodSpoilageManager.cs # Quản lý thực phẩm thối rữa sau 3 phút
│   ├── PlayerStatsManager.cs  # Máu, Đói, Khát, Thể Lực, Giấc Ngủ, Karma
│   └── DayManager.cs          # Quản lý chu kỳ Ngày/Đêm 30 Ngày
├── 🛠️ Editor/                 # Công cụ tự động 1-Click Setup trong Unity
│   ├── AISystemSetupTool.cs   # 1-Click Setup AI Thỏ, Lợn Rừng, Thổ Dân & Bẫy
│   ├── HarvestSystemSetupTool.cs # 1-Click Setup Nồi Nấu, Ô Đất, Lửa Trại, Bể Nước
│   └── CraftingSystemSetupTool.cs # 1-Click Setup 3 Bàn Sửa/Nâng/Đập
├── 🎮 Player/ & 🎨 UI/         # Điều khiển nhân vật, Camera, Bản đồ M, Balo B, Sách L
```

---

### 🎨 DESIGN SLOTS FOR UNITY INSPECTOR (DỄ DÀNG GẮN ASSET)
Tất cả các Script trong game đều được thiết kế **Inspector Asset Slots (Đục lỗ)** sẵn sàng cho phép bạn kéo thả 3D Models, Materials, Prefabs, Particle VFX và Sound Effects (SFX) tùy chỉnh:

```csharp
[Header("🎨 Custom Asset Slots (Kéo Thả Model / Prefab / VFX / SFX Của Bạn Vào Đây)")]
public GameObject worldModelPrefab;    // Model 3D thả ra ngoài thế giới
public GameObject equippedHandPrefab;  // Model 3D khi cầm trên tay nhân vật
public ParticleSystem useEffectVFX;    // Hiệu ứng hạt VFX khi sử dụng
public AudioClip useAudioSFX;          // Âm thanh SFX khi phát sinh hành động
```

---

## ⚡ BỘ CÔNG CỤ 1-CLICK SETUP TRONG UNITY

Dự án tích hợp sẵn các công cụ tự động hóa trên thanh Menu trên cùng của Unity Editor:

* **`Tools > Forest Between Us > Setup 30-Day Campaign`**: Tự động gắn hệ thống quản lý 30 Ngày Sinh Tồn & Ngày/Đêm.
* **`Tools > Forest Between Us > Setup Full Tutorial & UI`**: Tự động tạo Canvas HUD vòng tròn, Bản đồ phím **M**, Túi đồ phím **B**, Sách phím **L**.
* **`Tools > Forest Between Us > Setup Harvest & Resource System`**: Tạo Nồi Nấu Ăn, Ô Đất Trồng Trọt, Đống Lửa Trại, Bể Nước.
* **`Tools > Forest Between Us > Setup Repair, Upgrade & Salvage Workbenches`**: Tạo 3 Bàn Sửa Chữa, Nâng Cấp và Tinh Giản (Thu lại 50% nguyên liệu).
* **`Tools > Forest Between Us > Setup AI, Animals, Natives & Traps`**: Sinh Thỏ Rừng, Lợn Rừng, Thổ Dân K'Nu và Bẫy Săn Bắt.

---

## 🖥️ HƯỚNG DẪN BUILD & DEPLOY WEBGL

1. Mở Unity $\rightarrow$ chọn **`File > Build Settings`** $\rightarrow$ Switch Platform sang **WebGL**.
2. Tại **`Player Settings > Publishing Settings`**:
   * Chọn `Compression Format`: **Gzip** hoặc **Disabled**.
   * Tích chọn `Decompression Fallback`: **Enable**.
3. Bấm **BUILD** và chọn thư mục xuất file.
4. Tải thư mục build lên hosting tại `quachthanhlong.com` hoặc nhúng qua thẻ `<iframe>` trên bài viết WordPress.

---

## 👤 AUTHOR & CREDITS

* **Lead Developer & Designer:** Quách Thanh Long ([StephenSouth13](https://github.com/StephenSouth13))
* **Website:** [quachthanhlong.com](https://quachthanhlong.com)
* **Game Engine:** Unity 6 (High Definition Render Pipeline - HDRP)
* **Repository:** [https://github.com/StephenSouth13/The-Forest-Between-Us](https://github.com/StephenSouth13/The-Forest-Between-Us)

---

## 📜 LICENSE

Dự án này được phát hành dưới bản quyền **MIT License**. Xem thông tin chi tiết tại file [LICENSE.md](LICENSE.md).

Copyright (c) 2026 **Quách Thanh Long (StephenSouth13)**. All rights reserved.
