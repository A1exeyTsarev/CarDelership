using Microsoft.AspNetCore.Mvc;
using CarDelership.Data;
using Microsoft.EntityFrameworkCore;

namespace CarDelership.ViewComponents
{
    public class CartCountViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public CartCountViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            int count = 0;

            if (userId != null)
            {
                count = await _context.ShoppingCarts
                    .Where(c => c.User_Id == userId)
                    .SumAsync(c => c.Quantity);
            }

            return View(count);
        }
    }
}