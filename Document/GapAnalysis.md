# Báo cáo Trạng thái Dự án & Phân tích Chênh lệch (Gap Analysis)

**Dự án:** Symphony Ltd. Web Portal
**Ngày lập:** 07/01/2026
**Trạng thái:** Đang phát triển (Giai đoạn khởi tạo/User Management)

---

## 1. Phân tích Cơ sở Dữ liệu (Database)

Dưới đây là so sánh giữa **Thiết kế dự kiến** (trong `DatabaseDesign.md`) và **Source Code hiện tại** (`ApplicationDbContext.cs`).

| Bảng dữ liệu (Entity) | Trạng thái | Ghi chú / Code hiện có |
| :--- | :---: | :--- |
| **Users (Identity)** | ✅ Có sẵn | `ApplicationUser`, `UserDetails`, `StaffProfile`. Đã có cơ chế Auth cơ bản. |
| **Courses** | ❌ **Chưa có** | Cần tạo Model `Course`. |
| **ExamSessions** | ❌ **Chưa có** | Cần tạo Model `ExamSession`. |
| **ExamRegistrations** | ❌ **Chưa có** | Cần tạo Model `ExamRegistration` (liên kết User - Session). |
| **Classes** | ❌ **Chưa có** | Cần tạo Model `Class` (liên kết Course - Instructor). |
| **ClassEnrollments** | ❌ **Chưa có** | Cần tạo Model `ClassEnrollment` (liên kết Class - Student). |
| **Payments** | ❌ **Chưa có** | Cần tạo Model `Payment`. |
| **Branches** | ❌ **Chưa có** | Cần tạo Model `Branch`. |
| **Faqs** | ❌ **Chưa có** | Cần tạo Model `Faq`. |
| **ChatMessages** | ✅ Có sẵn | Model `ChatMessage` đã có (Chức năng mở rộng). |
| **UpgradeRequests** | ✅ Có sẵn | Model `UpgradeRequest` dùng để duyệt nâng cấp role? |

👉 **Kết luận:** Hiện tại dự án mới chỉ có khung quản lý User/Account. Toàn bộ nghiệp vụ cốt lõi (Core Business) về Đào tạo và Tuyển sinh chưa được triển khai.

---

## 2. Phân tích Chức năng (Functionality)

| Nhóm Chức năng | Chức năng cụ thể | Trạng thái | Ghi chú |
| :--- | :--- | :---: | :--- |
| **Hệ thống** | Đăng ký / Đăng nhập | ✅ Đã xong | Sử dụng Identity. |
| | Phân quyền (Roles) | ⚠️ Một phần | Đã có Role, nhưng chưa phân chia logic nghiệp vụ sâu. |
| | Chat nội bộ | ✅ Đã xong | Đã có Controller `Chat` và Model. |
| **Admin** | Quản lý Khóa học (CRUD) | ❌ Chưa có | Cần làm Controller, Views. |
| | Quản lý Kỳ tuyển sinh | ❌ Chưa có | |
| | Xếp lớp & Phân công GV | ❌ Chưa có | |
| | Quản lý Tài chính | ❌ Chưa có | |
| **Giảng viên** | Xem lớp & Học viên | ❌ Chưa có | |
| | Chấm điểm thi | ❌ Chưa có | |
| | Upload tài liệu | ❌ Chưa có | |
| **Học viên** | Đăng ký thi tuyển sinh | ❌ Chưa có | |
| | Tra cứu kết quả | ❌ Chưa có | |
| | Thanh toán học phí | ❌ Chưa có | |
| **Khách** | Xem danh sách khóa học | ❌ Chưa có | |
| | Liên hệ & FAQ | ❌ Chưa có | |

---

## 3. Kế hoạch Triển khai Tiếp theo (Action Plan)

Dựa trên phân tích trên, lộ trình triển khai đề xuất:

### Giai đoạn 1: Xây dựng Core Models & Database (Ưu tiên cao nhất)
*   **Bước 1:** Tạo các Class Model C# trong thư mục `Models`:
    *   `Course.cs`
    *   `ExamSession.cs`
    *   `ExamRegistration.cs` (Kết quả thi)
    *   `Class.cs`
    *   `ClassEnrollment.cs`
    *   `Payment.cs`
    *   `Branch.cs`, `Faq.cs`
*   **Bước 2:** Đăng ký `DbSet` trong `ApplicationDbContext.cs`.
*   **Bước 3:** Chạy Migration để cập nhật Database.

### Giai đoạn 2: Phát triển Backend (Admin Controllers)
*   Tạo `CoursesController` (CRUD Khóa học).
*   Tạo `ExamSessionsController` (Quản lý kỳ thi).
*   Tạo `ClassesController` (Quản lý lớp học).

### Giai đoạn 3: Phát triển Backend (Public & Student)
*   Chức năng Đăng ký thi tuyển sinh (User flow).
*   Chức năng Tra cứu kết quả và Thanh toán.

### Giai đoạn 4: Giao diện (Frontend)
*   Hoàn thiện trang chủ (Landing Page).
*   Các View cho từng chức năng trên.
