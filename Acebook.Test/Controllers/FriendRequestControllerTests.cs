
namespace Acebook.Test.Controllers;

public class FriendRequestControllerTests : NUnitTestBase
{
  private FriendRequestController _controller;
  private ILogger<FriendRequestController> _logger;
  private User _testUser;
  private User _testRequestUser;

  [SetUp]
  public void SetUp()
  {
    _logger = Mock.Of<ILogger<FriendRequestController>>();
    _controller = new FriendRequestController(_context, _logger);

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
    _context.SaveChanges();

    _controller.HttpContext.Session.SetInt32("user_id", _testUser.Id);

    _testRequestUser = new User
        {
            Name = "Request User",
            Email = "request@example.com",
            Password = "hashedPW1!",
            DateOfBirth = Dob(2000, 01, 01)
        };
    _context.Users.Add(_testRequestUser);
    _context.SaveChanges();

  }

  [TearDown]
  public void TearDown()
  {
    _controller?.Dispose();
  }

  [Test]
  public void ReceivedRequests_ShouldSeeRequests_WhenUserHasRequests()
  {
    var incomingRequest = new FriendRequest
    {
      UserId = _testRequestUser.Id,
      FriendId = _testUser.Id
    };
    _context.FriendRequests.Add(incomingRequest);
    _context.SaveChanges();

    var result = _controller.ReceivedRequests() as ViewResult;

    Assert.That(result, Is.Not.Null);

    var received = result.ViewData["ReceivedRequests"] as List<FriendRequest>;
    Assert.That(received.Count, Is.EqualTo(1));
  }

  [Test]
  public void ReceivedRequests_ShouldSeeNoRequests_WhenUserHasNoRequests()
  {
    var result = _controller.ReceivedRequests() as ViewResult;

    Assert.That(result, Is.Not.Null);

    var received = result.ViewData["ReceivedRequests"] as List<FriendRequest>;
    Assert.That(received.Count, Is.EqualTo(0));
  }

  [Test]
  public void CreateRequests_CanRequestFriend_WhenUserLoggedIn()
  {
    var result = _controller.CreateRequests(_testRequestUser.Id);

    Assert.That(((RedirectResult)result).Url, Is.EqualTo("/friends"));
  }

  [Test]
  public void AcceptRequest_CanAcceptFriend_WhenUserLoggedIn()
  {
    var incomingRequest = new FriendRequest
    {
      UserId = _testRequestUser.Id,
      FriendId = _testUser.Id
    };
    _context.FriendRequests.Add(incomingRequest);
    _context.SaveChanges();
    var result = _controller.AcceptRequest(_testRequestUser.Id, incomingRequest.Id);

    Assert.That(((RedirectResult)result).Url, Is.EqualTo("/friends"));
  }

  [Test]
  public void RejectRequest_CanRejectRequest_WhenUserLoggedIn()
  {
    var incomingRequest = new FriendRequest
    {
      UserId = _testRequestUser.Id,
      FriendId = _testUser.Id
    };
    _context.FriendRequests.Add(incomingRequest);
    _context.SaveChanges();
    var result = _controller.RejectRequest(incomingRequest.Id);

    Assert.That(((RedirectResult)result).Url, Is.EqualTo("/friends"));
  }

}