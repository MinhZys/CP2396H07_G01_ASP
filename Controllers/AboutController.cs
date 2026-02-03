using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Symphony.Portal.Web.Data;
using Symphony.Portal.Web.Models.ViewModels;

namespace Symphony.Portal.Web.Controllers
{
    public class AboutController : Controller
    {
        private readonly AppDbContext _context;

        public AboutController(AppDbContext context)
        {
            _context = context;
        }

        // /page/about-us
        [Route("page/about-us")]
        public async Task<IActionResult> Index()
        {
            var about = await _context.PageContents
                .AsNoTracking()
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Slug == "about-us" && p.IsActive);

            if (about == null) return NotFound();

            // sort ảnh cho ổn định khi render
            about.Images = about.Images
                .OrderBy(x => x.SortOrder)
                .ToList();

            var faqs = await _context.FAQs
                .AsNoTracking()
                .Where(f => f.IsActive)
                .OrderBy(f => f.DisplayOrder)
                .ToListAsync();

            var vm = new AboutFaqViewModel
            {
                AboutPage = about,
                FAQs = faqs
            };

            return View(vm); // Views/About/Index.cshtml
        }
    }
}
