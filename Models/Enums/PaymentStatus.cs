namespace Symphony.Portal.Web.Models.Enums
{
    public enum PaymentStatus
    {
        Pending = 0,     // Đã tạo, chưa thanh toán
        Paid = 1,        // Đã thanh toán
        Failed = 2,      // Thanh toán thất bại
        Cancelled = 3,   // Hủy
        Refunded = 4     // Hoàn tiền (nếu có)
    }
}
