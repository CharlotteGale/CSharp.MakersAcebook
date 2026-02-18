using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using acebook.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace acebook.Controllers;

public class UsersController : Controller
{
    private readonly AcebookDbContext _context;
    private readonly ILogger<UsersController> _logger;

    public UsersController(AcebookDbContext context, ILogger<UsersController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [Route("/signup")]
    [HttpGet]
    public IActionResult New()
    {
        return View();
    }

    [Route("/users")]
    [HttpPost]
    public IActionResult Create(User user)
    {
        if (!ModelState.IsValid)
        {
            return View("New", user);
        }
        
        var email = user.Email.Trim().ToLower();
        if (_context.Users.Any(u => u.Email.ToLower() == email))
        {
            ModelState.AddModelError("Email", "An account with this email already exists.");
            return View("New", user);
        }
        user.Email = email;


        user.DateOfBirth = DateTime.SpecifyKind(user.DateOfBirth!.Value, DateTimeKind.Utc);
        int age = user.GetAge();
        if (age < 13)
        {
            ModelState.AddModelError("DateOfBirth", "You must be at least 13 years old to sign up.");
            return View("New", user);
        }

        var hasher = new PasswordHasher<User>();
        user.Password = hasher.HashPassword(user, user.Password);
        _context.Users.Add(user);
        _context.SaveChanges();
        TempData["SuccessMessage"] = "Your profile has been created successfully. Log in to continue.";
        return new RedirectResult("/");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
[Route("/users/{id:int}")]
[HttpGet]
public IActionResult Show(int id)
{
    // Require login
    var currentUserId = HttpContext.Session.GetInt32("user_id");
    if (currentUserId == null) return Redirect("/");

    var user = _context.Users
        .Include(u => u.Posts)
        .Include(u => u.Friends)
        .FirstOrDefault(u => u.Id == id);

    if (user == null) return NotFound();

    var recentPosts = user.Posts
        .OrderByDescending(p => p.CreatedAt) 
        .Take(10)
        .ToList();

    var vm = new UserProfileViewModel
    {
        UserId = user.Id,
        Name = user.Name,
        // ProfilePhotoUrl = user.ProfilePhotoUrl
        RecentPosts = recentPosts,
        Friends = user.Friends.ToList()
    };

    return View("Show", vm);
}

}