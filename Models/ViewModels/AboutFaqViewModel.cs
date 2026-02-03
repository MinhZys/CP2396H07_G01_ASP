using Symphony.Portal.Web.Models;
using System.Collections.Generic;

namespace Symphony.Portal.Web.Models.ViewModels
{
    public class AboutFaqViewModel
    {
        public PageContent AboutPage { get; set; } = default!;
        public List<FAQ> FAQs { get; set; } = new();
    }
}
