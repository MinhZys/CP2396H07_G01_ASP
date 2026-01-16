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
    public class StudentProfilesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public StudentProfilesController(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public async Task<IActionResult> Index()
        {
            var students = await _context.Users
                .Where(u => u.Role.Name == "Student")
                .ToListAsync();

            var studentIds = students.Select(u => u.Id).ToList();
            var profiles = await _context.StudentProfiles
                .Where(p => studentIds.Contains(p.UserId))
                .ToListAsync();

            var modelList = new List<ProfileVM>();

            foreach (var student in students)
            {
                var profile = profiles.FirstOrDefault(p => p.UserId == student.Id);
                modelList.Add(new ProfileVM
                {
                    UserId = student.Id,
                    FullName = student.FullName,
                    Email = student.Email,
                    Role = "Student",
                    // Profile fields
                    DateOfBirth = profile?.DateOfBirth,
                    Gender = profile?.Gender ?? "",
                    PhoneNumber = profile?.PhoneNumber ?? "",
                    AddressLine = profile?.AddressLine ?? "",
                    AvatarUrl = profile?.AvatarUrl ?? "",
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

            var profile = await _context.StudentProfiles.FirstOrDefaultAsync(p => p.UserId == id);
            
            // Auto-init if missing so Admin can edit
            if (profile == null)
            {
                profile = new StudentProfile { UserId = id, FullName = user.FullName };
                _context.StudentProfiles.Add(profile);
                await _context.SaveChangesAsync();
            }

            var model = new ProfileVM
            {
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = "Student",
                DateOfBirth = profile.DateOfBirth,
                Gender = profile.Gender,
                PhoneNumber = profile.PhoneNumber,
                AddressLine = profile.AddressLine,
                AvatarUrl = profile.AvatarUrl
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

            var profile = await _context.StudentProfiles.FirstOrDefaultAsync(p => p.UserId == model.UserId);
            if (profile == null) return NotFound();

            // Support Avatar Update for Admin?
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

            // Update Profile fields
            user.FullName = model.FullName; // Sync name
            profile.FullName = model.FullName;
            profile.DateOfBirth = model.DateOfBirth;
            profile.Gender = model.Gender;
            profile.PhoneNumber = model.PhoneNumber;
            profile.AddressLine = model.AddressLine;

            _context.Users.Update(user);
            _context.StudentProfiles.Update(profile);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
