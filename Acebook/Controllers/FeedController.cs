using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using acebook.Models;
using acebook.ActionFilters;
using Microsoft.EntityFrameworkCore;

namespace acebook.Controllers;

public class FeedController : Controller
{
    private readonly AcebookDbContext _context;
    private readonly ILogger<FeedController> _logger;

    public FeedController(AcebookDbContext context, ILogger<FeedController> logger)
    {
        _context = context;
        _logger = logger;
    }
    [Route("/Feed")]
    [HttpGet]
    public IActionResult Index()
    {
        int? currentUserId = HttpContext.Session.GetInt32("user_id");
        if (currentUserId == null) return Redirect("/");
        var posts = _context.Posts
            .Include(p => p.User)
            .Include(p => p.Comments)
                .ThenInclude(c => c.User)
            .ToList();
        return View(posts);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
