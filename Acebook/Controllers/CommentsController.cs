using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using acebook.Models;
using acebook.ActionFilters;

namespace acebook.Controllers;

public class CommentsController : Controller
{ 
    private readonly AcebookDbContext _context;
    private readonly ILogger<CommentsController> _logger;

    public CommentsController(AcebookDbContext context, ILogger<CommentsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [Route("/comments/create")]
    [HttpPost]
    public IActionResult Create(int postID, string content) {
    int? currentUserId = HttpContext.Session.GetInt32("user_id");
    if (currentUserId == null) return Redirect("/");
    if (string.IsNullOrWhiteSpace(content)) {return Redirect("/feed");}
    var comment = new Comment
    {
        PostId = postID,
        UserId = currentUserId.Value, 
        Content = content
    };
    _context.Comments.Add(comment);
    _context.SaveChanges();
    return Redirect("/feed");
    }
}