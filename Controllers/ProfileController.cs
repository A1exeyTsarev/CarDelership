// Controllers/ProfileController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarDelership.Data;
using CarDelership.Models;
using Microsoft.AspNetCore.Http;

namespace CarDelership.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProfileController(ApplicationDbContext context)
        {
            _context = context;
        }

        private int? GetCurrentUserId()
        {
            // Получаем ID пользователя из сессии
            return HttpContext.Session.GetInt32("UserId");
        }

        // GET: /Profile/
        public async Task<IActionResult> Index()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.User_Id == userId);

            if (user == null)
            {
                return NotFound();
            }

            // Статистика
            var totalOrders = await _context.Orders.CountAsync(o => o.User_Id == userId);
            var wishlistCount = await _context.Wishlists.CountAsync(w => w.User_Id == userId);
            var cartCount = await _context.ShoppingCarts
                .Where(c => c.User_Id == userId)
                .SumAsync(c => c.Quantity);

            ViewBag.TotalOrders = totalOrders;
            ViewBag.WishlistCount = wishlistCount;
            ViewBag.CartCount = cartCount;

            return View(user);
        }

        // GET: /Profile/Orders
        public async Task<IActionResult> Orders()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var orders = await _context.Orders
                .Include(o => o.OrderStatus)
                .Include(o => o.DeliveryMethod)
                .Where(o => o.User_Id == userId)
                .OrderByDescending(o => o.CreatedDate)
                .ToListAsync();

            return View(orders);
        }

        // GET: /Profile/OrderDetails/5
        public async Task<IActionResult> OrderDetails(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var order = await _context.Orders
                .Include(o => o.OrderStatus)
                .Include(o => o.DeliveryMethod)
                .FirstOrDefaultAsync(o => o.Order_Id == id && o.User_Id == userId);

            if (order == null)
            {
                return NotFound();
            }

            var orderItems = await _context.OrderItems
                .Include(oi => oi.Car)
                .ThenInclude(c => c.CarImages)
                .Include(oi => oi.Car)
                .ThenInclude(c => c.Model)
                .ThenInclude(m => m.Manufacturer)
                .Where(oi => oi.Order_Id == id)
                .ToListAsync();

            ViewBag.OrderItems = orderItems;

            return View(order);
        }

        // GET: /Profile/Edit
        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: /Profile/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Users model)
        {
            var userId = GetCurrentUserId();
            if (userId == null || userId != model.User_Id)
            {
                return Unauthorized();
            }

            if (ModelState.IsValid)
            {
                var user = await _context.Users.FindAsync(userId);
                if (user != null)
                {
                    user.FullName = model.FullName;
                    user.Phone = model.Phone;
                    user.Email = model.Email;

                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Профиль успешно обновлен!";
                    return RedirectToAction("Index");
                }
            }

            return View(model);
        }
    }
}