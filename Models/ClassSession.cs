using System;
using System.ComponentModel.DataAnnotations;

namespace Symphony.Portal.Web.Models
{
    public class ClassSession
    {
        public int Id { get; set; }

        [Required]
        public int ClassScheduleId { get; set; }

        public ClassSchedule? ClassSchedule { get; set; }

        [Required]
        public DateTime SessionDate { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }

        public int? RoomId { get; set; }
        public Room? Room { get; set; }

        public string? InstructorId { get; set; }

        [Required]
        public SessionType SessionType { get; set; } = SessionType.Theory;

        [MaxLength(500)]
        public string? Note { get; set; }

        public bool IsCancelled { get; set; } = false;

        [MaxLength(300)]
        public string? CancelReason { get; set; }

        public int? RescheduledFromSessionId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public bool IsValidTimeRange()
            => EndTime > StartTime;
    }
}
