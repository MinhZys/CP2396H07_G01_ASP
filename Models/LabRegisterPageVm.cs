namespace Symphony.Portal.Web.Models
{
    public class LabRegisterPageVm
    {
        public GuestRegistrationVm Guest { get; set; } = new GuestRegistrationVm();
        public string? SelectedClassId { get; set; }
    }
}
