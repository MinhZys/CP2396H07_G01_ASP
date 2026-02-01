using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Symphony.Portal.Web.Data;
using Symphony.Portal.Web.Models;
using System.Linq;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;

namespace Symphony.Portal.Web.Controllers.Admin
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class SubjectsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public SubjectsController(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: Admin/Subjects
        public async Task<IActionResult> Index(string searchString)
        {
            var subjects = from s in _context.Subjects
                           select s;

            if (!string.IsNullOrEmpty(searchString))
            {
                subjects = subjects.Where(s => s.Name.Contains(searchString));
            }

            ViewData["CurrentFilter"] = searchString;
            return View(await subjects.ToListAsync());
        }

        // GET: Admin/Subjects/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Subjects/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,StudyTime,Description")] Subject subject, IFormFile? imageFile, string[] roadmapSteps)
        {
            // Auto-generate ID
            subject.Id = Guid.NewGuid().ToString();

            // Clear validation for Id since it's not bound
            ModelState.Remove("Id");

            if (ModelState.IsValid)
            {
                // Check collision just in case (very rare with GUID)
                if (SubjectExists(subject.Id))
                {
                    ModelState.AddModelError("Id", "Subject ID already exists.");
                    return View(subject);
                }

                // Handle Roadmap Steps
                var stepsList = roadmapSteps.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
                if (!stepsList.Any())
                {
                     ModelState.AddModelError("LearningRoadmap", "The Learning Roadmap must have at least one step.");
                     return View(subject);
                }
                subject.LearningRoadmap = JsonSerializer.Serialize(stepsList);

                // Handle Image Upload
                if (imageFile != null && imageFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "subjects");
                    if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(fileStream);
                    }
                    subject.Image = "/images/subjects/" + uniqueFileName;
                }

                _context.Add(subject);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Subject created successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(subject);
        }

        // GET: Admin/Subjects/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subject = await _context.Subjects.FindAsync(id);
            if (subject == null)
            {
                return NotFound();
            }
            return View(subject);
        }

        // POST: Admin/Subjects/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Name,StudyTime,Description")] Subject subject, IFormFile? imageFile, string[] roadmapSteps)
        {
            if (id != subject.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Handle Roadmap Steps
                    var stepsList = roadmapSteps.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
                    if (!stepsList.Any())
                    {
                        ModelState.AddModelError("LearningRoadmap", "The Learning Roadmap must have at least one step.");
                        // Keep current image to avoid losing it on error
                        var existing = await _context.Subjects.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
                        if (existing != null) subject.Image = existing.Image;
                        return View(subject);
                    }
                    subject.LearningRoadmap = JsonSerializer.Serialize(stepsList);

                     // Handle Image Upload
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "subjects");
                        if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(fileStream);
                        }
                        subject.Image = "/images/subjects/" + uniqueFileName;
                    }
                    else
                    {
                         // Keep existing image if no new file is uploaded
                        var existingSubject = await _context.Subjects.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
                        if (existingSubject != null)
                        {
                            subject.Image = existingSubject.Image;
                        }
                    }

                    _context.Update(subject);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubjectExists(subject.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["Success"] = "Subject updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(subject);
        }

        // GET: Admin/Subjects/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subject = await _context.Subjects
                .FirstOrDefaultAsync(m => m.Id == id);
            if (subject == null)
            {
                return NotFound();
            }

            return View(subject);
        }

        // POST: Admin/Subjects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var subject = await _context.Subjects.FindAsync(id);
            if (subject != null)
            {
                if (await _context.CourseSubjects.AnyAsync(cs => cs.SubjectId == id))
                {
                     TempData["Error"] = "Cannot delete this subject because it is linked to one or more courses.";
                     return RedirectToAction(nameof(Index));
                }

                _context.Subjects.Remove(subject);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Subject deleted successfully.";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool SubjectExists(string id)
        {
            return _context.Subjects.Any(e => e.Id == id);
        }
    }
}
