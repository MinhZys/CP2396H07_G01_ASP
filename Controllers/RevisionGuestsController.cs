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

        public async Task<IActionResult> Register()
        {
            ViewBag.RevisionPackages = await _context.RevisionPackages
                .Where(p => p.Status == RevisionPackageStatus.Open)
                .ToListAsync();

            return View();
        }

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
                var pkg = await _context.RevisionPackages.FindAsync(registration.RevisionPackageId);
                if (pkg == null) return BadRequest("Revision package not found");
                amount = pkg.Fee;
                registration.RevisionPackage = pkg; // để dùng update CurrentStudents bên dưới
            }

            // ============================================================
            // ✅ FIX #1: tạo/lấy Guest entity để Payment join ra đúng Payer
            // ============================================================
            // Lấy guest theo email (ưu tiên guest mới nhất)
            var guestEntity = await _context.Guests
                .Where(g => g.Email == registration.Email)
                .OrderByDescending(g => g.CreatedAt)
                .FirstOrDefaultAsync();

            if (guestEntity == null)
            {
                guestEntity = new Guest
                {
                    Id = Guid.NewGuid().ToString(),
                    FullName = registration.FullName,
                    Email = registration.Email,
                    PhoneNumber = registration.PhoneNumber,
                    Status = GuestRegistrationStatus.PendingPayment,
                    CreatedAt = DateTime.Now,
                    // để trace đây là đăng ký ôn thi
                    Description = $"REVISION:{registration.Id}"
                };

                _context.Guests.Add(guestEntity);
                await _context.SaveChangesAsync();
            }
            else
            {
                // optional: sync lại info nếu cần
                if (string.IsNullOrWhiteSpace(guestEntity.FullName))
                    guestEntity.FullName = registration.FullName;

                if (string.IsNullOrWhiteSpace(guestEntity.PhoneNumber))
                    guestEntity.PhoneNumber = registration.PhoneNumber;
            }

            // ============================================================
            // ✅ FIX #2: Purpose đúng cho ôn thi
            // ============================================================
            // Nếu bạn chưa muốn thêm PaymentPurpose.Revision thì dùng Subject
            var purpose = PaymentPurpose.Revision;

            // ===== TẠO PAYMENT =====
            var payment = new Payment
            {
                Id = Guid.NewGuid().ToString(),
                GuestId = guestEntity.Id,                 // ✅ quan trọng để hiển thị payer
                Amount = amount,
                PaymentMethod = method,
                PaymentDate = method == PaymentMethod.Cash ? DateTime.Now : DateTime.MinValue,
                ReceiptNumber = "RCP" + DateTime.Now.Ticks,
                Status = PaymentStatus.Pending,           // ✅ cash cũng Pending (đợi admin confirm)
                Purpose = purpose
            };

            _context.Payments.Add(payment);

            // ==================================================
            // =============== RẼ NHÁNH TẠI ĐÂY =================
            // ==================================================
            if (method == PaymentMethod.Cash)
            {
                // Cash: đợi admin confirm
                registration.Status = GuestRegistrationStatus.PaidPendingApproval;

                // update số lượng (nếu bạn muốn reserve slot ngay khi cash)
                registration.RevisionPackage.CurrentStudents++;

                if (registration.RevisionPackage.MaxStudents > 0 &&
                    registration.RevisionPackage.CurrentStudents >= registration.RevisionPackage.MaxStudents)
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
                // Online: chưa update registration/package, đợi VNPay callback xử lý Paid/Failed
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
