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

        public string StudentId { get; set; } = string.Empty;
        [ForeignKey("StudentId")]
        public User? Student { get; set; }

        public decimal Amount { get; set; }

        public PaymentMethod PaymentMethod { get; set; }

        public DateTime PaymentDate { get; set; }

        public string ReceiptNumber { get; set; } = string.Empty;
    }
}
