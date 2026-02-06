using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Symphony.Portal.Web.Data;
using Symphony.Portal.Web.Models;
using Symphony.Portal.Web.ViewModels.Schedule;

namespace Symphony.Portal.Web.Controllers.Admin
{
[Area("Admin")]
[Authorize(Roles = "Admin")]
[Route("admin/class-schedules")]
public class ClassSchedulesController : Controller
    {
        private readonly AppDbContext _context;

        public ClassSchedulesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: admin/class-schedules?classId=...
        [HttpGet("")]
        public async Task<IActionResult> Index(string? classId)
        {
            var query = _context.ClassSchedules
                .Include(x => x.Class)
                .OrderByDescending(x => x.CreatedAt)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(classId))
                query = query.Where(x => x.ClassId == classId);

            var data = await query.ToListAsync();
            return View(data);
        }

        // GET: admin/class-schedules/create?classId=...
        [HttpGet("create")]
        public async Task<IActionResult> Create(string? classId)
        {
            ViewBag.Classes = await _context.Classes
                .OrderBy(c => c.ClassName)
                .Select(c => new { c.Id, c.ClassName })
                .ToListAsync();

            var vm = new CreateScheduleVM
            {
                ClassId = classId ?? string.Empty,
                StartDate = DateTime.Today
            };
            return View(vm);
        }


