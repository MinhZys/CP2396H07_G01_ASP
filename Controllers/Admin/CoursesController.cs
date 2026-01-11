using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Symphony.Portal.Web.Data;
using Symphony.Portal.Web.Models;

namespace Symphony.Portal.Web.Controllers.Admin
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    [Route("Admin/[controller]/[action]")]
    public class CoursesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public CoursesController(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: Admin/Courses
        public async Task<IActionResult> Index()
        {
            return View(await _context.Courses.ToListAsync());
        }

        // GET: Admin/Courses/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Subjects = await _context.Subjects.ToListAsync();
            return View();
        }

        // POST: Admin/Courses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Course course, string[] selectedSubjectIds, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(course.Id)) course.Id = Guid.NewGuid().ToString();

                // Handle Image Upload
                if (imageFile != null && imageFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "courses");
                    if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(fileStream);
                    }
                    course.Image = "/images/courses/" + uniqueFileName;
                }

                // Add selected subjects
                if (selectedSubjectIds != null)
                {
                    foreach (var subjectId in selectedSubjectIds)
                    {
                        course.CourseSubjects.Add(new CourseSubject { CourseId = course.Id, SubjectId = subjectId });
                    }
                }

                _context.Add(course);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Subjects = await _context.Subjects.ToListAsync();
            return View(course);
        }

        // GET: Admin/Courses/Edit/5
        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null) return NotFound();

            var course = await _context.Courses
                .Include(c => c.CourseSubjects)
                .FirstOrDefaultAsync(m => m.Id == id);
                
            if (course == null) return NotFound();

            ViewBag.Subjects = await _context.Subjects.ToListAsync();
            ViewBag.SelectedSubjectIds = course.CourseSubjects.Select(cs => cs.SubjectId).ToList();

            return View(course);
        }

        // POST: Admin/Courses/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Course course, string[] selectedSubjectIds, IFormFile? imageFile)
        {
            if (id != course.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Handle Image Upload
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "courses");
                        if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(fileStream);
                        }
                        course.Image = "/images/courses/" + uniqueFileName;
                    }
                    else
                    {
                        // Keep existing image if no new file is uploaded
                        var existingCourse = await _context.Courses.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
                        if (existingCourse != null)
                        {
                            course.Image = existingCourse.Image;
                        }
                    }

                    // Update basic properties
                    _context.Update(course);
                    
                    // Update Subjects: Drop existing and add new
                    // Update Subjects: specific add/remove
                    var currentSubjects = await _context.CourseSubjects
                        .Where(cs => cs.CourseId == id)
                        .ToListAsync();

                    var selectedIds = selectedSubjectIds?.ToList() ?? new List<string>();
                    var currentIds = currentSubjects.Select(cs => cs.SubjectId).ToList();

                    // Remove unselected
                    var toRemove = currentSubjects.Where(cs => !selectedIds.Contains(cs.SubjectId));
                    _context.CourseSubjects.RemoveRange(toRemove);

                    // Add new
                    var toAdd = selectedIds.Where(id => !currentIds.Contains(id));
                    foreach (var subjectId in toAdd)
                    {
                        _context.CourseSubjects.Add(new CourseSubject { CourseId = id, SubjectId = subjectId });
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseExists(course.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
             ViewBag.Subjects = await _context.Subjects.ToListAsync();
             ViewBag.SelectedSubjectIds = selectedSubjectIds.ToList();
            return View(course);
        }

         // GET: Admin/Courses/Delete/5
        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null) return NotFound();

            var course = await _context.Courses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (course == null) return NotFound();

            return View(course);
        }

        // POST: Admin/Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course != null)
            {
                _context.Courses.Remove(course);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> SearchSubjects(string? term)
        {
            var query = _context.Subjects.AsQueryable();

            if (!string.IsNullOrEmpty(term))
            {
                query = query.Where(s => s.Name.Contains(term));
            }

            var subjects = await query
                .Select(s => new { s.Id, s.Name, s.StudyTime })
                .Take(10)
                .ToListAsync();

            return Json(subjects);
        }

        private bool CourseExists(string id)
        {
            return _context.Courses.Any(e => e.Id == id);
        }
    }
}
