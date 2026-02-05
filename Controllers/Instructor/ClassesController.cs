using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Symphony.Portal.Web.Data;
using Symphony.Portal.Web.Models;
using Symphony.Portal.Web.Models.Enums;
using System.Security.Claims;

namespace Symphony.Portal.Web.Areas.Instructor.Controllers
{
    [Area("Instructor")]
    [Authorize(Roles = "Instructor")]
    public class ClassesController : Controller
    {
        private readonly AppDbContext _context;

        public ClassesController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var instructorId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // 1) Mark Assigned -> Received
            var notReceived = await _context.Assignments
                .Where(a => a.InstructorId == instructorId
                         && a.AssignmentType == AssignmentType.Teaching
                         && a.Status == AssignmentStatus.Assigned)
                .ToListAsync();

            if (notReceived.Any())
            {
                foreach (var item in notReceived)
                    item.Status = AssignmentStatus.Received;

                await _context.SaveChangesAsync();
            }

            // 2) Load list for display (include Class + Instructor)
            var assignments = await _context.Assignments
                .Include(a => a.Class)
                .Include(a => a.Instructor)
                .Where(a => a.InstructorId == instructorId
                         && a.AssignmentType == AssignmentType.Teaching)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

            // 3) Student count per class
            var classIds = assignments.Select(a => a.ClassId).Distinct().ToList();

            var studentCounts = await _context.ClassAssignments
                .Where(ca => classIds.Contains(ca.ClassId))
                .GroupBy(ca => ca.ClassId)
                .Select(g => new { ClassId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.ClassId, x => x.Count);

            ViewBag.StudentCounts = studentCounts;

            return View(assignments);
        }

        // ======================
        // CANCEL (GET): show reason form
        // ======================
        [HttpGet]
        public async Task<IActionResult> Cancel(string id)
        {
            var instructorId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var assignment = await _context.Assignments
                .Include(a => a.Class)
                .FirstOrDefaultAsync(a => a.Id == id
                    && a.InstructorId == instructorId
                    && a.AssignmentType == AssignmentType.Teaching);

            if (assignment == null) return NotFound();

            return View(assignment);
        }

        // ======================
        // CANCEL (POST): save reason + set Cancelled
        // ======================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(string id, string cancellationReason)
        {
            var instructorId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var assignment = await _context.Assignments
                .FirstOrDefaultAsync(a => a.Id == id
                    && a.InstructorId == instructorId
                    && a.AssignmentType == AssignmentType.Teaching);

            if (assignment == null) return NotFound();

            if (string.IsNullOrWhiteSpace(cancellationReason))
            {
                ModelState.AddModelError("", "Please enter a cancellation reason.");
                assignment = await _context.Assignments.Include(a => a.Class)
                    .FirstOrDefaultAsync(a => a.Id == id);
                return View(assignment);
            }

            assignment.Status = AssignmentStatus.Cancelled;
            assignment.CancellationReason = cancellationReason.Trim();

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // ======================
        // ASSIGN LESSON (GET): show lesson dropdown + assigned list
        // ======================
        [HttpGet]
        public async Task<IActionResult> AssignLesson(string classId)
        {
            var cls = await _context.Classes.FirstOrDefaultAsync(c => c.Id == classId);
            if (cls == null) return NotFound();

            var lessons = await _context.Lessons
                .OrderBy(l => l.Title)
                .ToListAsync();

            ViewBag.ClassId = classId;
            ViewBag.ClassName = cls.ClassName;
            ViewBag.Lessons = new SelectList(lessons, "Id", "Title");

            var assigned = await _context.ClassLessons
                .Include(x => x.Lesson)
                .Where(x => x.ClassId == classId)
                .OrderByDescending(x => x.AssignedAt)
                .ToListAsync();

            return View(assigned);
        }

        // ======================
        // ASSIGN LESSON (POST): create mapping
        // ======================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignLesson(string classId, string lessonId)
        {
            var exists = await _context.ClassLessons
                .AnyAsync(x => x.ClassId == classId && x.LessonId == lessonId);

            if (!exists)
            {
                _context.ClassLessons.Add(new ClassLesson
                {
                    ClassId = classId,
                    LessonId = lessonId,
                    AssignedAt = DateTime.Now
                });
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(AssignLesson), new { classId });
        }

        // ======================
        // REMOVE LESSON FROM CLASS
        // ======================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveLesson(string id, string classId)
        {
            var item = await _context.ClassLessons.FirstOrDefaultAsync(x => x.Id == id);
            if (item != null)
            {
                _context.ClassLessons.Remove(item);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(AssignLesson), new { classId });
        }

        // ======================
        // COMPLETE CLASS (POST)
        // ======================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Complete(string id)
        {
            var instructorId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var assignment = await _context.Assignments
                .Include(a => a.Class)
                .FirstOrDefaultAsync(a => a.Id == id
                    && a.InstructorId == instructorId
                    && a.AssignmentType == AssignmentType.Teaching);

            if (assignment == null) return NotFound();

            // Mark Class as Completed (Signal for Admin)
            if (assignment.Class != null)
            {
                assignment.Class.Status = ClassStatus.Completed;
            }

            // Mark Assignment as Completed (Visual for Instructor)
            assignment.Status = AssignmentStatus.Completed;

            await _context.SaveChangesAsync();
            TempData["Success"] = "Class marked as Completed. Admin has been notified.";
            return RedirectToAction(nameof(Index));
        }

        // ======================
        // MANAGE EXAMS (GET)
        // ======================
        [HttpGet]
        public async Task<IActionResult> ManageExams(string classId)
        {
            var cls = await _context.Classes.FirstOrDefaultAsync(c => c.Id == classId);
            if (cls == null) return NotFound();

            var exams = await _context.ClassExams
                .Include(ce => ce.Course)
                .Include(ce => ce.ExamPaper)
                .Where(ce => ce.ClassId == classId)
                .OrderByDescending(ce => ce.ExamDate)
                .ToListAsync();

            ViewBag.ClassId = classId;
            ViewBag.ClassName = cls.ClassName;

            return View(exams);
        }

        // ======================
        // PUBLISH SCORE (POST)
        // ======================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PublishScore(string classExamId)
        {
            var exam = await _context.ClassExams.FirstOrDefaultAsync(e => e.Id == classExamId);
            if (exam == null) return NotFound();

            exam.IsScorePublished = true;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Exam scores have been published to students.";
            return RedirectToAction(nameof(ManageExams), new { classId = exam.ClassId });
        }
    }
}
