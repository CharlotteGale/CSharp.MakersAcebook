
namespace Acebook.Test.Controllers;

public class CommentsControllerTests : NUnitTestBase
{
    private CommentsController _controller;
    private ILogger<CommentsController> _logger;
    private User _testUser;
    private Post _testPost;

    [SetUp]
    public void SetUp()
    {
        _logger = Mock.Of<ILogger<CommentsController>>();
        _controller = new CommentsController(_context, _logger);

        var httpContext = new DefaultHttpContext();
        httpContext.Session = new MockHttpSession();

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        _testUser = new User
        {
            Name = "Test User",
            Email = "test@example.com",
            Password = "hashedPW1!"
        };
        _context.Users.Add(_testUser);
        _context.SaveChanges();

        _testPost = new Post
        {
            Content = "This is a test post",
            UserId = _testUser.Id
        };
        _context.Posts.Add(_testPost);
        _context.SaveChanges();

        _controller.HttpContext.Session.SetInt32("user_id", _testUser.Id);
    }

    [TearDown]
    public void TearDown()
    {
        _controller?.Dispose();
    }

    [Test]
    public void Create_ShouldRedirectToHome_WhenNotLoggedIn()
    {
        _controller.HttpContext.Session.Remove("user_id");

        var result = _controller.Create(_testPost.Id, "Nice post!");

        Assert.That(result, Is.TypeOf<RedirectResult>());
        Assert.That(((RedirectResult)result).Url, Is.EqualTo("/"),
                    "Should redirect to Home/Sign In");
    }

    [Test]
    public void Create_ShouldSaveComment_WhenValid()
    {
        var testComment = "This is a test comment";

        var result = _controller.Create(_testPost.Id, testComment);

        var savedComment = _context.Comments.FirstOrDefault(c => c.Content == testComment);
        Assert.That(savedComment, Is.Not.Null);
        Assert.That(savedComment.PostId, Is.EqualTo(_testPost.Id));
        Assert.That(savedComment.UserId, Is.EqualTo(_testUser.Id));
        Assert.That(((RedirectResult)result).Url, Is.EqualTo("/feed"),
                    "On successful commenting, user should see updated feed page");
    }

    [Test]
    public void Create_ShouldNotSaveComment_WhenContentIsEmpty()
    {
        var result = _controller.Create(_testPost.Id, "");

        var commentCount = _context.Comments.Count();

        Assert.That(commentCount, Is.EqualTo(0));
        Assert.That(((RedirectResult)result).Url, Is.EqualTo("/feed"),
                    "Should not allow empty comments - needs validation checks");
    }
}