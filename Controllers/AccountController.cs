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

        public AccountController(AppDbContext context)
        {
            _context = context;
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

                        TempData["SuccessMessage"] = "Đăng nhập thành công! Chào mừng bạn quay trở lại.";
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
                    var studentRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Student");
                    user = new User
                    {
                        Id = Guid.NewGuid().ToString(), // Fix: Generate ID
                        Email = email,
                        FullName = fullName ?? email,
                        Password = Guid.NewGuid().ToString(),
                        RoleId = studentRole != null ? studentRole.Id.ToString() : "0",
                        IsActive = true,
                        Role = studentRole
                    };
                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();
                }
                else if (string.IsNullOrEmpty(user.Id))
                {
                     // Self-heal: Fix missing ID by recreating the user
                     // Cannot modify Key 'Id' on tracked entity, must delete and recreate
                     var newId = Guid.NewGuid().ToString();
                     var existingRoleId = user.RoleId;
                     var existingFullName = user.FullName;
                     var existingEmail = user.Email;
                     var existingIsActive = user.IsActive;
                     var existingPassword = user.Password;
                     var existingRole = user.Role; // Keep reference to role to attach later if needed

                     _context.Users.Remove(user);
                     await _context.SaveChangesAsync();

                     var newUser = new User 
                     {
                        Id = newId,
                        Email = existingEmail,
                        FullName = existingFullName,
                        Password = existingPassword,
                        RoleId = existingRoleId,
                        IsActive = existingIsActive,
                        Role = existingRole
                     };
                     
                     _context.Users.Add(newUser);
                     await _context.SaveChangesAsync();
                     
                     user = newUser; // Update reference for claims creation
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
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                // Check if user exists
                if (await _context.Users.AnyAsync(u => u.Email == model.Email))
                {
                    ModelState.AddModelError(string.Empty, "Email này đã được sử dụng.");
                    return View(model);
                }

                var studentRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Student");
                if (studentRole == null)
                {
                     ModelState.AddModelError(string.Empty, "Lỗi hệ thống: Không tìm thấy quyền Học viên.");
                     return View(model);
                }

                var user = new User
                {
                    Email = model.Email,
                    FullName = model.FullName,
                    Password = model.Password,
                    RoleId = studentRole.Id,
                    IsActive = true,
                    Role = studentRole // Set navigation property for Redirect
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Auto Login
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id), // Fix: Add User ID claim
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim("FullName", user.FullName),
                    new Claim(ClaimTypes.Role, "Student")
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity));

                return RedirectToLocal(returnUrl, user);
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
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
    }
}
