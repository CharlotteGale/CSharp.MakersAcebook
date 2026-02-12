using Microsoft.AspNetCore.Mvc;
using acebook.Models;

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

    [HttpPost]
    [Route("/comments/create")]
    public IActionResult Create(int postId, string content)
    {
        int? currentUserId = HttpContext.Session.GetInt32("user_id");
        if (currentUserId == null) return Redirect("/");

        if (string.IsNullOrWhiteSpace(content))
            return Redirect("/feed");

        var comment = new Comment
        {
            PostId = postId,
            UserId = currentUserId.Value,
            Content = content
        };

        _context.Comments.Add(comment);
        _context.SaveChanges();

        return Redirect("/feed");
    }

    [HttpPost]
    [Route("/comments/edit")]
    public IActionResult Edit(int commentId, string content)
    {
        var userId = HttpContext.Session.GetInt32("user_id");
        if (userId == null) return Redirect("/");

        var comment = _context.Comments.FirstOrDefault(c => c.Id == commentId);
        if (comment == null) return Redirect("/feed");

        if (comment.UserId != userId)
            return Redirect("/feed");

        comment.Content = content;
        _context.SaveChanges();

        return Redirect("/feed");
    }
}
