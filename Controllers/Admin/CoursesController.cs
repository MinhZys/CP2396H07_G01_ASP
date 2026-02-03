using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Symphony.Portal.Web.Data;
using Symphony.Portal.Web.Models;
using Symphony.Portal.Web.Models.Enums;
using Symphony.Portal.Web.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;


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
        public async Task<IActionResult> Index(string searchString, string categoryId, CourseLevel? level, int? pageNumber)
        {
             var query = _context.Courses
                .Include(c => c.Category)
                .Include(c => c.Certificate)
                .Include(c => c.CourseSubjects)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(c => c.Title.Contains(searchString));
            }

            if (!string.IsNullOrEmpty(categoryId))
            {
                query = query.Where(c => c.CategoryId == categoryId);
            }

            if (level.HasValue)
            {
                query = query.Where(c => c.Level == level);
            }

            // Populate filter lists
            ViewData["Categories"] = new SelectList(_context.Categories, "Id", "Name", categoryId);
            
            // Pass current filter values back to view
            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentCategory"] = categoryId;
            ViewData["CurrentLevel"] = level;

            int pageSize = 10;
            return View(await PaginatedList<Course>.CreateAsync(query.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: Admin/Courses/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Subjects = await _context.Subjects.ToListAsync();
            
             // Fetch Instructors
            ViewBag.Instructors = await _context.Users
                .Where(u => u.Role.Name == RoleNames.Instructor)
                .Select(u => new { u.Id, u.FullName })
                .ToListAsync();

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            ViewData["CertificateId"] = new SelectList(_context.Certificates, "Id", "Name");
            return View();
        }

        // POST: Admin/Courses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,TuitionFee,DurationMonths,Level,IsActive,CategoryId,CertificateId")] Course course, string[] selectedSubjectIds, string[] selectedInstructorIds, IFormFile? imageFile)
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

                // Add selected instructors
                if (selectedInstructorIds != null)
                {
                    foreach (var instructorId in selectedInstructorIds)
                    {
                        course.CourseInstructors.Add(new CourseInstructor { CourseId = course.Id, InstructorId = instructorId });
                    }
                }

                // Calculate Duration from selected subjects
                if (selectedSubjectIds != null && selectedSubjectIds.Length > 0)
                {
                     var totalHours = await _context.Subjects
                        .Where(s => selectedSubjectIds.Contains(s.Id))
                        .SumAsync(s => s.StudyTime);
                     course.DurationMonths = totalHours;
                }

                _context.Add(course);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Course created successfully.";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Subjects = await _context.Subjects.ToListAsync();
            
             // Fetch Instructors
            ViewBag.Instructors = await _context.Users
                .Where(u => u.Role.Name == RoleNames.Instructor)
                .Select(u => new { u.Id, u.FullName })
                .ToListAsync();

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", course.CategoryId);
            ViewData["CertificateId"] = new SelectList(_context.Certificates, "Id", "Name", course.CertificateId);
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

             // Fetch Instructors
            ViewBag.Instructors = await _context.Users
                .Where(u => u.Role.Name == RoleNames.Instructor)
                .Select(u => new { u.Id, u.FullName })
                .ToListAsync();
            ViewBag.SelectedInstructorIds = await _context.CourseInstructors
                .Where(ci => ci.CourseId == id)
                .Select(ci => ci.InstructorId)
                .ToListAsync();

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", course.CategoryId);
            ViewData["CertificateId"] = new SelectList(_context.Certificates, "Id", "Name", course.CertificateId);

            return View(course);
        }

        // POST: Admin/Courses/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Title,Description,TuitionFee,DurationMonths,Level,IsActive,CategoryId,CertificateId")] Course course, string[] selectedSubjectIds, string[] selectedInstructorIds, IFormFile? imageFile)
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

                    // Update Instructors
                    var currentInstructors = await _context.CourseInstructors
                        .Where(ci => ci.CourseId == id)
                        .ToListAsync();
                    
                    var selectedInstIds = selectedInstructorIds?.ToList() ?? new List<string>();
                    var currentInstIds = currentInstructors.Select(ci => ci.InstructorId).ToList();

                    // Remove unselected
                    var instToRemove = currentInstructors.Where(ci => !selectedInstIds.Contains(ci.InstructorId));
                    _context.CourseInstructors.RemoveRange(instToRemove);

                    // Add new
                    var instToAdd = selectedInstIds.Where(id => !currentInstIds.Contains(id));
                    foreach (var instId in instToAdd)
                    {
                        _context.CourseInstructors.Add(new CourseInstructor { CourseId = id, InstructorId = instId });
                    }

                    // Calculate Duration from selected subjects (All current subjects after update)
                    if (selectedSubjectIds != null && selectedSubjectIds.Length > 0)
                    {
                         var totalHours = await _context.Subjects
                            .Where(s => selectedSubjectIds.Contains(s.Id))
                            .SumAsync(s => s.StudyTime);
                         course.DurationMonths = totalHours;
                         // We need to verify if EF Core tracks this change to 'course' object since we attached it via Update(course).
                         // Since 'course' is the entity being updated, setting the property should be enough, 
                         // but we called _context.Update(course) BEFORE this change. 
                         // To be safe, let's just modify the property. EF Core's ChangeTracker should detect it before SaveChangesAsync,
                         // mostly because 'course' instance is tracked. 
                         // HOWEVER, _context.Update(course) marks all properties as modified. 
                         // Changing it afterwards *should* work if the entity is tracked.
                         // Let's force update the property to be sure, or just re-issue update? 
                         // Simpler: Just set it and trust EF Check, or just move Update(course) later if possible?
                         // Moving Update later is risky with the relations logic above.
                         // Safest: Set it, and if needed, mark property as modified.
                         _context.Entry(course).Property(u => u.DurationMonths).IsModified = true;
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseExists(course.Id)) return NotFound();
                    else throw;
                }
                TempData["Success"] = "Course updated successfully.";
                return RedirectToAction(nameof(Index));
            }
             ViewBag.Subjects = await _context.Subjects.ToListAsync();
             ViewBag.SelectedSubjectIds = selectedSubjectIds.ToList();

              // Fetch Instructors
            ViewBag.Instructors = await _context.Users
                .Where(u => u.Role.Name == RoleNames.Instructor)
                .Select(u => new { u.Id, u.FullName })
                .ToListAsync();
            ViewBag.SelectedInstructorIds = selectedInstructorIds.ToList();

             ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", course.CategoryId);
             ViewData["CertificateId"] = new SelectList(_context.Certificates, "Id", "Name", course.CertificateId);
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
                // Check dependencies
                if (await _context.StudentRegistrations.AnyAsync(r => r.CourseId == id))
                {
                     TempData["Error"] = "Cannot delete this course because students have already registered for it.";
                     return RedirectToAction(nameof(Index));
                }

                if (await _context.Enrollments.AnyAsync(e => e.CourseId == id))
                {
                     TempData["Error"] = "Cannot delete this course because students are already enrolled in it.";
                     return RedirectToAction(nameof(Index));
                }

                // Optional: Check if used in CourseSubjects / CourseInstructors?
                // Usually these are composition parts of a course, so deleting the course implies deleting them.
                // However, user asked for "strict" constraints. 
                // But these are junction tables owned by the course conceptually.
                // Let's stick to "User Data" constraints (Registrations, Enrollments) as the primary blocker.
                // Deleting a course with defined subjects is usually intended if no one has bought it yet.
                
                // However, to be super safe/strict as requested:
                // We will rely on EF Core Cascade Delete for the Junction tables (CourseSubjects, CourseInstructors),
                // BUT we BLOCK if there is "Business Data" (Registrations, Enrollments).

                _context.Courses.Remove(course);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Course deleted successfully.";
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
