using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Symphony.Portal.Web.Data;
using Symphony.Portal.Web.Models;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Symphony.Portal.Web.Controllers.Instructor
{
    [Area("Instructor")]
    public class LessonsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public LessonsController(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: Admin/Lessons
        public async Task<IActionResult> Index(string searchString, string courseId, string subjectId, string classId)
        {
            var query = _context.Lessons
                .Include(l => l.Course)
                .Include(l => l.Subject)
                .Include(l => l.Class)  // Include the Class in the query
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(l => l.Title.Contains(searchString) || l.Description.Contains(searchString));
            }

            if (!string.IsNullOrEmpty(courseId))
            {
                query = query.Where(l => l.CourseId == courseId);
            }

            if (!string.IsNullOrEmpty(subjectId))
            {
                query = query.Where(l => l.SubjectId == subjectId);
            }

            if (!string.IsNullOrEmpty(classId))
            {
                query = query.Where(l => l.ClassId == classId);
            }

            ViewData["Courses"] = new SelectList(await _context.Courses.ToListAsync(), "Id", "Title", courseId);
            ViewData["Subjects"] = new SelectList(await _context.Subjects.ToListAsync(), "Id", "Name", subjectId);
            ViewData["Classes"] = new SelectList(await _context.Classes.ToListAsync(), "Id", "ClassName", classId);  // Add Class filter
            ViewData["CurrentFilter"] = searchString;

            return View(await query.ToListAsync());
        }

        // GET: Admin/Lessons/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null) return NotFound();

            var lesson = await _context.Lessons
                .Include(l => l.Course)
                .Include(l => l.Subject)
                .Include(l => l.Class)  // Include Class in details view
                .FirstOrDefaultAsync(m => m.Id == id);

            if (lesson == null) return NotFound();

            return View(lesson);
        }

        // GET: Admin/Lessons/Create
        public IActionResult Create()
        {
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Title");
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Name");
            ViewData["ClassId"] = new SelectList(_context.Classes, "Id", "ClassName");  // Add Class selection
            return View();
        }

        // POST: Admin/Lessons/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Lesson lesson, IFormFile? imageFile, IFormFile? contentFile)
        {
            if (ModelState.IsValid)
            {
                lesson.Id = Guid.NewGuid().ToString();

                // Handle Image Upload
                if (imageFile != null && imageFile.Length > 0)
                {
                    lesson.Image = await UploadFile(imageFile, "images/lessons/thumbs");
                }

                // Handle Content File Upload
                if (contentFile != null && contentFile.Length > 0)
                {
                    lesson.ContentLink = await UploadFile(contentFile, "uploads/lessons/files");
                }

                _context.Add(lesson);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Title", lesson.CourseId);
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Name", lesson.SubjectId);
            ViewData["ClassId"] = new SelectList(_context.Classes, "Id", "ClassName", lesson.ClassId);  // Add ClassId for the form
            return View(lesson);
        }

        // GET: Admin/Lessons/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null) return NotFound();

            var lesson = await _context.Lessons.FindAsync(id);
            if (lesson == null) return NotFound();

            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Title", lesson.CourseId);
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Name", lesson.SubjectId);
            ViewData["ClassId"] = new SelectList(_context.Classes, "Id", "ClassName", lesson.ClassId);  // Add ClassId for the form
            return View(lesson);
        }

        // POST: Admin/Lessons/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Lesson lesson, IFormFile? imageFile, IFormFile? contentFile)
        {
            if (id != lesson.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existingLesson = await _context.Lessons.AsNoTracking().FirstOrDefaultAsync(l => l.Id == id);

                    // Handle Image Upload (Replace if new one provided)
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        lesson.Image = await UploadFile(imageFile, "images/lessons/thumbs");
                    }
                    else
                    {
                        lesson.Image = existingLesson?.Image; // Keep existing
                    }

                    // Handle Content File Upload (Replace if new one provided)
                    if (contentFile != null && contentFile.Length > 0)
                    {
                        lesson.ContentLink = await UploadFile(contentFile, "uploads/lessons/files");
                    }
                    else
                    {
                        lesson.ContentLink = existingLesson?.ContentLink ?? ""; // Keep existing
                    }

                    _context.Update(lesson);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LessonExists(lesson.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Title", lesson.CourseId);
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Name", lesson.SubjectId);
            ViewData["ClassId"] = new SelectList(_context.Classes, "Id", "ClassName", lesson.ClassId);  // Add ClassId for the form
            return View(lesson);
        }

        // GET: Admin/Lessons/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null) return NotFound();

            var lesson = await _context.Lessons
                .Include(l => l.Course)
                .Include(l => l.Subject)
                .Include(l => l.Class)  // Include Class for Delete view
                .FirstOrDefaultAsync(m => m.Id == id);

            if (lesson == null) return NotFound();

            return View(lesson);
        }

        // POST: Admin/Lessons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var lesson = await _context.Lessons.FindAsync(id);
            if (lesson != null)
            {
                _context.Lessons.Remove(lesson);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool LessonExists(string id)
        {
            return _context.Lessons.Any(e => e.Id == id);
        }

        private async Task<string> UploadFile(IFormFile file, string folder)
        {
            string uploadsFolder = Path.Combine(_environment.WebRootPath, folder);
            if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

            string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return "/" + folder + "/" + uniqueFileName;
        }
    }
}
