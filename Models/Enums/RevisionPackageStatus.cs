namespace Symphony.Portal.Web.Models.Enums
{
    public enum RevisionPackageStatus
    {
        Draft,     // admin tạo
        Open,      // cho guest đăng ký
        Full,      // đủ số lượng
        Assigned,  // đã gán vào lớp
        Closed
    }
}
