using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Symphony.Portal.Web.Data;
using Symphony.Portal.Web.Models;
using Symphony.Portal.Web.Models.ViewModels;
using System.Security.Claims;

namespace Symphony.Portal.Web.Controllers.Instructor
{
    [Area("Instructor")]
    [Authorize(Roles = RoleNames.Instructor)]
    [Route("Instructor/[controller]/[action]")]
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
            
            var viewModel = new InstructorDashboardVM
            {
                MyClassCount = await _context.Assignments
                    .Where(a => a.InstructorId == userId && a.AssignmentType == Symphony.Portal.Web.Models.Enums.AssignmentType.Teaching)
                    .Select(a => a.ClassId)
                    .Distinct()
                    .CountAsync(),
                
                UpcomingExamCount = await _context.Assignments
                    .CountAsync(a => a.InstructorId == userId && 
                                    (a.AssignmentType == Symphony.Portal.Web.Models.Enums.AssignmentType.Invigilation || 
                                     a.AssignmentType == Symphony.Portal.Web.Models.Enums.AssignmentType.Grading) &&
                                     a.CreatedAt > DateTime.Now.AddDays(-7)), // Rough filter for recent/upcoming
                
                PendingGradingCount = await _context.Assignments
                    .CountAsync(a => a.InstructorId == userId && a.AssignmentType == Symphony.Portal.Web.Models.Enums.AssignmentType.Grading && a.Status == Symphony.Portal.Web.Models.Enums.AssignmentStatus.Assigned),
                
                MaterialCount = await _context.Materials
                    .CountAsync(m => _context.Assignments.Any(a => a.ClassId == m.ClassId && a.InstructorId == userId))
            };

            ViewData["Title"] = "Instructor Dashboard";
            return View(viewModel);
        }
    }
}
