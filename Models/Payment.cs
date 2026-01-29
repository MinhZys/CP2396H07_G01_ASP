using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Symphony.Portal.Web.Models.Enums;

namespace Symphony.Portal.Web.Models
{
    public class Payment
    {
        [Key]
        [StringLength(36)]
        public string Id { get; set; } = string.Empty;

        public string? StudentId { get; set; }
        [ForeignKey("StudentId")]
        public User? Student { get; set; }

        public string? GuestId { get; set; }
        [ForeignKey("GuestId")]
        public Guest? Guest { get; set; }

        public decimal Amount { get; set; }

        public PaymentMethod PaymentMethod { get; set; }

        [Required]
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

        public DateTime PaymentDate { get; set; }

        public string ReceiptNumber { get; set; } = string.Empty;
        public PaymentPurpose Purpose { get; set; }

    }
}
