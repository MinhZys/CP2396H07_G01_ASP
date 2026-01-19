using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Symphony.Portal.Web.Controllers
{
    public class MentorsController : Controller
    {
        private readonly Data.AppDbContext _context;

        public MentorsController(Data.AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // 1. Get all users with Instructor Role
            var instructorRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == Models.RoleNames.Instructor);
            if(instructorRole == null) return View(new List<Models.InstructorProfile>());

            var instructors = await _context.Users
                .Where(u => u.RoleId == instructorRole.Id && u.IsActive)
                .ToListAsync();

            var instructorIds = instructors.Select(u => u.Id).ToList();

            // 2. Get existing profiles
            var profiles = await _context.InstructorProfiles
                .Include(p => p.User)
                .Where(p => instructorIds.Contains(p.UserId))
                .ToListAsync();

            // 3. Create view model list (combining existing profiles with bare users who don't have profiles yet)
            var displayList = new List<Models.InstructorProfile>();

            foreach(var inst in instructors)
            {
                var profile = profiles.FirstOrDefault(p => p.UserId == inst.Id);
                if(profile != null)
                {
                    displayList.Add(profile);
                }
                else
                {
                    // Create dummy profile for display
                    displayList.Add(new Models.InstructorProfile
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserId = inst.Id,
                        User = inst,
                        FullName = inst.FullName,
                        Specialization = "Instructor", 
                        Bio = "Experienced IT Instructor",
                        YearsOfExperience = 0
                    });
                }
            }

            return View(displayList);
        }

        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            // Try to find profile first
            var profile = await _context.InstructorProfiles
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == id || p.UserId == id); // Allow lookup by ProfileId or UserId

            if (profile == null)
            {
                // Fallback: Check if it's a valid instructor user without a profile
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
                if (user != null)
                {
                    // Create temporary profile for display
                    profile = new Models.InstructorProfile
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserId = user.Id,
                        User = user,
                        FullName = user.FullName,
                        Specialization = "Instructor",
                        Bio = "Experienced IT Instructor",
                        YearsOfExperience = 0
                    };
                }
                else
                {
                    return NotFound();
                }
            }

            return View(profile);
        }
    }
}
