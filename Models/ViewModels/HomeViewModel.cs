using Symphony.Portal.Web.Models;

namespace Symphony.Portal.Web.Models.ViewModels
{
    public class HomeViewModel
    {
        public IEnumerable<Course> FeaturedCourses { get; set; } = new List<Course>();
        public IEnumerable<AdmissionExam> UpcomingExams { get; set; } = new List<AdmissionExam>();
    }
}
