# Cấu trúc Dự án Symphony Portal

Tài liệu này giải thích chi tiết về cấu trúc thư mục và kiến trúc của dự án **Symphony.Portal.Web**. Dự án được xây dựng theo mô hình **ASP.NET Core MVC** (Model-View-Controller) kết hợp với **Areas** để phân chia các phân hệ chức năng.

## 1. Tổng quan Cấu trúc

Dự án được tổ chức thành các thư mục chính sau:

### 📂 Areas/ (Phân vùng Chức năng)
Đây là phần quan trọng nhất để phân chia hệ thống thành các module riêng biệt. Mỗi Area hoạt động như một "dự án con" với Controllers, Models và Views riêng:

*   **`Admin/`**: Phân hệ dành riêng cho Quản trị viên.
    *   Chứa logic quản lý Khóa học, Kỳ thi, Người dùng.
    *   Được bảo vệ bởi `[Authorize(Roles = "Admin")]`.
*   **`Instructor/`**: Phân hệ dành cho Giảng viên.
    *   Quản lý lớp học được phân công, chấm điểm.
*   **`Student/`**: Phân hệ dành cho Học viên.
    *   Xem lịch học, kết quả thi, tài liệu.

**Mục đích:** Giúp code không bị lẫn lộn giữa các nhóm người dùng, dễ dàng bảo trì và mở rộng từng phân hệ độc lập.

### 📂 Controllers/ (Điều phối Chính)
Chứa các Controller cấp cao, không thuộc về Area cụ thể nào (thường là public hoặc dùng chung):

*   **`AccountController.cs`**: Quản lý Xác thực.
    *   Xử lý Đăng nhập (`Login`), Đăng ký (`Register`), Đăng xuất (`Logout`).
    *   Điều hướng người dùng đến đúng Area sau khi đăng nhập thành công.
*   **`PublicController.cs`**: Xử lý trang chủ công khai.
    *   Hiển thị thông tin cho khách vãng lai (Guest) như danh sách khóa học, kỳ thi sắp tới.

### 📂 Models/ (Dữ liệu & Nghiệp vụ)
Nơi định nghĩa cấu trúc dữ liệu và logic nghiệp vụ:

*   **`Identity/ApplicationUser.cs`**: Mở rộng từ `IdentityUser` mặc định.
    *   Thêm các trường tùy chỉnh như `FullName`, `Address`...
*   **`ViewModels/`**: Các Class dùng riêng cho giao diện (DTOs).
    *   Ví dụ: `LoginVM`, `RegisterVM`.
    *   Giúp kiểm tra dữ liệu đầu vào (Validation) trước khi xử lý logic.
*   **Domain Models** (`Course.cs`, `Class.cs`, `AdmissionExam.cs`...):
    *   Các thực thể cốt lõi của hệ thống.
    *   Sẽ được Entity Framework Core chuyển đổi thành các bảng trong Database.

### 📂 Data/ (Kết nối Cơ sở dữ liệu)
*   **`AppDbContext.cs`**: Lớp ngữ cảnh dữ liệu (DbContext).
    *   Cấu hình Entity Framework Core.
    *   Đại diện cho Database trong code, chứa các `DbSet` (bảng).
*   **`SeedData.cs`**: Khởi tạo dữ liệu mẫu.
    *   Tự động chạy khi ứng dụng khởi động lần đầu.
    *   Tạo các Role mặc định (Admin, Instructor, Student) và tài khoản Admin cao cấp.

### 📂 Views/ (Giao diện Người dùng)
*   **`Shared/_Layout.cshtml`**: Layout chính (Master Page).
    *   Chứa thanh điều hướng (Navbar), Footer, và tham chiếu CSS/JS.
    *   Sử dụng logic kiểm tra Role để hiển thị menu tương ứng cho từng loại người dùng.
*   **`Shared/_LoginPartial.cshtml`**: Partial View hiển thị trạng thái đăng nhập.

### 📂 wwwroot/ (Tài nguyên Tĩnh)
Thư mục chứa các file static được gửi trực tiếp xuống trình duyệt:
*   **`css/`**: Chứa file `auth.css` (style riêng cho trang Login) và `site.css` (chung).
*   **`js/`**, **`lib/`**: Chứa Javascript và thư viện bên thứ 3 (Bootstrap, jQuery).

### ⚙️ Program.cs (Cấu hình Ứng dụng)
Điểm khởi đầu của ứng dụng:
*   Đăng ký các dịch vụ (Dependency Injection): DbContext, Identity.
*   Cấu hình Pipeline xử lý Request.
*   Định kuyến (Routing) cho Areas và Controller mặc định.

### 📄 appsettings.json
*   Chứa chuỗi kết nối Database (`ConnectionStrings`).
*   Các cấu hình môi trường khác.

---
*Tài liệu này được tạo tự động bởi trợ lý ảo Symphony Project Assistant.*
