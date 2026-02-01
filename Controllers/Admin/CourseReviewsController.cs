using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Symphony.Portal.Web.Data;
using Symphony.Portal.Web.Models;
using Symphony.Portal.Web.Models.ViewModels;

namespace Symphony.Portal.Web.Controllers.Admin
{
    [Area("Admin")]
    public class CourseReviewsController : Controller
    {
        private readonly AppDbContext _context;

        public CourseReviewsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Admin/CourseReviews
        public async Task<IActionResult> Index(int? pageNumber)
        {
            var reviews = _context.CourseReviews
                .Include(c => c.Course)
                .Include(c => c.Student)
                .AsNoTracking();

            int pageSize = 10;
            return View(await PaginatedList<CourseReview>.CreateAsync(reviews, pageNumber ?? 1, pageSize));
        }

        // GET: Admin/CourseReviews/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null) return NotFound();

            var courseReview = await _context.CourseReviews
                .Include(c => c.Course)
                .Include(c => c.Student)
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (courseReview == null) return NotFound();

            return View(courseReview);
        }

        // GET: Admin/CourseReviews/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null) return NotFound();

            var courseReview = await _context.CourseReviews
                .Include(c => c.Course)
                .Include(c => c.Student)
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (courseReview == null) return NotFound();

            return View(courseReview);
        }

        // POST: Admin/CourseReviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var courseReview = await _context.CourseReviews.FindAsync(id);
            if (courseReview != null)
            {
                _context.CourseReviews.Remove(courseReview);
            }
            
            await _context.SaveChangesAsync();
            TempData["Success"] = "Course Review deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
