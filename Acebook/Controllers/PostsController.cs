using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using acebook.Models;
using acebook.ActionFilters;

namespace acebook.Controllers;

[ServiceFilter(typeof(AuthenticationFilter))]
public class PostsController : Controller
{
    private readonly AcebookDbContext _context;
    private readonly ILogger<PostsController> _logger;

    public PostsController(AcebookDbContext context, ILogger<PostsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [Route("/Posts")]
    [HttpGet]
    public IActionResult Index() {
      // AcebookDbContext dbContext = new AcebookDbContext();
      List<Post> posts = _context.Posts.ToList();
      ViewBag.Posts = posts;
      return View();
    }

    [Route("/Posts")]
    [HttpPost]
    public RedirectResult Create(Post post) {
      int currentUserId = HttpContext.Session.GetInt32("user_id").Value;
      post.UserId = currentUserId;
      _context.Posts.Add(post);
      _context.SaveChanges();
      return new RedirectResult("Posts");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
