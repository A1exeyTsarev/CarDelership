using System.Diagnostics;
using CarDelership.Data;
using CarDelership.Models;
using CarDelership.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarDelership.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }


        public async Task<IActionResult> Index()
        {
            // Загружаем автомобили И их изображения через Include
            var cars = await _context.Cars
                .Include(c => c.CarImages)  // ?? КЛЮЧЕВАЯ СТРОКА - подгружаем картинки
                .Where(c => c.Quantity > 0)
                .Take(8)
                .ToListAsync();

            var viewModel = new HomeViewModel
            {
                PopularCars = cars ?? new List<Cars>(),
                CartCount = 0
            };

            return View(viewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}