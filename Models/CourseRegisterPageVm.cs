using Symphony.Portal.Web.Models;

namespace Symphony.Portal.Web.Models
{
    public class CourseRegisterPageVm
    {
        public Course Course { get; set; } = default!;
        public GuestRegistrationVm Guest { get; set; } = new GuestRegistrationVm();
        public string CourseId => Course.Id;
    }
}
