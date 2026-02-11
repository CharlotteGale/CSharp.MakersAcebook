using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using acebook.Models;

namespace acebook.Controllers;

public class SessionsController : Controller
{
    private readonly AcebookDbContext _context;
    private readonly ILogger<SessionsController> _logger;

    public SessionsController(AcebookDbContext context, ILogger<SessionsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [Route("/")]
    [HttpGet]
    public IActionResult New()
    {
        return View();
    }

    [Route("/")]
    [HttpPost]
    public IActionResult Create(string email, string password) {
      User? user = _context.Users.FirstOrDefault(user => user.Email == email);
      if(user != null && user.Password == password)
      {
        HttpContext.Session.SetInt32("user_id", user.Id);
        return new RedirectResult("/Feed");
      }
      else
      {
        TempData["ErrorMessage"] = "Your email or password details are incorrect. Try again, bozo.";
        return new RedirectResult("/");
      }
    }

    [HttpPost]
    [Route("logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Remove("user_id"); 
        return RedirectToAction("New","Sessions");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
