using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using CarDelership.Data;
using CarDelership.Models;

namespace CarDelership.Controllers
{
    [Authorize]
    public class FeedbackController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FeedbackController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Support/Index
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = GetCurrentUserId();

            // ✅ ИСПРАВЛЕНО: используем правильное имя поля User_Id
            var feedbacks = await _context.Feedbacks
                .Where(f => f.User_Id == userId)  // Было UserId, стало User_Id
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();

            return View(feedbacks);
        }

        // GET: /Support/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Support/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Feedback feedback)
        {
            // Проверяем, что данные пришли
            if (feedback == null)
            {
                TempData["Error"] = "Ошибка: данные не получены";
                return View(feedback);
            }

            // Проверяем валидацию
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Пожалуйста, заполните все поля корректно";
                return View(feedback);
            }

            try
            {
                var user = await GetCurrentUser();
                var userId = GetCurrentUserId();

                // ✅ ИСПРАВЛЕНО: используем правильное имя поля User_Id
                feedback.User_Id = userId;           // Было UserId, стало User_Id
                feedback.CreatedAt = DateTime.Now;
                feedback.IsProcessed = false;
                feedback.IsClosed = false;
                feedback.IsClosedByUser = false;

                _context.Feedbacks.Add(feedback);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Ваше сообщение отправлено! Мы ответим вам в ближайшее время.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Ошибка при сохранении: {ex.Message}";
                return View(feedback);
            }
        }

        // GET: /Support/Details/{id}
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var userId = GetCurrentUserId();

            // ✅ ИСПРАВЛЕНО: используем правильное имя поля User_Id
            var feedback = await _context.Feedbacks
                .FirstOrDefaultAsync(f => f.Feedback_Id == id && f.User_Id == userId);

            if (feedback == null)
            {
                return NotFound();
            }

            // Отмечаем как прочитанное
            if (feedback.IsProcessed && string.IsNullOrEmpty(feedback.IsReadByUser))
            {
                feedback.IsReadByUser = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                await _context.SaveChangesAsync();
            }

            return View(feedback);
        }

        // POST: /Support/Close/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Close(int id)
        {
            var userId = GetCurrentUserId();

            // ✅ ИСПРАВЛЕНО: используем правильное имя поля User_Id
            var feedback = await _context.Feedbacks
                .FirstOrDefaultAsync(f => f.Feedback_Id == id && f.User_Id == userId);

            if (feedback != null)
            {
                feedback.IsClosedByUser = true;
                await _context.SaveChangesAsync();
                TempData["Success"] = "Обращение закрыто";
            }

            return RedirectToAction("Index");
        }

        // GET: /Support/HasUnread
        [HttpGet]
        public async Task<IActionResult> HasUnread()
        {
            var userId = GetCurrentUserId();

            // ✅ ИСПРАВЛЕНО: используем правильное имя поля User_Id
            var hasUnread = await _context.Feedbacks
                .AnyAsync(f => f.User_Id == userId &&
                               f.IsProcessed == true &&
                               f.IsClosed == false &&
                               f.IsClosedByUser == false &&
                               f.IsReadByUser == null);

            return Json(new { hasUnread = hasUnread });
        }

        private int GetCurrentUserId()
        {
            var userName = User.Identity.Name;
            var user = _context.Users.FirstOrDefault(u => u.Login == userName);
            return user?.User_Id ?? 0;
        }

        private async Task<Users> GetCurrentUser()
        {
            var userName = User.Identity.Name;
            return await _context.Users.FirstOrDefaultAsync(u => u.Login == userName);
        }
    }
}