using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Symphony.Portal.Web.Data;
using Symphony.Portal.Web.Models;
using Symphony.Portal.Web.Models.Enums;
using System.Security.Claims;

namespace Symphony.Portal.Web.Controllers
{
    public class ExamSessionController : Controller
    {
        private readonly AppDbContext _context;

        public ExamSessionController(AppDbContext context)
        {
            _context = context;
        }

        // Student starts an exam
        public async Task<IActionResult> Start(string entranceExamId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var exam = await _context.EntranceExams
                .Include(e => e.ExamPapers)
                .FirstOrDefaultAsync(e => e.Id == entranceExamId);

            if (exam == null || exam.Status != ExamStatus.Ongoing)
            {
                TempData["Error"] = "Kỳ thi này hiện không diễn ra.";
                return RedirectToAction("Index", "Home");
            }

            // Check if student already has a session
            var existingSession = await _context.StudentExamSessions
                .FirstOrDefaultAsync(s => s.EntranceExamId == entranceExamId && s.StudentId == userId);

            if (existingSession != null)
            {
                if (existingSession.Status == ExamSessionStatus.Taking)
                {
                    return RedirectToAction(nameof(Take), new { sessionId = existingSession.Id });
                }
                TempData["Error"] = "Bạn đã hoàn thành bài thi này.";
                return RedirectToAction("MyExams", "ExamResult");
            }

            // Assign a random paper from the exam
            var papers = exam.ExamPapers.ToList();
            if (!papers.Any())
            {
                TempData["Error"] = "Kỳ thi này chưa có đề thi.";
                return RedirectToAction("Index", "Home");
            }

            var random = new Random();
            var paper = papers[random.Next(papers.Count)];

            var session = new StudentExamSession
            {
                EntranceExamId = entranceExamId,
                StudentId = userId,
                ExamPaperId = paper.Id,
                StartTime = DateTime.Now,
                Status = ExamSessionStatus.Taking
            };

            _context.Add(session);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Take), new { sessionId = session.Id });
        }

        public async Task<IActionResult> Take(string sessionId)
        {
            var session = await _context.StudentExamSessions
                .Include(s => s.ExamPaper)
                .ThenInclude(p => p!.ExamPaperQuestions)
                .ThenInclude(epq => epq.Question)
                .ThenInclude(q => q!.Options)
                .FirstOrDefaultAsync(s => s.Id == sessionId);

            if (session == null || session.Status != ExamSessionStatus.Taking)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(session);
        }

        [HttpPost]
        public async Task<IActionResult> Submit(string sessionId, Dictionary<string, string> answers)
        {
            var session = await _context.StudentExamSessions
                .Include(s => s.ExamPaper)
                .ThenInclude(p => p!.ExamPaperQuestions)
                .ThenInclude(epq => epq.Question)
                .FirstOrDefaultAsync(s => s.Id == sessionId);

            if (session == null || session.Status != ExamSessionStatus.Taking) return NotFound();

            foreach (var epq in session.ExamPaper!.ExamPaperQuestions)
            {
                var question = epq.Question;
                if (question == null) continue;

                var studentAnswer = new StudentAnswer
                {
                    SessionId = session.Id,
                    QuestionId = question.Id
                };

                if (answers.TryGetValue(question.Id, out var answerValue))
                {
                    if (question.Type == QuestionType.MultipleChoice)
                    {
                        studentAnswer.SelectedOptionId = answerValue;
                    }
                    else
                    {
                        studentAnswer.EssayContent = answerValue;
                    }
                }

                _context.Add(studentAnswer);
            }

            session.EndTime = DateTime.Now;
            session.Status = ExamSessionStatus.Finished;
            await _context.SaveChangesAsync();

            // Note: Auto-grading can be triggered here or by Admin later.
            TempData["Success"] = "Bạn đã hoàn thành bài thi. Kết quả sẽ sớm được cập nhật.";
            return RedirectToAction("MyExams", "ExamResult");
        }
    }
}
