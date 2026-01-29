using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Symphony.Portal.Web.Data;
using Symphony.Portal.Web.Models;
using Symphony.Portal.Web.Models.Enums;

namespace Symphony.Portal.Web.Controllers
{
    public class RevisionGuestsController : Controller
    {
        private readonly AppDbContext _context;

        public RevisionGuestsController(AppDbContext context)
        {
            _context = context;
        }

        // =====================================================
        // GET: RevisionGuests/Register
        // =====================================================
        public async Task<IActionResult> Register()
        {
            ViewBag.RevisionPackages = await _context.RevisionPackages
                .Where(p => p.Status == RevisionPackageStatus.Open)
                .ToListAsync();

            return View();
        }

        // =====================================================
        // POST: RevisionGuests/Register
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RevisionRegisterVM vm)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.RevisionPackages = await _context.RevisionPackages
                    .Where(p => p.Status == RevisionPackageStatus.Open)
                    .ToListAsync();

                return View(vm);
            }

            var registration = new RevisionRegistration
            {
                Id = Guid.NewGuid().ToString(),
                RevisionPackageId = vm.RevisionPackageId,
                FullName = vm.FullName,
                Email = vm.Email,
                PhoneNumber = vm.PhoneNumber,
                Status = GuestRegistrationStatus.PendingPayment,
                CreatedAt = DateTime.Now
            };

            _context.RevisionRegistrations.Add(registration);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Payment), new { id = registration.Id });
        }


        // =====================================================
        // GET: RevisionGuests/Payment/{id}
        // =====================================================
        public async Task<IActionResult> Payment(string id)
        {
            if (id == null) return NotFound();

            var registration = await _context.RevisionRegistrations
                .Include(r => r.RevisionPackage)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (registration == null) return NotFound();

            if (registration.Status != GuestRegistrationStatus.PendingPayment)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(registration);
        }

        // =====================================================
        // POST: RevisionGuests/ConfirmPayment
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmPayment(string id, string paymentMethod)
        {
            var registration = await _context.RevisionRegistrations
                .Include(r => r.RevisionPackage)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (registration == null)
                return NotFound();

            if (registration.Status != GuestRegistrationStatus.PendingPayment)
                return BadRequest("Invalid registration status");

            // Parse payment method
            var method = Enum.TryParse<PaymentMethod>(paymentMethod, true, out var pm)
                ? pm
                : PaymentMethod.Online;

            // ===== LẤY PHÍ ÔN THI =====
            decimal amount = registration.RevisionPackage?.Fee ?? 0;

            if (registration.RevisionPackage == null)
            {
                var pkg = await _context.RevisionPackages
                    .FindAsync(registration.RevisionPackageId);

                if (pkg == null)
                    return BadRequest("Revision package not found");

                amount = pkg.Fee;
            }

            // ===== TẠO PAYMENT =====
            var payment = new Payment
            {
                Id = Guid.NewGuid().ToString(),
                Amount = amount,
                PaymentMethod = method,
                PaymentDate = DateTime.Now,
                ReceiptNumber = "RCP" + DateTime.Now.Ticks,
                Status = method == PaymentMethod.Cash
                            ? PaymentStatus.Paid
                            : PaymentStatus.Pending
            };

            _context.Payments.Add(payment);

            // =====================================================
            // RẼ NHÁNH THANH TOÁN
            // =====================================================

            if (method == PaymentMethod.Cash)
            {
                registration.Status = GuestRegistrationStatus.PaidPendingApproval;

                // update số lượng
                registration.RevisionPackage.CurrentStudents++;

                if (registration.RevisionPackage.CurrentStudents >=
                    registration.RevisionPackage.MaxStudents)
                {
                    registration.RevisionPackage.Status = RevisionPackageStatus.Full;
                }

                _context.Update(registration);
                _context.Update(registration.RevisionPackage);

                await _context.SaveChangesAsync();

                return View("PaymentSuccess");
            }
            else if (method == PaymentMethod.Online)
            {
                // chưa update registration
                await _context.SaveChangesAsync();

                return RedirectToAction(
                    "Create",
                    "VNPay",
                    new { paymentId = payment.Id }
                );
            }

            return BadRequest("Unsupported payment method");
        }
    }
}
