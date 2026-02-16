using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using acebook.Models;
using acebook.ActionFilters;

namespace acebook.Controllers;


public class LikesController : Controller
{
    private readonly AcebookDbContext _context;

    public LikesController(AcebookDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    [Route("likes/toggle")]
    public IActionResult Toggle(int postId)
    {
        var userId = HttpContext.Session.GetInt32("user_id");
        if (userId == null) return Redirect("/");

        var existingLike = _context.Likes
            .FirstOrDefault(l => l.PostId == postId && l.UserId == userId);

        if (existingLike != null)
        {
            // Unlike
            _context.Likes.Remove(existingLike);
        }
        else
        {
            // Like
            var like = new Like
            {
                PostId = postId,
                UserId = userId.Value
            };
            _context.Likes.Add(like);
        }

        _context.SaveChanges();
        return Redirect("/feed");
    }
}
