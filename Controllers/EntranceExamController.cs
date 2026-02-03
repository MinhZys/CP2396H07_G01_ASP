using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Symphony.Portal.Web.Data;
using Symphony.Portal.Web.Models.Enums;
using Symphony.Portal.Web.Models;

namespace Symphony.Portal.Web.Controllers
{
    public class EntranceExamController : Controller
    {
        private readonly AppDbContext _context;

        public EntranceExamController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var activeExams = await _context.EntranceExams
                .Where(e => e.IsActive && e.IsRegistrationOpen)
                .OrderBy(e => e.ExamDate)
                .ToListAsync();
            return View(activeExams);
        }

        public async Task<IActionResult> TakeExam(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var userEmail = User.Identity?.Name;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (user == null) return RedirectToAction("Login", "Account");

            // Check if user has already taken this exam
            var existingResult = await _context.ExamResults
                .AnyAsync(r => r.StudentId == user.Id && r.EntranceExamId == id);

            if (existingResult)
            {
                TempData["ErrorMessage"] = "You have already completed this exam.";
                return RedirectToAction("Dashboard", "Guests");
            }

            var entranceExam = await _context.EntranceExams
                .Include(e => e.ExamPaper)
                .ThenInclude(p => p.ExamPaperQuestions)
                .ThenInclude(pq => pq.Question)
                .ThenInclude(q => q.Options)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (entranceExam == null || entranceExam.ExamPaper == null)
            {
                return NotFound();
            }

            // Time-based entry check (5-minute window)
            // Allow if within first 5 mins OR if they have already started (session flag)
            var now = DateTime.Now;
            var sessionKey = $"ExamStarted_{user.Id}_{id}";
            var alreadyStarted = HttpContext.Session.GetString(sessionKey) == "true";

            if (!alreadyStarted)
            {
                if (now < entranceExam.ExamDate)
                {
                    TempData["ErrorMessage"] = "The exam has not started yet.";
                    return RedirectToAction("Dashboard", "Guests");
                }
                
                if (now > entranceExam.ExamDate.AddMinutes(5))
                {
                    TempData["ErrorMessage"] = "The entry window for this exam is closed. You can only join within 5 minutes of the start time.";
                    return RedirectToAction("Dashboard", "Guests");
                }

                // First time entering, set flag
                HttpContext.Session.SetString(sessionKey, "true");
            }

            return View(entranceExam);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitExam(string id, Dictionary<string, string> answers)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == User.Identity.Name);
            if (user == null) return RedirectToAction("Login", "Account");

            // Prevent resubmission
            var existingResult = await _context.ExamResults
                .AnyAsync(r => r.StudentId == user.Id && r.EntranceExamId == id);

            if (existingResult)
            {
                TempData["ErrorMessage"] = "Exam already submitted.";
                return RedirectToAction("Dashboard", "Guests");
            }

            var entranceExam = await _context.EntranceExams
                .Include(e => e.ExamPaper)
                .ThenInclude(p => p.ExamPaperQuestions)
                .ThenInclude(pq => pq.Question)
                .ThenInclude(q => q.Options)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (entranceExam == null || entranceExam.ExamPaper == null) return NotFound();

            double totalScore = 0;
            double earnedScore = 0;

            foreach (var paperQuestion in entranceExam.ExamPaper.ExamPaperQuestions)
            {
                var question = paperQuestion.Question;
                totalScore += question.Score;

                if (answers.TryGetValue(question.Id, out var selectedOptionId))
                {
                    var correctOption = question.Options.FirstOrDefault(o => o.IsCorrect);
                    if (correctOption != null && correctOption.Id == selectedOptionId)
                    {
                        earnedScore += question.Score;
                    }
                }
            }

            // Determine Pass/Fail (e.g., 50% threshold)
            bool isPassed = earnedScore >= (totalScore * 0.5);

            var result = new ExamResult
            {
                Id = Guid.NewGuid().ToString(),
                StudentId = user.Id,
                EntranceExamId = id,
                Score = earnedScore,
                IsPassed = isPassed,
                ExamDate = DateTime.Now
            };

            _context.ExamResults.Add(result);
            await _context.SaveChangesAsync();

            // Redirect to Dashboard or specific Result page
            return RedirectToAction("Dashboard", "Guests");
        }
    }
}
