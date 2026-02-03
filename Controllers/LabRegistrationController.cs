using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Symphony.Portal.Web.Data;
using Symphony.Portal.Web.Models;
using Symphony.Portal.Web.Models.Enums;

namespace Symphony.Portal.Web.Controllers
{
    public class LabRegistrationController : Controller
    {
        private readonly AppDbContext _context;

        public LabRegistrationController(AppDbContext context)
        {
            _context = context;
        }

        // =====================================================
        // GET: LabRegistration/Register
        // 1 trang duy nhất: hiển thị info guest + cho chọn lab
        // =====================================================
        [HttpGet]
        public async Task<IActionResult> Register()
        {
            // 1) load list lớp Lab cho dropdown
            ViewBag.LabClasses = await GetOpenLabClasses();

            // 2) lấy email từ user đang đăng nhập (nếu có)
            var email = User?.Identity?.Name;

            var vm = new LabRegisterPageVm
            {
                Guest = new GuestRegistrationVm
                {
                    Dob = DateTime.Today.AddYears(-18),
                    Purpose = PaymentPurpose.Lab,
                    PaymentMethod = PaymentMethod.Online
                }
            };

            // 3) nếu đã đăng nhập thì prefill info
            if (!string.IsNullOrWhiteSpace(email))
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (user != null)
                {
                    // lấy record guest gần nhất để fill phone/dob/address (nếu có)
                    var lastGuest = await _context.Guests
                        .Where(g => g.UserId == user.Id)
                        .OrderByDescending(g => g.CreatedAt)
                        .FirstOrDefaultAsync();

                    vm.Guest.FullName = user.FullName;
                    vm.Guest.Email = user.Email;

                    if (lastGuest != null)
                    {
                        vm.Guest.PhoneNumber = lastGuest.PhoneNumber;
                        vm.Guest.Dob = lastGuest.Dob;
                        vm.Guest.Address = lastGuest.Address;
                    }
                }
            }

            return View(vm);
        }

        // =====================================================
        // POST: LabRegistration/Register
        // tạo Guest (đăng ký) -> redirect Payment
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(LabRegisterPageVm vm)
        {
            // load list lại để render khi lỗi
            ViewBag.LabClasses = await GetOpenLabClasses();

            // validate chọn lớp
            if (string.IsNullOrWhiteSpace(vm.SelectedClassId))
                ModelState.AddModelError(nameof(vm.SelectedClassId), "Vui lòng chọn lớp Lab.");

            // load class + check đúng Lab
            Class? labClass = null;
            if (!string.IsNullOrWhiteSpace(vm.SelectedClassId))
            {
                labClass = await _context.Classes
                    .Include(c => c.ClassCategory)
                    .FirstOrDefaultAsync(c => c.Id == vm.SelectedClassId);

                if (labClass == null)
                    ModelState.AddModelError(nameof(vm.SelectedClassId), "Lớp Lab không tồn tại.");
                else if (!string.Equals(labClass.ClassCategory?.Name, "Lab", StringComparison.OrdinalIgnoreCase))
                    ModelState.AddModelError(nameof(vm.SelectedClassId), "Bạn chỉ được chọn lớp thuộc loại Lab.");
            }

            // set purpose cố định
            vm.Guest.Purpose = PaymentPurpose.Lab;

            if (!ModelState.IsValid)
                return View(vm);

            // Role Guest phải có trong DB
            var guestRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Guest");
            if (guestRole == null) return BadRequest("Role 'Guest' chưa có trong DB.");

            // tạo / cập nhật user theo email (giữ nguyên logic bạn đang dùng)
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == vm.Guest.Email);
            if (user == null)
            {
                user = new User
                {
                    Id = Guid.NewGuid().ToString(),
                    FullName = vm.Guest.FullName,
                    Email = vm.Guest.Email,
                    Password = "123",
                    IsActive = true,
                    RoleId = guestRole.Id
                };
                _context.Users.Add(user);
            }
            else
            {
                user.FullName = vm.Guest.FullName;
                if (string.IsNullOrWhiteSpace(user.RoleId))
                    user.RoleId = guestRole.Id;
            }

            // tạo Guest record đăng ký lab
            var guestEntity = new Guest
            {
                Id = Guid.NewGuid().ToString(),
                FullName = vm.Guest.FullName,
                Email = vm.Guest.Email,
                PhoneNumber = vm.Guest.PhoneNumber,
                Dob = vm.Guest.Dob,
                Address = vm.Guest.Address,
                UserId = user.Id,
                Status = GuestRegistrationStatus.PendingPayment,
                CreatedAt = DateTime.Now,

                // ✅ lưu ClassId vào Description
                Description = $"CLASS:{vm.SelectedClassId}"
            };

