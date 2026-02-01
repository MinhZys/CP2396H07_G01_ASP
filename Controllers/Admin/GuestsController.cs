using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Symphony.Portal.Web.Data;
using Symphony.Portal.Web.Models;
using Symphony.Portal.Web.Models.Enums;

namespace Symphony.Portal.Web.Controllers.Admin
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class GuestsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly Symphony.Portal.Web.Services.EmailService _emailService;

        public GuestsController(AppDbContext context, Symphony.Portal.Web.Services.EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // GET: Admin/Guests
        public async Task<IActionResult> Index(string status)
        {
            var query = _context.Guests.AsQueryable();

            if (!string.IsNullOrEmpty(status) && Enum.TryParse<GuestRegistrationStatus>(status, out var statusEnum))
            {
                query = query.Where(g => g.Status == statusEnum);
            }
            else
            {
                // Default to showing Pending/Paid
                query = query.Where(g => g.Status == GuestRegistrationStatus.PaidPendingApproval || g.Status == GuestRegistrationStatus.PendingPayment);
            }

            ViewBag.CurrentStatus = status;
            var classes = await _context.Classes.Include(c => c.ClassCategory).ToListAsync();
            ViewBag.Classes = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(classes, "Id", "ClassName");
            
            // Calculate remaining seats for modal
            var classesWithSeats = classes.Select(c => new 
            {
                c.Id,
                c.ClassName,
                c.NumberOfSeats,
                Status = c.Status,
                ClassCategory = new { Name = c.ClassCategory?.Name },
                RemainingSeats = c.NumberOfSeats - (
                    _context.Guests.Count(g => g.ClassId == c.Id && g.Status == GuestRegistrationStatus.Approved) + 
                    _context.Enrollments.Count(e => e.ClassId == c.Id)
                )
            }).ToList();

            ViewBag.ClassesList = classesWithSeats; // Pass projected object for JS
            return View(await query
                .Include(g => g.SelectedEntranceExam)
                .Include(g => g.User)
                .Include(g => g.Class) // Include Class for display
                .OrderByDescending(g => g.CreatedAt)
                .ToListAsync());
        }

        // POST: Admin/Guests/Approve/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(string id, string examRoom, string description, string classId, string examTime)
        {
            var guest = await _context.Guests.FindAsync(id);
            if (guest == null) return NotFound();

            if (guest.Status == GuestRegistrationStatus.Approved)
            {
                return RedirectToAction(nameof(Index));
            }

            // Check Seat Availability if Class is selected
            string className = "N/A";
            if (!string.IsNullOrEmpty(classId))
            {
                var targetClass = await _context.Classes.FindAsync(classId);
                if (targetClass != null)
                {
                    className = targetClass.ClassName; // Capture name for email
                    // Count occupied seats
                    var approvedGuestsCount = await _context.Guests.CountAsync(g => g.ClassId == classId && g.Status == GuestRegistrationStatus.Approved);
                    var enrollmentsCount = await _context.Enrollments.CountAsync(e => e.ClassId == classId);
                    
                    // Simple sum for now. 
                    var totalOccupied = approvedGuestsCount + enrollmentsCount;

                    if (totalOccupied >= targetClass.NumberOfSeats)
                    {
                        TempData["Error"] = $"Class {targetClass.ClassName} is full (Capacity: {targetClass.NumberOfSeats}, Occupied: {totalOccupied}). Please choose another class.";
                        return RedirectToAction(nameof(Index));
                    }
                }
            }

            // Create User Account
            // Check if email exists
            if (await _context.Users.AnyAsync(u => u.Email == guest.Email))
            {
                TempData["Error"] = "Email already exists in User system.";
                return RedirectToAction(nameof(Index));
            }

            var password = GenerateSecurePassword();
            var newUser = new User
            {
                Id = Guid.NewGuid().ToString(),
                FullName = guest.FullName,
                Email = guest.Email,
                // In production, Hash this password!
                Password = password, 
                RoleId = "4", // Guest Role ID
                IsActive = true
            };

            _context.Users.Add(newUser);
            
            // Link Guest and Update Info
            guest.UserId = newUser.Id;
            guest.Status = GuestRegistrationStatus.Approved;
            guest.ExamRoom = examRoom;
            
            // Append Time to Description if provided
            var finalDescription = description;
            if (!string.IsNullOrEmpty(examTime))
            {
                finalDescription = $"Exam Time: {examTime}\n" + description;
            }
            guest.Description = finalDescription;
            
            guest.ClassId = classId;

            _context.Guests.Update(guest);

            await _context.SaveChangesAsync();

            // Send Email
            try
            {
                string emailBody = $@"Dear {guest.FullName},

                Registration Approved!
                Here are your details:
                Exam Room: {examRoom}
                {(string.IsNullOrEmpty(examTime) ? "" : $"Time: {examTime}")}
                {(string.IsNullOrEmpty(classId) ? "" : $"Assigned Class: {className}")}

                Notes:
                {description}

                Login Details:
                Email: {guest.Email}
                Password: {password}

                Please change your password after login.

                Regards,
                Symphony Portal";

                await _emailService.SendEmailAsync(guest.Email, "Registration Approved - Symphony Portal", emailBody);
                TempData["Success"] = $"Guest Approved. User created. Email Sent.";
            }
            catch (Exception ex)
            {
                // Log the error to the console so the user can see it
                Console.WriteLine($"[Email Error] Failed to send email: {ex.ToString()}");
                TempData["Success"] = $"Guest Approved. User created. BUT Email failed: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/Guests/Reject/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(string id)
        {
             var guest = await _context.Guests.FindAsync(id);
            if (guest == null) return NotFound();

            guest.Status = GuestRegistrationStatus.Rejected;
            _context.Guests.Update(guest);
            await _context.SaveChangesAsync();
            
            TempData["Success"] = "Guest Registration Rejected.";

            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/Guests/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var guest = await _context.Guests.FindAsync(id);
            if (guest == null) return NotFound();

            // Find related payments
            var payments = await _context.Payments.Where(p => p.GuestId == id).ToListAsync();
            if (payments.Any())
            {
                _context.Payments.RemoveRange(payments);
            }

            _context.Guests.Remove(guest);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Guest Registration and related payments deleted.";
            return RedirectToAction(nameof(Index));
        }

        private string GenerateSecurePassword(int length = 10)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            var res = new System.Text.StringBuilder();
            var rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }
    }
}
