using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Symphony.Portal.Web.Data;
using Symphony.Portal.Web.Models;
using Symphony.Portal.Web.Models.ViewModels;
using System.Security.Claims;

namespace Symphony.Portal.Web.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public ProfileController(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return NotFound();

            var model = new ProfileVM
            {
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role?.Name ?? "User",
                IsInstructor = user.Role?.Name == RoleNames.Instructor
            };

            if (model.IsInstructor)
            {
                var profile = await _context.InstructorProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
                if (profile == null)
                {
                    // Auto-create if missing
                    profile = new InstructorProfile { UserId = userId, FullName = user.FullName };
                    _context.InstructorProfiles.Add(profile);
                    await _context.SaveChangesAsync();
                }

                // Map to VM
                model.DateOfBirth = profile.DateOfBirth;
                model.Gender = profile.Gender;
                model.PhoneNumber = profile.PhoneNumber;
                model.AddressLine = profile.AddressLine;
                model.AvatarUrl = profile.AvatarUrl;
                model.YearsOfExperience = profile.YearsOfExperience;
                model.Specialization = profile.Specialization;
                model.Bio = profile.Bio;
                model.Certifications = profile.Certifications;
                model.GithubUrl = profile.GithubUrl;
            }
            else
            {
                // Assumed Student
                var profile = await _context.StudentProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
                if (profile == null)
                {
                    // Auto-create if missing
                    profile = new StudentProfile { UserId = userId, FullName = user.FullName };
                    _context.StudentProfiles.Add(profile);
                    await _context.SaveChangesAsync();
                }

                // Map to VM
                model.DateOfBirth = profile.DateOfBirth;
                model.Gender = profile.Gender;
                model.PhoneNumber = profile.PhoneNumber;
                model.AddressLine = profile.AddressLine;
                model.AvatarUrl = profile.AvatarUrl;
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ProfileVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId != model.UserId) return Forbid(); // Simple check

            var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return NotFound();

            bool isInstructor = user.Role?.Name == RoleNames.Instructor;

            // Handle Avatar Upload
            if (model.AvatarImage != null)
            {
                string uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "avatars");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.AvatarImage.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.AvatarImage.CopyToAsync(fileStream);
                }

                model.AvatarUrl = "/uploads/avatars/" + uniqueFileName;
            }

            // Update User Table (FullName)
            if (user.FullName != model.FullName)
            {
                user.FullName = model.FullName;
            }

            if (isInstructor)
            {
                var profile = await _context.InstructorProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
                if (profile == null) return NotFound();

                profile.FullName = model.FullName;
                profile.DateOfBirth = model.DateOfBirth;
                profile.Gender = model.Gender;
                profile.PhoneNumber = model.PhoneNumber;
                profile.AddressLine = model.AddressLine;
                profile.AvatarUrl = model.AvatarUrl;
                profile.YearsOfExperience = model.YearsOfExperience;
                profile.Specialization = model.Specialization;
                profile.Bio = model.Bio;
                profile.Certifications = model.Certifications;
                profile.GithubUrl = model.GithubUrl;
                
                _context.InstructorProfiles.Update(profile);
            }
            else
            {
                var profile = await _context.StudentProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
                if (profile == null) return NotFound();

                profile.FullName = model.FullName;
                profile.DateOfBirth = model.DateOfBirth;
                profile.Gender = model.Gender;
                profile.PhoneNumber = model.PhoneNumber;
                profile.AddressLine = model.AddressLine;
                profile.AvatarUrl = model.AvatarUrl;

                _context.StudentProfiles.Update(profile);
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Cập nhật hồ sơ thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}
