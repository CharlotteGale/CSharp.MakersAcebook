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

    [Route("/friends/requests")]
    [HttpGet]

    public IActionResult ReceivedRequests() {
        int ActiveUserId = HttpContext.Session.GetInt32("user_id") ?? 0;
        var requests = _context.FriendRequests
            .Include(fr => fr.User) 
            .Where(fr => fr.FriendId == ActiveUserId)
            .ToList();

        ViewBag.Requests = requests;
        return View("~/Views/Friends/ReceivedRequests.cshtml");
        }

    [Route("/friends/not_friends_yet")]
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

    [Route("/friends/requests")]
    [HttpPost]
    public IActionResult AcceptRequest(int requestSenderId, int requestId)
    {
        Console.WriteLine($"else statement  {requestSenderId}");
        int ActiveUserId = HttpContext.Session.GetInt32("user_id") ?? 0;
        var user = _context.Users
             .Where(u => u.Id == ActiveUserId)
             .FirstOrDefault();
        var requestSender = _context.Users
             .Where(u => u.Id == requestSenderId)
             .FirstOrDefault();
        user.AddFriend(requestSender);


        _context.SaveChanges();

        return new RedirectResult("/friends");
    }

        //need to request friendship
        //need to be able to accept request
        //need to reject.
        // 
}