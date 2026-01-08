using Microsoft.AspNetCore.Identity;

namespace Symphony.Portal.Web.Models.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
    }
}
