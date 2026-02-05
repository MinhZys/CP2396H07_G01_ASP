using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Symphony.Portal.Web.Data;
using Symphony.Portal.Web.Models;
using Symphony.Portal.Web.Models.Enums;
using Symphony.Portal.Web.Models.ViewModels;

namespace Symphony.Portal.Web.Controllers.Admin
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    [Route("Admin/[controller]/[action]")]
    public class ClassExamsController : Controller
    {
        private readonly AppDbContext _context;

        public ClassExamsController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? pageNumber)
        {
            var classExamsQuery = _context.ClassExams
                .Include(ce => ce.Class)
                .Include(ce => ce.Course)
                .Include(ce => ce.ExamPaper)
                .OrderByDescending(ce => ce.CreatedAt)
                .AsNoTracking();

            int pageSize = 10;
            return View(await PaginatedList<ClassExam>.CreateAsync(classExamsQuery, pageNumber ?? 1, pageSize));
        }


        public IActionResult Create()
        {
            ViewBag.ClassId = new SelectList(_context.Classes.OrderBy(c => c.ClassName), "Id", "ClassName");
            ViewBag.CourseId = new SelectList(_context.Courses.OrderBy(c => c.Title), "Id", "Title");
            ViewBag.ExamPaperId = new SelectList(_context.ExamPapers.OrderBy(p => p.Title), "Id", "Title");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ClassId,CourseId,ExamPaperId,ExamDate,DurationOverride,Status")] ClassExam classExam)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(classExam.Id)) classExam.Id = Guid.NewGuid().ToString();
                classExam.CreatedAt = DateTime.Now;
                _context.Add(classExam);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Class Exam assigned successfully.";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.ClassId = new SelectList(_context.Classes, "Id", "ClassName", classExam.ClassId);
            ViewBag.CourseId = new SelectList(_context.Courses, "Id", "Title", classExam.CourseId);
            ViewBag.ExamPaperId = new SelectList(_context.ExamPapers, "Id", "Title", classExam.ExamPaperId);
            return View(classExam);
        }

        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null) return NotFound();

            var classExam = await _context.ClassExams.FindAsync(id);
            if (classExam == null) return NotFound();

            ViewBag.ClassId = new SelectList(_context.Classes, "Id", "ClassName", classExam.ClassId);
            ViewBag.CourseId = new SelectList(_context.Courses, "Id", "Title", classExam.CourseId);
            ViewBag.ExamPaperId = new SelectList(_context.ExamPapers, "Id", "Title", classExam.ExamPaperId);
            return View(classExam);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,ClassId,CourseId,ExamPaperId,ExamDate,DurationOverride,Status,CreatedAt")] ClassExam classExam)
        {
            if (id != classExam.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(classExam);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Class Exam updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClassExamExists(classExam.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.ClassId = new SelectList(_context.Classes, "Id", "ClassName", classExam.ClassId);
            ViewBag.CourseId = new SelectList(_context.Courses, "Id", "Title", classExam.CourseId);
            ViewBag.ExamPaperId = new SelectList(_context.ExamPapers, "Id", "Title", classExam.ExamPaperId);
            return View(classExam);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var classExam = await _context.ClassExams.FindAsync(id);
            if (classExam != null)
            {
                // Check if any students have already started or finished this exam
                var hasSessions = await _context.StudentExamSessions.AnyAsync(s => s.ClassExamId == id);
                if (hasSessions)
                {
                    TempData["Error"] = "Cannot delete this exam assignment because students have already interacted with it. Please cancel the exam instead if needed.";
                    return RedirectToAction(nameof(Index));
                }

                _context.ClassExams.Remove(classExam);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Class Exam deleted successfully.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ClassExamExists(string id)
        {
            return _context.ClassExams.Any(e => e.Id == id);
        }

        // API for Dependent Dropdowns
        [HttpGet]
        public async Task<JsonResult> GetCourseByClass(string classId)
        {
            if (string.IsNullOrEmpty(classId)) return Json(null);

            // Get Course linked to this Class
            var classObj = await _context.Classes.Include(c => c.Course).FirstOrDefaultAsync(c => c.Id == classId);
            if (classObj == null || classObj.Course == null) return Json(null);

            return Json(new { id = classObj.CourseId, name = classObj.Course.Title });
        }

        [HttpGet]
        public async Task<JsonResult> GetExamPapersByCourse(string courseId)
        {
            if (string.IsNullOrEmpty(courseId)) return Json(new List<object>());

            // Get Exam Papers for this Course
            var papers = await _context.ExamPapers
                .Where(p => p.CourseId == courseId)
                .OrderBy(p => p.Title)
                .Select(p => new {
                    id = p.Id,
                    title = p.Title,
                    duration = p.Duration
                })
                .ToListAsync();

            return Json(papers);
        }

        // ======================
        // VIEW RESULTS
        // ======================
        public async Task<IActionResult> Results(string id)
        {
            if (id == null) return NotFound();

            var classExam = await _context.ClassExams
                .Include(ce => ce.Class)
                .Include(ce => ce.Course)
                .Include(ce => ce.ExamPaper)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (classExam == null) return NotFound();

            var sessions = await _context.StudentExamSessions
                .Include(s => s.Student)
                .Where(s => s.ClassExamId == id)
                .OrderByDescending(s => s.TotalScore)
                .ToListAsync();

            ViewBag.ClassExam = classExam;

            return View(sessions);
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
            return RedirectToRequestUrlOrIndex();
        }

        // ======================
        // PUBLISH ALL SCORES FOR CLASS (POST)
        // ======================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PublishAllScores(string classId)
        {
            var exams = await _context.ClassExams.Where(e => e.ClassId == classId).ToListAsync();
            if (!exams.Any()) return NotFound();

            foreach (var exam in exams)
            {
                exam.IsScorePublished = true;
            }
            await _context.SaveChangesAsync();

            TempData["Success"] = "All exam scores for this class have been published.";
            return RedirectToRequestUrlOrIndex();
        }

        private IActionResult RedirectToRequestUrlOrIndex()
        {
            string referer = Request.Headers["Referer"].ToString();
            if (!string.IsNullOrEmpty(referer))
            {
                return Redirect(referer);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
