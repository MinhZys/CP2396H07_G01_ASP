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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userEmail = User.Identity?.Name;

            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == userId || u.Email == userEmail);

            if (user == null)
            {
                // If user is authenticated but not in DB, something is wrong
                return RedirectToAction("Login", "Account");
            }

            var model = new ProfileVM
            {
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role?.Name ?? "User",
                IsInstructor = user.Role?.Name == RoleNames.Instructor,
                IsGuest = user.Role?.Name == RoleNames.Guest
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
            else if (model.IsGuest)
            {
                var guest = await _context.Guests.FirstOrDefaultAsync(g => g.UserId == userId || g.Email == user.Email);
                if (guest != null)
                {
                    model.DateOfBirth = guest.Dob;
                    model.PhoneNumber = guest.PhoneNumber;
                    model.AddressLine = guest.Address;
                }
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
        public async Task<IActionResult> Index([FromForm] ProfileVM model)
        {
            // Remove AvatarUrl from validation as it's handled via file upload or existing value
            ModelState.Remove("AvatarUrl");

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == userId);
            
            if (user == null) return NotFound();

            bool isInstructor = user.Role?.Name == RoleNames.Instructor;
            bool isGuest = user.Role?.Name == RoleNames.Guest;

            if (!isInstructor && !isGuest)
            {
                ModelState.Remove("GithubUrl");
                ModelState.Remove("Specialization");
                ModelState.Remove("Bio");
                ModelState.Remove("YearsOfExperience");
                ModelState.Remove("Certifications");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (userId != model.UserId) return Forbid(); // Simple check

            // Handle Avatar Upload
            if (model.AvatarImage != null)
            {
                string uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "avatars");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(model.AvatarImage.FileName);
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

            else if (isGuest)
            {
                var guest = await _context.Guests.FirstOrDefaultAsync(g => g.UserId == userId || g.Email == user.Email);
                if (guest != null)
                {
                    guest.FullName = model.FullName;
                    if (model.DateOfBirth.HasValue) guest.Dob = model.DateOfBirth.Value;
                    guest.PhoneNumber = model.PhoneNumber;
                    guest.Address = model.AddressLine;
                    
                    _context.Guests.Update(guest);
                }
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
                if (!string.IsNullOrEmpty(model.AvatarUrl))
                {
                    profile.AvatarUrl = model.AvatarUrl;
                }

                _context.StudentProfiles.Update(profile);
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Cập nhật hồ sơ thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}
