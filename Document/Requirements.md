# Tài liệu Yêu cầu Hệ thống: Symphony Portal

Dựa trên mô tả dự án "Symphony Ltd.", tài liệu này chi tiết hóa các yêu cầu chức năng và phi chức năng của hệ thống.

## 1. Giới thiệu
**Symphony Ltd.** là học viện tư nhân chuyên đào tạo và cấp chứng chỉ CNTT (Networking, Java, Database...). Cần xây dựng một Cổng thông tin trực tuyến (Online Portal) để quảng bá dịch vụ, tuyển sinh và quản lý đào tạo, thay thế cho các phương thức thủ công (tờ rơi, quảng cáo truyền thống).

## 2. Nhóm Chức năng (Functional Modules)

### A. Phân hệ Quản trị (Admin Module)
Dành cho quản trị viên đăng nhập và quản lý toàn bộ dữ liệu của trung tâm.

1.  **Quản lý Khóa học & Nội dung (Courses & CMS):**
    *   **Khóa học:** Thêm/Sửa/Xóa danh sách các khóa học và chủ đề (topics) được giảng dạy.
    *   **Tin tức & CMS:** Cập nhật thông tin "Về chúng tôi" (About Us), "Tại sao chọn chúng tôi?" (Why to join), Quy trình tham gia khóa học (How to join).
    *   **FAQ:** Thêm/Sửa/Xóa các câu hỏi thường gặp (FAQ) về khóa học, học phí, giờ thực hành.
    *   **Liên hệ & Chi nhánh:** Quản lý danh sách các trung tâm (Centres), địa chỉ, số điện thoại.

2.  **Quản lý Tuyển sinh & Thi đầu vào (Entrance Exams):**
    *   **Lịch thi:** Tạo thông tin các kỳ thi tuyển sinh (thường tổ chức 6 tháng/lần). Cập nhật lệ phí thi.
    *   **Kết quả thi:** Nhập điểm thi, Số báo danh (Roll No), Tên học viên.
    *   **Phân loại (Segregation):** Dựa trên điểm thi, hệ thống/admin phân loại học viên vào lớp phù hợp (Cơ bản hoặc Nâng cao) và gán mức học phí tương ứng.

3.  **Quản lý Tài chính (Financials):**
    *   Xác định mức học phí và lệ phí thi (có thể thay đổi theo thời gian).
    *   Quản lý thông tin thanh toán của học viên (Tiền mặt, Séc/Cheque, DD).

### B. Phân hệ Khách & Học viên (Public/Student Module)
Các chức năng công khai (Normal site function) và dành cho học viên.

1.  **Thông tin Chung (Public Access):**
    *   **Trang chủ:** Giới thiệu ngắn gọn, tin tức khóa học mới.
    *   **Chi tiết khóa học:** Xem danh sách khóa học và nội dung chi tiết.
    *   **Thông tin Tuyển sinh:** Xem lịch thi tuyển sinh sắp tới, lệ phí thi.
    *   **FAQ & Liên hệ:** Xem câu hỏi thường gặp và danh sách chi nhánh.
    *   **Đăng ký dự thi:** Tải hoặc điền form đăng ký trực tuyến (chọn khóa học, nhập số biên lai thanh toán hoặc thông tin DD/Cheque), sau đó nhận phiếu báo thi (Roll No, ngày giờ, môn thi).

2.  **Tra cứu Kết quả (Exam Results):**
    *   Truy cập trang tra cứu -> Nhập Số báo danh (Roll Number).
    *   **Nếu có:** Hiển thị Điểm số, Lớp được xếp vào, Học phí tương ứng, Hạn chót đóng tiền.
    *   **Nếu không có:** Thông báo "Số báo danh không tồn tại".

3.  **Học tập & Tiện ích (Student Services):**
    *   **Thư viện:** Quy định phải xuất trình thẻ (ID Card) để vào, không được mượn sách về nhà.
    *   **Phòng Lab (Thực hành):**
        *   Trong khóa học: Miễn phí, mở cửa đến 9PM.
        *   Sau khóa học: Có phí ($1000 nếu đăng ký từ đầu, $1500 nếu đăng ký sau).

### C. Phân hệ Giảng viên (Instructor Module)
*(Mở rộng từ yêu cầu quản lý đào tạo & hỗ trợ thực hành)*

1.  **Quản lý Giảng dạy:**
    *   Xem lịch phân công dạy (Phòng, Thời gian).
    *   Xem danh sách học viên trong lớp được phân công.

2.  **Thi cử & Đánh giá:**
    *   **Phân công coi thi:** Xem lịch giám thị.
    *   **Chấm thi:** Chấm điểm thi đầu vào và thi cuối khóa -> Gửi kết quả về cho Admin để xếp lớp/cấp chứng chỉ.
    *   **Phản hồi:** Đưa ra nhận xét, gợi ý cải thiện cho học viên sau kỳ thi cuối khóa (Feedback).

3.  **Tài liệu:**
    *   Upload tài liệu học tập, bài giảng cho lớp.

---

## 3. Quy tắc Nghiệp vụ (Business Rules)

1.  **Phân loại & Học phí (Segregation & Fees):**
    *   **Thi đầu vào:** Dùng để đánh giá trình độ.
    *   **Lớp Cơ bản (With Basics):** Dành cho người chưa có kiến thức nền. Thời gian: **6 tháng**. Học phí: **~$6000**.
    *   **Lớp Nâng cao (Without Basics):** Dành cho người đã có kiến thức. Thời gian: **4 tháng**. Học phí: **~$4275**.

2.  **Thanh toán (Payment):**
    *   Phải đóng học phí ít nhất **1 ngày** trước khi khóa học bắt đầu.
    *   Hình thức: Tiền mặt (tại trung tâm, nhận biên lai), Séc (Cheque), Hối phiếu (DD).

3.  **Giờ thực hành thêm (Extra Lab Sessions):**
    *   Dịch vụ tùy chọn sau khóa học, có giảng viên hướng dẫn.
    *   Phí: **$1000** (nếu đăng ký ngay lúc nhập học) hoặc **$1500** (nếu đăng ký sau khi kết thúc khóa).
    *   Chỉ dành cho học viên của trung tâm.

---

## 4. Yêu cầu Phi chức năng

*   Giao diện thân thiện, dễ tra cứu thông tin.
*   Hệ thống bảo mật, phân quyền rõ ràng (Admin, Instructor, Student).
*   Cơ sở dữ liệu lưu trữ lịch sử thi, xếp lớp, thanh toán chính xác.
