using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Symphony.Portal.Web.Data;
using Symphony.Portal.Web.Models;
using Symphony.Portal.Web.Models.Enums; // Đảm bảo bạn có khai báo enum này ở đây
using System.Linq;
using System.Threading.Tasks;

namespace Symphony.Portal.Web.Controllers.Instructor
{
    [Area("Instructor")]
    [Authorize(Roles = RoleNames.Instructor)] // Chỉ giảng viên mới có quyền truy cập
    [Route("Instructor/[controller]/[action]")]
    public class ExamScheduleController : Controller
    {
        private readonly AppDbContext _context;

        public ExamScheduleController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Instructor/ExamSchedule/Index
        public async Task<IActionResult> Index()
        {
            // Lấy tất cả bài tập có AssignmentType là Invigilation (giám sát phòng thi)
            var assignments = await _context.Assignments
                .Include(a => a.Class)        // Lấy thông tin lớp
                .Include(a => a.Instructor)   // Lấy thông tin giảng viên
                .Where(a => a.AssignmentType == AssignmentType.Invigilation) // Chỉ lấy bài giám sát phòng thi
                .ToListAsync();

            return View(assignments); // Trả về danh sách các bài giám sát phòng thi cho view
        }

        // GET: Instructor/ExamSchedule/Cancel/5
        public async Task<IActionResult> Cancel(string id)
        {
            if (id == null) return NotFound();

            var assignment = await _context.Assignments
                .FirstOrDefaultAsync(a => a.Id == id);

            if (assignment == null) return NotFound();

            // Kiểm tra nếu Assignment chưa bị hủy bỏ
            if (assignment.Status != AssignmentStatus.Cancelled)
            {
                // Cập nhật trạng thái của bài giám sát thành "Cancelled"
                assignment.Status = AssignmentStatus.Cancelled;

                // Lưu thay đổi vào cơ sở dữ liệu
                _context.Update(assignment);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index)); // Quay lại danh sách các bài giám sát
        }
    }
}
