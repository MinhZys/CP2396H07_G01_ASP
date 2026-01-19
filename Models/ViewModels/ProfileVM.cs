using System.ComponentModel.DataAnnotations;

namespace Symphony.Portal.Web.Models.ViewModels
{
    public class ProfileVM
    {
        public string UserId { get; set; } = string.Empty;
        
        [Display(Name = "Họ và tên")]
        [Required(ErrorMessage = "Vui lòng nhập họ và tên")]
        public string FullName { get; set; } = string.Empty;
        
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Vai trò")]
        public string Role { get; set; } = string.Empty;

        public bool IsInstructor { get; set; }

        // Common Profile Fields
        [Display(Name = "Ngày sinh")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [Display(Name = "Giới tính")]
        public string Gender { get; set; } = string.Empty;

        [Display(Name = "Số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Display(Name = "Địa chỉ")]
        public string AddressLine { get; set; } = string.Empty;

        [Display(Name = "Ảnh đại diện")]
        public string AvatarUrl { get; set; } = string.Empty;

        [Display(Name = "Tải ảnh lên")]
        public IFormFile? AvatarImage { get; set; }

        // Instructor Specific
        [Display(Name = "Số năm kinh nghiệm")]
        public int YearsOfExperience { get; set; }

        [Display(Name = "Chuyên môn")]
        public string Specialization { get; set; } = string.Empty;

        [Display(Name = "Giới thiệu bản thân")]
        public string Bio { get; set; } = string.Empty;
        
        [Display(Name = "Chứng chỉ")]
        public string Certifications { get; set; } = string.Empty;

        [Display(Name = "Github URL")]
        public string GithubUrl { get; set; } = string.Empty;
    }
}
