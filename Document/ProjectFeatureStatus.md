# Trạng thái Tính năng Dự án (Feature Status)

Tài liệu này theo dõi tiến độ thực hiện các chức năng của dự án Symphony Ltd Portal.

---

## 1. Chức năng ĐÃ CÓ (Existing Features)

Hiện tại trong Source Code đã hoàn thành các phần sau:

| Module | Chức năng | Database Model tương ứng | Ghi chú kỹ thuật |
| :--- | :--- | :--- | :--- |
| **Authentication** | Đăng ký / Đăng nhập | `AspNetUsers` (Identity) | Sử dụng ASP.NET Core Identity mặc định. |
| **User Profile** | Cập nhật hồ sơ | `UserDetails`, `StaffProfile` | Đã tách bảng hồ sơ riêng. |
| **System** | Chat nội bộ | `ChatMessage` | Real-time chat (SignalR?). |
| **System** | Yêu cầu nâng quyền | `UpgradeRequest` | Duyệt role thủ công. |

---

## 2. Chức năng CẦN LÀM NGAY (Required -> Next Sprint)

Đây là các chức năng cốt lõi (Core) chưa có và cần code gấp trong giai đoạn này.

### A. Quản lý Khóa học & Thi tuyển sinh
| Chức năng | Yêu cầu Database | View cần làm |
| :--- | :--- | :--- |
| CRUD Khóa học | `Courses` | Danh sách, Thêm mới, Sửa. |
| Tạo Kỳ thi tuyển sinh | `ExamSessions` | Form tạo kỳ thi (đặt ngày, phí). |
| Đăng ký thi trực tuyến | `ExamRegistrations` | Form đăng ký cho Guest/Student. |
| Nhập điểm thi | `ExamRegistrations` | Bảng nhập điểm cho Giáo viên/Admin. |

### B. Quản lý Lớp học & Học viên
| Chức năng | Yêu cầu Database | View cần làm |
| :--- | :--- | :--- |
| Xếp lớp tự động/thủ công | `Classes`, `ClassEnrollments` | Giao diện chọn HV vào lớp. |
| Quản lý Lớp (GV) | `Classes` | GV xem danh sách lớp mình dạy. |
| Upload Tài liệu | `Documents` | Form upload file. |

### C. Tài chính
| Chức năng | Yêu cầu Database | View cần làm |
| :--- | :--- | :--- |
| Thanh toán Phí thi | `Payments` | Xác nhận đóng tiền thi. |
| Thanh toán Học phí | `Payments` | Xác nhận đóng học phí nhập học. |

---

## 3. Chức năng DỰ KIẾN (Planned / Nice-to-have)

Các chức năng mở rộng sẽ làm sau khi Core hoàn thành.

1.  **Phản hồi & Đánh giá:** Học viên đánh giá giảng viên (`Feedbacks` table).
2.  **Thống kê Dashboard:** Biểu đồ doanh thu, tỷ lệ đậu rớt.
3.  **Hệ thống thông báo:** Notification realtime khi có điểm.
4.  **CMS:** Quản lý bài viết Tin tức, FAQ động (`Faqs` table).
