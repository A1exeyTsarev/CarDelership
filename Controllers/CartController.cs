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

            // Проверка количества
            if (quantity < 1)
            {
                quantity = 1;
            }

            var car = await _context.Cars.FindAsync(carId);
            if (car == null)
            {
                return Json(new { success = false, message = "Товар не найден" });
            }

            // Проверяем, достаточно ли товара на складе
            if (car.Quantity < quantity)
            {
                return Json(new { success = false, message = $"Недостаточно товара. В наличии: {car.Quantity} шт." });
            }

            var cartItem = await _context.ShoppingCarts
                .FirstOrDefaultAsync(c => c.User_Id == userId && c.Car_Id == carId);

            if (cartItem != null)
            {
                // Проверяем, не превышает ли общее количество товар на складе
                int newQuantity = cartItem.Quantity + quantity;
                if (car.Quantity < newQuantity)
                {
                    return Json(new { success = false, message = $"Недостаточно товара. В наличии: {car.Quantity} шт., в корзине: {cartItem.Quantity} шт." });
                }
                cartItem.Quantity = newQuantity;
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
                .Include(c => c.Car)
                .FirstOrDefaultAsync(c => c.ShoppingCart_Id == cartId && c.User_Id == userId);

            if (cartItem != null)
            {
                if (quantity <= 0)
                {
                    _context.ShoppingCarts.Remove(cartItem);
                }
                else
                {
                    // Проверяем, достаточно ли товара на складе
                    if (cartItem.Car != null && cartItem.Car.Quantity < quantity)
                    {
                        return Json(new { success = false, message = $"Недостаточно товара. В наличии: {cartItem.Car.Quantity} шт." });
                    }
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

            // Проверяем актуальность количества товаров
            foreach (var item in cartItems)
            {
                if (item.Car != null && item.Car.Quantity < item.Quantity)
                {
                    TempData["Error"] = $"Товара \"{item.Car.Name}\" недостаточно на складе. В наличии: {item.Car.Quantity} шт.";
                    return RedirectToAction("Index");
                }
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
        public async Task<IActionResult> Checkout(int? deliveryMethodId)
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

            // Повторная проверка актуальности количества
            foreach (var item in cartItems)
            {
                if (item.Car != null && item.Car.Quantity < item.Quantity)
                {
                    TempData["Error"] = $"Товара \"{item.Car.Name}\" недостаточно на складе. В наличии: {item.Car.Quantity} шт.";
                    return RedirectToAction("Index");
                }
            }

            // Вычисляем сумму корзины с учетом количества
            var cartTotal = cartItems.Sum(item =>
                (item.Car?.DiscountPrice > 0 ? item.Car.DiscountPrice : item.Car?.Price ?? 0) * item.Quantity);

            // Проверка выбора способа доставки
            if (!deliveryMethodId.HasValue || deliveryMethodId == 0)
            {
                await PrepareCheckoutViewWithError(cartItems, cartTotal, "Пожалуйста, выберите способ доставки");
                return View();
            }

            // Проверка существования метода доставки
            var deliveryMethod = await _context.DeliveryMethods
                .FirstOrDefaultAsync(d => d.DeliveryMethods_Id == deliveryMethodId);

            if (deliveryMethod == null)
            {
                await PrepareCheckoutViewWithError(cartItems, cartTotal, "Выбранный способ доставки не существует");
                return View();
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Создаем заказ
                var orderTotal = cartTotal + deliveryMethod.Price;
                var newStatus = await _context.OrderStatuses.FirstOrDefaultAsync(s => s.Name == "Новый");

                var order = new Orders
                {
                    OrderNumber = $"ORD-{DateTime.Now:yyyyMMddHHmmss}",
                    OrderStatus_Id = newStatus?.OrderStatus_Id ?? 1,
                    OrderMethod_Id = 2,
                    DeliveryMethod_Id = deliveryMethodId.Value,
                    CreatedDate = DateTime.Now,
                    TotalAmount = orderTotal,
                    User_Id = userId.Value
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // Добавляем товары в заказ, уменьшаем количество и обновляем статус
                foreach (var item in cartItems)
                {
                    var price = item.Car?.DiscountPrice > 0 ? item.Car.DiscountPrice : item.Car?.Price ?? 0;
                    var orderItem = new OrderItems
                    {
                        Order_Id = order.Order_Id,
                        Car_Id = item.Car_Id,
                        Quantity = item.Quantity,  // ✅ Используем количество из корзины
                        PriceAtPurchase = price
                    };
                    _context.OrderItems.Add(orderItem);

                    if (item.Car != null)
                    {
                        // Уменьшаем количество товара на количество в корзине
                        item.Car.Quantity -= item.Quantity;

                        // Обновляем статус товара в зависимости от количества
                        if (item.Car.Quantity <= 0)
                        {
                            item.Car.AvailabilityStatus = "Нет в наличии";
                        }
                        else if (item.Car.Quantity <= 3)
                        {
                            item.Car.AvailabilityStatus = "Под заказ";
                        }
                        else
                        {
                            item.Car.AvailabilityStatus = "В наличии";
                        }
                    }
                }

                // Очищаем корзину
                _context.ShoppingCarts.RemoveRange(cartItems);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                TempData["Success"] = "Заказ успешно оформлен!";
                return RedirectToAction("OrderDetails", "Profile", new { id = order.Order_Id });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["Error"] = $"Ошибка при оформлении заказа: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        // Вспомогательный метод для подготовки представления с ошибкой
        private async Task PrepareCheckoutViewWithError(List<ShoppingCart> cartItems, decimal cartTotal, string errorMessage)
        {
            ViewBag.DeliveryMethods = await _context.DeliveryMethods.ToListAsync();
            ViewBag.CartItems = cartItems;
            ViewBag.Subtotal = cartTotal;
            ModelState.AddModelError("", errorMessage);
        }
    }
}