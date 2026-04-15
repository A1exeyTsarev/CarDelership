using CarDelership.Data;
using CarDelership.Models;
using CarDelership.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CarDelership.Controllers
{
    public class CatalogController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CatalogController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ✅ КАТАЛОГ С ОГРАНИЧЕНИЯМИ
        public async Task<IActionResult> Index(
            string searchTerm = "",
            string sortBy = "name_asc",
            int? minPrice = null,
            int? maxPrice = null,
            string selectedManufacturer = "",
            int? selectedYear = null,
            List<int> selectedTagIds = null,
            int page = 1)
        {
            // Если пользователь не авторизован - отправляем на логин
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            // ⚡ ПРОВЕРЯЕМ И ОБНОВЛЯЕМ СТАТУСЫ ТОВАРОВ
            await UpdateCarStatusesByQuantity();

            // ⭐⭐⭐ ОГРАНИЧЕНИЕ ПОИСКА - не больше 50 символов ⭐⭐⭐
            if (!string.IsNullOrEmpty(searchTerm) && searchTerm.Length > 50)
            {
                searchTerm = searchTerm.Substring(0, 50);
                TempData["Warning"] = "Поисковый запрос не может быть длиннее 50 символов";
            }

            // ⭐⭐⭐ ОГРАНИЧЕНИЯ ДЛЯ ЦЕНЫ ⭐⭐⭐
            // Минимальная цена не может быть отрицательной
            if (minPrice.HasValue && minPrice.Value < 0)
            {
                minPrice = 0;
                TempData["Warning"] = "Цена не может быть отрицательной";
            }

            // Максимальная цена не может быть отрицательной
            if (maxPrice.HasValue && maxPrice.Value < 0)
            {
                maxPrice = 0;
                TempData["Warning"] = "Цена не может быть отрицательной";
            }

            // Цена не может быть больше 10 000 000 (10 знаков)
            if (minPrice.HasValue && minPrice.Value > 10000000)
            {
                minPrice = 10000000;
                TempData["Warning"] = "Цена не может быть больше 10 000 000";
            }

            if (maxPrice.HasValue && maxPrice.Value > 10000000)
            {
                maxPrice = 10000000;
                TempData["Warning"] = "Цена не может быть больше 10 000 000";
            }

            // Проверка: минимальная цена не должна быть больше максимальной
            if (minPrice.HasValue && maxPrice.HasValue && minPrice.Value > maxPrice.Value)
            {
                TempData["Error"] = "Минимальная цена не может быть больше максимальной";
                // Меняем местами
                int temp = minPrice.Value;
                minPrice = maxPrice;
                maxPrice = temp;
            }

            // Базовый запрос
            var query = _context.Cars
                .Include(c => c.CarImages)
                .Include(c => c.Model)
                    .ThenInclude(m => m.Manufacturer)
                .Include(c => c.CarTags)
                    .ThenInclude(ct => ct.Tag)
                .Where(c => c.Quantity > 0);

            // ПОИСК
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(c => c.Name.Contains(searchTerm) ||
                                         c.Model.Manufacturer.Name.Contains(searchTerm) ||
                                         c.Model.Name.Contains(searchTerm));
            }

            // ФИЛЬТР ПО ЦЕНЕ
            if (minPrice.HasValue && minPrice.Value >= 0)
            {
                query = query.Where(c => c.Price >= minPrice.Value);
            }
            if (maxPrice.HasValue && maxPrice.Value > 0)
            {
                query = query.Where(c => c.Price <= maxPrice.Value);
            }

            // ФИЛЬТР ПО ПРОИЗВОДИТЕЛЮ
            if (!string.IsNullOrEmpty(selectedManufacturer))
            {
                query = query.Where(c => c.Model.Manufacturer.Name == selectedManufacturer);
            }

            // ФИЛЬТР ПО ГОДУ
            if (selectedYear.HasValue && selectedYear.Value >= 1900 && selectedYear.Value <= DateTime.Now.Year + 1)
            {
                query = query.Where(c => c.Year == selectedYear.Value);
            }

            // ФИЛЬТР ПО ТЕГАМ
            if (selectedTagIds != null && selectedTagIds.Any())
            {
                if (selectedTagIds.Count > 10)
                {
                    selectedTagIds = selectedTagIds.Take(10).ToList();
                    TempData["Warning"] = "Выбрано слишком много тегов. Применены первые 10.";
                }
                query = query.Where(c => c.CarTags.Any(ct => selectedTagIds.Contains(ct.Tag_Id)));
            }

            // СОРТИРОВКА
            switch (sortBy)
            {
                case "name_asc":
                    query = query.OrderBy(c => c.Name);
                    break;
                case "name_desc":
                    query = query.OrderByDescending(c => c.Name);
                    break;
                case "price_asc":
                    query = query.OrderBy(c => c.Price);
                    break;
                case "price_desc":
                    query = query.OrderByDescending(c => c.Price);
                    break;
                case "year_asc":
                    query = query.OrderBy(c => c.Year);
                    break;
                case "year_desc":
                    query = query.OrderByDescending(c => c.Year);
                    break;
                default:
                    query = query.OrderBy(c => c.Name);
                    break;
            }

            // ПАГИНАЦИЯ
            int totalCount = await query.CountAsync();
            int pageSize = 12;
            int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            if (page < 1) page = 1;
            if (page > totalPages && totalPages > 0) page = totalPages;

            var cars = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // ПОЛУЧАЕМ СПИСКИ ДЛЯ ФИЛЬТРОВ
            var manufacturers = await _context.Manufacturers.Select(m => m.Name).Distinct().ToListAsync();
            var years = await _context.Cars.Select(c => c.Year).Distinct().OrderByDescending(y => y).ToListAsync();
            var tags = await _context.Tags.ToListAsync();

            // СОЗДАЕМ ViewModel
            var viewModel = new CatalogViewModel
            {
                Cars = cars,
                SearchTerm = searchTerm,
                SortBy = sortBy,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                SelectedManufacturer = selectedManufacturer,
                SelectedYear = selectedYear,
                SelectedTagIds = selectedTagIds ?? new List<int>(),
                Manufacturers = manufacturers,
                Years = years,
                Tags = tags,
                CurrentPage = page,
                TotalPages = totalPages,
                PageSize = pageSize,
                TotalCount = totalCount
            };

            return View(viewModel);
        }

        // ✅ ДЕТАЛЬНАЯ СТРАНИЦА ТОВАРА
        public async Task<IActionResult> Details(int id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            var car = await _context.Cars
                .Include(c => c.CarImages)
                .Include(c => c.Model)
                    .ThenInclude(m => m.Manufacturer)
                .Include(c => c.Model)
                    .ThenInclude(m => m.BodyType)
                .Include(c => c.Model)
                    .ThenInclude(m => m.EngineType)
                .Include(c => c.Model)
                    .ThenInclude(m => m.Transmission)
                .Include(c => c.Color)
                .Include(c => c.CarTags)
                    .ThenInclude(ct => ct.Tag)
                .FirstOrDefaultAsync(c => c.Car_Id == id);

            if (car == null)
                return NotFound();

            await UpdateSingleCarStatus(car);

            return View(car);
        }

        // 🔧 ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ
        private async Task UpdateCarStatusesByQuantity()
        {
            var cars = await _context.Cars.ToListAsync();
            bool changes = false;

            foreach (var car in cars)
            {
                string newStatus = GetStatusByQuantity(car.Quantity);
                if (car.AvailabilityStatus != newStatus)
                {
                    car.AvailabilityStatus = newStatus;
                    changes = true;
                }
            }

            if (changes)
            {
                await _context.SaveChangesAsync();
            }
        }

        private async Task UpdateSingleCarStatus(Cars car)
        {
            string newStatus = GetStatusByQuantity(car.Quantity);
            if (car.AvailabilityStatus != newStatus)
            {
                car.AvailabilityStatus = newStatus;
                await _context.SaveChangesAsync();
            }
        }

        private string GetStatusByQuantity(int quantity)
        {
            if (quantity <= 0)
                return "Нет в наличии";
            else if (quantity > 0 && quantity <= 3)
                return "Под заказ";
            else
                return "В наличии";
        }
    }
}