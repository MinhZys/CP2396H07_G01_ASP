using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Symphony.Portal.Web.Data;
using Symphony.Portal.Web.Models;
using Symphony.Portal.Web.Models.Enums;

namespace Symphony.Portal.Web.Controllers
{
    public class CoursesController : Controller
    {
        private readonly AppDbContext _context;

        public CoursesController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string[] categories, string level, string sort, int page = 1)
        {
            var query = _context.Courses
                .Include(c => c.Category)
                .Include(c => c.CourseReviews)
                .AsQueryable();

            // Filter by Category
            if (categories != null && categories.Length > 0)
            {
                query = query.Where(c => c.Category != null && categories.Contains(c.Category.Name));
            }

            // Filter by Level
            if (!string.IsNullOrEmpty(level) && Enum.TryParse<CourseLevel>(level, true, out var levelEnum))
            {
                query = query.Where(c => c.Level == levelEnum);
            }

            // Sorting
            switch (sort)
            {
                case "Price: Low to High":
                    query = query.OrderBy(c => c.TuitionFee);
                    break;
                case "Price: High to Low":
                    query = query.OrderByDescending(c => c.TuitionFee);
                    break;
                case "Newest":
                    // Assuming created date or ID implies newness if no Date field
                    query = query.OrderByDescending(c => c.Id); 
                    break;
                case "Most Popular":
                default:
                    // Just default sorting for now
                    query = query.OrderBy(c => c.Title);
                    break;
            }

            // Pagination
            int pageSize = 12;
            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            ViewBag.Categories = await _context.Categories.Select(c => c.Name).Distinct().ToListAsync();
            ViewBag.CurrentCategories = categories;
            ViewBag.CurrentLevel = level;
            ViewBag.CurrentSort = sort;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(items);
        }

        public async Task<IActionResult> Details(string id)
        {
            if (id == null) return NotFound();

            var course = await _context.Courses
                .Include(c => c.Category)
                .Include(c => c.CourseSubjects).ThenInclude(cs => cs.Subject)
                .Include(c => c.CourseInstructors).ThenInclude(ci => ci.Instructor)
                .Include(c => c.CourseReviews).ThenInclude(cr => cr.Student)
                .Include(c => c.CourseReviews).ThenInclude(cr => cr.Student)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (course == null) return NotFound();
            
            // Calculate Average Rating
            ViewBag.AverageRating = course.CourseReviews.Any() ? course.CourseReviews.Average(r => r.Rating) : 0;
            ViewBag.ReviewCount = course.CourseReviews.Count;

            return View(course);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostReview(string id, int rating, string reviewText)
        {
            // Simple check if user is logged in
            // For now assuming we have a way to get current user ID, e.g. from claims or session
            // In a real app we'd use User.FindFirstValue(ClaimTypes.NameIdentifier)
            
            var userId = HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Details", new { id }) });
            }

            if (string.IsNullOrEmpty(id) || rating < 1 || rating > 5)
            {
                 return RedirectToAction(nameof(Details), new { id });
            }

            var review = new CourseReview
            {
                Id = Guid.NewGuid().ToString(),
                CourseId = id,
                StudentId = userId,
                Rating = rating,
                ReviewText = reviewText ?? string.Empty,
                ReviewDate = DateTime.Now,
                IsApproved = true // Auto-approve for now
            };

            _context.CourseReviews.Add(review);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id });
        }
    }
}
