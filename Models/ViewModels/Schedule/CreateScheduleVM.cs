using System.ComponentModel.DataAnnotations;

namespace Symphony.Portal.Web.ViewModels.Schedule
{
    public class CreateScheduleVM
    {
        [Required]
        public string ClassId { get; set; } = default!;

        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; } = DateTime.Today;

        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        // UI helper (optional)
        public string? ClassName { get; set; }
    }
}
