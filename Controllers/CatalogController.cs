using CarDelership.Data;
using CarDelership.Models;
using CarDelership.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarDelership.Controllers
{
    public class CatalogController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CatalogController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Catalog(string searchTerm, string sortBy,
            int? minPrice, int? maxPrice, string selectedBrand, int? selectedYear,
            List<int> selectedTags, int page = 1)
        {
            var query = _context.Cars
                .Include(c => c.CarTags)
                .ThenInclude(ct => ct.Tag)
                .Where(c => c.Quantity > 0)
                .AsQueryable();

            // ========== ПОИСК ==========
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(c => c.Name.Contains(searchTerm));
                ViewBag.SearchTerm = searchTerm;
            }

            // ========== ФИЛЬТРАЦИЯ ПО ЦЕНЕ ==========
            if (minPrice.HasValue)
            {
                query = query.Where(c => c.Price >= minPrice.Value);
            }
            if (maxPrice.HasValue)
            {
                query = query.Where(c => c.Price <= maxPrice.Value);
            }

            // ========== ФИЛЬТРАЦИЯ ПО БРЕНДУ ==========
            if (!string.IsNullOrEmpty(selectedBrand))
            {
                query = query.Where(c => c.Name.Contains(selectedBrand));
            }

            // ========== ФИЛЬТРАЦИЯ ПО ГОДУ ==========
            if (selectedYear.HasValue)
            {
                query = query.Where(c => c.Year == selectedYear.Value);
            }

            // ========== ФИЛЬТРАЦИЯ ПО ТЕГАМ ==========
            if (selectedTags != null && selectedTags.Any())
            {
                query = query.Where(c => c.CarTags.Any(ct => selectedTags.Contains(ct.Tag_Id)));
            }

            // ========== ПОДСЧЕТ ВСЕГО КОЛИЧЕСТВА ==========
            var totalCount = await query.CountAsync();

            // ========== СОРТИРОВКА ==========
            switch (sortBy)
            {
                case "price_asc":
                    query = query.OrderBy(c => c.Price);
                    break;
                case "price_desc":
                    query = query.OrderByDescending(c => c.Price);
                    break;
                case "year_desc":
                    query = query.OrderByDescending(c => c.Year);
                    break;
                case "year_asc":
                    query = query.OrderBy(c => c.Year);
                    break;
                case "name_asc":
                default:
                    query = query.OrderBy(c => c.Name);
                    break;
            }

            // ========== ПАГИНАЦИЯ ==========
            int pageSize = 12;
            var cars = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // ========== ПОЛУЧАЕМ СПИСКИ ДЛЯ ФИЛЬТРОВ ==========
            // Сначала получаем все автомобили
            var allCars = await _context.Cars.ToListAsync();

            // Обрабатываем бренды в памяти (исправленная часть)
            var brands = allCars
                .Select(c => c.Name.Split(' ').FirstOrDefault() ?? "")
                .Distinct()
                .Where(b => !string.IsNullOrEmpty(b))
                .OrderBy(b => b)
                .ToList();

            var years = await _context.Cars
                .Select(c => c.Year)
                .Distinct()
                .OrderByDescending(y => y)
                .ToListAsync();

            var tags = await _context.Tags.ToListAsync();

            // ========== СОЗДАЕМ VIEWMODEL ==========
            var viewModel = new CatalogViewModel
            {
                Cars = cars,
                SearchTerm = searchTerm ?? "",
                SortBy = sortBy ?? "name_asc",
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                SelectedBrand = selectedBrand ?? "",
                SelectedYear = selectedYear,
                Brands = brands,
                Years = years,
                Tags = tags,
                SelectedTagIds = selectedTags ?? new List<int>(),
                CurrentPage = page,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                PageSize = pageSize
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Details(int id)
        {
            var car = await _context.Cars
                .Include(c => c.CarTags)
                .ThenInclude(ct => ct.Tag)
                .FirstOrDefaultAsync(c => c.Car_Id == id);

            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }
    }
}