using System.ComponentModel.DataAnnotations;

namespace Symphony.Portal.Web.Models.ViewModels
{
    public class VerifyCodeVM
    {
        [Required(ErrorMessage = "Vui lòng nhập mã xác thực")]
        public string Code { get; set; } = string.Empty;

        public string? Email { get; set; } // To keep track of who is verifying
    }
}
