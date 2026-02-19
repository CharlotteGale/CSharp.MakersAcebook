using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using acebook.Models;

public class ProfileController : Controller
{
    private readonly AcebookDbContext _context;

    public ProfileController(AcebookDbContext context)
    {
        _context = context;
    }

        [HttpGet]
        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetInt32("user_id");
            if (userId == null)
            {
                return RedirectToAction("New", "Sessions");
            }

            var user = _context.Users
                .Include(u => u.Posts)
                .Include(u => u.Friends)
                .FirstOrDefault(u => u.Id == userId);

            if (user == null) return NotFound();

            var vm = new UserProfileViewModel
            {
                UserId = user.Id,
                Name = user.Name,
                DateOfBirth = user.DateOfBirth,
                RecentPosts = user.Posts
                    .OrderByDescending(p => p.CreatedAt)
                    .Take(10)
                    .ToList(),
                Friends = user.Friends.ToList()
            };

            return View("Index", vm);   // ⭐ THIS IS THE FIX
        }

}
