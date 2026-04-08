using Microsoft.AspNetCore.Mvc;
using CarDelership.Data;
using Microsoft.EntityFrameworkCore;

namespace CarDelership.ViewComponents
{
    public class WishlistCountViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public WishlistCountViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            int count = 0;

            if (userId != null)
            {
                count = await _context.Wishlists
                    .CountAsync(w => w.User_Id == userId);
            }

            return View(count);
        }
    }
}