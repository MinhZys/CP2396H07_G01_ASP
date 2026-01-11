using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Symphony.Portal.Web.Data;
using Symphony.Portal.Web.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Symphony.Portal.Web.Controllers.Admin
{
    [Area("Admin")]
    [Authorize(Roles = RoleNames.Admin)]
    [Route("Admin/[controller]/[action]")]
    public class CentersController : Controller
    {
        private readonly AppDbContext _context;

        public CentersController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Centers.ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Center center)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(center.Id)) center.Id = Guid.NewGuid().ToString();
                _context.Add(center);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(center);
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (id == null) return NotFound();
            var center = await _context.Centers.FindAsync(id);
            if (center == null) return NotFound();
            return View(center);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Center center)
        {
            if (id != center.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(center);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CenterExists(center.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(center);
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (id == null) return NotFound();
            var center = await _context.Centers.FirstOrDefaultAsync(m => m.Id == id);
            if (center == null) return NotFound();
            return View(center);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var center = await _context.Centers.FindAsync(id);
            if (center != null)
            {
                _context.Centers.Remove(center);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool CenterExists(string id)
        {
            return _context.Centers.Any(e => e.Id == id);
        }
    }
}
