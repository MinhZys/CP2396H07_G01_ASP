using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Symphony.Portal.Web.Data;
using Symphony.Portal.Web.Models;
using Symphony.Portal.Web.Models.Enums;

namespace Symphony.Portal.Web.Controllers.Instructor
{
    [Area("Instructor")]
    [Authorize(Roles = "Instructor,Admin")]
    [Route("Instructor/[controller]/[action]")]
    public class GradingController : Controller
    {
        private readonly AppDbContext _context;

        public GradingController(AppDbContext context)
        {
            _context = context;
        }

        // List of sessions waiting for grading
        public async Task<IActionResult> Index()
        {
            var sessions = await _context.StudentExamSessions
                .Include(s => s.Student)
                .Include(s => s.EntranceExam)
                .Include(s => s.ExamPaper)
                .OrderByDescending(s => s.EndTime)
                .ToListAsync();
            return View(sessions);
        }

        // View for grading a specific session
        public async Task<IActionResult> GradeSession(string id)
        {
            var session = await _context.StudentExamSessions
                .Include(s => s.Student)
                .Include(s => s.ExamPaper)
                .Include(s => s.Answers)
                .ThenInclude(a => a.Question)
                .ThenInclude(q => q!.Options)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (session == null) return NotFound();

            // Run auto-grading for MCQ if not already done
            await AutoGradeMCQuestions(session);

            return View(session);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitGrade(string sessionId, Dictionary<int, double> scores, Dictionary<int, string> notes)
        {
            var session = await _context.StudentExamSessions
                .Include(s => s.Answers)
                .FirstOrDefaultAsync(s => s.Id == sessionId);

            if (session == null) return NotFound();

            double totalScore = 0;

            foreach (var answer in session.Answers)
            {
                if (answer.Id != 0 && scores.ContainsKey(answer.Id))
                {
                    answer.EarnedScore = scores[answer.Id];
                    answer.IsGraded = true;
                }
                
                if (answer.Id != 0 && notes.ContainsKey(answer.Id))
                {
                    answer.ExaminerNote = notes[answer.Id];
                }

                totalScore += answer.EarnedScore;
            }

            session.TotalScore = totalScore;
            session.GradeLevel = CalculateLevel(totalScore);
            session.Status = ExamSessionStatus.Graded;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private async Task AutoGradeMCQuestions(StudentExamSession session)
        {
            bool changed = false;
            foreach (var answer in session.Answers.Where(a => !a.IsGraded))
            {
                var question = answer.Question;
                if (question != null && question.Type == QuestionType.MultipleChoice)
                {
                    var correctOption = question.Options.FirstOrDefault(o => o.IsCorrect);
                    if (correctOption != null && answer.SelectedOptionId == correctOption.Id)
                    {
                        answer.EarnedScore = question.Score;
                    }
                    else
                    {
                        answer.EarnedScore = 0;
                    }
                    answer.IsGraded = true;
                    changed = true;
                }
            }

            if (changed)
            {
                await _context.SaveChangesAsync();
            }
        }

        private string CalculateLevel(double score)
        {
            if (score >= 70) return "Level A";
            if (score >= 40) return "Level B";
            return "Level C";
        }
    }
}
