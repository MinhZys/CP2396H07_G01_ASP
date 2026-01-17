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

        [HttpGet]
        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null) return NotFound();
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();
            
            ViewBag.Roles = await _context.Roles.ToListAsync();
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, User user)
        {
            if (id != user.Id) return NotFound();

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

                // 4. Check if User has Student Profile or Instructor Profile
                if (await _context.StudentProfiles.AnyAsync(sp => sp.UserId == id))
                {
                     TempData["Error"] = "Không thể xóa tài khoản này vì đang có Hồ sơ Học viên (Student Profile).";
                     return RedirectToAction(nameof(Index));
                }
                
                if (await _context.InstructorProfiles.AnyAsync(ip => ip.UserId == id))
                {
                     TempData["Error"] = "Không thể xóa tài khoản này vì đang có Hồ sơ Giảng viên (Instructor Profile).";
                     return RedirectToAction(nameof(Index));
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
