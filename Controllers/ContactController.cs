using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Symphony.Portal.Web.Data;
using Symphony.Portal.Web.Models;

namespace Symphony.Portal.Web.Controllers
{
    public class ContactController : Controller
    {
        private readonly AppDbContext _context;

        public ContactController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var centers = await _context.Centers
                .AsNoTracking()
                .Where(c => c.IsActive)
                .OrderBy(c => c.DisplayOrder)
                .ToListAsync();

            return View(centers); // Views/Contact/Index.cshtml
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendMessage(string fullName, string email, string? centerId, string message)
        {
            if (string.IsNullOrWhiteSpace(fullName) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(message))
            {
                TempData["Error"] = "Please fill in all required fields.";
                return RedirectToAction(nameof(Index));
            }

            var msg = new ContactMessage
            {
                Id = Guid.NewGuid().ToString(),
                FullName = fullName.Trim(),
                Email = email.Trim(),
                CenterId = string.IsNullOrWhiteSpace(centerId) ? null : centerId,
                Message = message.Trim(),
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };

            _context.ContactMessages.Add(msg);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Message sent successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
