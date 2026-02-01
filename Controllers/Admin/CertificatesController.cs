using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Symphony.Portal.Web.Data;
using Symphony.Portal.Web.Models;
using Symphony.Portal.Web.Models.ViewModels;

namespace Symphony.Portal.Web.Controllers.Admin
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CertificatesController : Controller
    {
        private readonly AppDbContext _context;

        public CertificatesController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchString, int? pageNumber)
        {
            var certificates = from c in _context.Certificates
                             select c;

            if (!string.IsNullOrEmpty(searchString))
            {
                certificates = certificates.Where(s => s.Name.Contains(searchString));
            }

            ViewData["CurrentFilter"] = searchString;

            int pageSize = 10;
            return View(await PaginatedList<Certificate>.CreateAsync(certificates.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,IsActive")] Certificate certificate)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(certificate.Id)) certificate.Id = Guid.NewGuid().ToString();
                
                _context.Add(certificate);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Certificate created successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(certificate);
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (id == null) return NotFound();

            var certificate = await _context.Certificates.FindAsync(id);
            if (certificate == null) return NotFound();
            return View(certificate);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Name,Description,IsActive")] Certificate certificate)
        {
            if (id != certificate.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(certificate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CertificateExists(certificate.Id)) return NotFound();
                    else throw;
                }
                TempData["Success"] = "Certificate updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(certificate);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var certificate = await _context.Certificates.FindAsync(id);
            if (certificate != null)
            {
                if (await _context.Courses.AnyAsync(c => c.CertificateId == id))
                {
                     TempData["Error"] = "Cannot delete this certificate because it is being used by one or more courses.";
                     return RedirectToAction(nameof(Index));
                }

                _context.Certificates.Remove(certificate);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Certificate deleted successfully.";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool CertificateExists(string id)
        {
            return _context.Certificates.Any(e => e.Id == id);
        }
    }
}
