using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Symphony.Portal.Web.Data;
using Symphony.Portal.Web.Models;

namespace Symphony.Portal.Web.Controllers
{
    public class SubjectsController : Controller
    {
        private readonly AppDbContext _context;

        public SubjectsController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchString, string sort, int page = 1)
        {
            var query = _context.Subjects.AsQueryable();

            // Search by Name or Description
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(s => s.Name.Contains(searchString) || s.Description.Contains(searchString));
            }

            // Sorting
            switch (sort)
            {
                case "Name: Z to A":
                    query = query.OrderByDescending(s => s.Name);
                    break;
                case "Study Time: Low to High":
                    query = query.OrderBy(s => s.StudyTime);
                    break;
                case "Study Time: High to Low":
                    query = query.OrderByDescending(s => s.StudyTime);
                    break;
                case "Name: A to Z":
                default:
                    query = query.OrderBy(s => s.Name);
                    break;
            }

            // Pagination
            int pageSize = 12;
            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            ViewBag.CurrentSearch = searchString;
            ViewBag.CurrentSort = sort;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(items);
        }

        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subject = await _context.Subjects
                .FirstOrDefaultAsync(m => m.Id == id);

            if (subject == null)
            {
                return NotFound();
            }

            return View(subject);
        }
    }
}
