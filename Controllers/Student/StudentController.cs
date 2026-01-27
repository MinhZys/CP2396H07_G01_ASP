using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Symphony.Portal.Web.Data;
using System.Security.Claims;

namespace Symphony.Portal.Web.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private readonly AppDbContext _context;

        public StudentController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Student/ViewClasses
        public IActionResult ViewClasses()
        {
            var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            Console.WriteLine("LOGIN StudentId = " + studentId);

            var classes = _context.ClassAssignments
                .Where(ca => ca.StudentId == studentId)
                .Include(ca => ca.Class)
                .ThenInclude(c => c.ClassCategory)
                .ToList();

            Console.WriteLine("Classes count = " + classes.Count);

            return View("ViewClasses/ViewClasses", classes);
        }



    }
}
