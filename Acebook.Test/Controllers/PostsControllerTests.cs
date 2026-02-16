
namespace Acebook.Test.Controllers;

public class PostsControllerTests : NUnitTestBase
{
    private PostsController _controller;
    private ILogger<PostsController> _logger;
    private User _testUser;

    [SetUp]
    public void SetUp()
    {
        _logger = Mock.Of<ILogger<PostsController>>();
        _controller = new PostsController(_context, _logger);

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
    }

    [TearDown]
    public void TearDown()
    {
        _controller?.Dispose();
    }

    [Test]
    public void Create_ShouldSavePost_WhenPostIsValid()
    {
        var post = new Post
        {
            Content = "This is a valid post"
        };

        var result = _controller.Create(post);

        var savedPost = _context.Posts.FirstOrDefault(post => post.Content == "This is a valid post");
        Assert.That(savedPost, Is.Not.Null, 
                    "Posts should be saved to database");
        Assert.That(savedPost.UserId, Is.EqualTo(_testUser.Id),
                    "Post should be assigned to the logged in user");
        Assert.That(result.Url, Is.EqualTo("/Feed"));
        Assert.That(_controller.TempData.ContainsKey("InvalidPost"), Is.False,
                    "Should not have any validation errors");  
    }

    [Test]
    public void Create_ShouldNotSavePost_WhenContentIsInvalid()
    {
        var post = new Post
        {
            Content = ""
        };

        _controller.ModelState.AddModelError("Message", "Content field is required");

        var result = _controller.Create(post);

        var savedPost = _context.Posts.FirstOrDefault(post => post.Content == "");
        Assert.That(savedPost, Is.Null,
                    "Post should NOT save when invalid");
        Assert.That(result.Url, Is.EqualTo("/Feed"),
                    "Should redirect to /Feed on validation error");
        Assert.That(_controller.TempData.ContainsKey("InvalidPost"), Is.True,
                    "Should store invalid post in TempData");

        var invalidPostJson = _controller.TempData["InvalidPost"] as string;
        Assert.That(invalidPostJson, Is.Not.Null);

        var deserializePost = JsonSerializer.Deserialize<Post>(invalidPostJson);
        Assert.That(deserializePost.Content, Is.EqualTo(""));
    }
}