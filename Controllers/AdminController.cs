using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using CarDelership.Data;
using CarDelership.Models;

namespace CarDelership.Controllers
{
    [Authorize(Roles = "Администратор")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==================== УПРАВЛЕНИЕ ПОЛЬЗОВАТЕЛЯМИ ====================

        // GET: /Admin/Users
        [HttpGet]
        public async Task<IActionResult> Users()
        {
            var users = await _context.Users
                .Include(u => u.Role)
                .OrderByDescending(u => u.RegistrationDate)
                .ToListAsync();

            return View(users);
        }

        // POST: /Admin/ToggleUserStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleUserStatus(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return Json(new { success = false, message = "Пользователь не найден" });
            }

            // Нельзя заблокировать самого себя
            var currentUserId = HttpContext.Session.GetInt32("UserId");
            if (user.User_Id == currentUserId)
            {
                return Json(new { success = false, message = "Нельзя изменить статус самого себя" });
            }

            // Нельзя менять статус администратора
            if (user.Role_Id == 1)
            {
                return Json(new { success = false, message = "Нельзя изменять статус администратора" });
            }

            user.IsActive = !user.IsActive;
            await _context.SaveChangesAsync();

            string status = user.IsActive ? "активирован" : "заблокирован";
            return Json(new { success = true, message = $"Пользователь {status}", isActive = user.IsActive });
        }

        // POST: /Admin/ChangeUserRole
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeUserRole(int userId, int roleId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return Json(new { success = false, message = "Пользователь не найден" });
            }

            // Нельзя менять роль самого себя
            var currentUserId = HttpContext.Session.GetInt32("UserId");
            if (user.User_Id == currentUserId)
            {
                return Json(new { success = false, message = "Нельзя изменить роль самого себя" });
            }

            // Нельзя менять роль администратора
            if (user.Role_Id == 1)
            {
                return Json(new { success = false, message = "Нельзя изменять роль администратора" });
            }

            user.Role_Id = roleId;
            await _context.SaveChangesAsync();

            var role = await _context.Roles.FindAsync(roleId);
            return Json(new { success = true, message = $"Роль изменена на {role?.Name}", roleName = role?.Name });
        }

        // POST: /Admin/DeleteUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            var user = await _context.Users
                .Include(u => u.Orders)
                .Include(u => u.ShoppingCarts)
                .Include(u => u.CarComments)
                .Include(u => u.Wishlists)
                .FirstOrDefaultAsync(u => u.User_Id == userId);

            if (user == null)
            {
                return Json(new { success = false, message = "Пользователь не найден" });
            }

            // Нельзя удалить самого себя
            var currentUserId = HttpContext.Session.GetInt32("UserId");
            if (user.User_Id == currentUserId)
            {
                return Json(new { success = false, message = "Нельзя удалить самого себя" });
            }

            // Нельзя удалять администратора
            if (user.Role_Id == 1)
            {
                return Json(new { success = false, message = "Нельзя удалять администратора" });
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Пользователь удален" });
        }

        // ==================== ОБРАТНАЯ СВЯЗЬ ====================

        // GET: /Admin/Feedbacks
        [HttpGet]
        public async Task<IActionResult> Feedbacks(string status = "all")
        {
            IQueryable<Feedback> query = _context.Feedbacks
                .Include(f => f.User);

            switch (status)
            {
                case "unprocessed":
                    query = query.Where(f => !f.IsProcessed && !f.IsClosed);
                    break;
                case "processed":
                    query = query.Where(f => f.IsProcessed && !f.IsClosed);
                    break;
                case "closed":
                    query = query.Where(f => f.IsClosed || f.IsClosedByUser);
                    break;
            }

            var feedbacks = await query.OrderByDescending(f => f.CreatedAt).ToListAsync();
            ViewBag.CurrentStatus = status;
            return View(feedbacks);
        }

        // GET: /Admin/FeedbackDetails/{id}
        [HttpGet]
        public async Task<IActionResult> FeedbackDetails(int id)
        {
            var feedback = await _context.Feedbacks
                .Include(f => f.User)
                .FirstOrDefaultAsync(f => f.Feedback_Id == id);

            if (feedback == null)
                return NotFound();

            return View(feedback);
        }

        // POST: /Admin/ProcessFeedback
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessFeedback(int id, string adminComment)
        {
            var feedback = await _context.Feedbacks.FindAsync(id);
            if (feedback != null)
            {
                feedback.IsProcessed = true;
                feedback.ProcessedAt = DateTime.Now;
                feedback.ProcessedBy = User.Identity.Name;
                feedback.AdminComment = adminComment;
                await _context.SaveChangesAsync();
                TempData["Success"] = "Заявка отмечена как обработанная";
            }
            return RedirectToAction("Feedbacks");
        }

        // POST: /Admin/CloseFeedback
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CloseFeedback(int id)
        {
            var feedback = await _context.Feedbacks.FindAsync(id);
            if (feedback != null)
            {
                feedback.IsClosed = true;
                await _context.SaveChangesAsync();
                TempData["Success"] = "Заявка закрыта";
            }
            return RedirectToAction("Feedbacks");
        }
    }
}