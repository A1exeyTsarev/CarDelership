// Controllers/AccountController.cs (добавьте сохранение UserId в сессию)
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using CarDelership.Data;
using CarDelership.Models;
using CarDelership.ViewModels;

namespace CarDelership.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Login == model.Login && u.IsActive);

                if (user != null && user.Password == model.Password)
                {
                    // Сохраняем ID пользователя в сессию
                    HttpContext.Session.SetInt32("UserId", user.User_Id);

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.User_Id.ToString()),
                        new Claim(ClaimTypes.Name, user.FullName),
                        new Claim(ClaimTypes.Email, user.Email ?? ""),
                        new Claim("Login", user.Login),
                        new Claim(ClaimTypes.Role, user.Role?.Name ?? "Клиент")
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = model.RememberMe,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        return Redirect(returnUrl);

                    return RedirectToAction("Index", "Catalog");
                }

                ModelState.AddModelError("", "Неверный логин или пароль");
            }

            return View(model);
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Login == model.Login);

                if (existingUser != null)
                {
                    ModelState.AddModelError("Login", "Пользователь с таким логином уже существует");
                    return View(model);
                }

                var user = new Users
                {
                    Login = model.Login,
                    Password = model.Password,
                    FullName = model.FullName,
                    Phone = model.Phone,
                    Email = model.Email,
                    PassportData = model.PassportData,
                    Role_Id = 3,
                    IsActive = true,
                    RegistrationDate = DateTime.Now,
                    Discount = 0
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Сохраняем ID в сессию
                HttpContext.Session.SetInt32("UserId", user.User_Id);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.User_Id.ToString()),
                    new Claim(ClaimTypes.Name, user.FullName),
                    new Claim(ClaimTypes.Email, user.Email ?? ""),
                    new Claim("Login", user.Login),
                    new Claim(ClaimTypes.Role, "Клиент")
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                TempData["Success"] = "Регистрация прошла успешно!";
                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }

        // GET: /Account/Logout
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            // Очищаем сессию
            HttpContext.Session.Clear();

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/AccessDenied
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}