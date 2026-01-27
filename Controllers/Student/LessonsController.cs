using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Symphony.Portal.Web.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization; // Cần thêm cái này để check đăng nhập
using Symphony.Portal.Web.Models.Enums; // Để dùng RegistrationStatus
using System.Security.Claims; // Để lấy thông tin User

namespace Symphony.Portal.Web.Controllers.Student
{
    [Area("Student")]
    [Authorize] // Bắt buộc phải đăng nhập mới vào được Controller này
    public class LessonsController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly AppDbContext _context;

        public LessonsController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // Helper: Lấy Email của user hiện tại
        private string GetCurrentUserEmail()
        {
            // Giả định Email được lưu trong Name hoặc Claim Email
            return User.FindFirstValue(ClaimTypes.Email) ?? User.Identity?.Name ?? string.Empty;
        }

        // Helper: Kiểm tra User có quyền truy cập CourseId này không
        private async Task<bool> IsUserRegisteredForCourse(string email, string courseId)
        {
            return await _context.StudentRegistrations
                .AnyAsync(r => r.Email == email
                            && r.CourseId == courseId
                            && r.Status == RegistrationStatus.Approved); // Chỉ cho xem nếu đã Approved
        }

        public async Task<IActionResult> Download(string id)
        {
            var userEmail = GetCurrentUserEmail();
            if (string.IsNullOrEmpty(userEmail)) return Challenge();

            var lesson = await _context.Lessons.FindAsync(id);

            if (lesson == null || string.IsNullOrEmpty(lesson.ContentLink))
                return NotFound();

            // BẢO MẬT: Kiểm tra xem User có đăng ký khóa học chứa bài học này không
            if (!await IsUserRegisteredForCourse(userEmail, lesson.CourseId))
            {
                return Forbid(); // Trả về lỗi 403 Forbidden nếu không có quyền
            }

            var filePath = Path.Combine(_env.WebRootPath, lesson.ContentLink);

            if (!System.IO.File.Exists(filePath))
                return NotFound("File not found");

            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            var fileName = Path.GetFileName(filePath);

            return File(fileBytes, "application/octet-stream", fileName);
        }

        // Student xem danh sách bài học
        public async Task<IActionResult> Index(string? courseId)
        {
            var userEmail = GetCurrentUserEmail();
            if (string.IsNullOrEmpty(userEmail)) return Challenge();

            // 1. Lấy danh sách tất cả CourseId mà sinh viên này đã đăng ký và được duyệt
            var registeredCourseIds = await _context.StudentRegistrations
                .Where(r => r.Email == userEmail && r.Status == RegistrationStatus.Approved)
                .Select(r => r.CourseId)
                .ToListAsync();

            // Nếu sinh viên chưa đăng ký khóa nào, trả về danh sách rỗng hoặc thông báo
            if (!registeredCourseIds.Any())
            {
                return View(new List<Symphony.Portal.Web.Models.Lesson>()); // Trả về list rỗng
            }

            // 2. Tạo query lấy bài học
            var lessonsQuery = _context.Lessons
                .Include(l => l.Course)
                .Include(l => l.Subject)
                .AsQueryable();

            // 3. Logic lọc:
            if (!string.IsNullOrEmpty(courseId))
            {
                // Nếu User chọn filter theo 1 course cụ thể trên UI
                // Phải kiểm tra xem courseId đó có nằm trong danh sách đã đăng ký không (tránh hack URL)
                if (registeredCourseIds.Contains(courseId))
                {
                    lessonsQuery = lessonsQuery.Where(l => l.CourseId == courseId);
                }
                else
                {
                    // Nếu cố tình nhập CourseId mình chưa đăng ký -> Chặn hoặc trả về rỗng
                    return Forbid();
                }
            }
            else
            {
                // Nếu không chọn course cụ thể, hiển thị TẤT CẢ bài học của các khóa ĐÃ ĐĂNG KÝ
                lessonsQuery = lessonsQuery.Where(l => registeredCourseIds.Contains(l.CourseId));
            }

            return View(await lessonsQuery.ToListAsync());
        }

        // Student xem chi tiết bài học
        public async Task<IActionResult> Details(string id)
        {
            var userEmail = GetCurrentUserEmail();
            if (string.IsNullOrEmpty(userEmail)) return Challenge();

            var lesson = await _context.Lessons
                .Include(l => l.Course)
                .Include(l => l.Subject)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (lesson == null) return NotFound();

            // BẢO MẬT: Kiểm tra quyền xem chi tiết
            if (!await IsUserRegisteredForCourse(userEmail, lesson.CourseId))
            {
                return Forbid(); // Không cho xem nếu chưa đăng ký khóa học chứa bài này
            }

            return View(lesson);
        }
    }
}