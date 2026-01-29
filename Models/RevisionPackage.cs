using Symphony.Portal.Web.Models.Enums;

namespace Symphony.Portal.Web.Models
{
    public class RevisionPackage
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string Title { get; set; }
        public string Description { get; set; }

        public decimal Fee { get; set; }

        public int MaxStudents { get; set; }
        public int CurrentStudents { get; set; }

        public RevisionPackageStatus Status { get; set; }
        // Open / Full / Closed

        public DateTime CreatedAt { get; set; }

        public ICollection<RevisionRegistration> Registrations { get; set; }
    }
}