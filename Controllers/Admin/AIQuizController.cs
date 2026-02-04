using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Symphony.Portal.Web.Data;
using Symphony.Portal.Web.Models;
using Symphony.Portal.Web.Models.ViewModels;
using Symphony.Portal.Web.Services;

namespace Symphony.Portal.Web.Controllers.Admin
{
    [Authorize(Roles = RoleNames.Admin)]
    [Area("Admin")]
    public class AIQuizController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IOllamaService _ollamaService;
        private readonly ILogger<AIQuizController> _logger;

        public AIQuizController(AppDbContext context, IOllamaService ollamaService, ILogger<AIQuizController> logger)
        {
            _context = context;
            _ollamaService = ollamaService;
            _logger = logger;
        }

        /// <summary>
        /// Main page for AI Quiz Generator
        /// </summary>
        public async Task<IActionResult> Index()
        {
            // Check if Ollama is available
            var isAIAvailable = await _ollamaService.IsAvailableAsync();
            ViewBag.IsAIAvailable = isAIAvailable;

            // Get subjects for dropdown
            var subjects = await _context.Subjects
                .OrderBy(s => s.Name)
                .Select(s => new SelectListItem
                {
                    Value = s.Id,
                    Text = s.Name
                })
                .ToListAsync();

            ViewBag.Subjects = subjects;

            return View();
        }

        /// <summary>
        /// Generate questions using AI
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Generate([FromBody] QuizGenerationRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Content))
                {
                    return Json(new { success = false, error = "Vui lòng nhập nội dung bài học" });
                }

                if (request.QuestionCount < 1 || request.QuestionCount > 20)
                {
                    return Json(new { success = false, error = "Số câu hỏi phải từ 1 đến 20" });
                }

                var result = await _ollamaService.GenerateQuestionsAsync(
                    request.Content,
                    request.QuestionCount,
                    request.Difficulty
                );

                if (!result.Success)
                {
                    return Json(new { success = false, error = result.Error });
                }

                // Store subject ID in temp for saving later
                TempData["SubjectId"] = request.SubjectId;

                return Json(new
                {
                    success = true,
                    questions = result.Questions,
                    subjectId = request.SubjectId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating questions");
                return Json(new { success = false, error = "Đã xảy ra lỗi khi tạo câu hỏi" });
            }
        }

        /// <summary>
        /// Save generated questions to database
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveQuestions([FromBody] SaveQuestionsRequest request)
        {
            try
            {
                if (request.Questions == null || !request.Questions.Any())
                {
                    return Json(new { success = false, error = "Không có câu hỏi để lưu" });
                }

                var savedCount = 0;

                foreach (var q in request.Questions)
                {
                    if (string.IsNullOrWhiteSpace(q.Content)) continue;

                    var question = new Question
                    {
                        Id = Guid.NewGuid().ToString(),
                        Content = q.Content,
                        SubjectId = request.SubjectId,
                        Difficulty = q.Difficulty,
                        Type = Models.Enums.QuestionType.MultipleChoice,
                        Score = 1.0,
                        CreatedAt = DateTime.Now
                    };

                    // Add options
                    foreach (var opt in q.Options)
                    {
                        question.Options.Add(new QuestionOption
                        {
                            Id = Guid.NewGuid().ToString(),
                            QuestionId = question.Id,
                            Content = opt.Content,
                            IsCorrect = opt.IsCorrect
                        });
                    }

                    _context.Questions.Add(question);
                    savedCount++;
                }

                await _context.SaveChangesAsync();

                return Json(new
                {
                    success = true,
                    message = $"Đã lưu thành công {savedCount} câu hỏi",
                    count = savedCount
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving questions");
                return Json(new { success = false, error = "Đã xảy ra lỗi khi lưu câu hỏi" });
            }
        }

        /// <summary>
        /// Check AI service status
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> CheckStatus()
        {
            var isAvailable = await _ollamaService.IsAvailableAsync();
            return Json(new { available = isAvailable });
        }
    }

    public class SaveQuestionsRequest
    {
        public string SubjectId { get; set; } = string.Empty;
        public List<GeneratedQuestion> Questions { get; set; } = new();
    }
}
