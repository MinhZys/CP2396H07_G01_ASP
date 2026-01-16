using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Symphony.Portal.Web.Data;
using Symphony.Portal.Web.Models;
using Symphony.Portal.Web.Models.Enums;

namespace Symphony.Portal.Web.Controllers.Admin
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class GuestsController : Controller
    {
        private readonly AppDbContext _context;

        public GuestsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Guests
        public async Task<IActionResult> Index(string status)
        {
            var query = _context.Guests.AsQueryable();

            if (!string.IsNullOrEmpty(status) && Enum.TryParse<GuestRegistrationStatus>(status, out var statusEnum))
            {
                query = query.Where(g => g.Status == statusEnum);
            }
            else
            {
                // Default to showing Pending/Paid
                query = query.Where(g => g.Status == GuestRegistrationStatus.PaidPendingApproval || g.Status == GuestRegistrationStatus.PendingPayment);
            }

            ViewBag.CurrentStatus = status;
            return View(await query.OrderByDescending(g => g.CreatedAt).ToListAsync());
        }

        // POST: Admin/Guests/Approve/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(string id)
        {
            var guest = await _context.Guests.FindAsync(id);
            if (guest == null) return NotFound();

            if (guest.Status == GuestRegistrationStatus.Approved)
            {
                return RedirectToAction(nameof(Index));
            }

            // Create User Account
            // Check if email exists
            if (await _context.Users.AnyAsync(u => u.Email == guest.Email))
            {
                TempData["Error"] = "Email already exists in User system.";
                return RedirectToAction(nameof(Index));
            }

            var password = GenerateRandomPassword();
            var newUser = new User
            {
                Id = Guid.NewGuid().ToString(),
                FullName = guest.FullName,
                Email = guest.Email,
                // In production, Hash this password!
                Password = password, 
                RoleId = "4", // Guest Role ID from Seed
                IsActive = true
            };

            _context.Users.Add(newUser);
            
            // Link Guest
            guest.UserId = newUser.Id;
            guest.Status = GuestRegistrationStatus.Approved;
            _context.Guests.Update(guest);

            await _context.SaveChangesAsync();

            // Mock Email Sending
            TempData["Success"] = $"Guest Approved. User created. Password: {password}";

            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/Guests/Reject/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(string id)
        {
             var guest = await _context.Guests.FindAsync(id);
            if (guest == null) return NotFound();

            guest.Status = GuestRegistrationStatus.Rejected;
            _context.Guests.Update(guest);
            await _context.SaveChangesAsync();
            
            TempData["Success"] = "Guest Registration Rejected.";

            return RedirectToAction(nameof(Index));
        }

        private string GenerateRandomPassword()
        {
            return "Pass" + new Random().Next(1000, 9999);
        }
    }
}
