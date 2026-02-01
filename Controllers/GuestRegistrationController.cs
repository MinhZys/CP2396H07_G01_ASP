using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Symphony.Portal.Web.Data;
using Symphony.Portal.Web.Models;
using Symphony.Portal.Web.Models.Enums;
using System.ComponentModel.DataAnnotations;

public class GuestRegistrationController : Controller
{
    private readonly AppDbContext _context;

    public GuestRegistrationController(AppDbContext context)
    {
        _context = context;
    }

    public class GuestRegisterVm
    {
        [Required, StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required, EmailAddress, StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required, StringLength(15)]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required, DataType(DataType.Date)]
        public DateTime Dob { get; set; }

        [StringLength(255)]
        public string Address { get; set; } = string.Empty;

        [Required]
        public PaymentPurpose Purpose { get; set; }

        // EntranceExam
        public string? SelectedEntranceExamId { get; set; }

        // Course/Subject (hiện Guest model có ClassId)
        public string? ClassId { get; set; }

        [Required]
        public PaymentMethod PaymentMethod { get; set; }

        // ✅ Tạm thời cho Course/Subject nếu bạn chưa có mapping tính tiền
        public decimal? ManualAmount { get; set; }
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await LoadDropdowns();
        return View(new GuestRegisterVm { Dob = DateTime.Today.AddYears(-18) });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(GuestRegisterVm vm)
    {
        if (!ModelState.IsValid)
        {
            await LoadDropdowns();
            return View(vm);
        }

        // validate theo purpose
        if (vm.Purpose == PaymentPurpose.EntranceExam && string.IsNullOrWhiteSpace(vm.SelectedEntranceExamId))
        {
            ModelState.AddModelError(nameof(vm.SelectedEntranceExamId), "Vui lòng chọn kỳ thi đầu vào.");
            await LoadDropdowns();
            return View(vm);
        }

        if ((vm.Purpose == PaymentPurpose.Course || vm.Purpose == PaymentPurpose.Subject) && string.IsNullOrWhiteSpace(vm.ClassId))
        {
            ModelState.AddModelError(nameof(vm.ClassId), "Vui lòng chọn lớp/môn học.");
            await LoadDropdowns();
            return View(vm);
        }

        // Create Guest
        var guest = new Guest
        {
            Id = Guid.NewGuid().ToString(),
            FullName = vm.FullName,
            Email = vm.Email,
            PhoneNumber = vm.PhoneNumber,
            Dob = vm.Dob,
            Address = vm.Address,
            SelectedEntranceExamId = vm.SelectedEntranceExamId,
            ClassId = vm.ClassId,
            UserId = null,
            Status = GuestRegistrationStatus.PendingPayment,
            CreatedAt = DateTime.Now
        };
        _context.Guests.Add(guest);

        // Amount
        var amount = await ResolveAmountAsync(vm);
        if (amount <= 0)
        {
            ModelState.AddModelError(nameof(vm.ManualAmount), "Số tiền thanh toán không hợp lệ.");
            await LoadDropdowns();
            return View(vm);
        }

        // Payment
        var payment = new Payment
        {
            Id = Guid.NewGuid().ToString(),
            GuestId = guest.Id,
            Amount = amount,
            PaymentMethod = vm.PaymentMethod,
            Status = PaymentStatus.Pending,
            PaymentDate = DateTime.MinValue,
            ReceiptNumber = GenerateReceiptNumber(),
            Purpose = vm.Purpose
        };
        _context.Payments.Add(payment);

        await _context.SaveChangesAsync();

        // Redirect sang VNPayController cũ (✅ không cần IVnPayService)
        if (vm.PaymentMethod == PaymentMethod.Online)
        {
            return RedirectToAction("Create", "VNPay", new { paymentId = payment.Id });
        }

        // Cash
        return RedirectToAction(nameof(PendingCash), new { paymentId = payment.Id });
    }

    [HttpGet]
    public async Task<IActionResult> PendingCash(string paymentId)
    {
        var payment = await _context.Payments
            .Include(p => p.Guest)
            .FirstOrDefaultAsync(p => p.Id == paymentId);

        if (payment == null) return NotFound();
        return View(payment);
    }

    // ===== helpers =====

    private async Task LoadDropdowns()
    {
        ViewBag.EntranceExams = new SelectList(
            await _context.EntranceExams.Where(x => x.IsActive).ToListAsync(),
            "Id", "Title");

        ViewBag.Classes = new SelectList(
            await _context.Classes.ToListAsync(),
            "Id", "ClassName");
    }

    private async Task<decimal> ResolveAmountAsync(GuestRegisterVm vm)
    {
        if (vm.Purpose == PaymentPurpose.EntranceExam && vm.SelectedEntranceExamId != null)
        {
            var exam = await _context.EntranceExams.FirstOrDefaultAsync(x => x.Id == vm.SelectedEntranceExamId);
            return exam?.Fee ?? 0m;
        }

        // Course/Subject: tạm dùng ManualAmount để không đoán sai khi chưa biết map Class -> Course
        return vm.ManualAmount ?? 0m;
    }

    private static string GenerateReceiptNumber()
        => $"RCPT-{DateTime.Now:yyyyMMddHHmmss}-{Guid.NewGuid().ToString()[..6].ToUpper()}";
}
