using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Symphony.Portal.Web.Data;
using System.Security.Claims;

namespace Symphony.Portal.Web.Controllers.Instructor
{
    [Route("instructor/teaching-schedule")]
    public class TeachingScheduleController : Controller
    {
        private readonly AppDbContext _context;

        public TeachingScheduleController(AppDbContext context)
        {
            _context = context;
        }

        // GET: instructor/teaching-schedule?from=2026-02-01&to=2026-02-07
        [HttpGet("")]
        public async Task<IActionResult> Index(DateTime? from, DateTime? to)
        {
            var userId = GetCurrentUserId();

            // ✅ Nếu chưa đăng nhập -> chuyển qua flow login (cookie auth sẽ redirect)
            if (string.IsNullOrWhiteSpace(userId))
                return Challenge();

            // ✅ Nếu muốn chặn đúng role instructor (không phải instructor thì 403)
            if (!User.IsInRole("Instructor"))
                return Forbid();

            var start = (from ?? DateTime.Today).Date;
            var end = (to ?? start.AddDays(6)).Date;

            var sessions = await _context.ClassSessions
                .Include(s => s.Room)
                .Include(s => s.ClassSchedule)
                    .ThenInclude(cs => cs.Class)
                .Where(s =>
                    s.InstructorId == userId &&
                    !s.IsCancelled &&
                    s.SessionDate >= start && s.SessionDate <= end &&
                    s.ClassSchedule != null &&
                    s.ClassSchedule.IsPublished)
                .OrderBy(s => s.SessionDate).ThenBy(s => s.StartTime)
                .ToListAsync();

            ViewBag.From = start;
            ViewBag.To = end;
            return View(sessions);
        }

        private string? GetCurrentUserId()
        {
            // ✅ Ưu tiên chuẩn Identity:
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // ✅ fallback theo claim custom nếu bạn đang dùng:
            id ??= User.FindFirstValue("UserId");

            // ✅ fallback cuối nếu bạn lưu session (nếu có):
            // id ??= HttpContext.Session.GetString("UserId");

            return id;
        }
    }
}
