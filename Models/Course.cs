using System.ComponentModel.DataAnnotations;

namespace Symphony.Portal.Web.Models
{
    public class Course
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;
        
        public string Description { get; set; } = string.Empty;
        
        [Range(0, 10000)]
        public decimal TuitionFee { get; set; }
        
        public int DurationMonths { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public ICollection<Class> Classes { get; set; } = new List<Class>();
    }
}
