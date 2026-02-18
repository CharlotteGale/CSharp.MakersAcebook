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

    // ============================================================
    //  CONSOLIDATED FRIENDS PAGE (/friends)
    // ============================================================
    [Route("/friends")]
    [HttpGet]
    public IActionResult Index()
    {
        int ActiveUserId = HttpContext.Session.GetInt32("user_id") ?? 0;

        // FRIENDS
        var user = _context.Users
            .Include(u => u.Friends)
            .FirstOrDefault(u => u.Id == ActiveUserId);

        var friends = user?.Friends?.ToList() ?? new List<User>();
        ViewBag.Friends = friends;

        // EXISTING FRIEND IDS
        var existingFriendsIds = friends.Select(f => f.Id).ToList();

        // PENDING REQUESTS (IDs)
        var pending = _context.FriendRequests
            .Where(fr => fr.FriendId == ActiveUserId || fr.UserId == ActiveUserId)
            .Select(fr => fr.UserId == ActiveUserId ? fr.FriendId : fr.UserId)
            .ToHashSet();

        ViewBag.Pending = pending;

        // SUGGESTED FRIENDS
        var notFriends = _context.Users
            .Where(u => u.Id != ActiveUserId &&
                        !existingFriendsIds.Contains(u.Id) &&
                        !pending.Contains(u.Id))
            .ToList();

        ViewBag.NotFriends = notFriends;

        // RECEIVED REQUESTS
        var receivedRequests = _context.FriendRequests
            .Include(fr => fr.User)
            .Where(fr => fr.FriendId == ActiveUserId)
            .ToList();

        ViewBag.ReceivedRequests = receivedRequests;

        // SENT REQUESTS
        var sentRequests = _context.FriendRequests
            .Include(fr => fr.Friend)
            .Where(fr => fr.UserId == ActiveUserId)
            .ToList();

        ViewBag.SentRequests = sentRequests;

        return View();
    }

    // ============================================================
    //  UNFRIEND
    // ============================================================
    [Route("/friends/delete")]
    [HttpPost]
    public IActionResult DeleteFriend(int friendToDeleteId)
    {
        int ActiveUserId = HttpContext.Session.GetInt32("user_id") ?? 0;

        var user = _context.Users
            .Include(u => u.Friends)
            .FirstOrDefault(u => u.Id == ActiveUserId);

        var friendToDelete = _context.Users
            .Include(u => u.Friends)
            .FirstOrDefault(u => u.Id == friendToDeleteId);

        if (user != null && friendToDelete != null)
        {
            user.RemoveFriend(friendToDelete);
            _context.SaveChanges();
        }

        return new RedirectResult("/friends");
    }

    // ============================================================
    //  ACCEPT FRIEND REQUEST
    // ============================================================
    [Route("/friends/accept")]
    [HttpPost]
    public IActionResult AcceptRequest(int requestId, int requestSenderId)
    {
        int ActiveUserId = HttpContext.Session.GetInt32("user_id") ?? 0;

        var request = _context.FriendRequests
            .FirstOrDefault(fr => fr.Id == requestId);

        if (request != null)
        {
            var user = _context.Users
                .Include(u => u.Friends)
                .FirstOrDefault(u => u.Id == ActiveUserId);

            var sender = _context.Users
                .Include(u => u.Friends)
                .FirstOrDefault(u => u.Id == requestSenderId);

            if (user != null && sender != null)
            {
                user.AddFriend(sender);
                sender.AddFriend(user);
                _context.FriendRequests.Remove(request);
                _context.SaveChanges();
            }
        }

        return new RedirectResult("/friends");
    }

    // ============================================================
    //  REJECT FRIEND REQUEST
    // ============================================================
    [Route("/friends/reject")]
    [HttpPost]
    public IActionResult RejectRequest(int requestId)
    {
        var request = _context.FriendRequests
            .FirstOrDefault(fr => fr.Id == requestId);

        if (request != null)
        {
            _context.FriendRequests.Remove(request);
            _context.SaveChanges();
        }

        return new RedirectResult("/friends");
    }

    // // ============================================================
    // //  OLD SUGGESTED FRIENDS PAGE - now void
    // // ============================================================
    // [Route("/friends/not_friends_yet")]
    // [HttpGet]
    // public IActionResult NotFriends()
    // {
    //     int ActiveUserId = HttpContext.Session.GetInt32("user_id") ?? 0;

    //     var user = _context.Users
    //         .Include(u => u.Friends)
    //         .FirstOrDefault(u => u.Id == ActiveUserId);

    //     var existingFriendsIds = user?.Friends?.Select(f => f.Id).ToList() ?? new List<int>();

    //     var pending = _context.FriendRequests
    //         .Where(fr => fr.FriendId == ActiveUserId || fr.UserId == ActiveUserId)
    //         .Select(fr => fr.UserId == ActiveUserId ? fr.FriendId : fr.UserId)
    //         .ToHashSet();

    //     ViewBag.Pending = pending;

    //     var notFriends = _context.Users
    //         .Where(u => u.Id != ActiveUserId &&
    //                     !existingFriendsIds.Contains(u.Id) &&
    //                     !pending.Contains(u.Id))
    //         .ToList();

    //     ViewBag.NotFriends = notFriends;

    //     return View("NotFriends");
    // }
}
