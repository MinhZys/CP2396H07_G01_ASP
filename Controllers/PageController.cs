using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Symphony.Portal.Web.Data;

namespace Symphony.Portal.Web.Controllers
{
    public class PageController : Controller
    {
        private readonly AppDbContext _context;

        public PageController(AppDbContext context)
        {
            _context = context;
        }

        // Redirect old About page to new /about (SEO-safe)
        [Route("page/about-us")]
        public IActionResult AboutRedirect()
        {
            return RedirectToActionPermanent("Index", "About");
        }

        // /page/{slug}
        [Route("page/{slug}")]
        public async Task<IActionResult> View(string slug)
        {
            slug = (slug ?? string.Empty).Trim().ToLowerInvariant();

            var page = await _context.PageContents
                .AsNoTracking()
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Slug == slug && p.IsActive);

            if (page == null) return NotFound();

            // sort ảnh cho ổn định khi render
            page.Images = page.Images
                .OrderBy(x => x.SortOrder)
                .ToList();

            return View(page); // Views/Page/View.cshtml
        }
    }
}
