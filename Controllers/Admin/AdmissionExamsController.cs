using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Symphony.Portal.Web.Data;
using Symphony.Portal.Web.Models;

namespace Symphony.Portal.Web.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [Route("Admin/[controller]/[action]")]
    public class AdmissionExamsController : Controller
    {
        private readonly AppDbContext _context;

        public AdmissionExamsController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.AdmissionExams.OrderByDescending(e => e.ExamDate).ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdmissionExam admissionExam)
        {
            if (ModelState.IsValid)
            {
                _context.Add(admissionExam);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(admissionExam);
        }

        // Edit, Delete can be added similarly
         // GET: Admin/AdmissionExams/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var exam = await _context.AdmissionExams.FindAsync(id);
            if (exam == null) return NotFound();

            return View(exam);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AdmissionExam admissionExam)
        {
            if (id != admissionExam.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(admissionExam);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExamExists(admissionExam.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(admissionExam);
        }

        private bool ExamExists(int id)
        {
            return _context.AdmissionExams.Any(e => e.Id == id);
        }
    }
}
