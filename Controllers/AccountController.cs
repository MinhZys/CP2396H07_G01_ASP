using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Symphony.Portal.Web.Data;
using Symphony.Portal.Web.Models;
using Symphony.Portal.Web.Models.ViewModels;

namespace Symphony.Portal.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        private readonly Services.EmailService _emailService;

        public AccountController(AppDbContext context, Services.EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                // Simple Login Logic
                var user = await _context.Users.Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Email == model.Email);

                if (user != null)
                {
                    // Check if active
                    if (!user.IsActive)
                    {
                        ModelState.AddModelError(string.Empty, "Tài khoản của bạn đã bị vô hiệu hóa.");
                        return View(model);
                    }

                    // Check password (In real app, use hashing!)
                    if (user.Password == model.Password)
                    {
                        // Create Claims
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.NameIdentifier, user.Id), // Fix: Add User ID claim
                            new Claim(ClaimTypes.Name, user.Email),
                            new Claim(ClaimTypes.Email, user.Email),
                            new Claim("FullName", user.FullName)
                        };

                        if (user.Role != null)
                        {
                            claims.Add(new Claim(ClaimTypes.Role, user.Role.Name));
                        }

                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var authProperties = new AuthenticationProperties
                        {
                            IsPersistent = model.RememberMe
                        };

                        await HttpContext.SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            new ClaimsPrincipal(claimsIdentity),
                            authProperties);

                        TempData["Success"] = "Đăng nhập thành công! Chào mừng bạn quay trở lại.";
                        return RedirectToLocal(returnUrl, user);
                    }
                }

                ModelState.AddModelError(string.Empty, "Thông tin đăng nhập không hợp lệ.");
            }
            return View(model);
        }

        public async Task LoginGoogle()
        {
            var properties = new AuthenticationProperties
            {
                 RedirectUri = Url.Action("GoogleResponse")
            };
            properties.Items["prompt"] = "select_account";
            
            await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme, properties);
        }

        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var claims = result.Principal?.Identities.FirstOrDefault()?.Claims;
            var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var fullName = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            if (email != null)
            {
                var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Email == email);
                
                if (user == null)
                {
                    // Clear the session that was auto-signed in by the Google handler
                    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    TempData["Error"] = "Tài khoản không tồn tại. Vui lòng đăng ký và chờ quản trị viên phê duyệt.";
                    return RedirectToAction("Login");
                }

                if (!user.IsActive)
                {
                    // Clear the session that was auto-signed in by the Google handler
                    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    TempData["Error"] = "Tài khoản của bạn chưa được duyệt hoặc đã bị vô hiệu hóa.";
                    return RedirectToAction("Login");
                }

                // Fix: Properly Sign In with App Claims (Role)
                var appClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id), // Fix: Add User ID claim
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim("FullName", user.FullName)
                };

                if (user.Role != null)
                {
                    appClaims.Add(new Claim(ClaimTypes.Role, user.Role.Name));
                }

                var claimsIdentity = new ClaimsIdentity(appClaims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true // Google login usually persistent
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                return RedirectToLocal(null, user);
            }
            return RedirectToAction("Login");
        }



        [HttpGet]
        public IActionResult Register(string? returnUrl = null)
        {
            return RedirectToAction("Create", "GuestRegistration", new { returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterVM model, string? returnUrl = null)
        {
            // Redirect to unified guest registration flow
            return RedirectToAction("Create", "GuestRegistration", new { returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["Success"] = "Đăng xuất thành công!";
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return RedirectToAction("Index", "Home", new { area = "" });
        }

        private IActionResult RedirectToLocal(string? returnUrl, User? user)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            
            if (user != null && user.Role != null)
            {
                if (user.Role.Name == RoleNames.Admin)
                {
                    return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                }
                if (user.Role.Name == RoleNames.Instructor)
                {
                    return RedirectToAction("Index", "Dashboard", new { area = "Instructor" });
                }
                if (user.Role.Name == RoleNames.Student)
                {
                    // Fix: Redirect to Public Home instead of Student Area
                    return RedirectToAction("Index", "Home", new { area = "" });
                }
            }

            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordVM model)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
                if (user == null)
                {
                    // Don't reveal that the user does not exist
                    ModelState.AddModelError(string.Empty, "Nếu email tồn tại, mã xác thực đã được gửi.");
                    return View(model); 
                }

                // Generate OTP
                var otp = new Random().Next(100000, 999999).ToString();

                // Store in Session
                HttpContext.Session.SetString("ResetEmail", model.Email);
                HttpContext.Session.SetString("ResetCode", otp);

                // Send Email
                var subject = "Mã xác thực quên mật khẩu";
                var body = $"Mã xác thực của bạn là: {otp}";
                
                try 
                {
                    await _emailService.SendEmailAsync(model.Email, subject, body);
                } 
                catch(Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "Gửi email thất bại: " + ex.Message);
                    return View(model);
                }

                return RedirectToAction("VerifyCode");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult VerifyCode()
        {
            var email = HttpContext.Session.GetString("ResetEmail");
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("ForgotPassword");
            }
            return View(new VerifyCodeVM { Email = email });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult VerifyCode(VerifyCodeVM model)
        {
            var email = HttpContext.Session.GetString("ResetEmail");
            var code = HttpContext.Session.GetString("ResetCode");

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(code))
            {
                 return RedirectToAction("ForgotPassword");
            }

            if (model.Code == code)
            {
                HttpContext.Session.SetString("IsVerified", "true");
                return RedirectToAction("ResetPassword");
            }

            ModelState.AddModelError(string.Empty, "Mã xác thực không đúng.");
            model.Email = email;
            return View(model);
        }

        [HttpGet]
        public IActionResult ResetPassword()
        {
            var email = HttpContext.Session.GetString("ResetEmail");
            var isVerified = HttpContext.Session.GetString("IsVerified");

            if (string.IsNullOrEmpty(email) || isVerified != "true")
            {
                return RedirectToAction("ForgotPassword");
            }

            return View(new ResetPasswordVM { Email = email });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM model)
        {
             var email = HttpContext.Session.GetString("ResetEmail");
            var isVerified = HttpContext.Session.GetString("IsVerified");

            if (string.IsNullOrEmpty(email) || isVerified != "true")
            {
                return RedirectToAction("ForgotPassword");
            }

            if (ModelState.IsValid)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (user != null)
                {
                    user.Password = model.NewPassword; // Ideally hash this
                    _context.Update(user);
                    await _context.SaveChangesAsync();

                    // Clear Session
                    HttpContext.Session.Clear();

                    TempData["Success"] = "Đổi mật khẩu thành công. Vui lòng đăng nhập.";
                    return RedirectToAction("Login");
                }
            }
            return View(model);
        }

    }
}
