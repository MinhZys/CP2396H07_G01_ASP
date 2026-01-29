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

            // In a real scenario, we would check if the user is registered and allowed to take the exam here.
            
            return View(entranceExam);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitExam(string id, Dictionary<string, string> answers)
        {
            var userId = _context.Users.FirstOrDefault(u => u.Email == User.Identity.Name)?.Id;
            if (userId == null) return RedirectToAction("Login", "Account");

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
                StudentId = userId,
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
