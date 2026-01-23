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
        // ASSIGN BY SCORE (VIEW)
        // =========================
        public async Task<IActionResult> AssignByScore(decimal? minScore)
        {
            ViewBag.MinScore = minScore;

            // lấy danh sách học sinh đã được xếp lớp
            var assignedIds = await _context.ClassAssignments
                .Select(a => a.StudentId)
                .ToListAsync();

            if (minScore == null)
            {
                ViewBag.Students = new List<ExamResult>();
            }
            else
            {
                double min = Convert.ToDouble(minScore.Value);

                var students = await _context.ExamResults
                    .Include(e => e.Student)
                        .ThenInclude(s => s.Role)
                    .Where(e =>
                        e.Score >= min &&
                        e.IsPassed &&
                        e.Student != null &&
                        e.Student.Role != null &&
                        e.Student.Role.Name == "Student" &&
                        !assignedIds.Contains(e.StudentId)
                    )
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
        // ASSIGN BY SCORE (CONFIRM)
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignByScoreConfirm(decimal minScore, string classId)
        {
            var classObj = await _context.Classes.FindAsync(classId);
            if (classObj == null || classObj.NumberOfSeats <= 0)
            {
                TempData["Error"] = "Invalid or full class.";
                return RedirectToAction(nameof(AssignByScore), new { minScore });
            }

            double min = Convert.ToDouble(minScore);

            var assignedIds = await _context.ClassAssignments
                .Select(a => a.StudentId)
                .ToListAsync();

            var candidates = await _context.ExamResults
                .Include(e => e.Student)
                    .ThenInclude(s => s.Role)
                .Where(e =>
                    e.Score >= min &&
                    e.IsPassed &&
                    e.Student != null &&
                    e.Student.Role != null &&
                    e.Student.Role.Name == "Student" &&
                    !assignedIds.Contains(e.StudentId)
                )
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

            TempData["Success"] =
                $"Assigned {assignedCount} students to class '{classObj.ClassName}'.";

            return RedirectToAction(nameof(Index));
        }
    }
}
