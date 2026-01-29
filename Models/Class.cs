using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Symphony.Portal.Web.Models.Enums;

namespace Symphony.Portal.Web.Models
{
    public class Class
    {
        [Key]
        [StringLength(36)]
        public string Id { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Class Name")]
        public string ClassName { get; set; } = string.Empty;

        [Required]
        public string ClassCategoryId { get; set; }
        [ForeignKey("ClassCategoryId")]
        public ClassCategory? ClassCategory { get; set; }

        [Display(Name = "Number of Seats")]
        public int NumberOfSeats { get; set; }

        public ClassStatus Status { get; set; } = ClassStatus.Active;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public ICollection<RevisionRegistration> RevisionRegistrations { get; set; }

    }
}
