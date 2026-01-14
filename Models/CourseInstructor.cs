using System.ComponentModel.DataAnnotations.Schema;

namespace Symphony.Portal.Web.Models
{
    public class CourseInstructor
    {
        public string CourseId { get; set; }
        [ForeignKey("CourseId")]
        public Course Course { get; set; }

        public string InstructorId { get; set; }
        [ForeignKey("InstructorId")]
        public User Instructor { get; set; }
    }
}
