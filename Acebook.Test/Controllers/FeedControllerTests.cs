
namespace Acebook.Test.Controllers;

public class FeedControllerTests : NUnitTestBase
{
    private FeedController _controller;
    private ILogger<FeedController> _logger;
    private User _testUser;

    [SetUp]
    public void SetUp()
    {
        _logger = Mock.Of<ILogger<FeedController>>();
        _controller = new FeedController(_context, _logger);

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
    }

    [TearDown]
    public void TearDown()
    {
        _controller?.Dispose();
    }

    [Test]
    public void Index_ShouldReturnViewWithPosts_WhenUserIsLoggedIn()
    {
        var post1 = new Post
        {
            Content = "First post",
            UserId = _testUser.Id
        };
        var post2 = new Post
        {
            Content = "Second post",
            UserId = _testUser.Id
        };

        _context.Posts.AddRange(post1, post2);
        _context.SaveChanges();

        var result = _controller.Index();

        Assert.That(result, Is.TypeOf<ViewResult>());
        var viewResult = (ViewResult)result;

        var model = viewResult.Model as List<Post>;
        Assert.That(model, Is.Not.Null);
        Assert.That(model.Count, Is.EqualTo(2),
                    "Should return all posts");
        Assert.That(model[0].User, Is.Not.Null, 
                    "Should include User data");
    }

    [Test]
    public void Index_ShouldRedirectToHome_IfUserNotLoggedIn()
    {
        _controller.HttpContext.Session.Remove("user_id");

        var result = _controller.Index();

        Assert.That(result, Is.TypeOf<RedirectResult>());
        Assert.That(((RedirectResult)result).Url, Is.EqualTo("/"),
                    "Should redirect to Home/Sign In");
    }

    [Test]
    public void Index_ShouldDeserializeInvalidPost_WhenTempDataContainsIt()
    {
        var invalidPost = new Post
        {
            Content = ""
        };

        var errors = new Dictionary<string, string[]>
        {
            { "Message", new[] { "Contents is required" } }
        };

        _controller.TempData["InvalidPost"] = JsonSerializer.Serialize(invalidPost);
        _controller.TempData["ModelStateErrors"] = JsonSerializer.Serialize(errors);

        var result = _controller.Index();

        Assert.That(result, Is.TypeOf<ViewResult>());
        var viewResult = (ViewResult)result;

        Assert.That(viewResult.ViewData["InvalidPost"], Is.Not.Null,
                    "ViewBag should contain the invalid post");
        
        var deserializePost = viewResult.ViewData["InvalidPost"] as Post;
        Assert.That(deserializePost, Is.Not.Null);
        Assert.That(deserializePost.Content, Is.EqualTo(""),
                    "Should deserialize the invalid post correctly");

        Assert.That(_controller.ModelState.IsValid, Is.False,
                    "ModelState should be invalid");
        Assert.That(_controller.ModelState.ContainsKey("Message"), Is.True);
        Assert.That(_controller.ModelState["Message"].Errors.Count, Is.EqualTo(1));
        Assert.That(_controller.ModelState["Message"].Errors[0].ErrorMessage,
                    Is.EqualTo("Contents is required"));
    }
}