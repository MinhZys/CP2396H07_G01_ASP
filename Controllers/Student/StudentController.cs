using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting; // Cần để xử lý file
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Symphony.Portal.Web.Data;
using Symphony.Portal.Web.Models; // Namespace chứa Lesson, Material...
using Symphony.Portal.Web.Models.ViewModels;
using System.Security.Claims;

namespace Symphony.Portal.Web.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        // Constructor: Tiêm DbContext và Môi trường Hosting
        public StudentController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var viewModel = new StudentDashboardVM
            {
                MyClassCount = await _context.ClassAssignments.CountAsync(ca => ca.StudentId == studentId),
                UpcomingExamCount = await _context.ClassExams
                    .CountAsync(ce => ce.ExamDate > DateTime.Now && 
                                     _context.ClassAssignments.Any(ca => ca.StudentId == studentId && ca.ClassId == ce.ClassId)),
                UnreadNotificationCount = await _context.Notifications
                    .CountAsync(n => n.UserId == studentId && !n.IsRead),
                RecentAssignments = await _context.Assignments
                    .Where(a => _context.ClassAssignments.Any(ca => ca.StudentId == studentId && ca.ClassId == a.ClassId))
                    .OrderByDescending(a => a.CreatedAt)
                    .Take(5)
                    .ToListAsync()
            };

            return View(viewModel);
        }

        // ==========================================
        // 1. DANH SÁCH LỚP HỌC
        // ==========================================
        public IActionResult ViewClasses()
        {
            var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var classes = _context.ClassAssignments
                .Where(ca => ca.StudentId == studentId)
                .Include(ca => ca.Class)
                .ThenInclude(c => c.ClassCategory)
                .ToList();

            return View("ViewClasses/ViewClasses", classes);
        }

        // ==========================================
        // 2. XEM DANH SÁCH BÀI HỌC (LESSONS)
        // ==========================================
        public async Task<IActionResult> ViewLessons(string classId)
        {
            if (string.IsNullOrEmpty(classId)) return RedirectToAction(nameof(ViewClasses));

            var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // BẢO MẬT: Kiểm tra học sinh có thuộc lớp này không
            var isAssigned = await _context.ClassAssignments
                .AnyAsync(ca => ca.ClassId == classId && ca.StudentId == studentId);

            if (!isAssigned) return RedirectToAction(nameof(ViewClasses));

            // Lấy danh sách bài học
            var lessons = await _context.Lessons
                .Include(l => l.Subject)
                .Where(l => l.ClassId == classId)
                .OrderBy(l => l.Title) // Sắp xếp theo tên hoặc thứ tự bài học
                .ToListAsync();

            // Lấy tên lớp để hiển thị
            var className = await _context.Classes
                .Where(c => c.Id == classId)
                .Select(c => c.ClassName)
                .FirstOrDefaultAsync();

            ViewData["ClassName"] = className;
            ViewData["ClassId"] = classId;

            return View("Lessons/ViewLessons", lessons);
        }

        // ==========================================
        // 3. TẢI BÀI HỌC (DOWNLOAD LESSON) - QUAN TRỌNG
        // ==========================================
        public async Task<IActionResult> DownloadLesson(string lessonId)
        {
            if (string.IsNullOrEmpty(lessonId)) return NotFound();

            var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Lấy thông tin bài học
            var lesson = await _context.Lessons.FirstOrDefaultAsync(l => l.Id == lessonId);
            if (lesson == null) return NotFound("Lesson not found.");

            // --- BẢO MẬT CẤP CAO ---
            // Kiểm tra xem học sinh (đang đăng nhập) có học lớp chứa bài học này không?
            var isStudentInClass = await _context.ClassAssignments
                .AnyAsync(ca => ca.StudentId == studentId && ca.ClassId == lesson.ClassId);

            if (!isStudentInClass)
            {
                // Nếu không đúng lớp -> Chặn truy cập
                return Forbid();
            }
            // -----------------------

            if (string.IsNullOrEmpty(lesson.ContentLink)) return NotFound("No content available.");

            // TRƯỜNG HỢP A: Link Online (Google Drive, Youtube, Zoom...)
            if (lesson.ContentLink.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                return Redirect(lesson.ContentLink);
            }

            // TRƯỜNG HỢP B: File nội bộ (trên server wwwroot)
            // Giả sử ContentLink lưu trong DB là: /uploads/lessons/bai1.pdf
            var webRootPath = _webHostEnvironment.WebRootPath;
            var filePath = Path.Combine(webRootPath, lesson.ContentLink.TrimStart('/'));

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("File not found on server.");
            }

            var fileName = Path.GetFileName(filePath);
            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);

            // Trả về file
            return File(fileBytes, "application/octet-stream", fileName);
        }

        // ==========================================
        // 4. XEM DANH SÁCH BÀI THI (EXAMS)
        // ==========================================
        public async Task<IActionResult> ViewExams(string classId)
        {
            if (string.IsNullOrEmpty(classId)) return RedirectToAction(nameof(ViewClasses));

            var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // BẢO MẬT: Kiểm tra học sinh có thuộc lớp này không
            var isAssigned = await _context.ClassAssignments
                .AnyAsync(ca => ca.ClassId == classId && ca.StudentId == studentId);

            if (!isAssigned) return RedirectToAction(nameof(ViewClasses));

            // Lấy danh sách Class Exam
            var exams = await _context.ClassExams
                .Include(ce => ce.Course)
                .Include(ce => ce.ExamPaper)
                .Where(ce => ce.ClassId == classId)
                .OrderByDescending(ce => ce.ExamDate)
                .ToListAsync();

            // Lấy danh sách các bài thi mà học sinh này đã làm trong lớp này
            var studentSessions = await _context.StudentExamSessions
                .Where(s => s.StudentId == studentId && s.ClassExamId != null && s.ClassExam!.ClassId == classId)
                .ToListAsync();

            // Chuyển thành Dictionary để dễ tra cứu trong View: ClassExamId -> StudentExamSession
            var sessionDict = studentSessions.ToDictionary(s => s.ClassExamId!, s => s);

            var className = await _context.Classes
                .Where(c => c.Id == classId)
                .Select(c => c.ClassName)
                .FirstOrDefaultAsync();

            ViewData["ClassName"] = className;
            ViewData["ClassId"] = classId;
            ViewBag.StudentSessions = sessionDict;

            return View("Exams/ViewExams", exams);
        }
    }
}