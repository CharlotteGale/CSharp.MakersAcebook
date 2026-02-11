using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using acebook.Models;
using acebook.ActionFilters;
using Microsoft.EntityFrameworkCore;

namespace acebook.Controllers;

[ServiceFilter(typeof(AuthenticationFilter))]
public class FriendsController : Controller
{
    private readonly AcebookDbContext _context;
    private readonly ILogger<FriendsController> _logger;

    public FriendsController(AcebookDbContext context, ILogger<FriendsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [Route("/friends")]
    [HttpGet]
    //GetAllFriends
    public IActionResult Index() {
        int ActiveUserId = HttpContext.Session.GetInt32("user_id") ?? 0;
        var user = _context.Users
            .Include(u => u.Friends) 
            .FirstOrDefault(u => u.Id == ActiveUserId);

        ViewBag.Friends = user?.Friends?.ToList() ?? new List<User>();
        return View();
    }
}