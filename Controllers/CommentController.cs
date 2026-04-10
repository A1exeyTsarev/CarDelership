using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarDelership.Data;
using CarDelership.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;

namespace CarDelership.Controllers
{
    public class CommentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CommentController> _logger;

        public CommentController(ApplicationDbContext context, ILogger<CommentController> logger)
        {
            _context = context;
            _logger = logger;
        }

        private int? GetCurrentUserId()
        {
            return HttpContext.Session.GetInt32("UserId");
        }

        // GET: /Comment/GetComments?carId=1
        [HttpGet]
        public async Task<IActionResult> GetComments(int carId)
        {
            try
            {
                var comments = await _context.CarComments
                    .Include(c => c.Author)
                    .Where(c => c.Car_Id == carId)
                    .OrderByDescending(c => c.CreatedAt)
                    .Select(c => new
                    {
                        id = c.CarComment_Id,
                        authorName = c.Author != null && !string.IsNullOrEmpty(c.Author.FullName)
                            ? c.Author.FullName
                            : (c.Author != null && !string.IsNullOrEmpty(c.Author.Login)
                                ? c.Author.Login
                                : "Пользователь"),
                        commentText = c.CommentText,
                        createdAt = c.CreatedAt.ToString("dd.MM.yyyy HH:mm"),
                        isOwner = c.Author_Id == GetCurrentUserId()
                    })
                    .ToListAsync();

                return Json(comments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при загрузке комментариев для автомобиля {CarId}", carId);
                return Json(new List<object>());
            }
        }

        // POST: /Comment/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int carId, string commentText)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Json(new { success = false, message = "Необходимо авторизоваться" });
                }

                if (string.IsNullOrWhiteSpace(commentText))
                {
                    return Json(new { success = false, message = "Текст комментария не может быть пустым" });
                }

                if (commentText.Length > 2000)
                {
                    return Json(new { success = false, message = "Комментарий не должен превышать 2000 символов" });
                }

                var car = await _context.Cars.FindAsync(carId);
                if (car == null)
                {
                    return Json(new { success = false, message = "Автомобиль не найден" });
                }

                var comment = new CarComments
                {
                    Car_Id = carId,
                    Author_Id = userId.Value,
                    CommentText = commentText.Trim(),
                    CreatedAt = DateTime.Now
                };

                _context.CarComments.Add(comment);
                await _context.SaveChangesAsync();

                var user = await _context.Users.FindAsync(userId);
                var authorName = user != null && !string.IsNullOrEmpty(user.FullName)
                    ? user.FullName
                    : (user != null && !string.IsNullOrEmpty(user.Login)
                        ? user.Login
                        : "Пользователь");

                return Json(new
                {
                    success = true,
                    message = "Комментарий добавлен",
                    comment = new
                    {
                        id = comment.CarComment_Id,
                        authorName = authorName,
                        commentText = comment.CommentText,
                        createdAt = comment.CreatedAt.ToString("dd.MM.yyyy HH:mm"),
                        isOwner = true
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при добавлении комментария");
                return Json(new { success = false, message = "Произошла ошибка при добавлении комментария" });
            }
        }

        // POST: /Comment/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int commentId)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Json(new { success = false, message = "Необходимо авторизоваться" });
                }

                var comment = await _context.CarComments
                    .FirstOrDefaultAsync(c => c.CarComment_Id == commentId);

                if (comment == null)
                {
                    return Json(new { success = false, message = "Комментарий не найден" });
                }

                if (comment.Author_Id != userId)
                {
                    return Json(new { success = false, message = "Вы можете удалять только свои комментарии" });
                }

                _context.CarComments.Remove(comment);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Комментарий удален" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении комментария");
                return Json(new { success = false, message = "Ошибка при удалении комментария" });
            }
        }
    }
}