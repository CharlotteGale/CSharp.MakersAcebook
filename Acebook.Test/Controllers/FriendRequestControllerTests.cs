
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

        _testUser = new User
        {
            Name = "Test User",
            Email = "test@example.com",
            Password = "Password1!"
        };
        _context.Users.Add(_testUser);
        _context.SaveChanges();

        _controller.HttpContext.Session.SetInt32("user_id", _testUser.Id);

        _testRequestUser = new User
        {
            Name = "Request User",
            Email = "request@example.com",
            Password = "Password1!"
        };
        _context.Users.Add(_testRequestUser);
        _context.SaveChanges();

        _controller.HttpContext.Session.SetInt32("user_id", _testRequestUser.Id);
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
            UserId = _testUser.Id,
            FriendId = _testRequestUser.Id
        };
        _context.FriendRequests.Add(incomingRequest);
        _context.SaveChanges();

        var result = _controller.ReceivedRequests() as ViewResult;

        Assert.That(result, Is.Not.Null);

        var received = result.ViewData["ReceivedRequests"] as List<FriendRequest>;
        Assert.That(received.Count, Is.EqualTo(1));
    }

    
}