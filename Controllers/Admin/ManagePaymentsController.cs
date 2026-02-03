using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Symphony.Portal.Web.Data;
using Symphony.Portal.Web.Models;
using Symphony.Portal.Web.Models.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Symphony.Portal.Web.Controllers.Admin
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    [Route("Admin/[controller]/[action]")]
    public class ManagePaymentsController : Controller
    {
        private readonly AppDbContext _context;

        public ManagePaymentsController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(
    string? searchString,
    PaymentStatus? status,
    PaymentMethod? method,
    PaymentPurpose? purpose)
        {
            var query = _context.Payments
                .Include(p => p.Student)
                .Include(p => p.Guest)
                .AsQueryable();
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(p =>
                    p.ReceiptNumber.Contains(searchString) ||
                    (p.Student != null && p.Student.FullName.Contains(searchString)) ||
                    (p.Guest != null && p.Guest.FullName.Contains(searchString)) ||
                    (p.Student != null && p.Student.Email.Contains(searchString)) ||
                    (p.Guest != null && p.Guest.Email.Contains(searchString)));
            }
            if (status.HasValue)
            {
                query = query.Where(p => p.Status == status.Value);
            }
            if (method.HasValue)
            {
                query = query.Where(p => p.PaymentMethod == method.Value);
            }
            if (purpose.HasValue)
            {
                query = query.Where(p => p.Purpose == purpose.Value);
            }
            ViewData["Statuses"] = new SelectList(
                Enum.GetValues(typeof(PaymentStatus)).Cast<PaymentStatus>());
            ViewData["Methods"] = new SelectList(
                Enum.GetValues(typeof(PaymentMethod)).Cast<PaymentMethod>());
            ViewData["Purposes"] = new SelectList(
                Enum.GetValues(typeof(PaymentPurpose)).Cast<PaymentPurpose>());
            ViewData["CurrentSearch"] = searchString;
            ViewData["CurrentStatus"] = status;
            ViewData["CurrentMethod"] = method;
            ViewData["CurrentPurpose"] = purpose;

            return View(await query
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync());
        }
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return NotFound();

            var payment = await _context.Payments
                .Include(p => p.Student)
                .Include(p => p.Guest)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (payment == null) return NotFound();
            string? registeredTitle = null;
            string? registeredType = null;

            switch (payment.Purpose)
            {
                case PaymentPurpose.EntranceExam:
                    if (payment.Guest?.SelectedEntranceExamId != null)
                    {
                        var exam = await _context.EntranceExams
                            .FirstOrDefaultAsync(e => e.Id == payment.Guest.SelectedEntranceExamId);

                        registeredType = "Entrance Exam";
                        registeredTitle = exam?.Title;
                    }
                    break;

                case PaymentPurpose.Course:
                    if (payment.Guest?.Description?.StartsWith("COURSE:") == true)
                    {
                        var courseId = payment.Guest.Description.Replace("COURSE:", "").Trim();
                        var course = await _context.Courses.FindAsync(courseId);

                        registeredType = "Course";
                        registeredTitle = course?.Title;
                    }
                    break;

                case PaymentPurpose.Lab:
                    if (payment.Guest?.Description?.StartsWith("CLASS:") == true)
                    {
                        var classId = payment.Guest.Description.Replace("CLASS:", "").Trim();
                        var lab = await _context.Classes.FindAsync(classId);

                        registeredType = "Lab Class";
                        registeredTitle = lab?.ClassName;
                    }
                    break;

                case PaymentPurpose.Revision:
                    var revision = await _context.RevisionRegistrations
                        .Include(r => r.RevisionPackage)
                        .FirstOrDefaultAsync(r =>
                            r.Email == payment.Guest!.Email);

                    registeredType = "Revision Package";
                    registeredTitle = revision?.RevisionPackage?.Title;
                    break;
            }

            ViewBag.RegisteredType = registeredType;
            ViewBag.RegisteredTitle = registeredTitle;

            return View(payment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsPaid(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return RedirectToAction(nameof(Index));

            var payment = await _context.Payments
                .Include(p => p.Guest)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (payment == null)
            {
                TempData["Error"] = "Payment not found.";
                return RedirectToAction(nameof(Index));
            }

            if (payment.Status != PaymentStatus.Pending)
            {
                TempData["Error"] = "Only pending payments can be confirmed.";
                return RedirectToAction(nameof(Index));
            }

            payment.Status = PaymentStatus.Paid;
            payment.PaymentDate = DateTime.Now;
            if (payment.PaymentMethod == PaymentMethod.Cash &&
                (payment.Purpose == PaymentPurpose.Course || payment.Purpose == PaymentPurpose.Lab))
            {
                var guest = payment.Guest;
                var userId = guest?.UserId;

                if (!string.IsNullOrWhiteSpace(userId))
                {
                    var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                    var studentRole = await _context.Roles
                        .FirstOrDefaultAsync(r => r.Name == RoleNames.Student);

                    if (user != null && studentRole != null)
                    {
                        // đổi role
                        user.RoleId = studentRole.Id;
                        payment.StudentId = user.Id;
                        payment.GuestId = null;
                    }
                }
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = "Payment confirmed successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsFailed(string id)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment == null) return NotFound();

            payment.Status = PaymentStatus.Failed;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Đã cập nhật trạng thái: Thất bại.";
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(string id)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment == null) return NotFound();

            payment.Status = PaymentStatus.Cancelled;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Đã hủy thanh toán.";
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Refund(string id)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment == null) return NotFound();

            if (payment.Status != PaymentStatus.Paid)
            {
                TempData["Error"] = "Chỉ hoàn tiền cho Payment đã thanh toán.";
                return RedirectToAction(nameof(Index));
            }

            payment.Status = PaymentStatus.Refunded;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Hoàn tiền thành công.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id, string? searchString, PaymentStatus? status, PaymentMethod? method, PaymentPurpose? purpose)
        {
            if (string.IsNullOrWhiteSpace(id))
                return RedirectToAction(nameof(Index), new { searchString, status, method, purpose });

            var payment = await _context.Payments
                .Include(p => p.Student)
                .Include(p => p.Guest)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (payment == null)
            {
                TempData["Error"] = "Payment not found.";
                return RedirectToAction(nameof(Index), new { searchString, status, method, purpose });
            }
            if (payment.Status == PaymentStatus.Paid)
            {
                TempData["Error"] = "Cannot delete a PAID payment. Please cancel/refund instead.";
                return RedirectToAction(nameof(Index), new { searchString, status, method, purpose });
            }

            var vnpTxns = await _context.VNPayTransactions
                .Where(t => t.PaymentId == payment.Id)
                .ToListAsync();
            if (vnpTxns.Any())
                _context.VNPayTransactions.RemoveRange(vnpTxns);

            _context.Payments.Remove(payment);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Payment deleted successfully.";
            return RedirectToAction(nameof(Index), new { searchString, status, method, purpose });
        }

    }
}
