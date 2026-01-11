using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Symphony.Portal.Web.Models
{
    public class CourseSubject
    {
        public string CourseId { get; set; } = string.Empty;
        public Course? Course { get; set; }

        public string SubjectId { get; set; } = string.Empty;
        public Subject? Subject { get; set; }
    }
}
