using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using CarDelership.Data;
using CarDelership.Models;

namespace CarDelership.Controllers
{
    [Authorize] // Требует авторизации для всех действий
    public class OrderManagementController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderManagementController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Проверка роли менеджера
        private bool IsManager()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            return userRole == "Менеджер" || userRole == "Администратор";
        }

        // GET: /OrderManagement/Index
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!IsManager())
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var orders = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderStatus)
                .Include(o => o.DeliveryMethod)
                .OrderByDescending(o => o.CreatedDate)
                .ToListAsync();

            return View(orders);
        }

        // GET: /OrderManagement/UpdateStatus/{id}
        [HttpGet]
        public async Task<IActionResult> UpdateStatus(int id)
        {
            if (!IsManager())
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderStatus)
                .FirstOrDefaultAsync(o => o.Order_Id == id);

            if (order == null)
            {
                return NotFound();
            }

            ViewBag.OrderStatuses = await _context.OrderStatuses.ToListAsync();
            return View(order);
        }

        // POST: /OrderManagement/UpdateStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int orderId, int orderStatusId)
        {
            if (!IsManager())
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                TempData["Error"] = "Заказ не найден";
                return RedirectToAction("Index");
            }

            var oldStatus = await _context.OrderStatuses.FindAsync(order.OrderStatus_Id);
            var newStatus = await _context.OrderStatuses.FindAsync(orderStatusId);

            order.OrderStatus_Id = orderStatusId;

            if (newStatus?.Name == "Доставлен" || newStatus?.Name == "Отменен")
            {
                order.CompleteDate = DateTime.Now;
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = $"Статус заказа №{order.OrderNumber} изменен с '{oldStatus?.Name}' на '{newStatus?.Name}'";
            return RedirectToAction("Index");
        }

        // GET: /OrderManagement/Details/{id}
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            if (!IsManager())
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderStatus)
                .Include(o => o.DeliveryMethod)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Car)
                .ThenInclude(c => c.Model)
                .FirstOrDefaultAsync(o => o.Order_Id == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }
    }
}