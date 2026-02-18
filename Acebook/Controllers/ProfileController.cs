using Microsoft.AspNetCore.Mvc;
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

        var user = _context.Users.FirstOrDefault(u => u.Id == userId);
        return View(user);
    }
}
