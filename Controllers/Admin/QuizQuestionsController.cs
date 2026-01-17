using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Symphony.Portal.Web.Data;
using Symphony.Portal.Web.Models;

namespace Symphony.Portal.Web.Controllers.Admin
{
    [Area("Admin")]
    public class QuizQuestionsController : Controller
    {
        private readonly AppDbContext _context;

        public QuizQuestionsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Admin/QuizQuestions
        public async Task<IActionResult> Index(string quizId)
        {
            var questions = _context.QuizQuestions.Include(q => q.Quiz).AsQueryable();
            
            if (!string.IsNullOrEmpty(quizId))
            {
                questions = questions.Where(q => q.QuizId == quizId);
                ViewData["CurrentQuizId"] = quizId;
            }

            return View(await questions.ToListAsync());
        }

        // GET: Admin/QuizQuestions/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null) return NotFound();

            var question = await _context.QuizQuestions
                .Include(q => q.Quiz)
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (question == null) return NotFound();

            return View(question);
        }

        // GET: Admin/QuizQuestions/Create
        public IActionResult Create(string quizId)
        {
            if (quizId != null)
            {
                ViewData["QuizId"] = new SelectList(_context.Quizzes, "Id", "Name", quizId);
                ViewData["FixedQuizId"] = quizId;
            }
            else
            {
                 ViewData["QuizId"] = new SelectList(_context.Quizzes, "Id", "Name");
            }
           
            return View();
        }

        // POST: Admin/QuizQuestions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,QuizId,QuestionText,OptionA,OptionB,OptionC,OptionD,CorrectOption,Points")] QuizQuestion quizQuestion)
        {
            if (ModelState.IsValid)
            {
                _context.Add(quizQuestion);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Quiz Question created successfully.";
                return RedirectToAction("Details", "Quizzes", new { id = quizQuestion.QuizId });
            }
            ViewData["QuizId"] = new SelectList(_context.Quizzes, "Id", "Name", quizQuestion.QuizId);
            return View(quizQuestion);
        }

        // GET: Admin/QuizQuestions/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null) return NotFound();

            var quizQuestion = await _context.QuizQuestions.FindAsync(id);
            if (quizQuestion == null) return NotFound();
            
            ViewData["QuizId"] = new SelectList(_context.Quizzes, "Id", "Name", quizQuestion.QuizId);
            return View(quizQuestion);
        }

        // POST: Admin/QuizQuestions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,QuizId,QuestionText,OptionA,OptionB,OptionC,OptionD,CorrectOption,Points")] QuizQuestion quizQuestion)
        {
            if (id != quizQuestion.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(quizQuestion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QuizQuestionExists(quizQuestion.Id)) return NotFound();
                    else throw;
                }
                TempData["Success"] = "Quiz Question updated successfully.";
                return RedirectToAction("Details", "Quizzes", new { id = quizQuestion.QuizId });
            }
            ViewData["QuizId"] = new SelectList(_context.Quizzes, "Id", "Name", quizQuestion.QuizId);
            return View(quizQuestion);
        }

        // GET: Admin/QuizQuestions/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null) return NotFound();

            var quizQuestion = await _context.QuizQuestions
                .Include(q => q.Quiz)
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (quizQuestion == null) return NotFound();

            return View(quizQuestion);
        }

        // POST: Admin/QuizQuestions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var quizQuestion = await _context.QuizQuestions.FindAsync(id);
            string? quizId = null;
            if (quizQuestion != null)
            {
                quizId = quizQuestion.QuizId;
                _context.QuizQuestions.Remove(quizQuestion);
            }
            
            await _context.SaveChangesAsync();
            TempData["Success"] = "Quiz Question deleted successfully.";
            
            if (!string.IsNullOrEmpty(quizId))
                return RedirectToAction("Details", "Quizzes", new { id = quizId });
            return RedirectToAction(nameof(Index));
        }

        private bool QuizQuestionExists(string id)
        {
            return _context.QuizQuestions.Any(e => e.Id == id);
        }
    }
}
