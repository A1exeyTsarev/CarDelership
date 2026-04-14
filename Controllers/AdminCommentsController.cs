using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using CarDelership.Data;
using CarDelership.Models;

namespace CarDelership.Controllers
{
    [Authorize(Roles = "Администратор")]
    public class AdminCommentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminCommentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /AdminComments/Index
        [HttpGet]
        public async Task<IActionResult> Index(string status = "pending")
        {
            IQueryable<CarComments> query = _context.CarComments
                .Include(c => c.Author)
                .Include(c => c.Car);

            if (status == "pending")
            {
                query = query.Where(c => !c.IsApproved);
            }
            else if (status == "approved")
            {
                query = query.Where(c => c.IsApproved);
            }

            var comments = await query
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            ViewBag.CurrentStatus = status;
            return View(comments);
        }

        // POST: /AdminComments/Approve/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var comment = await _context.CarComments.FindAsync(id);
            if (comment != null)
            {
                comment.IsApproved = true;
                comment.ApprovedAt = DateTime.Now;
                comment.ApprovedBy = User.Identity.Name;
                await _context.SaveChangesAsync();

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = true, message = "Комментарий одобрен" });
                }
                TempData["Success"] = "Комментарий одобрен и опубликован";
            }
            return RedirectToAction("Index");
        }

        // POST: /AdminComments/Reject/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id)
        {
            var comment = await _context.CarComments.FindAsync(id);
            if (comment != null)
            {
                _context.CarComments.Remove(comment);
                await _context.SaveChangesAsync();

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = true, message = "Комментарий отклонен" });
                }
                TempData["Success"] = "Комментарий отклонен и удален";
            }
            return RedirectToAction("Index");
        }

        // POST: /AdminComments/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var comment = await _context.CarComments.FindAsync(id);
            if (comment != null)
            {
                _context.CarComments.Remove(comment);
                await _context.SaveChangesAsync();

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = true, message = "Комментарий удален" });
                }
                TempData["Success"] = "Комментарий удален";
            }
            return RedirectToAction("Index");
        }
    }
}