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
        private readonly ApplicationDbContext _context;  // ? добавить

        // ? изменить конструктор
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;  // ? добавить
        }

        // ? изменить метод Index
        public async Task<IActionResult> Index()
        {
            var cars = await _context.Cars
                .Where(c => c.Quantity > 0)  // только те, что в наличии
                .Take(8)  // первые 8
                .ToListAsync();

            var viewModel = new HomeViewModel
            {
                PopularCars = cars ?? new List<Cars>(),  // если null, то пустой список
                CartCount = 0
            };

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}