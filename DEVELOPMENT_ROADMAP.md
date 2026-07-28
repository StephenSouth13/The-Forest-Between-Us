# Development Roadmap: Complete 30-Day Campaign & 5 Multi-Endings

## 📜 Kịch Bản Chi Tiết 30 Ngày (5 Hồi Gameplay)

- **HỒI 1: KHỞI ĐẦU BÍ ẨN (Ngày 1 - 3)**: Nhập môn sinh tồn, nhặt đài Radio 01, hái Dưa Hấu Hạt Đen, học phím bấm & né tránh.
- **HỒI 2: CHIẾN ĐẤU & KHÁNG CỰ (Ngày 4 - 10: 7 Ngày Chiến Đấu)**: Mở khóa Nỏ Tần Số, chống chịu 2 đợt Trăng Máu Quái Vực (**Wave Defense 01 & 02**), săn Miniboss Shadow Berserker.
- **HỒI 3: KHÁM PHÁ CỔ ĐẠI (Ngày 11 - 20: 10 Ngày Thám Hiểm)**: Phế tích Mai An Tiêm, giải đố mạch điện cổ đại, diệt quái bay Shadow Drakes, thu thập 4 Chìa Khóa Tần Số Trụ Vũ Trụ.
- **HỒI 4: ĐẠI CHIẾN RANH GIỚI (Ngày 21 - 29: 9 Ngày Cố Thủ)**: Xây pháo đài vô tuyến, đại chiến Trùm Cổ Đại **Void Leviathan**.
- **HỒI 5: NGÀY PHÁN QUYẾT (Ngày 30)**: Lựa chọn 1 trong 5 Cái Kết Vận Mệnh (True Ending, Mai An Tiem Echo, Dark King, Sacrifice, Time Loop Secret).

---

## 🛠️ Bộ Mã Nguồn C# Đã Viết Sẵn Cho 30 Ngày

1. [Campaign30DaysManager.cs](file:///d:/VTC_Academy/game3d/The-Forest-Between-Us/The%20Forest%20Between%20Us/Assets/_GAME/Scripts/Manager/Campaign30DaysManager.cs): Khai báo 100% dữ liệu Nhiệm vụ, Cốt truyện, Mục tiêu, đợt Quái cho toàn bộ Ngày 1 đến Ngày 30.
2. [DayManager.cs](file:///d:/VTC_Academy/game3d/The-Forest-Between-Us/The%20Forest%20Between%20Us/Assets/_GAME/Scripts/Manager/DayManager.cs): Quản lý chu kỳ 30 ngày, đổi thời gian Ngày/Đêm, ánh sáng và sương mù.
3. [CombatManager.cs](file:///d:/VTC_Academy/game3d/The-Forest-Between-Us/The%20Forest%20Between%20Us/Assets/_GAME/Scripts/Combat/CombatManager.cs): Tấn công vũ khí (Nỏ, Cận chiến), spawn đợt quái tràn vào (Wave Defense).
4. [EndingManager.cs](file:///d:/VTC_Academy/game3d/The-Forest-Between-Us/The%20Forest%20Between%20Us/Assets/_GAME/Scripts/Manager/EndingManager.cs) & [EndingUIController.cs](file:///d:/VTC_Academy/game3d/The-Forest-Between-Us/The%20Forest%20Between%20Us/Assets/_GAME/Scripts/UI/EndingUIController.cs): Quản lý 5 Kết thúc khác nhau (Ending 1 đến Ending 5).
5. [Campaign30DaysSetupTool.cs](file:///d:/VTC_Academy/game3d/The-Forest-Between-Us/The%20Forest%20Between%20Us/Assets/_GAME/Scripts/Editor/Campaign30DaysSetupTool.cs): Tool 1-Click tự động gắn toàn bộ Manager vào Unity Editor.

---

## 🎮 Hướng Dẫn Chi Tiết Thao Tác Trong Unity Editor (Dành Cho Bạn)

### **BƯỚC 1: TỰ ĐỘNG THIẾT LẬP (1-CLICK SETUP)**
1. Mở dự án Unity của bạn tại `d:\VTC_Academy\game3d\The-Forest-Between-Us\The Forest Between Us`.
2. Trên thanh menu trên cùng của Unity, chọn **`Tools > Forest Between Us > Setup 30-Day Campaign`**.
3. Unity sẽ tự động tạo GameObject `GameManagers` và gắn đầy đủ `Campaign30DaysManager`, `DayManager`, `CombatManager`, `EndingManager`, `QuestManager`, `MissionManager`.

### **BƯỚC 2: GẮN KÍCH HOẠT UI (Tùy chọn)**
1. Mở menu **`Tools > Forest Between Us > Setup Inventory UI`** (để tạo Backpack UI).
2. Kéo đèn `Directional Light` trong Scene vào ô `directionalLight` của component `DayManager`.

### **BƯỚC 3: NHẤN PLAY CHƠI VÀ TEST GAME!**
1. Bấm nút **PLAY (▶️)** trong Unity.
2. Game sẽ tự động nạp **Ngày 1** và nhảy lần lượt đến **Ngày 30** khi bạn hoàn thành nhiệm vụ!
