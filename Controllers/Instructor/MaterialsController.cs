using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Symphony.Portal.Web.Data;
using Symphony.Portal.Web.Models;
using System.Security.Claims;

namespace Symphony.Portal.Web.Controllers.Instructor
{
    [Area("Instructor")]
    [Authorize(Roles = "Instructor")]
    [Route("Instructor/[controller]/[action]")]
    public class MaterialsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public MaterialsController(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public async Task<IActionResult> Index(string? classId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var query = _context.Materials
                .Include(m => m.Class)
                .Where(m => m.Class.InstructorId == userId);

            if (!string.IsNullOrEmpty(classId))
            {
                query = query.Where(m => m.ClassId == classId);
                ViewData["SelectedClassId"] = classId;
            }

            // Populate class dropdown for filter/upload
            var myClasses = await _context.Classes
                .Where(c => c.InstructorId == userId)
                .Select(c => new { c.Id, Name = $"{c.Name} ({c.Course.Title})" })
                .ToListAsync();
            
            ViewBag.Classes = new SelectList(myClasses, "Id", "Name", classId);

            var materials = await query.OrderByDescending(m => m.UploadDate).ToListAsync();
            return View(materials);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(Material material, IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                // Validate file type/size if needed
                string uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "materials");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                material.Id = Guid.NewGuid().ToString();
                material.FilePath = "/uploads/materials/" + uniqueFileName;
                material.FileType = Path.GetExtension(file.FileName).ToUpper().Replace(".", "");
                material.UploadDate = DateTime.Now;

                // Explicitly valid if ClassId is present
                if(!string.IsNullOrEmpty(material.ClassId))
                {
                    _context.Add(material);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index), new { classId = material.ClassId });
                }
            }
             
            // If failed, return to Index with error (simplified for now)
            return RedirectToAction(nameof(Index)); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var material = await _context.Materials.FindAsync(id);
            if (material != null)
            {
                // Verify ownership via Class -> Instructor
                var isOwner = await _context.Classes.AnyAsync(c => c.Id == material.ClassId && c.InstructorId == User.FindFirstValue(ClaimTypes.NameIdentifier));
                
                if(isOwner)
                {
                    // Optional: Delete physical file
                    try
                    {
                        string webRootPath = _environment.WebRootPath;
                        string fullPath = Path.Combine(webRootPath, material.FilePath.TrimStart('/'));
                        if (System.IO.File.Exists(fullPath))
                        {
                            System.IO.File.Delete(fullPath);
                        }
                    }
                    catch { /* Log error */ }

                    _context.Materials.Remove(material);
                    await _context.SaveChangesAsync();
                }
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
