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

        public ExamResultsController(AppDbContext context)
        {
            _context = context;
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
    }
}
