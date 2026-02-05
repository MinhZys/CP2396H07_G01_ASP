using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Symphony.Portal.Web.Data;
using Symphony.Portal.Web.Models;
using Symphony.Portal.Web.Models.ViewModels;

namespace Symphony.Portal.Web.Controllers.Admin
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ExamResultsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly Symphony.Portal.Web.Services.EmailService _emailService;
        private readonly Symphony.Portal.Web.Services.INotificationService _notificationService;

        public ExamResultsController(
            AppDbContext context, 
            Symphony.Portal.Web.Services.EmailService emailService,
            Symphony.Portal.Web.Services.INotificationService notificationService)
        {
            _context = context;
            _emailService = emailService;
            _notificationService = notificationService;
        }

        // GET: Admin/ExamResults
        public async Task<IActionResult> Index(string examId, int? pageNumber)
        {
            var query = _context.ExamResults
                .Include(r => r.Student)
                .Include(r => r.EntranceExam)
                .AsQueryable();

            if (!string.IsNullOrEmpty(examId))
            {
                query = query.Where(r => r.EntranceExamId == examId);
            }

            ViewBag.EntranceExams = await _context.EntranceExams.ToListAsync();
            ViewBag.SelectedExamId = examId;

            int pageSize = 10;
            return View(await PaginatedList<ExamResult>.CreateAsync(
                query.OrderByDescending(r => r.ExamDate).AsNoTracking(), 
                pageNumber ?? 1, 
                pageSize));
        }

        // GET: Admin/ExamResults/FinalExams
        public async Task<IActionResult> FinalExams(string classId, int? pageNumber)
        {
            var query = _context.StudentExamSessions
                .Include(s => s.Student)
                .Include(s => s.ClassExam)
                .ThenInclude(ce => ce!.Class)
                .Include(s => s.ClassExam)
                .ThenInclude(ce => ce!.Course)
                .Include(s => s.ExamPaper)
                .Where(s => s.ClassExamId != null) // Only final exams
                .AsQueryable();

            if (!string.IsNullOrEmpty(classId))
            {
                query = query.Where(s => s.ClassExam!.ClassId == classId);
            }

            ViewBag.Classes = await _context.Classes.OrderBy(c => c.ClassName).ToListAsync();
            ViewBag.SelectedClassId = classId;

            int pageSize = 10;
            return View(await PaginatedList<StudentExamSession>.CreateAsync(
                query.OrderByDescending(s => s.StartTime).AsNoTracking(),
                pageNumber ?? 1,
                pageSize));
        }

        [HttpPost]
        public async Task<IActionResult> SendResultEmail(string id)
        {
            var result = await _context.ExamResults
                .Include(r => r.Student)
                .Include(r => r.EntranceExam)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (result == null)
            {
                return Json(new { success = false, message = "Result not found." });
            }

            if (result.Student == null || string.IsNullOrEmpty(result.Student.Email))
            {
                return Json(new { success = false, message = "Student email not found." });
            }

            string subject = $"Exam Result: {result.EntranceExam?.Title}";
            string status = result.IsPassed ? "PASSED" : "FAILED";
            string body = $@"
Hello {result.Student.FullName},

Here is your exam result details:

Exam: {result.EntranceExam?.Title}
Date: {result.ExamDate:dd/MM/yyyy HH:mm}
Score: {result.Score}
Result: {status}

Best regards,
Symphony Portal Team";

            try
            {
                await _emailService.SendEmailAsync(result.Student.Email, subject, body);
                return Json(new { success = true, message = "Email sent successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error sending email: {ex.Message}" });
            }
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
            return RedirectToRequestUrlOrAction(nameof(FinalExams));
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
            return RedirectToRequestUrlOrAction(nameof(FinalExams));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NotifyCertificate(string sessionId)
        {
            var session = await _context.StudentExamSessions
                .Include(s => s.Student)
                .Include(s => s.ClassExam)
                .ThenInclude(ce => ce!.Course)
                .Include(s => s.ExamPaper)
                .FirstOrDefaultAsync(s => s.Id == sessionId);

            if (session == null || session.ClassExam == null || session.ClassExam.Course == null)
            {
                TempData["Error"] = "Session or Course not found.";
                return RedirectToRequestUrlOrAction(nameof(FinalExams));
            }

            var studentId = session.StudentId;
            var courseId = session.ClassExam.CourseId;
            var course = session.ClassExam.Course;

            // 1. Check if passed
            if (session.TotalScore < course.PassingScore)
            {
                TempData["Error"] = "Student did not reach the passing score.";
                return RedirectToRequestUrlOrAction(nameof(FinalExams));
            }

            // 2. Issue Certificate if not already issued
            var existingCert = await _context.StudentCertificates
                .FirstOrDefaultAsync(sc => sc.StudentId == studentId && sc.CourseId == courseId);

            if (existingCert == null)
            {
                var cert = new StudentCertificate
                {
                    StudentId = studentId,
                    CourseId = courseId,
                    CertificateId = course.CertificateId,
                    AverageScore = session.TotalScore,
                    IssueDate = DateTime.Now,
                    CertificateCode = "CERT-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper()
                };
                _context.StudentCertificates.Add(cert);

                // Mark enrollment as completed
                var enrollment = await _context.Enrollments
                    .FirstOrDefaultAsync(e => e.ClassId == session.ClassExam.ClassId && e.StudentId == studentId);
                if (enrollment != null)
                {
                    enrollment.IsCompleted = true;
                    _context.Update(enrollment);
                }
            }

            // 3. Send Notification
            string title = "Certificate Earned!";
            string message = $"Congratulations! You have successfully completed the exam for '{course.Title}' and earned your certificate. Please contact Symphony Center to receive your official hard-copy certificate.";
            
            await _notificationService.CreateNotificationAsync(studentId, title, message);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Certificate notification sent to {session.Student?.FullName} successfully!";
            return RedirectToRequestUrlOrAction(nameof(FinalExams));
        }

        private IActionResult RedirectToRequestUrlOrAction(string actionName)
        {
            string referer = Request.Headers["Referer"].ToString();
            if (!string.IsNullOrEmpty(referer))
            {
                return Redirect(referer);
            }
            return RedirectToAction(actionName);
        }
    }
}
