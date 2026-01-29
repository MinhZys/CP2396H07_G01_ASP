using Symphony.Portal.Web.Models.Enums;

namespace Symphony.Portal.Web.Models
{
public class RevisionRegistration
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    // Guest info
    public string FullName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }

    // FK gói ôn
    public string RevisionPackageId { get; set; }
    public RevisionPackage RevisionPackage { get; set; }

    // Sau này admin gán lớp
    public string? ClassId { get; set; }
    public Class? Class { get; set; }

    public GuestRegistrationStatus Status { get; set; }
    // PendingPayment / PaidPendingApproval / Approved

    public DateTime CreatedAt { get; set; }
}
}