        // POST: admin/class-schedules/create
        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateScheduleVM vm)
        {
            // reload dropdown khi return View
            async Task LoadClassesAsync()
            {
                ViewBag.Classes = await _context.Classes
                    .OrderBy(c => c.ClassName)
                    .Select(c => new { c.Id, c.ClassName })
                    .ToListAsync();
            }

            if (!ModelState.IsValid)
            {
                await LoadClassesAsync();
                return View(vm);
            }

            // ✅ check FK trước khi insert (đỡ nổ SQL)
            bool classExists = await _context.Classes.AnyAsync(c => c.Id == vm.ClassId);
            if (!classExists)
            {
                ModelState.AddModelError(nameof(vm.ClassId), "Class không tồn tại. Vui lòng chọn đúng lớp.");
                await LoadClassesAsync();
                return View(vm);
            }

            bool exists = await _context.ClassSchedules.AnyAsync(x => x.ClassId == vm.ClassId && x.Status != ScheduleStatus.Archived);
            if (exists)
            {
                ModelState.AddModelError("", "Lớp này đã có Schedule (chưa Archived).");
                await LoadClassesAsync();
                return View(vm);
            }

            var schedule = new ClassSchedule
            {
                ClassId = vm.ClassId,
                StartDate = vm.StartDate.Date,
                EndDate = vm.EndDate?.Date,
                Status = ScheduleStatus.Draft,
                IsPublished = false,
                IsLocked = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.ClassSchedules.Add(schedule);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = schedule.Id });
        }


        // GET: admin/class-schedules/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Details(int id)
        {
            var schedule = await _context.ClassSchedules
                .Include(x => x.Class)
                .Include(x => x.Sessions.OrderBy(s => s.SessionDate).ThenBy(s => s.StartTime))
                .FirstOrDefaultAsync(x => x.Id == id);

            if (schedule == null) return NotFound();
            return View(schedule);
        }

        // GET: admin/class-schedules/{id}/edit
        [HttpGet("{id:int}/edit")]
        public async Task<IActionResult> Edit(int id)
        {
            var schedule = await _context.ClassSchedules.FindAsync(id);
            if (schedule == null) return NotFound();
            if (schedule.IsLocked) return BadRequest("Schedule is locked.");

            var vm = new CreateScheduleVM
            {
                ClassId = schedule.ClassId,
                ClassName = schedule.Class?.ClassName,
                StartDate = schedule.StartDate,
                EndDate = schedule.EndDate
            };
            ViewBag.ScheduleId = id;
            return View(vm);
        }

        // POST: admin/class-schedules/{id}/edit
        [HttpPost("{id:int}/edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CreateScheduleVM vm)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ScheduleId = id;
                return View(vm);
            }

            var schedule = await _context.ClassSchedules.FindAsync(id);
            if (schedule == null) return NotFound();
            if (schedule.IsLocked) return BadRequest("Schedule is locked.");

            schedule.StartDate = vm.StartDate.Date;
            schedule.EndDate = vm.EndDate?.Date;
            schedule.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id });
        }

        // POST: admin/class-schedules/{id}/delete
        [HttpPost("{id:int}/delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var schedule = await _context.ClassSchedules.FindAsync(id);
            if (schedule == null) return NotFound();

            _context.ClassSchedules.Remove(schedule);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: admin/class-schedules/{id}/sessions
        [HttpGet("{id:int}/sessions")]
        public async Task<IActionResult> Sessions(int id)
        {
            var schedule = await _context.ClassSchedules
                .Include(x => x.Class)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (schedule == null) return NotFound();

            var sessions = await _context.ClassSessions
                .Include(s => s.Room)
                .Where(s => s.ClassScheduleId == id)
                .OrderBy(s => s.SessionDate).ThenBy(s => s.StartTime)
                .ToListAsync();

            ViewBag.Schedule = schedule;
            return View(sessions);
        }

        // GET: admin/class-schedules/{id}/generate
        [HttpGet("{id:int}/generate")]
        public async Task<IActionResult> Generate(int id)
        {
            var schedule = await _context.ClassSchedules.FindAsync(id);
            if (schedule == null) return NotFound();
            if (schedule.IsLocked) return BadRequest("Schedule is locked.");

            ViewBag.Instructors = await _context.Users.Where(u => u.RoleId == "2" && u.IsActive).OrderBy(u => u.FullName).ToListAsync();

            var vm = new GenerateScheduleVM
            {
                ScheduleId = id,
                FromDate = schedule.StartDate,
                ToDate = schedule.EndDate ?? schedule.StartDate.AddMonths(4),
                DaysOfWeek = new List<int> { 1, 3, 5 }, // Mon/Wed/Fri default
                StartTime = new TimeSpan(18, 0, 0),
                EndTime = new TimeSpan(20, 0, 0),
                SessionType = SessionType.Theory
            };

            return View(vm);
        }

        // POST: admin/class-schedules/{id}/generate
        [HttpPost("{id:int}/generate")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Generate(int id, GenerateScheduleVM vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var schedule = await _context.ClassSchedules.FindAsync(id);
            if (schedule == null) return NotFound();
            if (schedule.IsLocked) return BadRequest("Schedule is locked.");
            if (vm.EndTime <= vm.StartTime)
            {
                ModelState.AddModelError("", "EndTime phải lớn hơn StartTime.");
                return View(vm);
            }

            var from = vm.FromDate.Date;
            var to = vm.ToDate.Date;
            if (to < from)
            {
                ModelState.AddModelError("", "ToDate phải >= FromDate.");
                return View(vm);
            }

            // Holidays (active)
            var holidayDates = await _context.Holidays
                .Where(h => h.IsActive)
                .Select(h => h.Date.Date)
                .ToListAsync();

            // Generate
            var sessionsToAdd = new List<ClassSession>();
            for (var d = from; d <= to; d = d.AddDays(1))
            {
                int dow = ((int)d.DayOfWeek == 0) ? 7 : (int)d.DayOfWeek; // Sunday=7
                if (!vm.DaysOfWeek.Contains(dow)) continue;
                if (holidayDates.Contains(d)) continue;

                sessionsToAdd.Add(new ClassSession
                {
                    ClassScheduleId = id,
                    SessionDate = d,
                    StartTime = vm.StartTime,
                    EndTime = vm.EndTime,
                    RoomId = vm.RoomId,
                    InstructorId = vm.InstructorId,
                    SessionType = vm.SessionType,
                    CreatedAt = DateTime.UtcNow
                });
            }

            if (sessionsToAdd.Count == 0)
            {
                ModelState.AddModelError("", "Không tạo được buổi nào (check ngày/holiday/daysOfWeek).");
                return View(vm);
            }

            // optional: basic conflict check (room/instructor overlap)
            // (Bạn có thể tách thành service sau)
            foreach (var s in sessionsToAdd)
            {
                if (s.RoomId.HasValue)
                {
                    bool roomConflict = await _context.ClassSessions.AnyAsync(x =>
                        x.RoomId == s.RoomId &&
                        x.SessionDate == s.SessionDate &&
                        !x.IsCancelled &&
                        s.StartTime < x.EndTime && s.EndTime > x.StartTime);

                    if (roomConflict)
                    {
                        ModelState.AddModelError("", $"Trùng phòng ngày {s.SessionDate:yyyy-MM-dd}.");
                        return View(vm);
                    }
                }

                if (!string.IsNullOrWhiteSpace(s.InstructorId))
                {
                    bool teacherConflict = await _context.ClassSessions.AnyAsync(x =>
                        x.InstructorId == s.InstructorId &&
                        x.SessionDate == s.SessionDate &&
                        !x.IsCancelled &&
                        s.StartTime < x.EndTime && s.EndTime > x.StartTime);

                    if (teacherConflict)
                    {
                        ModelState.AddModelError("", $"Trùng giảng viên ngày {s.SessionDate:yyyy-MM-dd}.");
                        return View(vm);
                    }
                }
            }

            _context.ClassSessions.AddRange(sessionsToAdd);
            schedule.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Sessions), new { id });
        }

        // POST: admin/class-schedules/{id}/publish
        [HttpPost("{id:int}/publish")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Publish(int id)
        {
            var schedule = await _context.ClassSchedules.FindAsync(id);
            if (schedule == null) return NotFound();
            if (schedule.IsLocked) return BadRequest("Schedule is locked.");

            schedule.IsPublished = true;
            schedule.Status = ScheduleStatus.Published;
            schedule.PublishedAt = DateTime.UtcNow;
            schedule.PublishedByUserId = GetCurrentUserId();
            schedule.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id });
        }

        // POST: admin/class-schedules/{id}/lock
        [HttpPost("{id:int}/lock")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Lock(int id)
        {
            var schedule = await _context.ClassSchedules.FindAsync(id);
            if (schedule == null) return NotFound();

            schedule.IsLocked = true;
            schedule.Status = ScheduleStatus.Locked;
            schedule.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id });
        }

        // POST: admin/class-schedules/{id}/unlock
        [HttpPost("{id:int}/unlock")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unlock(int id)
        {
            var schedule = await _context.ClassSchedules.FindAsync(id);
            if (schedule == null) return NotFound();

            schedule.IsLocked = false;
            if (schedule.IsPublished) schedule.Status = ScheduleStatus.Published;
            else schedule.Status = ScheduleStatus.Draft;

            schedule.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id });
        }

        // TODO: nối vào hệ auth hiện tại của bạn (Session/Cookie)
        private string? GetCurrentUserId()
        {
            // ví dụ: return HttpContext.Session.GetString("UserId");
            return null;
        }
    }
}
