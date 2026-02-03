using System.ComponentModel.DataAnnotations;
using Symphony.Portal.Web.Models;
using Symphony.Portal.Web.Models.Enums;

namespace Symphony.Portal.Web.ViewModels.Schedule
{
    public class SessionVM
    {
        public int Id { get; set; }

        [Required]
        public int ClassScheduleId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime SessionDate { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }

        public int? RoomId { get; set; }
        public string? InstructorId { get; set; }

        [Required]
        public SessionType SessionType { get; set; } = SessionType.Theory;

        [MaxLength(500)]
        public string? Note { get; set; }

        public bool IsCancelled { get; set; }
        public string? CancelReason { get; set; }

        // UI helper text (không bind xuống DB)
        public string? RoomName { get; set; }
        public string? InstructorName { get; set; }
        public string? ClassName { get; set; }
    }
}
