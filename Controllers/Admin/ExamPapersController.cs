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
    [Route("Admin/[controller]/[action]")]
    public class ExamPapersController : Controller
    {
        private readonly AppDbContext _context;

        public ExamPapersController(AppDbContext context)
        {
            _context = context;
        }

        // --- Question Management ---

        public async Task<IActionResult> Questions(string? subjectId, string? search)
        {
            var query = _context.Questions.Include(q => q.Subject).AsQueryable();

            if (!string.IsNullOrEmpty(subjectId))
            {
                query = query.Where(q => q.SubjectId == subjectId);
            }

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(q => q.Content.Contains(search));
            }

            ViewBag.Subjects = await _context.Subjects.ToListAsync();
            return View(await query.ToListAsync());
        }

        public IActionResult CreateQuestion()
        {
            ViewBag.Subjects = _context.Subjects.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateQuestions(string SubjectId, List<QuestionDto> questions)
        {
            if (questions == null || !questions.Any())
            {
                TempData["Error"] = "Vui lòng thêm ít nhất 1 câu hỏi.";
                return RedirectToAction(nameof(CreateQuestion));
            }

            if (string.IsNullOrEmpty(SubjectId))
            {
                TempData["Error"] = "Vui lòng chọn môn học.";
                return RedirectToAction(nameof(CreateQuestion));
            }

            try
            {
                foreach (var qDto in questions)
                {
                    var question = new Question
                    {
                        Content = qDto.Content,
                        Type = QuestionType.MultipleChoice,
                        SubjectId = SubjectId,
                        Score = qDto.Score,
                        Difficulty = "Trung bình" // Default value
                    };

                    _context.Add(question);

                    // Create 4 options
                    for (int i = 0; i < 4; i++)
                    {
                        if (i < qDto.Options?.Count)
                        {
                            var option = new QuestionOption
                            {
                                Id = Guid.NewGuid().ToString(),
                                QuestionId = question.Id,
                                Content = qDto.Options[i],
                                IsCorrect = (i == qDto.CorrectIndex)
                            };
                            _context.Add(option);
                        }
                    }
                }

                await _context.SaveChangesAsync();
                TempData["Success"] = $"Đã thêm {questions.Count} câu hỏi thành công!";
                return RedirectToAction(nameof(Questions));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Có lỗi xảy ra: " + ex.Message;
                return RedirectToAction(nameof(CreateQuestion));
            }
        }

        // DTO for binding multiple questions
        public class QuestionDto
        {
            public string Content { get; set; } = string.Empty;
            public double Score { get; set; } = 1.0;
            public List<string> Options { get; set; } = new List<string>();
            public int CorrectIndex { get; set; }
        }

        // --- Exam Paper Management ---

        public async Task<IActionResult> Index()
        {
            var papers = await _context.ExamPapers
                .Include(p => p.Subject)
                .Include(p => p.ExamPaperQuestions)
                .ToListAsync();
            return View(papers);
        }

        public async Task<IActionResult> CreatePaper()
        {
            ViewBag.Subjects = await _context.Subjects.ToListAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreatePaperWithQuestions(string Title, string SubjectId, int Duration, List<QuestionDto> questions)
        {
            if (string.IsNullOrEmpty(Title) || string.IsNullOrEmpty(SubjectId))
            {
                TempData["Error"] = "Vui lòng điền đầy đủ thông tin đề thi.";
                return RedirectToAction(nameof(CreatePaper));
            }

            if (questions == null || !questions.Any())
            {
                TempData["Error"] = "Vui lòng thêm ít nhất 1 câu hỏi cho đề thi.";
                return RedirectToAction(nameof(CreatePaper));
            }

            try
            {
                // Create ExamPaper
                var examPaper = new ExamPaper
                {
                    Title = Title,
                    SubjectId = SubjectId,
                    Duration = Duration
                };
                _context.Add(examPaper);

                int order = 1;
                foreach (var qDto in questions)
                {
                    // Create Question
                    var question = new Question
                    {
                        Content = qDto.Content,
                        Type = QuestionType.MultipleChoice,
                        SubjectId = SubjectId,
                        Score = qDto.Score,
                        Difficulty = "Trung bình"
                    };
                    _context.Add(question);

                    // Create 4 Options
                    for (int i = 0; i < 4; i++)
                    {
                        if (i < qDto.Options?.Count)
                        {
                            var option = new QuestionOption
                            {
                                Id = Guid.NewGuid().ToString(),
                                QuestionId = question.Id,
                                Content = qDto.Options[i],
                                IsCorrect = (i == qDto.CorrectIndex)
                            };
                            _context.Add(option);
                        }
                    }

                    // Link Question to ExamPaper
                    _context.Add(new ExamPaperQuestion
                    {
                        ExamPaperId = examPaper.Id,
                        QuestionId = question.Id,
                        Order = order++
                    });
                }

                await _context.SaveChangesAsync();
                TempData["Success"] = $"Đã tạo đề thi '{Title}' với {questions.Count} câu hỏi thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Có lỗi xảy ra: " + ex.Message;
                return RedirectToAction(nameof(CreatePaper));
            }
        }

        public async Task<IActionResult> Details(string id)
        {
            var paper = await _context.ExamPapers
                .Include(p => p.Subject)
                .Include(p => p.ExamPaperQuestions)
                .ThenInclude(epq => epq.Question)
                .ThenInclude(q => q.Options)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (paper == null) return NotFound();
            return View(paper);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var paper = await _context.ExamPapers
                .Include(p => p.ExamPaperQuestions)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (paper == null) return NotFound();

            bool isInUse = await _context.StudentExamSessions.AnyAsync(s => s.ExamPaperId == id);
            if (isInUse)
            {
                TempData["Error"] = "Đề thi này đang được sử dụng trong các kỳ thi đã hoặc đang tổ chức, không thể xóa.";
                return RedirectToAction(nameof(Index));
            }

            _context.ExamPaperQuestions.RemoveRange(paper.ExamPaperQuestions);
            _context.ExamPapers.Remove(paper);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Đề thi đã được xóa thành công.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> EditPaper(string id)
        {
            var paper = await _context.ExamPapers
                .Include(p => p.ExamPaperQuestions)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (paper == null) return NotFound();

            // Check if paper is in use
            bool isInUse = await _context.StudentExamSessions.AnyAsync(s => s.ExamPaperId == id);
            if (isInUse)
            {
                TempData["Error"] = "Đề thi này đang được sử dụng trong các kỳ thi đã hoặc đang tổ chức, không thể chỉnh sửa.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Subjects = await _context.Subjects.ToListAsync();
            ViewBag.Questions = await _context.Questions.Include(q => q.Subject).ToListAsync();
            ViewBag.SelectedQuestionIds = paper.ExamPaperQuestions.Select(epq => epq.QuestionId).ToList();
            return View(paper);
        }

        [HttpPost]
        public async Task<IActionResult> EditPaper(string id, ExamPaper paper, string[] selectedQuestionIds)
        {
            if (id != paper.Id) return NotFound();

            // Check if paper is in use
            bool isInUse = await _context.StudentExamSessions.AnyAsync(s => s.ExamPaperId == id);
            if (isInUse)
            {
                TempData["Error"] = "Đề thi này đang được sử dụng trong các kỳ thi đã hoặc đang tổ chức, không thể chỉnh sửa.";
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                var existingPaper = await _context.ExamPapers
                    .Include(p => p.ExamPaperQuestions)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (existingPaper == null) return NotFound();

                // Update basic properties
                existingPaper.Title = paper.Title;
                existingPaper.Duration = paper.Duration;
                existingPaper.SubjectId = paper.SubjectId;

                // Remove old questions
                _context.ExamPaperQuestions.RemoveRange(existingPaper.ExamPaperQuestions);

                // Add new questions
                int order = 1;
                foreach (var qId in selectedQuestionIds ?? Array.Empty<string>())
                {
                    _context.Add(new ExamPaperQuestion
                    {
                        ExamPaperId = existingPaper.Id,
                        QuestionId = qId,
                        Order = order++
                    });
                }

                await _context.SaveChangesAsync();
                TempData["Success"] = "Đề thi đã được cập nhật thành công.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Subjects = await _context.Subjects.ToListAsync();
            ViewBag.Questions = await _context.Questions.ToListAsync();
            ViewBag.SelectedQuestionIds = selectedQuestionIds?.ToList() ?? new List<string>();
            return View(paper);
        }

        // --- Edit Question ---

        public async Task<IActionResult> EditQuestion(string id)
        {
            var question = await _context.Questions
                .Include(q => q.Options)
                .Include(q => q.Subject)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (question == null) return NotFound();

            ViewBag.Subjects = await _context.Subjects.ToListAsync();
            return View(question);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditQuestion(string id, Question question, List<QuestionOption> options)
        {
            if (id != question.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var existingQuestion = await _context.Questions
                    .Include(q => q.Options)
                    .FirstOrDefaultAsync(q => q.Id == id);

                if (existingQuestion == null) return NotFound();

                // Update question properties
                existingQuestion.Content = question.Content;
                existingQuestion.Type = question.Type;
                existingQuestion.SubjectId = question.SubjectId;
                existingQuestion.Difficulty = question.Difficulty;
                existingQuestion.Score = question.Score;

                // Remove old options
                _context.QuestionOptions.RemoveRange(existingQuestion.Options);

                // Add new options
                if (question.Type == QuestionType.MultipleChoice && options != null)
                {
                    foreach (var option in options)
                    {
                        option.Id = Guid.NewGuid().ToString();
                        option.QuestionId = existingQuestion.Id;
                        _context.Add(option);
                    }
                }

                await _context.SaveChangesAsync();
                TempData["Success"] = "Câu hỏi đã được cập nhật thành công.";
                return RedirectToAction(nameof(Questions));
            }

            ViewBag.Subjects = await _context.Subjects.ToListAsync();
            return View(question);
        }

        // --- Delete Question ---

        [HttpPost]
        public async Task<IActionResult> DeleteQuestion(string id)
        {
            var question = await _context.Questions
                .Include(q => q.Options)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (question == null) return NotFound();

            // Check if question is used in any exam paper
            bool isInUse = await _context.ExamPaperQuestions.AnyAsync(epq => epq.QuestionId == id);
            if (isInUse)
            {
                TempData["Error"] = "Câu hỏi này đang được sử dụng trong các đề thi, không thể xóa.";
                return RedirectToAction(nameof(Questions));
            }

            _context.QuestionOptions.RemoveRange(question.Options);
            _context.Questions.Remove(question);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Câu hỏi đã được xóa thành công.";
            return RedirectToAction(nameof(Questions));
        }
    }
}
