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

        // ✅ КАТАЛОГ С ФИЛЬТРАЦИЕЙ
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

            // Базовый запрос
            var query = _context.Cars
                .Include(c => c.CarImages)
                .Include(c => c.Model)
                    .ThenInclude(m => m.Manufacturer)
                .Include(c => c.CarTags)
                    .ThenInclude(ct => ct.Tag)
                .Where(c => c.Quantity > 0);

            // 🔍 ПОИСК
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(c => c.Name.Contains(searchTerm) ||
                                         c.Model.Manufacturer.Name.Contains(searchTerm));
            }

            // 💰 ФИЛЬТР ПО ЦЕНЕ
            if (minPrice.HasValue)
            {
                query = query.Where(c => c.Price >= minPrice.Value);
            }
            if (maxPrice.HasValue)
            {
                query = query.Where(c => c.Price <= maxPrice.Value);
            }

            // 🏭 ФИЛЬТР ПО ПРОИЗВОДИТЕЛЮ
            if (!string.IsNullOrEmpty(selectedManufacturer))
            {
                query = query.Where(c => c.Model.Manufacturer.Name == selectedManufacturer);
            }

            // 📅 ФИЛЬТР ПО ГОДУ
            if (selectedYear.HasValue)
            {
                query = query.Where(c => c.Year == selectedYear.Value);
            }

            // 🏷️ ФИЛЬТР ПО ТЕГАМ
            if (selectedTagIds != null && selectedTagIds.Any())
            {
                query = query.Where(c => c.CarTags.Any(ct => selectedTagIds.Contains(ct.Tag_Id)));
            }

            // 📊 СОРТИРОВКА
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

            // 📄 ПАГИНАЦИЯ
            int totalCount = await query.CountAsync();
            int pageSize = 12;
            int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var cars = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // 📋 ПОЛУЧАЕМ СПИСКИ ДЛЯ ФИЛЬТРОВ
            var manufacturers = await _context.Manufacturers.Select(m => m.Name).Distinct().ToListAsync();
            var years = await _context.Cars.Select(c => c.Year).Distinct().OrderByDescending(y => y).ToListAsync();
            var tags = await _context.Tags.ToListAsync();

            // ✅ СОЗДАЕМ ViewModel
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
                .Include(c => c.CarTags)
                    .ThenInclude(ct => ct.Tag)
                .FirstOrDefaultAsync(c => c.Car_Id == id);

            if (car == null)
                return NotFound();

            return View(car);
        }
    }
}