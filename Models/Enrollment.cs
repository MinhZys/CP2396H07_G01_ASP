using System.ComponentModel.DataAnnotations.Schema;

namespace Symphony.Portal.Web.Models
{
    public class Enrollment
    {
        public int Id { get; set; }
        
        public int ClassId { get; set; }
        public Class? Class { get; set; }

        public int StudentId { get; set; }
        [ForeignKey("StudentId")]
        public User? Student { get; set; }

        public DateTime EnrolledDate { get; set; } = DateTime.Now;
        
        public bool IsApproved { get; set; } = false;
        public bool IsPaid { get; set; } = false;
        public string? PaymentReference { get; set; } // Receipt or Check No 
    }
}
