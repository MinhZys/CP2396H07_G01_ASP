# Kịch bản Thuyết trình: Tích hợp AI vào Hệ thống Symphony Academy

**Người trình bày**: [Tên của bạn]
**Thời lượng dự kiến**: 3-5 phút
**Mục tiêu**: Giải thích cách hệ thống tích hợp AI (Ollama) và demo cấu hình.

---

## 1. Mở đầu (Giới thiệu)
**Lời thoại**:
"Xin chào mọi người. Hôm nay em xin trình bày về tính năng nổi bật nhất trong đồ án lần này: **Tích hợp Trí tuệ nhân tạo (AI) chạy cục bộ (Local AI)** để hỗ trợ đào tạo.
Thay vì sử dụng các API trả phí như OpenAI, em đã chọn giải pháp **Ollama** kết hợp với model **Llama 3.2** để đảm bảo tính riêng tư, tốc độ và hoàn toàn miễn phí."

---

## 2. Phần Cấu hình (Configuration)
*(Thao tác: Mở IDE, show file `appsettings.json`)*

**Lời thoại**:
"Đầu tiên, về phần cấu hình. Việc kết nối với AI được thực hiện rất đơn giản thông qua file `appsettings.json`.
Như mọi người thấy ở đây (dòng 29-33), em có section `Ollama`.
- **BaseUrl**: Trỏ về `localhost:11434`, đây là cổng mặc định mà Ollama service chạy trên máy tính.
- **Model**: Em đang sử dụng `llama3.2:latest`. Điểm hay của kiến trúc này là nếu muốn đổi sang model khác thông minh hơn như `gemma2` hay `mistral`, em chỉ cần sửa dòng text này là xong, không cần đụng vào code."

*(Thao tác: Mở file `Services/OllamaService.cs`)*

**Lời thoại**:
"Logic giao tiếp được đóng gói trong `OllamaService`. Service này sẽ gửi HTTP Request đến Ollama và nhận về câu trả lời.
Nó đóng vai trò như một 'cây cầu' giữa Website ASP.NET Core và bộ não AI."

---

## 3. Demo Tính năng 1: AI Chatbot (Hỗ trợ học viên)
*(Thao tác: Mở trang chủ, bật Chat Widget lên)*

**Lời thoại**:
"Tính năng đầu tiên là Chatbot hỗ trợ 24/7.
Khi người dùng bật chế độ 'AI Mode' và đặt câu hỏi, ví dụ: *'Học lập trình bắt đầu từ đâu?'*..."

*(Thao tác: Gõ câu hỏi và chờ câu trả lời)*

**Lời thoại**:
"Hệ thống sẽ không phản hồi bằng những câu if-else cứng nhắc. Thay vào đó, SignalR sẽ đẩy câu hỏi sang OllamaService.
Tại đây, em có cấu hình một **System Prompt** (như là: *'Bạn là trợ lý ảo thân thiện của Symphony Academy'*). Điều này giúp AI trả lời đúng trọng tâm giáo dục, lịch sự và chuyên nghiệp như một tư vấn viên thực thụ."

---

## 4. Demo Tính năng 2: AI Quiz Generator (Hỗ trợ giảng viên)
*(Thao tác: Vào Admin Dashboard -> System -> AI Quiz Generator)*

**Lời thoại**:
"Tính năng thứ hai giúp giải quyết nỗi đau đầu của giảng viên: **Soạn đề thi trắc nghiệm**.
Thông thường để soạn 10 câu hỏi mất 30 phút. Với AI, chỉ mất 30 giây."

*(Thao tác: Copy một đoạn văn bản (ví dụ bài giới thiệu C#) paste vào ô Input. Chọn môn học, số lượng 5 câu)*

**Lời thoại**:
"Ở đây em áp dụng kỹ thuật **Prompt Engineering** để ép AI trả về dữ liệu dưới dạng JSON chuẩn.
Khi em bấm nút 'Tạo câu hỏi', server sẽ gửi nội dung bài học cho AI kèm mệnh lệnh: *'Hãy tạo 5 câu hỏi từ nội dung này và trả về JSON'*.
ASP.NET Core sau đó sẽ bắt lấy JSON này và render ra giao diện trực quan cho giảng viên xem trước, chỉnh sửa và lưu vào ngân hàng câu hỏi."

---

## 5. Kết luận
**Lời thoại**:
"Tóm lại, việc tích hợp Ollama giúp hệ thống của chúng ta thông minh hơn, tự động hóa được các tác vụ thủ công (như tư vấn, soạn bài) mà vẫn giữ được dữ liệu an toàn trên chính server của trường.
Em xin kết thúc phần trình bày."
