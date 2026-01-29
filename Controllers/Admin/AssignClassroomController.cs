using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Symphony.Portal.Web.Data;
using Symphony.Portal.Web.Models;
using Symphony.Portal.Web.Models.Enums;

namespace Symphony.Portal.Web.Controllers.Admin
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AssignClassroomsController : Controller
    {
        private readonly AppDbContext _context;

        public AssignClassroomsController(AppDbContext context)
        {
            _context = context;
        }

        // =========================
        // INDEX (LIST)
        // =========================
        public async Task<IActionResult> Index()
        {
            var data = await _context.ClassAssignments
                .Include(a => a.Student)
                    .ThenInclude(u => u.Role)
                .Include(a => a.Class)
                    .ThenInclude(c => c.ClassCategory)
                .ToListAsync();

            return View(data);
        }

        // =========================
        // CREATE (MANUAL ASSIGN)
        // =========================
        public async Task<IActionResult> Create()
        {
            ViewBag.Students = await _context.Users
                .Include(u => u.Role)
                .Where(u => u.Role != null && u.Role.Name == "Student" && u.IsActive)
                .ToListAsync();

            ViewBag.Classes = await _context.Classes
                .Where(c => c.NumberOfSeats > 0 && c.Status == ClassStatus.Active)
                .ToListAsync();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string studentId, string classId)
        {
            bool exists = await _context.ClassAssignments
                .AnyAsync(a => a.StudentId == studentId);

            if (exists)
            {
                TempData["Error"] = "Student already assigned to a class.";
                return RedirectToAction(nameof(Create));
            }

            var classObj = await _context.Classes.FindAsync(classId);
            if (classObj == null || classObj.NumberOfSeats <= 0)
            {
                TempData["Error"] = "Invalid or full class.";
                return RedirectToAction(nameof(Create));
            }

            var assignment = new ClassAssignment
            {
                StudentId = studentId,
                ClassId = classId
            };

            classObj.NumberOfSeats--;

            _context.ClassAssignments.Add(assignment);
            _context.Classes.Update(classObj);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Student assigned successfully.";
            return RedirectToAction(nameof(Index));
        }

        // =========================
        // EDIT (CHANGE CLASS)
        // =========================
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null) return NotFound();

            var assignment = await _context.ClassAssignments
                .Include(a => a.Student)
                    .ThenInclude(u => u.Role)
                .Include(a => a.Class)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (assignment == null) return NotFound();

            ViewBag.Classes = await _context.Classes
                .Where(c => c.NumberOfSeats > 0 || c.Id == assignment.ClassId)
                .ToListAsync();

            return View(assignment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, string classId)
        {
            var assignment = await _context.ClassAssignments.FindAsync(id);
            if (assignment == null) return NotFound();

            if (assignment.ClassId != classId)
            {
                var oldClass = await _context.Classes.FindAsync(assignment.ClassId);
                var newClass = await _context.Classes.FindAsync(classId);

                if (newClass == null || newClass.NumberOfSeats <= 0)
                {
                    TempData["Error"] = "Target class is full or invalid.";
                    return RedirectToAction(nameof(Edit), new { id });
                }

                if (oldClass != null) oldClass.NumberOfSeats++;
                newClass.NumberOfSeats--;

                assignment.ClassId = classId;

                _context.Classes.UpdateRange(oldClass!, newClass);
                _context.ClassAssignments.Update(assignment);
                await _context.SaveChangesAsync();
            }

            TempData["Success"] = "Assignment updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        // =========================
        // DELETE (UNASSIGN)
        // =========================
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null) return NotFound();

            var assignment = await _context.ClassAssignments
                .Include(a => a.Student)
                .Include(a => a.Class)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (assignment == null) return NotFound();

            return View(assignment);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var assignment = await _context.ClassAssignments.FindAsync(id);
            if (assignment == null) return RedirectToAction(nameof(Index));

            var classObj = await _context.Classes.FindAsync(assignment.ClassId);
            if (classObj != null)
            {
                classObj.NumberOfSeats++;
                _context.Classes.Update(classObj);
            }

            _context.ClassAssignments.Remove(assignment);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Student unassigned successfully.";
            return RedirectToAction(nameof(Index));
        }

        // =========================
        // ASSIGN BY SCORE (VIEW) - UPDATED LEVEL A, B, C
        // =========================
        public async Task<IActionResult> AssignByScore(string level)
        {
            ViewBag.Level = level; // Lưu level đã chọn để hiển thị lại ở View

            // Lấy danh sách học sinh đã được xếp lớp để loại trừ
            var assignedIds = await _context.ClassAssignments
                .Select(a => a.StudentId)
                .ToListAsync();

            if (string.IsNullOrEmpty(level))
            {
                ViewBag.Students = new List<ExamResult>();
            }
            else
            {
                // Logic lọc điểm theo Level
                // Level A: 70 - 100
                // Level B: 40 - dưới 70
                // Level C: Dưới 40

                var query = _context.ExamResults
                    .Include(e => e.Student)
                        .ThenInclude(s => s.Role)
                    .Where(e =>
                        e.Student != null &&
                        e.Student.Role != null &&
                        e.Student.Role.Name == "Student" &&
                        !assignedIds.Contains(e.StudentId)
                        // Lưu ý: Nếu Level C là rớt, bạn có thể cần bỏ check e.IsPassed tùy nghiệp vụ
                        && e.IsPassed
                    );

                if (level == "A")
                {
                    query = query.Where(e => e.Score >= 70);
                }
                else if (level == "B")
                {
                    query = query.Where(e => e.Score >= 40 && e.Score < 70);
                }
                else if (level == "C")
                {
                    query = query.Where(e => e.Score < 40);
                }

                var students = await query
                    .OrderByDescending(e => e.Score)
                    .ToListAsync();

                ViewBag.Students = students;
            }

            ViewBag.Classes = await _context.Classes
                .Where(c => c.NumberOfSeats > 0 && c.Status == ClassStatus.Active)
                .ToListAsync();

            return View();
        }

        // =========================
        // ASSIGN BY SCORE (CONFIRM) - UPDATED
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignByScoreConfirm(string level, string classId)
        {
            var classObj = await _context.Classes.FindAsync(classId);
            if (classObj == null || classObj.NumberOfSeats <= 0)
            {
                TempData["Error"] = "Invalid or full class.";
                return RedirectToAction(nameof(AssignByScore), new { level });
            }

            // Lấy lại danh sách ID đã xếp lớp
            var assignedIds = await _context.ClassAssignments
                .Select(a => a.StudentId)
                .ToListAsync();

            // Tái tạo logic query giống hệt bên trên để lấy danh sách cần add
            var query = _context.ExamResults
                .Include(e => e.Student)
                    .ThenInclude(s => s.Role)
                .Where(e =>
                    e.Student != null &&
                    e.Student.Role != null &&
                    e.Student.Role.Name == "Student" &&
                    !assignedIds.Contains(e.StudentId)
                    && e.IsPassed
                );

            if (level == "A")
            {
                query = query.Where(e => e.Score >= 70);
            }
            else if (level == "B")
            {
                query = query.Where(e => e.Score >= 40 && e.Score < 70);
            }
            else if (level == "C")
            {
                query = query.Where(e => e.Score < 40);
            }

            var candidates = await query
                .OrderByDescending(e => e.Score)
                .ToListAsync();

            int assignedCount = 0;

            foreach (var result in candidates)
            {
                if (classObj.NumberOfSeats <= 0)
                    break;

                var assignment = new ClassAssignment
                {
                    StudentId = result.StudentId,
                    ClassId = classId
                };

                classObj.NumberOfSeats--;
                assignedCount++;

                _context.ClassAssignments.Add(assignment);
            }

            _context.Classes.Update(classObj);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Assigned {assignedCount} students (Level {level}) to class '{classObj.ClassName}'.";

            return RedirectToAction(nameof(Index));
        }
    }
}
