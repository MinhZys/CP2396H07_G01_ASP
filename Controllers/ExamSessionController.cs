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

        // Student starts a final exam for a class/subject
        public async Task<IActionResult> StartFinal(string classExamId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var classExam = await _context.ClassExams
                .Include(ce => ce.ExamPaper)
                .FirstOrDefaultAsync(ce => ce.Id == classExamId);

            if (classExam == null) return NotFound();

            // Auto-start if time has arrived
            if (classExam.Status == ClassExamStatus.Scheduled && classExam.ExamDate <= DateTime.Now)
            {
                classExam.Status = ClassExamStatus.InProgress;
                _context.Update(classExam);
                await _context.SaveChangesAsync();
            }

            if (classExam.Status != ClassExamStatus.InProgress)
            {
                TempData["Error"] = "Kỳ thi này hiện không diễn ra hoặc chưa bắt đầu.";
                return RedirectToAction("Index", "Home");
            }

            // Check if student already has a session for this specific class exam
            var existingSession = await _context.StudentExamSessions
                .FirstOrDefaultAsync(s => s.ClassExamId == classExamId && s.StudentId == userId);

            if (existingSession != null)
            {
                if (existingSession.Status == ExamSessionStatus.Taking)
                {
                    return RedirectToAction(nameof(Take), new { sessionId = existingSession.Id });
                }
                TempData["Error"] = "Bạn đã hoàn thành bài thi này.";
                return RedirectToAction("MyExams", "ExamResult");
            }

            var session = new StudentExamSession
            {
                ClassExamId = classExamId,
                StudentId = userId,
                ExamPaperId = classExam.ExamPaperId,
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
            
            // Auto-grading for Multiple Choice questions
            double score = 0;
            foreach (var answer in session.Answers)
            {
                var question = await _context.Questions.Include(q => q.Options).FirstOrDefaultAsync(q => q.Id == answer.QuestionId);
                if (question != null && question.Type == QuestionType.MultipleChoice)
                {
                    var correctOption = question.Options?.FirstOrDefault(o => o.IsCorrect);
                    if (correctOption != null && answer.SelectedOptionId == correctOption.Id)
                    {
                        answer.EarnedScore = 1.0; // Assume 1 point per question for now, or use complex logic
                        score += 1.0;
                    }
                    answer.IsGraded = true;
                }
            }
            session.TotalScore = score;

            await _context.SaveChangesAsync();

            // Notify user
            TempData["Success"] = "Bạn đã hoàn thành bài thi. Vui lòng chờ giảng viên công bố kết quả.";

            // Contextual Redirect: If it's a class exam, redirect back to the Class Exams view
            if (!string.IsNullOrEmpty(session.ClassExamId))
            {
                var classExam = await _context.ClassExams.FindAsync(session.ClassExamId);
                if (classExam != null)
                {
                    return RedirectToAction("ViewExams", "Student", new { classId = classExam.ClassId });
                }
            }

            // Default redirect (for Entrance Exams)
            return RedirectToAction("MyExams", "ExamResult");
        }
    }
}
