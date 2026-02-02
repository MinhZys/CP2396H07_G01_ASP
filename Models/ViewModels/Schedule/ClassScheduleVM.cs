using Symphony.Portal.Web.Models;

namespace Symphony.Portal.Web.ViewModels.Schedule
{
    public class ClassScheduleVM
    {
        public int Id { get; set; }

        public string ClassId { get; set; } = default!;
        public string? ClassName { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public ScheduleStatus Status { get; set; }

        public bool IsPublished { get; set; }
        public bool IsLocked { get; set; }

        public DateTime? PublishedAt { get; set; }
        public string? PublishedByUserId { get; set; }

        public int TotalSessions { get; set; }

        // dùng cho màn Details
        public List<SessionVM> Sessions { get; set; } = new();
    }
}
