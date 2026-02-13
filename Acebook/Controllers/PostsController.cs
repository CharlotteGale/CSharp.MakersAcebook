using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using acebook.Models;
using acebook.ActionFilters;
using System.Text.Json;

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
      List<Post> posts = _context.Posts.ToList();
      ViewBag.Posts = posts;
      return View();
    }

    [Route("/Posts")]
    [HttpPost]
    public RedirectResult Create(Post post) {
      
      ModelState.Remove("UserId");
      ModelState.Remove("User");

      if (!ModelState.IsValid)
      {
        // Post conversion
        TempData["InvalidPost"] = JsonSerializer.Serialize(post);
        /// <summary>
        /// This takes a Post object and converts it to a JSON string.
        /// Stores it into TempData, which survives one redirect.
        /// </summary>
        
        // Validation error extraction and conversion
        TempData["ModelStateErrors"] = JsonSerializer.Serialize(
          ModelState.Where(x => x.Value.Errors.Count > 0)
            .ToDictionary(
              kvp => kvp.Key,
              kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
            )
        );
        /// <summary>
        /// Captures the errors found in ModelState, converts it to a dictionary.
        /// Which is then converted to JSON and stored in TempData.
        /// </summary>
        return new RedirectResult("/Feed");
      }

      int currentUserId = HttpContext.Session.GetInt32("user_id").Value;
      post.UserId = currentUserId;
      _context.Posts.Add(post);
      _context.SaveChanges();
      return new RedirectResult("/feed");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
