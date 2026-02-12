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
    [Route("/friends/not_friends_yet")]
    [HttpGet]
    //GetAllUsers
    public IActionResult NotFriends() {
        int ActiveUserId = HttpContext.Session.GetInt32("user_id") ?? 0;

        var existingFriendsIds = _context.Users
            .Where(u => u.Id == ActiveUserId)
            .SelectMany(u => u.Friends) //Puts everything in one big list
            .Select(f => f.Id)
            .ToList();
        
        var allOtherUsers = _context.Users
            .Include(u => u.Friends) 
            .Where(u => u.Id != ActiveUserId && !existingFriendsIds.Contains(u.Id))
            .ToList();

        var requests = _context.FriendRequests
            .Where(fr => fr.FriendId == ActiveUserId && fr.Pending == true || fr.UserId == ActiveUserId && fr.Pending == true)
            .Select(fr => fr.UserId == ActiveUserId ? fr.FriendId : fr.UserId)
            // This is saying that the fr.UserId is equal to the ActiveUserId, add the fr.FriendId. If it is not true, then add the fr.UserId, all into a List. 
            .ToHashSet();

        ViewBag.NotFriends = allOtherUsers;
        ViewBag.Pending = requests;
        return View();
    }
}