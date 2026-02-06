using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Symphony.Portal.Web.Data;
using Symphony.Portal.Web.Models;
using Symphony.Portal.Web.Models.Enums;

namespace Symphony.Portal.Web.Controllers
{
    public class CourseRegistrationController : Controller
    {
        private readonly AppDbContext _context;

        public CourseRegistrationController(AppDbContext context)
        {
            _context = context;
        }

        // =====================================================
        // GET: CourseRegistration/Confirm?courseId=xxx
        // =====================================================
        [HttpGet]
        public async Task<IActionResult> Confirm(string courseId)
        {
            if (string.IsNullOrWhiteSpace(courseId)) return NotFound();

            var course = await _context.Courses
                .Include(c => c.Category)
                .Include(c => c.Certificate)
                .FirstOrDefaultAsync(c => c.Id == courseId);

            if (course == null) return NotFound();

            var vm = new CourseRegisterPageVm
            {
                Course = course,
                Guest = new GuestRegistrationVm
                {
                    Dob = DateTime.Today.AddYears(-18),
                    Purpose = PaymentPurpose.Course,
                    PaymentMethod = PaymentMethod.Online
                }
            };

            // ================== 🔥 AUTO FILL USER INFO ==================
            var email = User?.Identity?.Name;
            if (!string.IsNullOrWhiteSpace(email))
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (user != null)
                {
                    vm.Guest.FullName = user.FullName;
                    vm.Guest.Email = user.Email;

                    // lấy Guest gần nhất để fill thêm thông tin
                    var lastGuest = await _context.Guests
                        .Where(g => g.UserId == user.Id)
                        .OrderByDescending(g => g.CreatedAt)
                        .FirstOrDefaultAsync();

                    if (lastGuest != null)
                    {
                        vm.Guest.PhoneNumber = lastGuest.PhoneNumber;
                        vm.Guest.Dob = lastGuest.Dob;
                        vm.Guest.Address = lastGuest.Address;
                    }
                }
            }
            // ============================================================

            return View(vm);
        }


        // =====================================================
        // POST: CourseRegistration/Confirm
        // tạo Guest trước -> chuyển Payment(id)
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Confirm(string courseId, GuestRegistrationVm guest)
        {
            if (string.IsNullOrWhiteSpace(courseId)) return NotFound();

            var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == courseId);
            if (course == null) return NotFound();

            guest.Purpose = PaymentPurpose.Course;

            if (!ModelState.IsValid)
                return View(new CourseRegisterPageVm { Course = course, Guest = guest });

            // Role Guest phải có trong DB
            var guestRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Guest");
            if (guestRole == null) return BadRequest("Role 'Guest' does not exist in the database.");

            // tạo / cập nhật user theo email
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == guest.Email);
            if (user == null)
            {
                user = new User
                {
                    Id = Guid.NewGuid().ToString(),
                    FullName = guest.FullName,
                    Email = guest.Email,
                    Password = "123",
                    IsActive = true,
                    RoleId = guestRole.Id
                };
                _context.Users.Add(user);
            }
            else
            {
                user.FullName = guest.FullName;
                if (string.IsNullOrWhiteSpace(user.RoleId))
                    user.RoleId = guestRole.Id;
            }

            // tạo Guest record (giống RevisionRegistration)
            var guestEntity = new Guest
            {
                Id = Guid.NewGuid().ToString(),
                FullName = guest.FullName,
                Email = guest.Email,
                PhoneNumber = guest.PhoneNumber,
                Dob = guest.Dob,
                Address = guest.Address,
                UserId = user.Id,
                Status = GuestRegistrationStatus.PendingPayment,
                CreatedAt = DateTime.Now,

                // lưu CourseId và ExtraPractice vào Description
                Description = $"COURSE:{courseId}|EXTRA:{guest.HasExtraPractice}"
            };

            _context.Guests.Add(guestEntity);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Payment), new { id = guestEntity.Id });
        }

        // =====================================================
        // GET: CourseRegistration/Payment?id=guestId
        // =====================================================
        [HttpGet]
        public async Task<IActionResult> Payment(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return NotFound();

            var guest = await _context.Guests.FirstOrDefaultAsync(g => g.Id == id);
            if (guest == null) return NotFound();

            if (guest.Status != GuestRegistrationStatus.PendingPayment)
                return RedirectToAction("Index", "Home");

            var courseId = ExtractCourseId(guest.Description);
            if (string.IsNullOrWhiteSpace(courseId))
                return BadRequest("Guest does not contain a COURSE id.");

            var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == courseId);
            if (course == null) return NotFound();

            var vm = new CourseRegisterPageVm
            {
                Course = course,
                Guest = new GuestRegistrationVm
                {
                    FullName = guest.FullName,
                    Email = guest.Email,
                    PhoneNumber = guest.PhoneNumber,
                    Dob = guest.Dob,
                    Address = guest.Address,
                    Purpose = PaymentPurpose.Course,
                    PaymentMethod = PaymentMethod.Online
                }
            };

            // Calculate total amount for display
            var hasExtra = guest.Description?.Contains("EXTRA:True", StringComparison.OrdinalIgnoreCase) ?? false;
            ViewBag.TotalAmount = course.TuitionFee + (hasExtra ? 1000m : 0m);
            ViewBag.HasExtra = hasExtra;
            ViewBag.GuestId = guest.Id; // để view post lại id
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmPayment(string id, string paymentMethod)
        {
            var guest = await _context.Guests.FirstOrDefaultAsync(g => g.Id == id);
            if (guest == null) return NotFound();

            if (guest.Status != GuestRegistrationStatus.PendingPayment)
                return BadRequest("Invalid guest status");

            var courseId = ExtractCourseId(guest.Description);
            if (string.IsNullOrWhiteSpace(courseId))
                return BadRequest("Guest does not contain a COURSE id.");

            var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == courseId);
            if (course == null) return NotFound();

            var method = Enum.TryParse<PaymentMethod>(paymentMethod, true, out var pm)
                ? pm
                : PaymentMethod.Online;

            var hasExtra = guest.Description?.Contains("EXTRA:True", StringComparison.OrdinalIgnoreCase) ?? false;
            var totalAmount = course.TuitionFee + (hasExtra ? 1000m : 0m);

            var payment = new Payment
            {
                Id = Guid.NewGuid().ToString(),
                GuestId = guest.Id,
                Amount = totalAmount,
                PaymentMethod = method,
                PaymentDate = DateTime.Now,

                ReceiptNumber = "RCP" + DateTime.Now.Ticks,
                Status = PaymentStatus.Pending,

                Purpose = PaymentPurpose.Course
            };

            _context.Payments.Add(payment);

            if (method == PaymentMethod.Cash)
            {
                guest.Status = GuestRegistrationStatus.PendingPayment;
                _context.Update(guest);

                await _context.SaveChangesAsync();
                return View("~/Views/CourseRegistration/PaymentSuccess.cshtml");
            }
            else if (method == PaymentMethod.Online)
            {
                await _context.SaveChangesAsync();
                return RedirectToAction("Create", "VNPay", new { paymentId = payment.Id });
            }

            return BadRequest("Unsupported payment method");
        }


        private static string? ExtractCourseId(string? description)
        {
            if (string.IsNullOrWhiteSpace(description)) return null;
            if (!description.StartsWith("COURSE:", StringComparison.OrdinalIgnoreCase)) return null;
            return description.Substring("COURSE:".Length).Trim();
        }
    }
}
