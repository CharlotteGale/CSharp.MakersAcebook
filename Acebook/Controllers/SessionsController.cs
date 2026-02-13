using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using acebook.Models;
using Microsoft.AspNetCore.Identity;
using System.IO.Compression;

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
    public IActionResult Create(string email, string password)
    {
        User? user = _context.Users.FirstOrDefault(user => user.Email == email);


        if (user == null)
        {
            TempData["ErrorMessage"] = "Your email or password details are incorrect.";
            return new RedirectResult("/");
        }
        
        var hasher = new PasswordHasher<User>();
        // var result = hasher.VerifyHashedPassword(user, user.Password, password);
        PasswordVerificationResult result; 
        bool isDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
        bool isPlainTextPassword = user.Password.Length < 20;
        if(isDevelopment && isPlainTextPassword)
        {
            result = user.Password == password
                ? PasswordVerificationResult.Success
                :PasswordVerificationResult.Failed;
        }
        else {
            result = hasher.VerifyHashedPassword(user, user.Password, password);
             }

        if (result == PasswordVerificationResult.Success || result == PasswordVerificationResult.SuccessRehashNeeded)
        {
            if (result == PasswordVerificationResult.SuccessRehashNeeded)
            {
                user.Password = hasher.HashPassword(user, password);
                _context.SaveChanges();
            }
            HttpContext.Session.SetInt32("user_id", user.Id);
            return new RedirectResult("/Feed");
        }
        else
        {
            TempData["ErrorMessage"] = "Your email or password details are incorrect.";
            return new RedirectResult("/");
        }

    }

    [HttpPost]
    [Route("logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Remove("user_id");
        return RedirectToAction("New", "Sessions");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
