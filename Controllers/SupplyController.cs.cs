// Controllers/SupplyController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using CarDelership.Data;
using CarDelership.Models;
using CarDelership.Models.ViewModels;

namespace CarDelership.Controllers
{
    [Authorize(Roles = "Администратор")]
    public class SupplyController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SupplyController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Supply/Index
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var supplies = await _context.Supplies
                .Include(s => s.Supplier)
                .Include(s => s.StatusSupply)
                .Include(s => s.SupplyItems)
                    .ThenInclude(si => si.Car)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();

            ViewBag.Cars = await _context.Cars.ToListAsync();
            ViewBag.Suppliers = await _context.Suppliers.ToListAsync();
            ViewBag.Statuses = await _context.StatusSupplies.ToListAsync();

            return View(supplies);
        }

        // POST: /Supply/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(List<SupplyItemViewModel> items, int supplierId)
        {
            if (items == null || !items.Any() || items.All(i => i.CarId == 0))
            {
                TempData["Error"] = "Добавьте хотя бы один автомобиль";
                return RedirectToAction("Index");
            }

            // ⭐⭐⭐ НОВОЕ ОГРАНИЧЕНИЕ: максимальное количество автомобилей в одной поставке - 10 ⭐⭐⭐
            int totalQuantity = items.Where(i => i.CarId > 0 && i.Quantity > 0).Sum(i => i.Quantity);
            const int MAX_SUPPLY_QUANTITY = 10;

            if (totalQuantity > MAX_SUPPLY_QUANTITY)
            {
                TempData["Error"] = $"В одной поставке можно указать не более {MAX_SUPPLY_QUANTITY} автомобилей. " +
                                   $"Сейчас указано: {totalQuantity} шт. Уменьшите количество.";
                return RedirectToAction("Index");
            }

            // Проверка, что каждый товар имеет положительное количество
            foreach (var item in items.Where(i => i.CarId > 0))
            {
                if (item.Quantity <= 0)
                {
                    TempData["Error"] = $"Количество для автомобиля должно быть больше 0";
                    return RedirectToAction("Index");
                }
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Создаем поставку
                var supply = new Supply
                {
                    Supplier_Id = supplierId,
                    Status_Id = 1, // Создано
                    CreatedAt = DateTime.Now
                };

                _context.Supplies.Add(supply);
                await _context.SaveChangesAsync();

                // Добавляем позиции поставки
                foreach (var item in items.Where(i => i.CarId > 0 && i.Quantity > 0))
                {
                    var supplyItem = new SupplyItem
                    {
                        Supply_Id = supply.Supply_Id,
                        Car_Id = item.CarId,
                        Quantity = item.Quantity
                    };
                    _context.SupplyItems.Add(supplyItem);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["Success"] = $"Поставка успешно создана. Всего автомобилей: {totalQuantity} шт.";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["Error"] = $"Ошибка при создании: {ex.Message}";
            }

            return RedirectToAction("Index");
        }

        // POST: /Supply/UpdateStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int supplyId, int statusId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var supply = await _context.Supplies
                    .Include(s => s.SupplyItems)
                        .ThenInclude(si => si.Car)
                    .FirstOrDefaultAsync(s => s.Supply_Id == supplyId);

                if (supply == null)
                {
                    TempData["Error"] = "Поставка не найдена";
                    return RedirectToAction("Index");
                }

                int oldStatusId = supply.Status_Id;
                supply.Status_Id = statusId;

                if (statusId == 3 && oldStatusId != 3) // Завершено
                {
                    foreach (var item in supply.SupplyItems)
                    {
                        if (item.Car != null)
                        {
                            item.Car.Quantity += item.Quantity;
                        }
                    }
                }

                if (statusId == 3)
                {
                    supply.CompletedAt = DateTime.Now;
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                if (statusId == 3 && oldStatusId != 3)
                {
                    TempData["Success"] = "Поставка завершена. Количество товаров обновлено.";
                }
                else
                {
                    TempData["Success"] = "Статус поставки обновлен";
                }
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["Error"] = $"Ошибка при обновлении статуса: {ex.Message}";
            }

            return RedirectToAction("Index");
        }

        // POST: /Supply/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int supplyId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var supply = await _context.Supplies
                    .Include(s => s.SupplyItems)
                    .FirstOrDefaultAsync(s => s.Supply_Id == supplyId);

                if (supply != null)
                {
                    _context.SupplyItems.RemoveRange(supply.SupplyItems);
                    _context.Supplies.Remove(supply);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    TempData["Success"] = "Поставка удалена";
                }
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