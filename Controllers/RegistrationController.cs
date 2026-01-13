using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Symphony.Portal.Web.Data;
using Symphony.Portal.Web.Models;
using Symphony.Portal.Web.Models.Enums;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Symphony.Portal.Web.Controllers
{
    public class RegistrationController : Controller
    {
        private readonly AppDbContext _context;

        public RegistrationController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Registration/Create?courseId=xxx
        public IActionResult Create(string? courseId)
        {
            ViewBag.CourseId = new SelectList(_context.Courses.Where(c => c.IsActive), "Id", "Title", courseId);
            ViewBag.CenterId = new SelectList(_context.Centers, "Id", "Name");

            var model = new StudentRegistration();
            if (!string.IsNullOrEmpty(courseId))
            {
                model.CourseId = courseId;
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StudentRegistration registration)
        {
            if (ModelState.IsValid)
            {
                if(string.IsNullOrEmpty(registration.Id)) registration.Id = Guid.NewGuid().ToString();
                registration.RegisteredAt = DateTime.Now;
                registration.Status = RegistrationStatus.Pending;

                _context.Add(registration);
                await _context.SaveChangesAsync();
                 
                // In a real app, send email here or redirect to payment if ExtraPractice is True
                
                return RedirectToAction(nameof(Success));
            }
            
            ViewBag.CourseId = new SelectList(_context.Courses.Where(c => c.IsActive), "Id", "Title", registration.CourseId);
            ViewBag.CenterId = new SelectList(_context.Centers, "Id", "Name", registration.CenterId);
            return View(registration);
        }

        public IActionResult Success()
        {
            return View();
        }
    }
}
