using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Symphony.Portal.Web.Data;
using Symphony.Portal.Web.Models.Enums;
using System.Security.Claims;
[Area("Instructor")]
[Authorize(Roles = "Instructor")]
public class ClassesController : Controller
{
    private readonly AppDbContext _context;

    public ClassesController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var instructorId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // 1. Lấy các assignment chưa nhận (Assigned)
        var notReceived = await _context.Assignments
            .Where(a => a.InstructorId == instructorId
                     && a.AssignmentType == AssignmentType.Teaching
                     && a.Status == AssignmentStatus.Assigned)
            .ToListAsync();

        // 2. Đánh dấu là Received
        if (notReceived.Any())
        {
            foreach (var item in notReceived)
            {
                item.Status = AssignmentStatus.Received;
            }

            await _context.SaveChangesAsync();
        }

        // 3. Load danh sách hiển thị
        var assignments = await _context.Assignments
            .Include(a => a.Class)
            .Where(a => a.InstructorId == instructorId
                     && a.AssignmentType == AssignmentType.Teaching)
            .ToListAsync();

        return View(assignments);
    }
}

