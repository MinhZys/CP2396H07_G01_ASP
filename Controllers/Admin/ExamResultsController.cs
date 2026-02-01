using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Symphony.Portal.Web.Data;
using Symphony.Portal.Web.Models;

namespace Symphony.Portal.Web.Controllers.Admin
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ExamResultsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly Symphony.Portal.Web.Services.EmailService _emailService;

        public ExamResultsController(AppDbContext context, Symphony.Portal.Web.Services.EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // GET: Admin/ExamResults
        public async Task<IActionResult> Index(string examId)
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

            return View(await query.OrderByDescending(r => r.ExamDate).ToListAsync());
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
    }
}
