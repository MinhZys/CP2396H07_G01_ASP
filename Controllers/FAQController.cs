using Microsoft.AspNetCore.Mvc;
using Symphony.Portal.Web.Data;

namespace Symphony.Portal.Web.Controllers
{
    public class FAQController : Controller
    {
        private readonly AppDbContext _context;

        public FAQController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var faqs = _context.FAQs
                               .Where(f => f.IsActive)
                               .OrderBy(f => f.DisplayOrder)
                               .ToList();

            return View(faqs);
        }
    }
}
