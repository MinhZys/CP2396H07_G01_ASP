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
    public class UsersController : Controller
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchString, string statusFilter)
        {
            var usersQuery = _context.Users.Include(u => u.Role).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                usersQuery = usersQuery.Where(u => u.Email.Contains(searchString) || u.FullName.Contains(searchString));
            }

            if (!string.IsNullOrEmpty(statusFilter))
            {
                if (statusFilter == "active")
                {
                    usersQuery = usersQuery.Where(u => u.IsActive);
                }
                else if (statusFilter == "inactive")
                {
                    usersQuery = usersQuery.Where(u => !u.IsActive);
                }
            }

            var users = await usersQuery.ToListAsync();
            ViewData["CurrentFilter"] = searchString;
            ViewData["StatusFilter"] = statusFilter;
            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Roles = await _context.Roles.ToListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(User user, string password)
        {
            if (ModelState.IsValid)
            {
                if (await _context.Users.AnyAsync(u => u.Email == user.Email)) {
                    ModelState.AddModelError("Email", "Email đã tồn tại.");
                    ViewBag.Roles = await _context.Roles.ToListAsync();
                    return View(user);
                }

                // If RoleId is invalid or not selected, default to Student
                if (string.IsNullOrEmpty(user.RoleId)) user.RoleId = "3"; // Default Student ID if seeded

                // Update ID if empty
                if (string.IsNullOrEmpty(user.Id)) user.Id = Guid.NewGuid().ToString();

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                
                TempData["Success"] = "User created successfully.";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Roles = await _context.Roles.ToListAsync();
            return View(user);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null) return NotFound();
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();
            
            ViewBag.Roles = await _context.Roles.ToListAsync();
            return View(user);
        }

        [HttpPost("{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, User user)
        {
            if (id != user.Id) return NotFound();
            
            // Password is not edited here, remove from validation
            ModelState.Remove("Password");

            if (ModelState.IsValid)
            {
                try
                {
                    var existingUser = await _context.Users.FindAsync(id);
                    if (existingUser == null) return NotFound();

                    existingUser.FullName = user.FullName;
                    existingUser.Email = user.Email;
                    existingUser.IsActive = user.IsActive;
                    existingUser.RoleId = user.RoleId; // Allow updating role
                    
                    _context.Update(existingUser);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id)) return NotFound();
                    else throw;
                }
                TempData["Success"] = "User updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Roles = await _context.Roles.ToListAsync();
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                // 1. Check if User is a Guest (UserId)
                if (await _context.Guests.AnyAsync(g => g.UserId == id))
                {
                    TempData["Error"] = "Không thể xóa tài khoản này vì đang liên kết với một hồ sơ Khách (Guest).";
                    return RedirectToAction(nameof(Index));
                }

                // 2. Check if User is an Instructor in a Course (InstructorId)
                if (await _context.CourseInstructors.AnyAsync(ci => ci.InstructorId == id))
                {
                    TempData["Error"] = "Không thể xóa tài khoản này vì đang là Giảng viên của một hoặc nhiều Khóa học.";
                    return RedirectToAction(nameof(Index));
                }

                // 3. Check if User is a Student in Enrollment (StudentId)
                if (await _context.Enrollments.AnyAsync(e => e.StudentId == id))
                {
                    TempData["Error"] = "Không thể xóa tài khoản này vì đang có thông tin Nhập học (Enrollment).";
                    return RedirectToAction(nameof(Index));
                }

                // 4. Cascade Delete Profiles (As requested)
                var studentProfile = await _context.StudentProfiles.FirstOrDefaultAsync(sp => sp.UserId == id);
                if (studentProfile != null)
                {
                    _context.StudentProfiles.Remove(studentProfile);
                }

                var instructorProfile = await _context.InstructorProfiles.FirstOrDefaultAsync(ip => ip.UserId == id);
                if (instructorProfile != null)
                {
                    _context.InstructorProfiles.Remove(instructorProfile);
                }

                // 5. Check if it's the LAST Admin
                if (user.RoleId == "1") // Admin Role ID
                {
                    var adminCount = await _context.Users.CountAsync(u => u.RoleId == "1");
                    if (adminCount <= 1)
                    {
                        TempData["Error"] = "Không thể xóa Admin cuối cùng của hệ thống!";
                        return RedirectToAction(nameof(Index));
                    }
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                TempData["Success"] = "User deleted successfully.";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            // Prevent deactivating the last Active Admin
            if (user.IsActive && user.RoleId == "1")
            {
                var activeAdminCount = await _context.Users.CountAsync(u => u.RoleId == "1" && u.IsActive);
                if (activeAdminCount <= 1)
                {
                     TempData["Error"] = "Không thể khóa Admin đang hoạt động cuối cùng của hệ thống!";
                     return RedirectToAction(nameof(Index));
                }
            }

            user.IsActive = !user.IsActive;
            await _context.SaveChangesAsync();

            TempData["Success"] = "User status updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(string id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
