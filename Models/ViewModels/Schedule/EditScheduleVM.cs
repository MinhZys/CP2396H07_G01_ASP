using System.ComponentModel.DataAnnotations;

namespace Symphony.Portal.Web.ViewModels.Schedule
{
    public class EditScheduleVM
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string ClassId { get; set; } = default!;

        [DataType(DataType.Date)]
        [Required]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        // UI helper
        public string? ClassName { get; set; }

        // giữ thêm để view hiển thị trạng thái
        public bool IsPublished { get; set; }
        public bool IsLocked { get; set; }
    }
}
