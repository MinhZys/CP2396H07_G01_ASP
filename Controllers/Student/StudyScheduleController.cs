using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Symphony.Portal.Web.Data;

namespace Symphony.Portal.Web.Controllers.Student
{
    [Authorize(Roles = "Student")]
    [Route("student/study-schedule")]
    public class StudyScheduleController : Controller
    {
        private readonly AppDbContext _context;

        public StudyScheduleController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /student/study-schedule?from=...&to=...
        [HttpGet("")]
        public async Task<IActionResult> Index(DateTime? from, DateTime? to)
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            var classId = await _context.ClassAssignments
                .Where(x => x.StudentId == userId)
                .OrderByDescending(x => x.AssignedAt)
                .Select(x => x.ClassId)
                .FirstOrDefaultAsync();

            if (string.IsNullOrWhiteSpace(classId))
            {
                ViewBag.Message = "Bạn chưa được xếp lớp.";
                return View(new List<Symphony.Portal.Web.Models.ClassSession>());
            }

            var start = (from ?? DateTime.Today).Date;
            var end = (to ?? start.AddDays(6)).Date;

            var schedule = await _context.ClassSchedules
                .Where(s => s.ClassId == classId && s.IsPublished)
                .OrderByDescending(s => s.PublishedAt)
                .FirstOrDefaultAsync();

            if (schedule == null)
            {
                ViewBag.Message = "Lịch học chưa được công bố.";
                return View(new List<Symphony.Portal.Web.Models.ClassSession>());
            }

            var sessions = await _context.ClassSessions
                .Include(s => s.Room)
                .Where(s =>
                    s.ClassScheduleId == schedule.Id &&
                    !s.IsCancelled &&
                    s.SessionDate >= start && s.SessionDate <= end)
                .OrderBy(s => s.SessionDate)
                .ThenBy(s => s.StartTime)
                .ToListAsync();

            ViewBag.From = start;
            ViewBag.To = end;
            ViewBag.ClassId = classId;
            ViewBag.ScheduleId = schedule.Id;

            return View(sessions);
        }

        private string? GetCurrentUserId()
        {
            // Identity chuẩn: NameIdentifier
            return User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue("sub"); // fallback nếu bạn dùng JWT kiểu sub
        }
    }
}
