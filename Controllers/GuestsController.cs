using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Symphony.Portal.Web.Data;
using Symphony.Portal.Web.Models;
using Symphony.Portal.Web.Models.Enums;

namespace Symphony.Portal.Web.Controllers
{
    public class GuestsController : Controller
    {
        private readonly AppDbContext _context;

        public GuestsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Guests/Register
        public async Task<IActionResult> Register()
        {
            ViewBag.EntranceExams = await _context.EntranceExams
                .Where(e => e.IsActive && e.ExamDate > DateTime.Now)
                .ToListAsync();
            return View();
        }

        // POST: Guests/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(Guest guest)
        {
            if (ModelState.IsValid)
            {
                guest.Status = GuestRegistrationStatus.PendingPayment;
                guest.CreatedAt = DateTime.Now;
                
                _context.Guests.Add(guest);
                await _context.SaveChangesAsync();
                
                return RedirectToAction(nameof(Payment), new { id = guest.Id });
            }
            
            ViewBag.EntranceExams = await _context.EntranceExams
                .Where(e => e.IsActive && e.ExamDate > DateTime.Now)
                .ToListAsync();
            return View(guest);
        }

        // GET: Guests/Payment/5
        public async Task<IActionResult> Payment(string id)
        {
            if (id == null) return NotFound();

            var guest = await _context.Guests
                .Include(g => g.SelectedEntranceExam)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (guest == null) return NotFound();

            if (guest.Status != GuestRegistrationStatus.PendingPayment)
            {
                // Already paid or processed
                return RedirectToAction("Index", "Home"); 
            }

            return View(guest);
        }

        // POST: Guests/ConfirmPayment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmPayment(string id, string paymentMethod) // Simplified
        {
            var guest = await _context.Guests.FindAsync(id);
            if (guest == null) return NotFound();

            if (guest.Status == GuestRegistrationStatus.PendingPayment)
            {
                guest.Status = GuestRegistrationStatus.PaidPendingApproval;
                
                // Create Payment Record
                var payment = new Payment
                {
                    Id = Guid.NewGuid().ToString(),
                    GuestId = guest.Id,
                    Amount = guest.SelectedEntranceExam?.Fee ?? 0, // Fallback need handling
                    PaymentMethod = Enum.TryParse<PaymentMethod>(paymentMethod, out var pm) ? pm : PaymentMethod.Online,
                    PaymentDate = DateTime.Now,
                    ReceiptNumber = "RCP" + DateTime.Now.Ticks.ToString()
                };
                
                // Fetch Fee if null (lazy loading might not work here on tracked entity without include)
                if (guest.SelectedEntranceExam == null)
                {
                     // Re-fetch to get fee if needed, or assume front-end passed it. 
                     // Ideally logic:
                     var exam = await _context.EntranceExams.FindAsync(guest.SelectedEntranceExamId);
                     if (exam != null) payment.Amount = exam.Fee;
                }

                _context.Payments.Add(payment);
                _context.Update(guest);
                await _context.SaveChangesAsync();
            }

            return View("PaymentSuccess");
        }

        // GET: Guests/Dashboard
        [Authorize(Roles = "Guest")]
        public async Task<IActionResult> Dashboard()
        {
            var userEmail = User.Identity?.Name;
            // Find Guest record linked to this User
            // or we look up User first
            // Current User Id
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (user == null) return RedirectToAction("Login", "Account");

            var guest = await _context.Guests
                .Include(g => g.SelectedEntranceExam)
                .Include(g => g.Class) // Include Class Info
                .FirstOrDefaultAsync(g => g.UserId == user.Id || g.Email == user.Email); // Fallback to Email match if Link missing

            if (guest == null)
            {
                return View("NoGuestRecord");
            }

            // Get Exam Results
            var results = await _context.ExamResults
                .Include(r => r.EntranceExam)
                .Where(r => r.StudentId == user.Id)
                .ToListAsync();

            ViewBag.ExamResults = results;

            return View(guest);
        }
    }
}