            _context.Guests.Add(guestEntity);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Payment), new { id = guestEntity.Id });
        }

        // =====================================================
        // GET: LabRegistration/Payment?id=guestId
        // =====================================================
        [HttpGet]
        public async Task<IActionResult> Payment(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return NotFound();

            var guest = await _context.Guests.FirstOrDefaultAsync(g => g.Id == id);
            if (guest == null) return NotFound();

            if (guest.Status != GuestRegistrationStatus.PendingPayment)
                return RedirectToAction("Index", "Home");

            var classId = ExtractClassId(guest.Description);
            if (string.IsNullOrWhiteSpace(classId))
                return BadRequest("Guest không chứa CLASS id.");

            var labClass = await _context.Classes
                .Include(c => c.ClassCategory)
                .FirstOrDefaultAsync(c => c.Id == classId);

            if (labClass == null) return NotFound();

            if (!string.Equals(labClass.ClassCategory?.Name, "Lab", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Class này không phải loại Lab.");

            var vm = new LabRegisterPageVm
            {
                SelectedClassId = labClass.Id,
                Guest = new GuestRegistrationVm
                {
                    FullName = guest.FullName,
                    Email = guest.Email,
                    PhoneNumber = guest.PhoneNumber,
                    Dob = guest.Dob,
                    Address = guest.Address,
                    Purpose = PaymentPurpose.Lab,
                    PaymentMethod = PaymentMethod.Online
                }
            };

            ViewBag.GuestId = guest.Id;
            ViewBag.LabClass = labClass; // để view hiển thị class + fee
            return View(vm);
        }

        // =====================================================
        // POST: LabRegistration/ConfirmPayment
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmPayment(string id, string paymentMethod)
        {
            var guest = await _context.Guests.FirstOrDefaultAsync(g => g.Id == id);
            if (guest == null) return NotFound();

            if (guest.Status != GuestRegistrationStatus.PendingPayment)
                return BadRequest("Invalid guest status");

            var classId = ExtractClassId(guest.Description);
            if (string.IsNullOrWhiteSpace(classId))
                return BadRequest("Guest không chứa CLASS id.");

            var labClass = await _context.Classes
                .Include(c => c.ClassCategory)
                .FirstOrDefaultAsync(c => c.Id == classId);

            if (labClass == null) return NotFound();

            if (!string.Equals(labClass.ClassCategory?.Name, "Lab", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Class này không phải loại Lab.");

            var method = Enum.TryParse<PaymentMethod>(paymentMethod, true, out var pm)
                ? pm
                : PaymentMethod.Online;

            var amount = labClass.Fee;

            var payment = new Payment
            {
                Id = Guid.NewGuid().ToString(),
                GuestId = guest.Id,
                Amount = amount,
                PaymentMethod = method,
                PaymentDate = DateTime.Now,
                ReceiptNumber = "RCP" + DateTime.Now.Ticks,
                Status = PaymentStatus.Pending,
                Purpose = PaymentPurpose.Lab
            };

            _context.Payments.Add(payment);

            if (method == PaymentMethod.Cash)
            {
                guest.Status = GuestRegistrationStatus.PendingPayment;
                _context.Update(guest);
                await _context.SaveChangesAsync();
                return View("~/Views/LabRegistration/PaymentSuccess.cshtml");
            }
            else if (method == PaymentMethod.Online)
            {
                await _context.SaveChangesAsync();
                return RedirectToAction("Create", "VNPay", new { paymentId = payment.Id });
            }

            return BadRequest("Unsupported payment method");
        }

        // ===== helpers =====
        private async Task<List<SelectListItem>> GetOpenLabClasses()
        {
            var labCategory = await _context.ClassCategories.FirstOrDefaultAsync(x => x.Name == "Lab");
            if (labCategory == null) return new List<SelectListItem>();

            var labs = await _context.Classes
                .Where(c => c.ClassCategoryId == labCategory.Id)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            return labs.Select(c => new SelectListItem
            {
                Value = c.Id,
                Text = $"{c.ClassName} - {c.Fee:N0}đ"
            }).ToList();
        }

        private static string? ExtractClassId(string? description)
        {
            if (string.IsNullOrWhiteSpace(description)) return null;
            if (!description.StartsWith("CLASS:", StringComparison.OrdinalIgnoreCase)) return null;
            return description.Substring("CLASS:".Length).Trim();
        }
    }
}
