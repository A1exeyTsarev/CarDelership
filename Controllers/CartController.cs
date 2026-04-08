// Controllers/CartController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarDelership.Data;
using CarDelership.Models;
using Microsoft.AspNetCore.Http;

namespace CarDelership.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        private int? GetCurrentUserId()
        {
            return HttpContext.Session.GetInt32("UserId");
        }

        // GET: /Cart/
        public async Task<IActionResult> Index()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var cartItems = await _context.ShoppingCarts
                .Include(c => c.Car)
                .ThenInclude(c => c.CarImages)
                .Include(c => c.Car)
                .ThenInclude(c => c.Model)
                .ThenInclude(m => m.Manufacturer)
                .Include(c => c.Car)
                .ThenInclude(c => c.Color)
                .Where(c => c.User_Id == userId)
                .ToListAsync();

            decimal total = 0;
            foreach (var item in cartItems)
            {
                var price = item.Car?.DiscountPrice > 0 ? item.Car.DiscountPrice : item.Car?.Price ?? 0;
                total += price * item.Quantity;
            }

            ViewBag.Total = total;
            ViewBag.CartItems = cartItems;

            return View(cartItems);
        }

        // POST: /Cart/Add
        [HttpPost]
        public async Task<IActionResult> Add(int carId, int quantity = 1)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Json(new { success = false, message = "Необходимо авторизоваться" });
            }

            var car = await _context.Cars.FindAsync(carId);
            if (car == null || car.Quantity < quantity)
            {
                return Json(new { success = false, message = "Товар недоступен" });
            }

            var cartItem = await _context.ShoppingCarts
                .FirstOrDefaultAsync(c => c.User_Id == userId && c.Car_Id == carId);

            if (cartItem != null)
            {
                cartItem.Quantity += quantity;
            }
            else
            {
                cartItem = new ShoppingCart
                {
                    User_Id = userId.Value,
                    Car_Id = carId,
                    Quantity = quantity,
                    AddedAt = DateTime.Now
                };
                _context.ShoppingCarts.Add(cartItem);
            }

            await _context.SaveChangesAsync();

            var totalItems = await _context.ShoppingCarts
                .Where(c => c.User_Id == userId)
                .SumAsync(c => c.Quantity);

            return Json(new { success = true, message = "Товар добавлен в корзину", count = totalItems });
        }

        // POST: /Cart/Update
        [HttpPost]
        public async Task<IActionResult> Update(int cartId, int quantity)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Json(new { success = false });
            }

            var cartItem = await _context.ShoppingCarts
                .FirstOrDefaultAsync(c => c.ShoppingCart_Id == cartId && c.User_Id == userId);

            if (cartItem != null)
            {
                if (quantity <= 0)
                {
                    _context.ShoppingCarts.Remove(cartItem);
                }
                else
                {
                    cartItem.Quantity = quantity;
                }
                await _context.SaveChangesAsync();
            }

            return Json(new { success = true });
        }

        // POST: /Cart/Remove
        [HttpPost]
        public async Task<IActionResult> Remove(int cartId)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Json(new { success = false });
            }

            var cartItem = await _context.ShoppingCarts
                .FirstOrDefaultAsync(c => c.ShoppingCart_Id == cartId && c.User_Id == userId);

            if (cartItem != null)
            {
                _context.ShoppingCarts.Remove(cartItem);
                await _context.SaveChangesAsync();
            }

            return Json(new { success = true });
        }

        // GET: /Cart/Checkout
        public async Task<IActionResult> Checkout()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var cartItems = await _context.ShoppingCarts
                .Include(c => c.Car)
                .Where(c => c.User_Id == userId)
                .ToListAsync();

            if (!cartItems.Any())
            {
                return RedirectToAction("Index");
            }

            ViewBag.DeliveryMethods = await _context.DeliveryMethods.ToListAsync();
            ViewBag.CartItems = cartItems;

            // Рассчитываем общую сумму
            decimal total = 0;
            foreach (var item in cartItems)
            {
                var price = item.Car?.DiscountPrice > 0 ? item.Car.DiscountPrice : item.Car?.Price ?? 0;
                total += price * item.Quantity;
            }
            ViewBag.Subtotal = total;

            return View();
        }

        // POST: /Cart/Checkout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(int deliveryMethodId)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var cartItems = await _context.ShoppingCarts
                .Include(c => c.Car)
                .Where(c => c.User_Id == userId)
                .ToListAsync();

            if (!cartItems.Any())
            {
                return RedirectToAction("Index");
            }

            var deliveryMethod = await _context.DeliveryMethods.FindAsync(deliveryMethodId);
            decimal deliveryPrice = deliveryMethod?.Price ?? 0;
            decimal subtotal = 0;

            foreach (var item in cartItems)
            {
                var price = item.Car?.DiscountPrice > 0 ? item.Car.DiscountPrice : item.Car?.Price ?? 0;
                subtotal += price * item.Quantity;
            }

            decimal total = subtotal + deliveryPrice;

            // Получаем статус "Новый"
            var newStatus = await _context.OrderStatuses.FirstOrDefaultAsync(s => s.Name == "Новый");

            var order = new Orders
            {
                OrderNumber = $"ORD-{DateTime.Now:yyyyMMddHHmmss}",
                OrderStatus_Id = newStatus?.OrderStatus_Id ?? 1,
                OrderMethod_Id = 2, // Онлайн
                DeliveryMethod_Id = deliveryMethodId,
                CreatedDate = DateTime.Now,
                TotalAmount = total,
                User_Id = userId.Value
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Добавляем товары в заказ и уменьшаем количество на складе
            foreach (var item in cartItems)
            {
                var price = item.Car?.DiscountPrice > 0 ? item.Car.DiscountPrice : item.Car?.Price ?? 0;
                var orderItem = new OrderItems
                {
                    Order_Id = order.Order_Id,
                    Car_Id = item.Car_Id,
                    Quantity = item.Quantity,
                    PriceAtPurchase = price
                };
                _context.OrderItems.Add(orderItem);

                if (item.Car != null)
                {
                    item.Car.Quantity -= item.Quantity;
                }
            }

            // Очищаем корзину
            _context.ShoppingCarts.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Заказ успешно оформлен!";
            return RedirectToAction("OrderDetails", "Profile", new { id = order.Order_Id });
        }
    }
}