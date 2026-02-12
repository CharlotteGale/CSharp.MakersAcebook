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

    // internal List<FriendRequest> PendingRequests()
    // {
    //     int ActiveUserId = HttpContext.Session.GetInt32("user_id") ?? 0;
    //     var requests = _context.FriendRequests
    //         .Include(fr => fr.User) 
    //         .Where(fr => fr.FriendId == ActiveUserId && fr.Pending == true || fr.UserId == ActiveUserId && fr.Pending == true)
    //         .ToList();
    //         return requests;
    // }

    [Route("/friends/requests")]
    [HttpGet]

    public IActionResult ReceivedRequests() {
        int ActiveUserId = HttpContext.Session.GetInt32("user_id") ?? 0;
        var requests = _context.FriendRequests
            .Include(fr => fr.User) 
            .Where(fr => fr.FriendId == ActiveUserId && fr.Pending == true)
            .ToList();

        ViewBag.Requests = requests;
        return View("~/Views/Friends/ReceivedRequests.cshtml");
        }

    [Route("/friends/requests")]
    [HttpPost]
    public IActionResult CreateRequests(int profileId) {
        int ActiveUserId = HttpContext.Session.GetInt32("user_id") ?? 0;
        FriendRequest request = new FriendRequest
        {
            UserId = ActiveUserId,
            FriendId = profileId,
        };
        _context.FriendRequests.Add(request);
        _context.SaveChanges();

        return new RedirectResult("/friends");
        }
        
        //need to request friendship
        //need to be able to accept request
        //need to reject.
}