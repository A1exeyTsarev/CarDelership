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