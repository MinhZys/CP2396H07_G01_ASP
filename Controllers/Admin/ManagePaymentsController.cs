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

        // =========================
        // GET: Admin/ManagePayments
        // =========================
        public async Task<IActionResult> Index(
            string? searchString,
            PaymentStatus? status,
            PaymentMethod? method)
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
                    (p.Guest != null && p.Guest.FullName.Contains(searchString)));
            }

            if (status.HasValue)
            {
                query = query.Where(p => p.Status == status);
            }

            if (method.HasValue)
            {
                query = query.Where(p => p.PaymentMethod == method);
            }

            ViewData["Statuses"] = new SelectList(
                Enum.GetValues(typeof(PaymentStatus)).Cast<PaymentStatus>());

            ViewData["Methods"] = new SelectList(
                Enum.GetValues(typeof(PaymentMethod)).Cast<PaymentMethod>());

            ViewData["CurrentSearch"] = searchString;
            ViewData["CurrentStatus"] = status;
            ViewData["CurrentMethod"] = method;

            return View(await query
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync());
        }

        // =========================
        // GET: Admin/ManagePayments/Details/5
        // =========================
        public async Task<IActionResult> Details(string? id)
        {
            if (id == null) return NotFound();

            var payment = await _context.Payments
                .Include(p => p.Student)
                .Include(p => p.Guest)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (payment == null) return NotFound();

            return View(payment);
        }

        // =========================
        // POST: Mark As Paid
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsPaid(string id)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment == null) return NotFound();

            if (payment.Status == PaymentStatus.Paid)
            {
                TempData["Error"] = "This payment has already been confirmed.";
                return RedirectToAction(nameof(Index));
            }

            payment.Status = PaymentStatus.Paid;
            payment.PaymentDate = DateTime.Now;

            await _context.SaveChangesAsync();
            TempData["Success"] = "Payment confirmed successfully.";

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // POST: Mark As Failed
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsFailed(string id)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment == null) return NotFound();

            payment.Status = PaymentStatus.Failed;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Payment status updated: Failed.";
            return RedirectToAction(nameof(Index));
        }

        // =========================
        // POST: Cancel Payment
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(string id)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment == null) return NotFound();

            payment.Status = PaymentStatus.Cancelled;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Payment cancelled successfully.";
            return RedirectToAction(nameof(Index));
        }

        // =========================
        // POST: Refund
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Refund(string id)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment == null) return NotFound();

            if (payment.Status != PaymentStatus.Paid)
            {
                TempData["Error"] = "Refund is only available for paid payments.";
                return RedirectToAction(nameof(Index));
            }

            payment.Status = PaymentStatus.Refunded;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Payment refunded successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
