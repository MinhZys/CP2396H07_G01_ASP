using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Symphony.Portal.Web.Data;
using Symphony.Portal.Web.Models;
using Symphony.Portal.Web.Models.Enums;
using System.Security.Claims;

namespace Symphony.Portal.Web.Controllers
{
    public class ExamResultController : Controller
    {
        private readonly AppDbContext _context;

        public ExamResultController(AppDbContext context)
        {
            _context = context;
        }

        // List of all exams for the current student
        public async Task<IActionResult> MyExams()
        {
            // Assuming we use standard ASP.NET Identity or a custom UserId claim
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var sessions = await _context.StudentExamSessions
                .Include(s => s.EntranceExam)
                .Include(s => s.ClassExam)
                .ThenInclude(ce => ce.Course)
                .Include(s => s.ExamPaper)
                .Where(s => s.StudentId == userId)
                .OrderByDescending(s => s.StartTime)
                .ToListAsync();

            return View(sessions);
        }

        // Detailed result of a session
        public async Task<IActionResult> Details(string id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var session = await _context.StudentExamSessions
                .Include(s => s.EntranceExam)
                .Include(s => s.ExamPaper)
                .Include(s => s.Answers)
                .ThenInclude(a => a.Question)
                .ThenInclude(q => q!.Options)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (session == null) return NotFound();

            // Security check: only the student who took the exam or an admin can view results
            // if (session.StudentId != userId && !User.IsInRole("Admin")) return Forbid();

            return View(session);
        }

        public async Task<IActionResult> CheckCourseCompletion(string sessionId)
        {
            var session = await _context.StudentExamSessions
                .Include(s => s.ClassExam)
                .ThenInclude(ce => ce!.Class)
                .ThenInclude(c => c!.ClassCategory)
                .FirstOrDefaultAsync(s => s.Id == sessionId);

            if (session == null || string.IsNullOrEmpty(session.ClassExamId)) return NotFound();

            var @class = session.ClassExam!.Class;
            var studentId = session.StudentId;

            // Find all subjects in the course associated with this class
            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.ClassId == @class!.Id && e.StudentId == studentId);

            if (enrollment == null || string.IsNullOrEmpty(enrollment.CourseId))
            {
                return RedirectToAction(nameof(MyExams));
            }

            var courseId = enrollment.CourseId;
            var course = await _context.Courses
                .Include(c => c.CourseSubjects)
                .FirstOrDefaultAsync(c => c.Id == courseId);

            if (course == null) return NotFound();

            var requiredSubjectIds = course.CourseSubjects.Select(cs => cs.SubjectId).ToList();

            // Get all final exam sessions for this student in this course for this class
            var studentSessions = await _context.StudentExamSessions
                .Include(s => s.ClassExam)
                .Where(s => s.StudentId == studentId && 
                            s.ClassExam != null && 
                            s.ClassExam.ClassId == @class!.Id &&
                            s.ClassExam.CourseId == courseId)
                .ToListAsync();

            // Check if student has finished the course exam
            var finishedExams = studentSessions
                .Where(s => s.Status == ExamSessionStatus.Finished)
                .ToList();

            if (!finishedExams.Any())
            {
                TempData["Info"] = "Bạn chưa hoàn thành kỳ thi kết thúc khóa học.";
                return RedirectToAction(nameof(MyExams));
            }

            // Calculate average score (passing the course exam is enough if there's only one)
            double latestScore = finishedExams.OrderByDescending(s => s.StartTime).First().TotalScore;
            double average = latestScore; 

            // Check if passed
            if (average >= course.PassingScore)
            {
                // Issue Certificate
                var existingCert = await _context.StudentCertificates
                    .FirstOrDefaultAsync(sc => sc.StudentId == studentId && sc.CourseId == courseId);

                if (existingCert == null)
                {
                    var cert = new StudentCertificate
                    {
                        StudentId = studentId,
                        CourseId = courseId,
                        CertificateId = course.CertificateId,
                        AverageScore = average,
                        IssueDate = DateTime.Now,
                        CertificateCode = "CERT-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper()
                    };
                    _context.StudentCertificates.Add(cert);

                    // Mark enrollment as completed
                    enrollment.IsCompleted = true;
                    _context.Update(enrollment);

                    await _context.SaveChangesAsync();
                    TempData["Success"] = $"Chúc mừng! Bạn đã đạt điểm trung bình {average:F2} và được cấp chứng chỉ.";
                }
                return RedirectToAction(nameof(MyCertificates));
            }
            else
            {
                // Failed - Record attempt and require retake
                var attempt = await _context.ExamAttempts
                    .Where(ea => ea.StudentId == studentId && ea.CourseId == courseId)
                    .OrderByDescending(ea => ea.AttemptNumber)
                    .FirstOrDefaultAsync();

                int nextAttempt = (attempt?.AttemptNumber ?? 0) + 1;

                var newAttempt = new ExamAttempt
                {
                    StudentId = studentId,
                    CourseId = courseId,
                    AttemptNumber = nextAttempt,
                    AverageScore = average,
                    Status = ExamAttemptStatus.Failed,
                    RetakeFeePaid = false
                };
                _context.ExamAttempts.Add(newAttempt);
                await _context.SaveChangesAsync();

                TempData["Error"] = $"Điểm trung bình của bạn là {average:F2}, không đủ để đạt chứng chỉ (Yêu cầu: {course.PassingScore}). Bạn cần đăng ký thi lại.";
                return RedirectToAction(nameof(RetakeRegistration), new { attemptId = newAttempt.Id });
            }
        }

        public async Task<IActionResult> MyCertificates()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var certs = await _context.StudentCertificates
                .Include(sc => sc.Course)
                .Include(sc => sc.Certificate)
                .Where(sc => sc.StudentId == userId && sc.IsValid)
                .ToListAsync();

            return View(certs);
        }

        public async Task<IActionResult> RetakeRegistration(string attemptId)
        {
            var attempt = await _context.ExamAttempts
                .Include(ea => ea.Course)
                .FirstOrDefaultAsync(ea => ea.Id == attemptId);

            if (attempt == null) return NotFound();

            return View(attempt);
        }
    }
}
