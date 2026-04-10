using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using CarDelership.Data;
using CarDelership.Models;

namespace CarDelership.Controllers
{
    [Authorize] // Требует авторизации для всех действий
    public class CarManagementController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CarManagementController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Проверка роли менеджера
        private bool IsManager()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            return userRole == "Менеджер" || userRole == "Администратор";
        }

        // GET: /CarManagement/Index
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!IsManager())
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var cars = await _context.Cars
                .Include(c => c.Model)
                .ThenInclude(m => m.Manufacturer)
                .Include(c => c.Color)
                .Include(c => c.CarImages)
                .ToListAsync();

            return View(cars);
        }

        // GET: /CarManagement/UpdateStatus/{id}
        [HttpGet]
        public async Task<IActionResult> UpdateStatus(int id)
        {
            if (!IsManager())
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var car = await _context.Cars
                .Include(c => c.Model)
                .FirstOrDefaultAsync(c => c.Car_Id == id);

            if (car == null)
            {
                return NotFound();
            }

            ViewBag.AvailabilityStatuses = new List<string> { "В наличии", "Под заказ", "Нет в наличии", "Скоро в продаже" };
            return View(car);
        }

        // POST: /CarManagement/UpdateStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int carId, string availabilityStatus, int? quantity)
        {
            if (!IsManager())
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var car = await _context.Cars.FindAsync(carId);
            if (car == null)
            {
                TempData["Error"] = "Товар не найден";
                return RedirectToAction("Index");
            }

            var oldStatus = car.AvailabilityStatus;
            car.AvailabilityStatus = availabilityStatus;

            if (quantity.HasValue)
            {
                car.Quantity = quantity.Value;
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = $"Статус товара '{car.Name}' изменен с '{oldStatus}' на '{availabilityStatus}'";
            return RedirectToAction("Index");
        }

        // GET: /CarManagement/Edit/{id}
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (!IsManager())
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var car = await _context.Cars
                .Include(c => c.Model)
                .Include(c => c.Color)
                .Include(c => c.CarImages)
                .FirstOrDefaultAsync(c => c.Car_Id == id);

            if (car == null)
            {
                return NotFound();
            }

            ViewBag.Models = await _context.Models.ToListAsync();
            ViewBag.Colors = await _context.Colors.ToListAsync();
            return View(car);
        }

        // POST: /CarManagement/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Cars car)
        {
            if (!IsManager())
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            if (ModelState.IsValid)
            {
                _context.Update(car);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Товар успешно обновлен";
                return RedirectToAction("Index");
            }

            ViewBag.Models = await _context.Models.ToListAsync();
            ViewBag.Colors = await _context.Colors.ToListAsync();
            return View(car);
        }
    }
}