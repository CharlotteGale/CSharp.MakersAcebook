using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using acebook.Models;
using acebook.ActionFilters;

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
        List<Post> posts = _context.Posts.ToList();
        ViewBag.Posts = posts;
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
