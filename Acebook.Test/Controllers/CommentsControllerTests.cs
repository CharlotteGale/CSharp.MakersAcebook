
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

        Assert.That(commentCount, Is.EqualTo(28));
        Assert.That(((RedirectResult)result).Url, Is.EqualTo("/feed"),
                    "Should not allow empty comments - needs validation checks");
    }

    [Test]
    public void Edit_ShouldUpdateContent_WhenUserIsOwner()
    {
        var comment = new Comment
        {
            PostId = _testPost.Id,
            UserId = _testUser.Id,
            Content = "Old comment"
        };
        _context.Comments.Add(comment);
        _context.SaveChanges();

        var result = _controller.Edit(comment.Id, "Updated comment");

        var updatedComment = _context.Comments.Find(comment.Id);
        Assert.That(updatedComment.Content, Is.EqualTo("Updated comment"));
        Assert.That(((RedirectResult)result).Url, Is.EqualTo("/feed"));
    }

    [Test]
    public void Edit_ShouldNotUpdate_WhenUserIsNotOwner()
    {
        DateTime Dob(int year, int month, int day) => DateTime.SpecifyKind(new DateTime(year, month, day), DateTimeKind.Utc);
        var otherUser = new User
        {
            Name = "Other user",
            Email = "other@test.com",
            Password = "HashPW1!",
            DateOfBirth = Dob(2000, 01, 01)

        };
        _context.Users.Add(otherUser);
        _context.SaveChanges();

        var comment = new Comment
        {
            PostId = _testPost.Id,
            UserId = otherUser.Id,
            Content = "Don't touch this"
        };
        _context.Comments.Add(comment);
        _context.SaveChanges();

        var result = _controller.Edit(comment.Id, "Hacked!");

        var refreshedComment = _context.Comments.Find(comment.Id);
        Assert.That(refreshedComment.Content, Is.EqualTo("Don't touch this"));
        Assert.That(((RedirectResult)result).Url, Is.EqualTo("/feed"),
                    "Should not update comment - might need validation/error messages?");
    }

    [Test]
    public void Delete_ShouldRemoveComment_WhenUserIsOwner()
    {
        var comment = new Comment
        {
            PostId = _testPost.Id,
            UserId = _testUser.Id,
            Content = "Delete this"
        };
        _context.Comments.Add(comment);
        _context.SaveChanges();

        var result = _controller.Delete(comment.Id);

        var deletedComment = _context.Comments.Find(comment.Id);
        Assert.That(deletedComment, Is.Null);
        Assert.That(((RedirectResult)result).Url, Is.EqualTo("/feed"));
    }

    [Test]
    public void Delete_ShouldRedirectToFeed_IfCommentDoesNotExist()
    {
        var result = _controller.Delete(999);

        Assert.That(((RedirectResult)result).Url, Is.EqualTo("/feed"),
                    "Should redirect to feed if comment doesn't exist - graceful UI/UX handling req?");
    }
}