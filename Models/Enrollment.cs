using System.ComponentModel.DataAnnotations.Schema;
using Symphony.Portal.Web.Models.Identity;

namespace Symphony.Portal.Web.Models
{
    public class Enrollment
    {
        public int Id { get; set; }
        
        public int ClassId { get; set; }
        public Class? Class { get; set; }

        public string StudentId { get; set; } = string.Empty;
        [ForeignKey("StudentId")]
        public ApplicationUser? Student { get; set; }

        public DateTime EnrolledDate { get; set; } 
    }
}
