using System.ComponentModel.DataAnnotations;

namespace Symphony.Portal.Web.Models.ViewModels
{
    public class ProfileVM
    {
        public string UserId { get; set; } = string.Empty;
        
        [Display(Name = "Full Name")]
        [Required(ErrorMessage = "Please enter your full name")]
        public string FullName { get; set; } = string.Empty;
        
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Role")]
        public string Role { get; set; } = string.Empty;

        public bool IsInstructor { get; set; }
        public bool IsGuest { get; set; }

        // Common Profile Fields
        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [Display(Name = "Gender")]
        public string Gender { get; set; } = string.Empty;

        [Display(Name = "Phone Number")]
        [Phone(ErrorMessage = "Invalid phone number")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Display(Name = "Address")]
        public string AddressLine { get; set; } = string.Empty;

        [Display(Name = "Avatar")]
        public string AvatarUrl { get; set; } = string.Empty;

        [Display(Name = "Upload New Avatar")]
        public IFormFile? AvatarImage { get; set; }

        // Instructor Specific
        [Display(Name = "Years of Experience")]
        public int YearsOfExperience { get; set; }

        [Display(Name = "Specialization")]
        public string Specialization { get; set; } = string.Empty;

        [Display(Name = "Bio")]
        public string Bio { get; set; } = string.Empty;
        
        [Display(Name = "Certifications")]
        public string Certifications { get; set; } = string.Empty;

        [Display(Name = "Github URL")]
        public string GithubUrl { get; set; } = string.Empty;
    }
}
