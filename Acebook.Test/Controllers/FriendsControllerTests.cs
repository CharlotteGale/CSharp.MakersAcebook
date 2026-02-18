namespace Acebook.Test.Controllers;

public class FriendsControllerTests : NUnitTestBase
{
  private FriendsController _controller;
  private ILogger<FriendsController> _logger;
  private User _testUser;
  private User _testRequestUser;

  [SetUp]
  public void SetUp()
  {
    _logger = Mock.Of<ILogger<FriendsController>>();
    _controller = new FriendsController(_context, _logger);

    var httpContext = new DefaultHttpContext();
    httpContext.Session = new MockHttpSession();

    _controller.ControllerContext = new ControllerContext
    {
      HttpContext = httpContext
    };

    _controller.TempData = new TempDataDictionary(
        httpContext,
        Mock.Of<ITempDataProvider>()
    );

    DateTime Dob(int year, int month, int day) => DateTime.SpecifyKind(new DateTime(year, month, day), DateTimeKind.Utc);
        _testUser = new User
        {
            Name = "Test User",
            Email = "test@example.com",
            Password = "hashedPW1!",
            DateOfBirth = Dob(2000, 01, 01)
        };
    _context.Users.Add(_testUser);
    
    _testRequestUser = new User
    {
      Name = "Request User",
      Email = "request@example.com",
      Password = "Password1!",
      DateOfBirth = Dob(2000,01,01)
    };
    _context.Users.Add(_testRequestUser);
    _context.SaveChanges();

    _controller.HttpContext.Session.SetInt32("user_id", _testUser.Id);

  }
    
  [TearDown]
  public void TearDown()
  {
    _controller?.Dispose();
  }

  [Test]
  public void Index_ShouldSeeNoFriends_WhenUserHasNoFriendsInJoinTable()
  {
    var result = _controller.Index() as ViewResult;

    Assert.That(result, Is.Not.Null);

    var friendsList = result.ViewData["Friends"] as List<User>;
    
    Assert.That(friendsList, Is.Not.Null);
    Assert.That(friendsList.Count, Is.EqualTo(0));
  }

  [Test]
  public void Index_ShouldSeeFriends_WhenUserHasFriendsInJoinTable()
  {
    _testUser.AddFriend(_testRequestUser);
    var result = _controller.Index() as ViewResult;

    Assert.That(result, Is.Not.Null);
    var friendsList = result.ViewData["Friends"] as List<User>;
    
    Assert.That(friendsList, Is.Not.Null);
    Assert.That(friendsList.Count, Is.EqualTo(1));
  }

  [Test]
  public void Index_ShouldSeePotentialFriends_WhenThereAreUsersNotFriendsWith()
  {
    var result = _controller.Index() as ViewResult;

    Assert.That(result, Is.Not.Null);
    var notFriendsList = result.ViewData["NotFriends"] as List<User>;
    
    Assert.That(notFriendsList, Is.Not.Null);
    Assert.That(notFriendsList.Count, Is.EqualTo(21));
  }

  [Test]
  public void Index_ShouldSeePendingFriends_WhenUserHasPendingRequests()
  {
    var incomingRequest = new FriendRequest
    {
      UserId = _testRequestUser.Id,
      FriendId = _testUser.Id
    };
    _context.FriendRequests.Add(incomingRequest);
    _context.SaveChanges();

    var result = _controller.Index() as ViewResult;
    Assert.That(result, Is.Not.Null);
    var pendingRequests = result.ViewData["Pending"] as HashSet<int>;
    
    Assert.That(pendingRequests, Is.Not.Null);
    Assert.That(pendingRequests.Count, Is.EqualTo(1));
    Assert.That(pendingRequests.Contains(_testRequestUser.Id), Is.True);
  }

  [Test]
  public void DeleteFriend_ShouldSeeFriends_WhenUserHasDeletedAFriend()
  {
    _testUser.AddFriend(_testRequestUser);
    _controller.DeleteFriend(_testRequestUser.Id);

    var indexResult = _controller.Index() as ViewResult;
    var friendsList = indexResult.ViewData["Friends"] as List<User>;

    Assert.That(friendsList.Count, Is.EqualTo(0));
  }

  [Test]
  public void AcceptRequest_ShouldCreateBiDirectionalFriendship_AndRemoveRequest()
  {
    var request = new FriendRequest
    {
      UserId = _testRequestUser.Id,
      FriendId = _testUser.Id
    };
    _context.FriendRequests.Add(request);
    _context.SaveChanges();

    var result = _controller.AcceptRequest(request.Id, _testRequestUser.Id);
    var activeUserInDb = _context.Users
                        .Include(u => u.Friends)
                        .First(u => u.Id == _testUser.Id);
    var otherUserInDb = _context.Users
                        .Include(u => u.Friends)
                        .First(u => u.Id == _testRequestUser.Id);

    Assert.That(activeUserInDb.Friends.Any(f => f.Id == _testRequestUser.Id), 
                Is.True, "Active user should have Friend in list");
    Assert.That(otherUserInDb.Friends.Any(f => f.Id == _testUser.Id), 
                Is.True, "Friend should have Active user in list");

    var deletedRequest = _context.FriendRequests.Find(request.Id);
    Assert.That(deletedRequest, Is.Null, "FriendRequest record should be removed");

    Assert.That(result, Is.TypeOf<RedirectResult>());
    Assert.That(((RedirectResult)result).Url, Is.EqualTo("/friends"));
  }

  [Test]
  public void RejectRequest_ShouldDeleteRequest_WithoutAddingFriend()
    {
        var request = new FriendRequest
        {
            UserId = _testRequestUser.Id,
            FriendId = _testUser.Id
        };
        _context.FriendRequests.Add(request);
        _context.SaveChanges();

        var result = _controller.RejectRequest(request.Id);

        var deletedRequest = _context.FriendRequests.Find(request.Id);
        Assert.That(deletedRequest, Is.Null);

        var activeUserInDb = _context.Users
                            .Include(u => u.Friends)
                            .First(u => u.Id == _testRequestUser.Id);
        Assert.That(activeUserInDb.Friends.Count, Is.EqualTo(0), 
                    "No friendship should be created");
        
        Assert.That(((RedirectResult)result).Url, Is.EqualTo("/friends"));
    }
}