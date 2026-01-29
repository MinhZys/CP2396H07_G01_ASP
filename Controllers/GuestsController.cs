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
                .Where(e => e.IsActive && e.IsRegistrationOpen && e.ExamDate > DateTime.Now)
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
                .Where(e => e.IsActive && e.IsRegistrationOpen && e.ExamDate > DateTime.Now)
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
        public async Task<IActionResult> ConfirmPayment(string id, string paymentMethod)
        {
            var guest = await _context.Guests
                .Include(g => g.SelectedEntranceExam)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (guest == null)
                return NotFound();

            if (guest.Status != GuestRegistrationStatus.PendingPayment)
                return BadRequest("Invalid guest status");

            // Parse payment method
            var method = Enum.TryParse<PaymentMethod>(paymentMethod, true, out var pm)
                ? pm
                : PaymentMethod.Online;

            // ===== LẤY PHÍ THI =====
            decimal amount = guest.SelectedEntranceExam?.Fee ?? 0;

            if (guest.SelectedEntranceExam == null)
            {
                var exam = await _context.EntranceExams
                    .FindAsync(guest.SelectedEntranceExamId);

                if (exam == null)
                    return BadRequest("Entrance exam not found");

                amount = exam.Fee;
            }

            // ===== TẠO PAYMENT (CHUNG CHO CẢ 2) =====
            var payment = new Payment
            {
                Id = Guid.NewGuid().ToString(),
                GuestId = guest.Id,
                Amount = amount,
                PaymentMethod = method,
                PaymentDate = DateTime.Now,
                ReceiptNumber = "RCP" + DateTime.Now.Ticks,
                Status = method == PaymentMethod.Cash
                            ? PaymentStatus.Paid
                            : PaymentStatus.Pending,
                Purpose = PaymentPurpose.EntranceExam
            };

            _context.Payments.Add(payment);

            // ==================================================
            // =============== RẼ NHÁNH TẠI ĐÂY =================
            // ==================================================

            if (method == PaymentMethod.Cash)
            {
                // 👉 LOGIC CŨ – GIỮ NGUYÊN
                guest.Status = GuestRegistrationStatus.PaidPendingApproval;

                _context.Update(guest);
                await _context.SaveChangesAsync();

                return View("PaymentSuccess");
            }
            else if (method == PaymentMethod.Online)
            {
                // 👉 KHÔNG update guest status vội
                // 👉 ĐỢI VNPay callback

                await _context.SaveChangesAsync();

                // Redirect sang trang VNPay riêng
                return RedirectToAction(
                    "Create",
                    "VNPay",
                    new { paymentId = payment.Id }
                );
            }

            return BadRequest("Unsupported payment method");
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
                .Include(g => g.Class)
                .FirstOrDefaultAsync(g => g.UserId == user.Id || g.Email == user.Email);

            if (guest == null)
            {
                return View("NoGuestRecord");
            }
            
            // Auto-fix link if missing
            if (guest.UserId == null && guest.Email == user.Email)
            {
                guest.UserId = user.Id;
                _context.Update(guest);
                await _context.SaveChangesAsync();
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
