using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Symphony.Portal.Web.Data;
using Symphony.Portal.Web.Models;
using Symphony.Portal.Web.Models.ViewModels;

namespace Symphony.Portal.Web.Controllers.Admin
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class InstructorProfilesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public InstructorProfilesController(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public async Task<IActionResult> Index()
        {
            var instructors = await _context.Users
                .Where(u => u.Role.Name == "Instructor")
                .ToListAsync();

            var instructorIds = instructors.Select(u => u.Id).ToList();
            var profiles = await _context.InstructorProfiles
                .Where(p => instructorIds.Contains(p.UserId))
                .ToListAsync();

            var modelList = new List<ProfileVM>();

            foreach (var inst in instructors)
            {
                var profile = profiles.FirstOrDefault(p => p.UserId == inst.Id);
                modelList.Add(new ProfileVM
                {
                    UserId = inst.Id,
                    FullName = inst.FullName,
                    Email = inst.Email,
                    Role = "Instructor",
                    IsInstructor = true,
                    // Profile fields
                    DateOfBirth = profile?.DateOfBirth,
                    Gender = profile?.Gender ?? "",
                    PhoneNumber = profile?.PhoneNumber ?? "",
                    AddressLine = profile?.AddressLine ?? "",
                    AvatarUrl = profile?.AvatarUrl ?? "",
                    YearsOfExperience = profile?.YearsOfExperience ?? 0,
                    Specialization = profile?.Specialization ?? "",
                    Bio = profile?.Bio ?? "",
                    Certifications = profile?.Certifications ?? "",
                    GithubUrl = profile?.GithubUrl ?? ""
                });
            }

            return View(modelList);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            var profile = await _context.InstructorProfiles.FirstOrDefaultAsync(p => p.UserId == id);
            
            // Auto-init
            if (profile == null)
            {
                profile = new InstructorProfile { UserId = id, FullName = user.FullName };
                _context.InstructorProfiles.Add(profile);
                await _context.SaveChangesAsync();
            }

            var model = new ProfileVM
            {
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = "Instructor",
                IsInstructor = true,
                DateOfBirth = profile.DateOfBirth,
                Gender = profile.Gender,
                PhoneNumber = profile.PhoneNumber,
                AddressLine = profile.AddressLine,
                AvatarUrl = profile.AvatarUrl,
                YearsOfExperience = profile.YearsOfExperience,
                Specialization = profile.Specialization,
                Bio = profile.Bio,
                Certifications = profile.Certifications,
                GithubUrl = profile.GithubUrl
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProfileVM model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _context.Users.FindAsync(model.UserId);
            if (user == null) return NotFound();

            var profile = await _context.InstructorProfiles.FirstOrDefaultAsync(p => p.UserId == model.UserId);
            if (profile == null) return NotFound();

            if (model.AvatarImage != null)
            {
                string uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "avatars");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.AvatarImage.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.AvatarImage.CopyToAsync(fileStream);
                }

                model.AvatarUrl = "/uploads/avatars/" + uniqueFileName;
                profile.AvatarUrl = model.AvatarUrl;
            }

            user.FullName = model.FullName;
            profile.FullName = model.FullName;
            profile.DateOfBirth = model.DateOfBirth;
            profile.Gender = model.Gender;
            profile.PhoneNumber = model.PhoneNumber;
            profile.AddressLine = model.AddressLine;
            // Instructor specific
            profile.YearsOfExperience = model.YearsOfExperience;
            profile.Specialization = model.Specialization;
            profile.Bio = model.Bio;
            profile.Certifications = model.Certifications;
            profile.GithubUrl = model.GithubUrl;

            _context.Users.Update(user);
            _context.InstructorProfiles.Update(profile);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Instructor Profile updated successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
