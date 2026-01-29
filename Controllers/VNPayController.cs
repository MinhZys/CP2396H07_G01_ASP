using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Symphony.Portal.Web.Data;
using Symphony.Portal.Web.Helpers;
using Symphony.Portal.Web.Models;
using Symphony.Portal.Web.Models.Enums;

public class VNPayController : Controller
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;

    public VNPayController(AppDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    public async Task<IActionResult> Create(string paymentId)
    {
        var payment = await _context.Payments
            .Include(p => p.Guest)
            .FirstOrDefaultAsync(p => p.Id == paymentId);

        if (payment == null) return NotFound();

        // ✅ OrderInfo theo Purpose (không phá EntranceExam cũ)
        var orderInfo = payment.Purpose switch
        {
            PaymentPurpose.EntranceExam => $"Le phi du thi - {payment.Guest?.FullName}",
            PaymentPurpose.Course => $"Hoc phi khoa hoc - {payment.Guest?.FullName}",
            PaymentPurpose.Subject => $"Hoc phi mon hoc - {payment.Guest?.FullName}",
            _ => $"Thanh toan - {payment.Guest?.FullName}"
        };

        var txn = new VNPayTransaction
        {
            Id = Guid.NewGuid().ToString(),
            PaymentId = payment.Id,
            VnpTxnRef = DateTime.Now.Ticks.ToString(),
            VnpAmount = (long)(payment.Amount * 100),
            VnpOrderInfo = orderInfo,
            VnpCreateDate = DateTime.Now.ToString("yyyyMMddHHmmss"),
            VnpBankCode = "",
            Status = VNPayTransactionStatus.Pending
        };

        _context.VNPayTransactions.Add(txn);
        await _context.SaveChangesAsync();

        var vnp = new VNPayLibrary();
        vnp.AddRequestData("vnp_Version", "2.1.0");
        vnp.AddRequestData("vnp_Command", "pay");
        vnp.AddRequestData("vnp_TmnCode", _config["Vnpay:TmnCode"]);
        vnp.AddRequestData("vnp_Amount", txn.VnpAmount.ToString());
        vnp.AddRequestData("vnp_BankCode", txn.VnpBankCode);
        vnp.AddRequestData("vnp_CreateDate", txn.VnpCreateDate);
        vnp.AddRequestData("vnp_CurrCode", "VND");
        vnp.AddRequestData("vnp_IpAddr", HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1");
        vnp.AddRequestData("vnp_Locale", "vn");
        vnp.AddRequestData("vnp_OrderInfo", txn.VnpOrderInfo);
        vnp.AddRequestData("vnp_OrderType", "other");
        vnp.AddRequestData("vnp_ReturnUrl", _config["Vnpay:ReturnUrl"]); // phải là /VNPay/Return
        vnp.AddRequestData("vnp_TxnRef", txn.VnpTxnRef);

        var paymentUrl = vnp.CreateRequestUrl(
            _config["Vnpay:BaseUrl"],
            _config["Vnpay:HashSecret"]
        );

        return Redirect(paymentUrl);
    }

    public async Task<IActionResult> Return()
    {
        var vnp = new VNPayLibrary();

        foreach (var key in Request.Query.Keys)
        {
            if (key.StartsWith("vnp_"))
                vnp.AddResponseData(key, Request.Query[key].ToString());
        }

        var secureHash = Request.Query["vnp_SecureHash"].ToString();
        if (!vnp.ValidateSignature(secureHash, _config["Vnpay:HashSecret"]))
            return View("VNPayError");

        string txnRef = Request.Query["vnp_TxnRef"].ToString();
        string responseCode = Request.Query["vnp_ResponseCode"].ToString();

        if (string.IsNullOrEmpty(txnRef))
            return View("VNPayError");

        var txn = await _context.VNPayTransactions
            .Include(t => t.Payment)
                .ThenInclude(p => p.Guest)
            .FirstOrDefaultAsync(t => t.VnpTxnRef == txnRef);

        if (txn == null)
            return View("VNPayError");

        if (responseCode == "00")
        {
            txn.Status = VNPayTransactionStatus.Success;
            txn.Payment.Status = PaymentStatus.Paid;
            txn.Payment.PaymentDate = DateTime.Now;

            // Guest status
            if (txn.Payment.Guest != null)
                txn.Payment.Guest.Status = GuestRegistrationStatus.PaidPendingApproval;

            // ✅ Nếu thanh toán COURSE (hoặc SUBJECT) thành công -> nâng role Guest -> Student
            if (txn.Payment.Purpose == PaymentPurpose.Course || txn.Payment.Purpose == PaymentPurpose.Subject)
            {
                var studentRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == RoleNames.Student);
                if (studentRole != null && txn.Payment.Guest?.UserId != null)
                {
                    var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == txn.Payment.Guest.UserId);
                    if (user != null)
                        user.RoleId = studentRole.Id;
                }
            }
        }
        else
        {
            txn.Status = VNPayTransactionStatus.Failed;
            txn.Payment.Status = PaymentStatus.Failed;
        }

        await _context.SaveChangesAsync();
        return View(responseCode == "00" ? "VNPaySuccess" : "VNPayFailed", txn);
    }
}
