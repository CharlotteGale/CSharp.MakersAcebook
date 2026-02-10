using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using acebook.Models;

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
    public RedirectResult Create(User user) {
        _context.Users.Add(user);
        _context.SaveChanges();
        return new RedirectResult("/posts");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
