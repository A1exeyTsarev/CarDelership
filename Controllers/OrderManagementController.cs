// Controllers/OrderManagementController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using CarDelership.Data;
using CarDelership.Models;

namespace CarDelership.Controllers
{
    [Authorize(Roles = "Администратор,Менеджер")]
    public class OrderManagementController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderManagementController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /OrderManagement/Index
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var orders = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderStatus)
                .Include(o => o.DeliveryMethod)
                .OrderByDescending(o => o.CreatedDate)
                .ToListAsync();

            return View(orders);
        }

        // GET: /OrderManagement/Details/{id}
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderStatus)
                .Include(o => o.DeliveryMethod)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Car)
                    .ThenInclude(c => c.CarImages)
                .FirstOrDefaultAsync(o => o.Order_Id == id);

            if (order == null)
            {
                return NotFound();
            }

            ViewBag.OrderItems = order.OrderItems?.ToList() ?? new List<OrderItems>();

            return View(order);
        }

        // POST: /OrderManagement/UpdateStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, int statusId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderStatus)
                .FirstOrDefaultAsync(o => o.Order_Id == id);

            if (order == null)
            {
                TempData["Error"] = "Заказ не найден";
                return RedirectToAction("Index");
            }

            order.OrderStatus_Id = statusId;

            if (statusId == 5 || statusId == 7) // Доставлен или Завершен
            {
                order.CompleteDate = DateTime.Now;
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = $"Статус заказа #{order.OrderNumber} обновлен на \"{order.OrderStatus?.Name}\"";
            return RedirectToAction("Index");
        }

        // POST: /OrderManagement/Cancel/{id} - ИСПРАВЛЕННЫЙ МЕТОД
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Order_Id == id);

            if (order == null)
            {
                TempData["Error"] = "Заказ не найден";
                return RedirectToAction("Index");
            }

            // Проверяем, можно ли отменить заказ
            if (order.OrderStatus_Id == 5 || order.OrderStatus_Id == 6 || order.OrderStatus_Id == 7)
            {
                TempData["Error"] = $"Заказ #{order.OrderNumber} нельзя отменить (статус: {order.OrderStatus?.Name})";
                return RedirectToAction("Index");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Возвращаем товары на склад
                foreach (var item in order.OrderItems)
                {
                    var car = await _context.Cars.FindAsync(item.Car_Id);
                    if (car != null)
                    {
                        car.Quantity += item.Quantity;

                        // Обновляем статус товара
                        if (car.Quantity <= 0)
                        {
                            car.AvailabilityStatus = "Нет в наличии";
                        }
                        else if (car.Quantity <= 3)
                        {
                            car.AvailabilityStatus = "Под заказ";
                        }
                        else
                        {
                            car.AvailabilityStatus = "В наличии";
                        }
                    }
                }

                // Меняем статус заказа на "Отменен"
                order.OrderStatus_Id = 6; // Отменен

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["Success"] = $"Заказ #{order.OrderNumber} успешно отменен. Товары возвращены на склад.";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["Error"] = $"Ошибка при отмене заказа: {ex.Message}";
            }

            return RedirectToAction("Index");
        }

        // POST: /OrderManagement/Delete (альтернативный метод удаления)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Order_Id == id);

            if (order == null)
            {
                TempData["Error"] = "Заказ не найден";
                return RedirectToAction("Index");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Удаляем позиции заказа
                _context.OrderItems.RemoveRange(order.OrderItems);
                // Удаляем сам заказ
                _context.Orders.Remove(order);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["Success"] = $"Заказ #{order.OrderNumber} удален";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["Error"] = $"Ошибка при удалении: {ex.Message}";
            }

            return RedirectToAction("Index");
        }
    }
}