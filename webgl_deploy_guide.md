# CẨM NANG BUILD WEBGL & ĐĂNG GAME LÊN WEBSITE QUACHTHANHLONG.COM

Cẩm nang này hướng dẫn chi tiết từ A-Z cách xuất game WebGL từ Unity 6 (6.0.3) và đưa game lên trang web cá nhân **quachthanhlong.com**.

---

## 💾 1. LƯU DỮ LIỆU GAME (DATABASE QUESTION)

### ❓ Trả lời câu hỏi: Có cần setup Database (MySQL/PostgreSQL) không?
👉 **KHÔNG CẦN SETUP DATABASE BÊN NGOÀI!**

**Lý do:**
1. *The Forest Between Us* là game **Single Player (Chơi đơn)**.
2. Bộ code đã được tích hợp sẵn [SaveSystem.cs](file:///d:/VTC_Academy/game3d/The-Forest-Between-Us/The%20Forest%20Between%20Us/Assets/_GAME/Scripts/Manager/SaveSystem.cs) sử dụng **`PlayerPrefs`**.
3. Khi bạn xuất game WebGL và người chơi mở trên trang web `quachthanhlong.com`, trình duyệt sẽ tự động lưu tiến trình (Ngày 1-30, Điểm Karma, Vật phẩm) vào bộ nhớ **IndexedDB / LocalStorage** của trình duyệt người đó.
4. **Ưu điểm**:
   - ⚡ Tốc độ lưu/nạp cực nhanh (0ms).
   - 💰 Miễn phí 100% (Không tốn tiền thuê server Database).
   - 🔒 Không lo bảo trì backend hay bảo mật database server.

---

## 🔄 2. HỆ THỐNG CHUYỂN MÀN CHƠI (SCENE TRANSITIONS)

Hệ thống chuyển cảnh đã được xây dựng hoàn chỉnh 100% trong [SceneTransitionManager.cs](file:///d:/VTC_Academy/game3d/The-Forest-Between-Us/The%20Forest%20Between%20Us/Assets/_GAME/Scripts/Manager/SceneTransitionManager.cs).

- Tự động nạp cảnh mượt mà giữa **`Home.unity`** (Main Menu) và **`Tutorial.unity` / `GameMap.unity`**.
- Hỗ trợ màn hình hiệu ứng **Cinematic Fade In/Out**, thanh tiến trình **Loading %**, hiệu ứng xoay **Spinner** và các câu gợi ý **Tips** xoay vòng khi load game.

---

## 🚀 3. HƯỚNG DẪN XUẤT GAME WEBGL TỪ UNITY 6 (6.0.3)

### Bước 1: Mở Build Settings trong Unity 6
1. Trong Unity Editor, chọn menu **File > Build Profiles** (hoặc **File > Build Settings**).
2. Tại danh sách Platform bên trái, chọn **WebGL**.
3. Nếu nút ghi **Switch Platform**, bấm **Switch Platform** (đợi Unity chuyển đổi asset sang WebGL).
4. Nhìn danh sách `Scenes In Build`, đảm bảo đã add 2 scene:
   - `Assets/Scenes/Home.unity` (vị trí 0)
   - `Assets/Scenes/Tutorial.unity` (vị trí 1)

### Bước 2: Cấu Hình Player Settings Cho Web (Chuẩn Tốc Độ High Performance)
1. Bấm nút **Player Settings...** ở góc dưới bên trái.
2. Tại mục **Player > Settings for WebGL**:
   - **Company Name**: `QuachThanhLong`
   - **Product Name**: `The Forest Between Us`
3. Tại phần **Resolution and Presentation**:
   - **Default Canvas Width**: `1280`
   - **Default Canvas Height**: `720`
   - **WebGL Template**: Chọn `Default` (hoặc `Minimal`).
4. Tại phần **Publishing Settings**:
   - **Compression Format**: Chọn `Gzip` hoặc `Disabled` *(Khuyên chọn Disabled hoặc Gzip nếu host trên cPanel/WordPress đơn giản để tránh lỗi Server MIME Header)*.
   - **Decompression Fallback**: Tích chọn `Enable`.

### Bước 3: Xuất File Build (Build Output)
1. Quay lại window Build Settings, bấm nút **BUILD**.
2. Unity sẽ hỏi chọn thư mục xuất. Hãy tạo một thư mục tên `WebBuild` trên máy tính.
3. Chờ Unity biên dịch. Sau khi hoàn thành, thư mục `WebBuild` sẽ chứa:
   - 📄 `index.html` (File chạy trang web)
   - 📁 `Build/` (Chứa file `.wasm`, `.data`, `.framework.js`)
   - 📁 `TemplateData/` (Chứa icon và giao diện loading màn hình web)

---

## 🌐 4. ĐĂNG GAME LÊN WEBSITE QUACHTHANHLONG.COM

### **CÁCH 1: Tải Trực Tiếp Lên Hosting / cPanel (Dễ Nhất & Đẹp Nhất)**

1. Đăng nhập vào cPanel / Manager Hosting của trang web `quachthanhlong.com`.
2. Mở cửa sổ **File Manager** -> Truy cập thư mục root `public_html`.
3. Tạo một thư mục mới tên `forest` (đường dẫn sẽ là `quachthanhlong.com/forest/`).
4. Tải (Upload) toàn bộ các file trong thư mục `WebBuild` (`index.html`, thư mục `Build`, thư mục `TemplateData`) vào thư mục `forest`.
5. 👉 **HOÀN THÀNH!** Bây giờ bạn mở trình duyệt và gõ `quachthanhlong.com/forest/` là có thể chơi game trực tiếp trên web!

---

### **CÁCH 2: Nhúng Game Vào Bài Viết / Trang WordPress Của quachthanhlong.com**

Nếu website của bạn dùng WordPress hoặc Custom HTML và bạn muốn game nằm lọt lòng trong bài viết:

1. Đăng lên hosting như Cách 1 để có đường dẫn link game (ví dụ: `https://quachthanhlong.com/forest/index.html`).
2. Mở trình chỉnh sửa bài viết trên website `quachthanhlong.com`.
3. Thêm một thẻ HTML (Custom HTML Block) và dán đoạn mã `iframe` sau:

```html
<div style="text-align: center; margin: 20px 0;">
    <h2 style="color: #00FF00;">TRẢI NGHIỆM GAME: THE FOREST BETWEEN US (3D)</h2>
    <iframe src="https://quachthanhlong.com/forest/index.html" 
            width="1280" 
            height="720" 
            style="border: 2px solid #333; border-radius: 8px; box-shadow: 0px 4px 20px rgba(0,0,0,0.5);" 
            allowfullscreen>
    </iframe>
    <p style="color: #aaa; margin-top: 10px;">Gợi ý: Bấm phím <b>F</b> để nhặt đồ, <b>B</b> mở túi đồ, <b>K</b> mở Crafting, <b>C</b> cúi người, <b>X</b> nằm trườn.</p>
</div>
```

4. Bấm **Lưu Bài Viết / Update**. Giờ đây người chơi truy cập bài viết trên `quachthanhlong.com` có thể bấm chơi game 3D ngay lập tức!
