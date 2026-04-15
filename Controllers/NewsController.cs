// Controllers/NewsController.cs
using Microsoft.AspNetCore.Mvc;
using CarDelership.Data;
using Microsoft.EntityFrameworkCore;

namespace CarDelership.Controllers
{
    public class NewsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NewsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /News/
        public async Task<IActionResult> Index()
        {
            // Здесь можно загружать новости из базы данных
            // Пока возвращаем временное представление
            return View();
        }

        // GET: /News/Details/5
        public async Task<IActionResult> Details(int id)
        {
            return View();
        }
    }
}