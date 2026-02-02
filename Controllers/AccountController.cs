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
                        ModelState.AddModelError(string.Empty, "Your account has been disabled.");
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

                        TempData["Success"] = "Login successful! Welcome back.";
                        return RedirectToLocal(returnUrl, user);
                    }
                }

                ModelState.AddModelError(string.Empty, "Invalid login credentials.");
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
            // Try to authenticate against the Google/External provider
            var result = await HttpContext.AuthenticateAsync();
            if (!result.Succeeded)
            {
                return RedirectToAction("Login");
            }

            var claims = result.Principal?.Claims;
            var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var fullName = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            if (email != null)
            {
                var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Email == email);
                
                if (user == null)
                {
                    // Clear the session that was auto-signed in by the Google handler
                    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    TempData["Error"] = "Account does not exist. Please register and wait for administrator approval.";
                    return RedirectToAction("Login");
                }

                if (!user.IsActive)
                {
                    // Clear the session that was auto-signed in by the Google handler
                    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    TempData["Error"] = "Your account has not been approved or has been disabled.";
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
            TempData["Success"] = "Logged out successfully!";
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
                    ModelState.AddModelError(string.Empty, "If the email exists, a verification code has been sent.");
                    return View(model); 
                }

                // Generate OTP
                var otp = new Random().Next(100000, 999999).ToString();

                // Store in Session
                HttpContext.Session.SetString("ResetEmail", model.Email);
                HttpContext.Session.SetString("ResetCode", otp);

                // Send Email
                var subject = "Password reset verification code";
                var body = $"Your verification code is: {otp}";
                
                try 
                {
                    await _emailService.SendEmailAsync(model.Email, subject, body);
                } 
                catch(Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "Failed to send email: " + ex.Message);
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

            ModelState.AddModelError(string.Empty, "Incorrect verification code.");
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

                    TempData["Success"] = "Password reset successful. Please login.";
                    return RedirectToAction("Login");
                }
            }
            return View(model);
        }

    }
}
