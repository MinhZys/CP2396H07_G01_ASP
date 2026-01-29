using System;
using System.ComponentModel.DataAnnotations;
using Symphony.Portal.Web.Models.Enums;

namespace Symphony.Portal.Web.Models
{
    public class GuestRegistrationVm
    {
        [Required, StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required, EmailAddress, StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required, StringLength(15)]
        [RegularExpression(@"^(0|\+84)(3|5|7|8|9)[0-9]{8}$",
            ErrorMessage = "Số điện thoại không hợp lệ (VD: 0912345678 hoặc +84912345678)")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required, DataType(DataType.Date)]
        public DateTime Dob { get; set; }

        [StringLength(255)]
        public string Address { get; set; } = string.Empty;

        // Mục đích thanh toán
        [Required]
        public PaymentPurpose Purpose { get; set; }

        // Thi đầu vào
        public string? SelectedEntranceExamId { get; set; }

        // Đăng ký học (hiện model Guest có ClassId)
        public string? ClassId { get; set; }

        [Required]
        public PaymentMethod PaymentMethod { get; set; }
    }
}
