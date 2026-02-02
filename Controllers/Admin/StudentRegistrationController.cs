using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Symphony.Portal.Web.Data;
using Symphony.Portal.Web.Models;
using Symphony.Portal.Web.Models.Enums;
using Symphony.Portal.Web.Models.ViewModels;

namespace Symphony.Portal.Web.Controllers.Admin
{
    [Area("Admin")]
    [Authorize(Roles = RoleNames.Admin)]
    [Route("Admin/[controller]/[action]")]
    public class StudentRegistrationController : Controller
    {
        private readonly AppDbContext _context;

        public StudentRegistrationController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Admin/StudentRegistration
        public async Task<IActionResult> Index(int? pageNumber)
        {
            var query = _context.StudentRegistrations
                .Include(r => r.Course)
                .Include(r => r.Center)
                .OrderByDescending(r => r.RegisteredAt)
                .AsNoTracking();
            
            int pageSize = 10;
            return View(await PaginatedList<StudentRegistration>.CreateAsync(query, pageNumber ?? 1, pageSize));
        }

        // GET: Admin/StudentRegistration/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null) return NotFound();

            var registration = await _context.StudentRegistrations
                .Include(r => r.Course)
                .Include(r => r.Center)
                .Include(r => r.ExamDetail)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (registration == null) return NotFound();

            return View(registration);
        }

        // GET: Admin/StudentRegistration/Approve/5
        public async Task<IActionResult> Approve(string id)
        {
            if (id == null) return NotFound();
            var registration = await _context.StudentRegistrations.FindAsync(id);
            if (registration == null) return NotFound();
            
            // Should verify if already approved?
            ViewBag.RegistrationId = id;
            return View(new ExamDetail { RegistrationId = id });
        }

        // POST: Admin/StudentRegistration/Approve
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(ExamDetail examDetail)
        {
            if (ModelState.IsValid)
            {
                var registration = await _context.StudentRegistrations.FindAsync(examDetail.RegistrationId);
                if (registration == null) return NotFound();

                // 1. Create Exam Detail
                if(string.IsNullOrEmpty(examDetail.Id)) examDetail.Id = Guid.NewGuid().ToString();
                _context.Add(examDetail);

                // 2. Update status
                registration.Status = RegistrationStatus.Approved;
                _context.Update(registration);

                // 3. Save
                await _context.SaveChangesAsync();
                
                // TODO: Send Email Notification here
                TempData["Success"] = "Registration approved successfully.";

                return RedirectToAction(nameof(Index));
            }
            return View(examDetail);
        }

        // POST: Admin/StudentRegistration/Reject/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(string id)
        {
             var registration = await _context.StudentRegistrations.FindAsync(id);
             if (registration != null)
             {
                 registration.Status = RegistrationStatus.Rejected;
                 await _context.SaveChangesAsync();
                 TempData["Success"] = "Registration rejected.";
             }
             return RedirectToAction(nameof(Index));
        }
    }
}
