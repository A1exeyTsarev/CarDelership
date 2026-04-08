// Controllers/WishlistController.cs
using CarDelership.Data;
using CarDelership.Models;
using CarDelership.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarDelership.Controllers
{
    public class WishlistController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WishlistController(ApplicationDbContext context)
        {
            _context = context;
        }

        private int? GetCurrentUserId()
        {
            return HttpContext.Session.GetInt32("UserId");
        }

        // GET: /Wishlist/
        public async Task<IActionResult> Index()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var wishlistItems = await _context.Wishlists
                .Include(w => w.Car)
                .ThenInclude(c => c.CarImages)
                .Include(w => w.Car)
                .ThenInclude(c => c.Model)
                .ThenInclude(m => m.Manufacturer)
                .Include(w => w.Car)
                .ThenInclude(c => c.Color)
                .Where(w => w.User_Id == userId)
                .ToListAsync();

            return View(wishlistItems);
        }

        // POST: /Wishlist/Add
        [HttpPost]
        public async Task<IActionResult> Add(int carId)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Json(new { success = false, message = "Необходимо авторизоваться" });
            }

            // Проверяем, не добавлен ли уже
            var exists = await _context.Wishlists
                .AnyAsync(w => w.User_Id == userId && w.Car_Id == carId);

            if (exists)
            {
                return Json(new { success = false, message = "Автомобиль уже в избранном" });
            }

            var wishlist = new Wishlist
            {
                User_Id = userId.Value,
                Car_Id = carId,
                AddedAt = DateTime.Now
            };

            _context.Wishlists.Add(wishlist);
            await _context.SaveChangesAsync();

            var count = await _context.Wishlists.CountAsync(w => w.User_Id == userId);

            return Json(new { success = true, message = "Добавлено в избранное", count = count });
        }

        // POST: /Wishlist/Remove
        [HttpPost]
        public async Task<IActionResult> Remove(int carId)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Json(new { success = false, message = "Необходимо авторизоваться" });
            }

            var wishlistItem = await _context.Wishlists
                .FirstOrDefaultAsync(w => w.User_Id == userId && w.Car_Id == carId);

            if (wishlistItem != null)
            {
                _context.Wishlists.Remove(wishlistItem);
                await _context.SaveChangesAsync();
            }

            var count = await _context.Wishlists.CountAsync(w => w.User_Id == userId);

            return Json(new { success = true, message = "Удалено из избранного", count = count });
        }
    }
}