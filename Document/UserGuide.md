# Tài liệu Hướng dẫn Sử dụng & Báo cáo Kỹ thuật: Symphony Portal

## 1. Tổng quan Dự án
**Symphony Portal** là cổng thông tin đào tạo trực tuyến dành cho Học viện Symphony Ltd., được xây dựng trên nền tảng .NET hiện đại. Hệ thống cung cấp giải pháp số hóa quy trình tuyển sinh, quản lý khóa học và tra cứu thông tin cho Giảng viên, Học viên và Khách.

## 2. Công nghệ & Công cụ Sử dụng
Hệ thống được xây dựng dựa trên các tiêu chuẩn công nghiệp mới nhất:

*   **Framework:** ASP.NET Core 8.0 (MVC Pattern).
*   **Ngôn ngữ:** C# 12.
*   **Cơ sở dữ liệu:** SQL Server (Lưu trữ quan hệ).
*   **ORM:** Entity Framework Core 8 (Code-First Migration).
*   **Bảo mật:** ASP.NET Core Identity (Authentication & Authorization).
*   **Giao diện (Frontend):**
    *   Razor Views (.cshtml).
    *   Bootstrap 5 (Responsive Layout).
    *   Bootstrap Icons.
    *   CSS tùy chỉnh (Glassmorphism & Modern UI).

## 3. Các Chức năng Đã Triển khai

### A. Phân hệ Xác thực & Phân quyền (Authentication)
*   **Đăng ký (Register):**
    *   Cho phép người dùng mới tạo tài khoản (Mặc định Role: **Student**).
    *   Validate dữ liệu chặt chẽ (Email, Password policy).
*   **Đăng nhập (Login):**
    *   Hỗ trợ "Ghi nhớ đăng nhập" (Remember Me).
    *   Tự động chuyển hướng (Redirect) thông minh dựa trên Role của người dùng:
        *   **Admin** -> Admin Dashboard.
        *   **Student** -> Student Portal.
        *   **Instructor** -> Instructor Area.
*   **Đăng xuất (Logout):** Bảo mật với Anti-Forgery Token.

### B. Phân hệ Quản trị viên (Admin Module)
Truy cập tại: `/Admin/Dashboard`
1.  **Dashboard:** Xem tổng quan các module quản lý.
2.  **Quản lý Khóa học (Course Management):**
    *   Xem danh sách khóa học hiện có.
    *   Tạo mới khóa học (Tên, Học phí, Thời lượng, Mô tả).
    *   Chỉnh sửa / Xóa / Kích hoạt ngưng kích hoạt khóa học.
3.  **Quản lý Kỳ thi Tuyển sinh (Admission Exams):**
    *   Lên lịch các kỳ thi tuyển sinh mới.
    *   Thiết lập lệ phí thi ($1000/$1500).

### C. Phân hệ Cổng thông tin Công cộng (Public Portal)
Truy cập tại: `/Home`
*   **Trang chủ:**
    *   Hiển thị các **Khóa học Nổi bật** (Featured Courses) đang hoạt động.
    *   Hiển thị lịch **Kỳ thi Tuyển sinh** sắp tới.
    *   Nút CTA (Call-to-action) đăng ký nhanh.
*   **Bảo mật:** Guest không thể truy cập các trang quản trị.

---

## 4. Hướng dẫn Cài đặt & Sử dụng (User Manual)

### Bước 1: Khởi chạy Ứng dụng
1.  Mở dự án bằng Visual Studio hoặc VS Code.
2.  Mở Terminal và chạy lệnh:
    ```bash
    dotnet run
    ```
3.  Truy cập địa chỉ hiển thị (ví dụ: `http://localhost:5124`).

### Bước 2: Đăng nhập Quản trị (Admin)
Hệ thống đã tạo sẵn tài khoản Admin cao cấp nhất:
*   **Email:** `admin@symphony.local`
*   **Password:** `Admin@12345`

*Thao tác:*
1.  Nhấn **Login** trên thanh menu.
2.  Nhập thông tin trên.
3.  Bạn sẽ được chuyển đến trang **Admin Dashboard**.
4.  Tại đây, hãy thử vào "Manage Courses" để tạo thêm khóa học mới.

### Bước 3: Đăng ký Học viên (Student)
1.  Đăng xuất (nếu đang login).
2.  Nhấn biểu tượng **Register** hoặc nút "Register Now" ở trang chủ.
3.  Điền thông tin cá nhân.
4.  Sau khi đăng ký thành công, bạn sẽ được chuyển đến trang **My Learning** dành cho học viên.

### Bước 4: Kiểm tra Trang chủ (Guest View)
1.  Đăng xuất khỏi hệ thống.
2.  Trang chủ sẽ hiển thị danh sách các khóa học và kỳ thi mà Admin vừa tạo.
3.  Khách có thể xem thông tin chi tiết và nhấn "Register to Apply" để tạo tài khoản.

## 5. Cấu trúc Thư mục Kỹ thuật
*   `/Areas`: Chứa các phân hệ riêng biệt (Admin/Student/Instructor).
*   `/Data`: Chứa cấu hình Database (AppDbContext) và Seed Data.
*   `/Models`: Chứa các thực thể (Entity) và ViewModels.
*   `/Views`: Chứa giao diện người dùng chung.
*   `/wwwroot`: Chứa tài nguyên tĩnh (CSS, JS, Images).
