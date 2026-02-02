using System.ComponentModel.DataAnnotations;
using Symphony.Portal.Web.Models;
using Symphony.Portal.Web.Models.Enums;

namespace Symphony.Portal.Web.ViewModels.Schedule
{
    public class GenerateScheduleVM
    {
        [Required]
        public int ScheduleId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime FromDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime ToDate { get; set; }

        /// <summary>
        /// Thứ trong tuần theo ISO: 1..7 (Mon..Sun)
        /// </summary>
        [Required]
        [MinLength(1, ErrorMessage = "Chọn ít nhất 1 ngày trong tuần.")]
        public List<int> DaysOfWeek { get; set; } = new();

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }

        public int? RoomId { get; set; }
        public string? InstructorId { get; set; }

        [Required]
        public SessionType SessionType { get; set; } = SessionType.Theory;

        public string? Note { get; set; }

        // UI helper (optional)
        public string? ClassName { get; set; }
    }
}
