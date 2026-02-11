using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using acebook.Models;
using acebook.ActionFilters;
using Microsoft.EntityFrameworkCore;

namespace acebook.Controllers;

[ServiceFilter(typeof(AuthenticationFilter))]
public class FriendRequestController : Controller
{
    private readonly AcebookDbContext _context;
    private readonly ILogger<FriendRequestController> _logger;

    public FriendRequestController(AcebookDbContext context, ILogger<FriendRequestController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [Route("/friends/request")]
    [HttpGet]

    public IActionResult Index() {
        int ActiveUserId = HttpContext.Session.GetInt32("user_id") ?? 0;
        var requests = _context.FriendRequests
            .Include(u => u.FriendId) 
            .FirstOrDefault(u => u.FriendId == ActiveUserId);

        ViewBag.Requests = requests?.
        return View();
        }
}