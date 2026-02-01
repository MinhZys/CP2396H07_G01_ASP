using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Symphony.Portal.Web.Data;
using Symphony.Portal.Web.Models;
using Symphony.Portal.Web.Models.Enums;

namespace CP2396H07_G01.Controllers.Admin
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    [Route("Admin/[controller]/[action]")]
    public class EntranceExamsController : Controller
    {
        private readonly AppDbContext _context;

        public EntranceExamsController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.EntranceExams.ToListAsync());
        }

        public IActionResult Create()
        {
            ViewBag.ExamPapers = _context.ExamPapers.Select(p => new { p.Id, p.Title }).ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EntranceExam entranceExam)
        {
            if (ModelState.IsValid)
            {
                entranceExam.Id = Guid.NewGuid().ToString();
                await _context.SaveChangesAsync();
                TempData["Success"] = "Entrance exam created successfully!";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.ExamPapers = _context.ExamPapers.Select(p => new { p.Id, p.Title }).ToList();
            return View(entranceExam);
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (id == null) return NotFound();

            var entranceExam = await _context.EntranceExams.FindAsync(id);
            if (entranceExam == null) return NotFound();
            
            ViewBag.ExamPapers = _context.ExamPapers.Select(p => new { p.Id, p.Title }).ToList();
            return View(entranceExam);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, EntranceExam entranceExam)
        {
            if (id != entranceExam.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Entrance exam updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EntranceExamExists(entranceExam.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.ExamPapers = _context.ExamPapers.Select(p => new { p.Id, p.Title }).ToList();
            return View(entranceExam);
        }


        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var entranceExam = await _context.EntranceExams.FindAsync(id);
            if (entranceExam != null)
            {
                if (entranceExam.IsActive || entranceExam.IsRegistrationOpen)
                {
                    TempData["Error"] = "Cannot delete an active or open entrance exam!";
                    return RedirectToAction(nameof(Index));
                }

                _context.EntranceExams.Remove(entranceExam);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Entrance exam deleted successfully!";
            }
            else
            {
                TempData["Error"] = "Entrance exam not found.";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool EntranceExamExists(string id)
        {
            return _context.EntranceExams.Any(e => e.Id == id);
        }
    }
}
