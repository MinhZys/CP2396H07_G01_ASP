using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
        private readonly IWebHostEnvironment _env;

        public CMSController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
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
                if (string.IsNullOrEmpty(faq.Id)) faq.Id = Guid.NewGuid().ToString();

                _context.Add(faq);
                await _context.SaveChangesAsync();
                TempData["Success"] = "FAQ created successfully.";
                return RedirectToAction(nameof(ManageFAQs));
            }
            return View(faq);
        }

        public async Task<IActionResult> EditFAQ(string? id)
        {
            if (id == null) return NotFound();
            var faq = await _context.FAQs.FindAsync(id);
            if (faq == null) return NotFound();
            return View(faq);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditFAQ(string id, FAQ faq)
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
                TempData["Success"] = "FAQ updated successfully.";
                return RedirectToAction(nameof(ManageFAQs));
            }
            return View(faq);
        }

        public async Task<IActionResult> DeleteFAQ(string? id)
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
            var pages = await _context.PageContents
                .Include(p => p.Images)
                .OrderByDescending(p => p.LastUpdated)
                .ToListAsync();

            // đảm bảo FeaturedImage sync nếu có Images.IsFeatured (optional nhưng tốt)
            foreach (var p in pages)
            {
                if (p.Images != null && p.Images.Count > 0)
                {
                    var featured = p.Images.OrderBy(x => x.SortOrder).FirstOrDefault(x => x.IsFeatured)
                                  ?? p.Images.OrderBy(x => x.SortOrder).FirstOrDefault();

                    if (featured != null)
                        p.FeaturedImage = featured.ImageUrl;
                }
            }

            return View(pages);
        }


        // ---------- CREATE ----------
        public IActionResult CreatePage()
        {
            return View(new PageContent { IsActive = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePage(PageContent page, List<IFormFile>? imageFiles)
        {
            if (!ModelState.IsValid) return View(page);

            page.Slug = (page.Slug ?? string.Empty).Trim().ToLowerInvariant();

            if (string.IsNullOrWhiteSpace(page.Slug))
            {
                ModelState.AddModelError(nameof(PageContent.Slug), "Slug is required.");
                return View(page);
            }

            // chặn trùng slug
            var slugExists = await _context.PageContents.AnyAsync(p => p.Slug == page.Slug);
            if (slugExists)
            {
                ModelState.AddModelError(nameof(PageContent.Slug), "Slug already exists. Please choose another one.");
                return View(page);
            }

            // normalize file list
            var files = (imageFiles ?? new List<IFormFile>())
                .Where(f => f != null && f.Length > 0)
                .ToList();

            if (files.Count > 10)
            {
                ModelState.AddModelError(string.Empty, "You can upload up to 10 images only.");
                return View(page);
            }

            static bool IsAllowedImage(IFormFile f)
            {
                var allowedExt = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                { ".jpg", ".jpeg", ".png", ".webp", ".gif" };

                var ext = Path.GetExtension(f.FileName);
                if (string.IsNullOrWhiteSpace(ext) || !allowedExt.Contains(ext)) return false;

                if (string.IsNullOrWhiteSpace(f.ContentType)) return false;
                if (!f.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase)) return false;

                return true;
            }

            foreach (var f in files)
            {
                if (!IsAllowedImage(f))
                {
                    ModelState.AddModelError(string.Empty, $"File '{f.FileName}' is not a supported image type.");
                    return View(page);
                }
            }

            page.Id = Guid.NewGuid().ToString();
            page.LastUpdated = DateTime.UtcNow;

            _context.PageContents.Add(page);

            // Save multiple images -> PageImages
            for (int i = 0; i < files.Count; i++)
            {
                var url = await SaveImage(files[i]);

                var pi = new PageImage
                {
                    Id = Guid.NewGuid().ToString(),
                    PageContentId = page.Id,
                    ImageUrl = url,
                    SortOrder = i,
                    IsFeatured = (i == 0)
                };

                _context.PageImages.Add(pi);

                // sync featured thumbnail
                if (i == 0)
                    page.FeaturedImage = url;
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = "Page created successfully.";
            return RedirectToAction(nameof(ManagePages));
        }

        // ---------- EDIT ----------
        public async Task<IActionResult> EditPage(string id)
        {
            var page = await _context.PageContents
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (page == null) return NotFound();

            page.Images = page.Images.OrderBy(x => x.SortOrder).ToList();
            return View(page);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPage(string id, PageContent page, List<IFormFile>? imageFiles, string? featuredImageId)
        {
            if (id != page.Id) return NotFound();

            var existing = await _context.PageContents
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (existing == null) return NotFound();

            if (!ModelState.IsValid)
            {
                existing.Images = existing.Images.OrderBy(x => x.SortOrder).ToList();
                return View(existing);
            }

            existing.Title = page.Title;
            existing.Content = page.Content;
            existing.IsActive = page.IsActive;
            existing.LastUpdated = DateTime.UtcNow;

            // add new images (total <= 10)
            var newFiles = (imageFiles ?? new List<IFormFile>())
                .Where(f => f != null && f.Length > 0)
                .ToList();

            if (newFiles.Count > 0)
            {
                static bool IsAllowedImage(IFormFile f)
                {
                    var allowedExt = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                    { ".jpg", ".jpeg", ".png", ".webp", ".gif" };

                    var ext = Path.GetExtension(f.FileName);
                    if (string.IsNullOrWhiteSpace(ext) || !allowedExt.Contains(ext)) return false;

                    if (string.IsNullOrWhiteSpace(f.ContentType)) return false;
                    if (!f.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase)) return false;

                    return true;
                }

                foreach (var f in newFiles)
                {
                    if (!IsAllowedImage(f))
                    {
                        ModelState.AddModelError(string.Empty, $"File '{f.FileName}' is not a supported image type.");
                        existing.Images = existing.Images.OrderBy(x => x.SortOrder).ToList();
                        return View(existing);
                    }
                }

                var currentCount = existing.Images?.Count ?? 0;
                if (currentCount + newFiles.Count > 10)
                {
                    ModelState.AddModelError(string.Empty, $"Total images cannot exceed 10. Current: {currentCount}, adding: {newFiles.Count}.");
                    existing.Images = existing.Images.OrderBy(x => x.SortOrder).ToList();
                    return View(existing);
                }

                int nextOrder = existing.Images.Any() ? existing.Images.Max(x => x.SortOrder) + 1 : 0;

                foreach (var f in newFiles)
                {
                    var url = await SaveImage(f);

                    _context.PageImages.Add(new PageImage
                    {
                        Id = Guid.NewGuid().ToString(),
                        PageContentId = existing.Id,
                        ImageUrl = url,
                        SortOrder = nextOrder++,
                        IsFeatured = false
                    });
                }
            }

            // set featured
            if (!string.IsNullOrWhiteSpace(featuredImageId))
            {
                foreach (var img in existing.Images)
                    img.IsFeatured = (img.Id == featuredImageId);

                var featured = existing.Images.FirstOrDefault(x => x.Id == featuredImageId);
                if (featured != null)
                    existing.FeaturedImage = featured.ImageUrl;
            }
            else
            {
                // ensure at least one featured if images exist
                if (existing.Images.Any() && !existing.Images.Any(x => x.IsFeatured))
                {
                    var first = existing.Images.OrderBy(x => x.SortOrder).First();
                    first.IsFeatured = true;
                    existing.FeaturedImage = first.ImageUrl;
                }
                else
                {
                    var featured = existing.Images.FirstOrDefault(x => x.IsFeatured);
                    if (featured != null)
                        existing.FeaturedImage = featured.ImageUrl;
                }
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = "Page updated successfully.";
            return RedirectToAction(nameof(ManagePages));
        }

        // delete one image
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePageImage(string id, string imageId)
        {
            var img = await _context.PageImages
                .FirstOrDefaultAsync(x => x.Id == imageId && x.PageContentId == id);

            if (img == null)
            {
                TempData["Error"] = "Image not found.";
                return RedirectToAction(nameof(EditPage), new { id });
            }

            _context.PageImages.Remove(img);
            await _context.SaveChangesAsync();

            // recompute featured
            var page = await _context.PageContents
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (page != null)
            {
                // nếu không còn featured hoặc featured bị xoá -> set ảnh đầu làm featured
                var featured = page.Images.OrderBy(x => x.SortOrder).FirstOrDefault(x => x.IsFeatured)
                               ?? page.Images.OrderBy(x => x.SortOrder).FirstOrDefault();

                page.FeaturedImage = featured?.ImageUrl;

                if (featured != null)
                {
                    foreach (var i in page.Images)
                        i.IsFeatured = (i.Id == featured.Id);
                }

                await _context.SaveChangesAsync();
            }

            TempData["Success"] = "Image deleted.";
            return RedirectToAction(nameof(EditPage), new { id });
        }

        // ---------- DELETE ----------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePage(string id)
        {
            var page = await _context.PageContents.FindAsync(id);
            if (page != null)
            {
                // nếu FK cascade không bật, xoá thủ công
                var imgs = await _context.PageImages.Where(x => x.PageContentId == id).ToListAsync();
                if (imgs.Any()) _context.PageImages.RemoveRange(imgs);

                _context.PageContents.Remove(page);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Page deleted successfully.";
            }
            else
            {
                TempData["Error"] = "Page not found.";
            }

            return RedirectToAction(nameof(ManagePages));
        }

        #endregion

        #region Helpers

        private async Task<string> SaveImage(IFormFile file)
        {
            var uploadDir = Path.Combine(_env.WebRootPath, "uploads", "pages");
            Directory.CreateDirectory(uploadDir);

            var ext = Path.GetExtension(file.FileName);
            var fileName = $"{Guid.NewGuid():N}{ext}";
            var filePath = Path.Combine(uploadDir, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return "/uploads/pages/" + fileName;
        }

        #endregion

        #region Contact Management

        public async Task<IActionResult> ManageContact()
        {
            var vm = new Symphony.Portal.Web.Models.ViewModels.ManageContactVM
            {
                Centers = await _context.Centers
                    .OrderBy(c => c.DisplayOrder)
                    .ThenBy(c => c.Name)
                    .ToListAsync(),

                Messages = await _context.ContactMessages
                    .Include(m => m.Center)
                    .OrderByDescending(m => m.CreatedAt)
                    .ToListAsync()
            };

            return View(vm); // Views/Admin/CMS/ManageContact.cshtml
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCenter(Center center)
        {
            if (string.IsNullOrWhiteSpace(center.Name))
            {
                TempData["Error"] = "Center name is required.";
                return RedirectToAction(nameof(ManageContact));
            }

            if (string.IsNullOrEmpty(center.Id))
                center.Id = Guid.NewGuid().ToString();

            _context.Centers.Add(center);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Center created.";
            return RedirectToAction(nameof(ManageContact));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCenter(Center center)
        {
            var existing = await _context.Centers.FindAsync(center.Id);
            if (existing == null)
            {
                TempData["Error"] = "Center not found.";
                return RedirectToAction(nameof(ManageContact));
            }

            existing.Name = center.Name;
            existing.Address = center.Address;
            existing.Phone = center.Phone;
            existing.OpenHours = center.OpenHours;
            existing.Latitude = center.Latitude;
            existing.Longitude = center.Longitude;
            existing.DisplayOrder = center.DisplayOrder;
            existing.IsActive = center.IsActive;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Center updated.";
            return RedirectToAction(nameof(ManageContact));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCenter(string id)
        {
            var center = await _context.Centers.FindAsync(id);
            if (center != null)
            {
                _context.Centers.Remove(center);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Center deleted.";
            }
            else TempData["Error"] = "Center not found.";

            return RedirectToAction(nameof(ManageContact));
        }

        // ------- Messages -------

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkMessageRead(string id)
        {
            var msg = await _context.ContactMessages.FindAsync(id);
            if (msg == null)
            {
                TempData["Error"] = "Message not found.";
                return RedirectToAction(nameof(ManageContact));
            }

            msg.IsRead = true;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Message marked as read.";
            return RedirectToAction(nameof(ManageContact));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMessage(string id)
        {
            var msg = await _context.ContactMessages.FindAsync(id);
            if (msg != null)
            {
                _context.ContactMessages.Remove(msg);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Message deleted.";
            }
            else TempData["Error"] = "Message not found.";

            return RedirectToAction(nameof(ManageContact));
        }

        #endregion
    }
}
