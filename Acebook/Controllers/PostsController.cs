using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using acebook.Models;
using acebook.ActionFilters;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

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
[HttpPost]
public RedirectResult Create(string content, IFormFile? imageFile=null)
{
    int currentUserId = HttpContext.Session.GetInt32("user_id").Value;

    string? imagePath = null;

    // Save image if uploaded
    if (imageFile != null && imageFile.Length > 0)
    {
        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
        var filePath = Path.Combine(uploadsFolder, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            imageFile.CopyTo(stream);
        }

        imagePath = "/uploads/" + fileName;
    }

    // Validation 
    bool hasContent = !string.IsNullOrWhiteSpace(content);
    bool hasImage = imagePath != null;

    if (!hasContent && !hasImage)
    {
        ModelState.AddModelError("Content", "You must provide content or an image.");
    }

    // Build the post
    var user = _context.Users.First(u => u.Id == currentUserId);

    var post = new Post
    {
        Content = content,
        ImgLink = imagePath,
        UserId = currentUserId,
        User = user
    };

    // If validation failed, return errors
    if (!ModelState.IsValid)
    {
        TempData["InvalidPost"] = JsonSerializer.Serialize(post);

        TempData["ModelStateErrors"] = JsonSerializer.Serialize(
            ModelState.Where(x => x.Value.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                )
        );

        return new RedirectResult("/Feed");
    }

    // Save post
    _context.Posts.Add(post);
    _context.SaveChanges();

    return new RedirectResult("/Feed");
}




    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }


    [HttpPost]
    [Route("/posts/edit")]
    public IActionResult Edit(int postId, string content)
    {
        var userId = HttpContext.Session.GetInt32("user_id");
        if (userId == null) return Redirect("/");

        var post = _context.Posts.FirstOrDefault(p => p.Id == postId);
        if (post == null) return Redirect("/feed");

        if (post.UserId != userId)
            return Redirect("/feed");

        post.Content = content;
        post.UpdatedAt = DateTime.UtcNow;
        _context.SaveChanges();

        return Redirect("/feed");
    }
    [HttpPost]
    [Route("posts/delete")]
    public IActionResult Delete(int postId)
    {
        var userId = HttpContext.Session.GetInt32("user_id");
        if (userId == null) return Redirect("/");
        var post = _context.Posts.FirstOrDefault(p => p.Id == postId);
        if (post == null) return Redirect("/feed");

        // Only owner can delete
        if (post.UserId != userId) return Redirect("/feed");

        _context.Posts.Remove(post);
        _context.SaveChanges();

        return Redirect("/feed");
    }
    
    [Route("/posts/{id:int}")]
    [HttpGet]
    public IActionResult Show(int id)
    {
        var userId = HttpContext.Session.GetInt32("user_id");
        if (userId == null) return Redirect("/");

        var post = _context.Posts
            .Include(p => p.User)
            .Include(p => p.Likes)
            .Include(p => p.Comments)
                .ThenInclude(c => c.User)
            .FirstOrDefault(p => p.Id == id);

        if (post == null) return NotFound();

        return View(post);
    }



}
