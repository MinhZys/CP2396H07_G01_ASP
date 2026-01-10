using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Symphony.Portal.Web.Data;
using Symphony.Portal.Web.Models;

namespace Symphony.Portal.Web.Controllers.Admin
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    [Route("Admin/[controller]/[action]")]
    public class CMSController : Controller
    {
        private readonly AppDbContext _context;

        public CMSController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        #region FAQ Management
        public async Task<IActionResult> ManageFAQs()
        {
            return View(await _context.FAQs.OrderBy(f => f.DisplayOrder).ToListAsync());
        }

        public IActionResult CreateFAQ()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFAQ(FAQ faq)
        {
            if (ModelState.IsValid)
            {
                _context.Add(faq);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ManageFAQs));
            }
            return View(faq);
        }

        public async Task<IActionResult> EditFAQ(int? id)
        {
            if (id == null) return NotFound();
            var faq = await _context.FAQs.FindAsync(id);
            if (faq == null) return NotFound();
            return View(faq);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditFAQ(int id, FAQ faq)
        {
            if (id != faq.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(faq);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.FAQs.Any(e => e.Id == faq.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(ManageFAQs));
            }
            return View(faq);
        }

        public async Task<IActionResult> DeleteFAQ(int? id)
        {
             if (id == null) return NotFound();
             var faq = await _context.FAQs.FindAsync(id);
             if (faq != null)
             {
                 _context.FAQs.Remove(faq);
                 await _context.SaveChangesAsync();
             }
             return RedirectToAction(nameof(ManageFAQs));
        }
        #endregion

        #region Page Content Management
        public async Task<IActionResult> ManagePages()
        {
            // Seed default pages if not exist
            var defaultSlugs = new[] { "about-us", "why-join-us", "how-to-join", "contact-us" };
            foreach (var slug in defaultSlugs)
            {
                if (!await _context.PageContents.AnyAsync(p => p.Slug == slug))
                {
                    _context.PageContents.Add(new PageContent
                    {
                        Slug = slug,
                        Title = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(slug.Replace("-", " ")),
                        Content = "Content goes here..."
                    });
                }
            }
            if (_context.ChangeTracker.HasChanges()) await _context.SaveChangesAsync();

            return View(await _context.PageContents.ToListAsync());
        }

        public async Task<IActionResult> EditPage(int? id)
        {
            if (id == null) return NotFound();
            var page = await _context.PageContents.FindAsync(id);
            if (page == null) return NotFound();
            return View(page);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPage(int id, PageContent page)
        {
            if (id != page.Id) return NotFound();

            if (ModelState.IsValid)
            {
                 try
                {
                    page.LastUpdated = DateTime.Now;
                    _context.Update(page);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.PageContents.Any(e => e.Id == page.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(ManagePages));
            }
            return View(page);
        }
        #endregion
    }
}
