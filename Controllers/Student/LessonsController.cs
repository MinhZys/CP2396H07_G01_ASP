using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Symphony.Portal.Web.Data;
using Microsoft.AspNetCore.Hosting;


namespace Symphony.Portal.Web.Controllers.Student
{
    [Area("Student")]
    public class LessonsController : Controller
    {
        private readonly IWebHostEnvironment _env;

        private readonly AppDbContext _context;

        public LessonsController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<IActionResult> Download(string id)
        {
            var lesson = await _context.Lessons.FindAsync(id);
            if (lesson == null || string.IsNullOrEmpty(lesson.ContentLink))
                return NotFound();

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
            var lessons = _context.Lessons
                .Include(l => l.Course)
                .Include(l => l.Subject)
                .AsQueryable();

            if (!string.IsNullOrEmpty(courseId))
            {
                lessons = lessons.Where(l => l.CourseId == courseId);
            }

            return View(await lessons.ToListAsync());
        }

        // Student xem chi tiết bài học
        public async Task<IActionResult> Details(string id)
        {
            var lesson = await _context.Lessons
                .Include(l => l.Course)
                .Include(l => l.Subject)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (lesson == null) return NotFound();

            return View(lesson);
        }
    }
}
