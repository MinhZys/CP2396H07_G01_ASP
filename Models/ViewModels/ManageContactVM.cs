using Symphony.Portal.Web.Models;
using System.Collections.Generic;

namespace Symphony.Portal.Web.Models.ViewModels
{
    public class ManageContactVM
    {
        // phần 1: centers
        public List<Center> Centers { get; set; } = new();

        // phần 2: messages
        public List<ContactMessage> Messages { get; set; } = new();

        // create forms
        public Center NewCenter { get; set; } = new();
    }
}
