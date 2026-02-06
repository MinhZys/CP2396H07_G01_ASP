using Symphony.Portal.Web.Models;
using Symphony.Portal.Web.Models.ViewModels;

namespace Symphony.Portal.Web.Models.ViewModels
{
    public class AdminDashboardVM
    {
        public int StudentCount { get; set; }
        public int InstructorCount { get; set; }
        public int CourseCount { get; set; }
        public int UpcomingExamCount { get; set; }
        public int PendingRegistrationCount { get; set; }
    }

    public class InstructorDashboardVM
    {
        public int MyClassCount { get; set; }
        public int UpcomingExamCount { get; set; }
        public int PendingGradingCount { get; set; }
        public int MaterialCount { get; set; }
    }

    public class StudentDashboardVM
    {
        public int MyClassCount { get; set; }
        public int UpcomingExamCount { get; set; }
        public int UnreadNotificationCount { get; set; }
        public double? LatestGrade { get; set; }
        public List<Assignment> RecentAssignments { get; set; } = new();
    }
}
