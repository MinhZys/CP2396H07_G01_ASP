namespace Symphony.Portal.Web.ViewModels.Schedule
{
    public class WeeklyScheduleVM
    {
        // khoảng thời gian hiển thị
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        // context cho UI
        public string? Title { get; set; }           // "Lịch dạy tuần", "Lịch học tuần"
        public string? ClassId { get; set; }
        public string? ClassName { get; set; }
        public string? InstructorId { get; set; }
        public string? InstructorName { get; set; }

        public int? ScheduleId { get; set; }

        // danh sách session trong tuần
        public List<SessionVM> Sessions { get; set; } = new();

        // helper: nhóm theo ngày để vẽ calendar dễ hơn
        public Dictionary<DateTime, List<SessionVM>> SessionsByDate
        {
            get
            {
                return Sessions
                    .OrderBy(s => s.SessionDate)
                    .ThenBy(s => s.StartTime)
                    .GroupBy(s => s.SessionDate.Date)
                    .ToDictionary(g => g.Key, g => g.ToList());
            }
        }
    }
}
