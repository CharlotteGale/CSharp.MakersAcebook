using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using acebook.Models;
using acebook.ActionFilters;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

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
        // Check if TempData has invalid post data
        if(TempData["InvalidPost"] != null)
        {
            var invalidPost = JsonSerializer.Deserialize<Post>(TempData["InvalidPost"].ToString());
            ///<summary>
            /// Gets the JSON string and converts it back to a Post object.
            /// </summary>
            
            var errors = JsonSerializer.Deserialize<Dictionary<string, string[]>>(
                TempData["ModelStateErrors"].ToString()
            );
            ///<summary>
            /// Gets the JSON string of errors and converts back to the Dictionary
            /// </summary>
            
            foreach(var error in errors)
            {
                foreach(var errorMessage in error.Value)
                {
                    ModelState.AddModelError(error.Key, errorMessage);
                }
            }

            ViewBag.InvalidPost = invalidPost;
        }

        int? currentUserId = HttpContext.Session.GetInt32("user_id");
        if (currentUserId == null) return Redirect("/");

        var currentUser = _context.Users
            .Include(u => u.Friends)
            .FirstOrDefault(u => u.Id == currentUserId);

        var allowedUserIds = currentUser.Friends
            .Select(f => f.Id)
            .Append(currentUser.Id) 
            .ToList();

        var posts = _context.Posts
            .Where(p => allowedUserIds.Contains(p.UserId))   
            .Include(p => p.User)
            .Include(p => p.Comments).ThenInclude(c => c.User)
            .Include(p => p.Likes)
            .OrderByDescending(p => p.CreatedAt)
            .ToList();


        // Sort comments oldest-first
        foreach (var post in posts)
        {
            post.Comments = post.Comments
                .OrderBy(c => c.CreatedAt)
                .ToList();
        }

        return View(posts);


    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
