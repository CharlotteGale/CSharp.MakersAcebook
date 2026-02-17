
namespace Acebook.Test.Controllers;

public class LikesControllerTests : NUnitTestBase
{
  private LikesController _controller;
  private User _testUser;
  private Post _testPost;

  [SetUp]
  public void SetUp()
  {
    _controller = new LikesController(_context);

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
      Password = "Password1!"
    };
    _context.Users.Add(_testUser);
    _context.SaveChanges();

    _testPost = new Post
    {
      Content = "Test post for likes",
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
  public void Toggle_ShouldCreateLike_WhenPostIsNotLiked()
  {
    var result = _controller.Toggle(_testPost.Id);

    var like = _context.Likes.FirstOrDefault(
        l => l.PostId == _testPost.Id && l.UserId == _testUser.Id
    );

    Assert.That(like, Is.Not.Null,
                "Like should be created");
    Assert.That(like.PostId, Is.EqualTo(_testPost.Id));
    Assert.That(like.UserId, Is.EqualTo(_testUser.Id));

    Assert.That(result, Is.TypeOf<RedirectResult>());
    Assert.That(((RedirectResult)result).Url, Is.EqualTo("/feed"));
  }

  [Test]
  public void Toggle_ShouldRemoveLike_WhenPostIsAlreadyLiked()
  {
    var existingLike = new Like
    {
      PostId = _testPost.Id,
      UserId = _testUser.Id
    };
    _context.Likes.Add(existingLike);
    _context.SaveChanges();

    var result = _controller.Toggle(_testPost.Id);

    _context.ChangeTracker.Clear();

    var like = _context.Likes.FirstOrDefault(
        l => l.PostId == _testPost.Id && l.UserId == _testUser.Id
    );

    Assert.That(like, Is.Null,
                "Like should be removed");

    Assert.That(result, Is.TypeOf<RedirectResult>());
    Assert.That(((RedirectResult)result).Url, Is.EqualTo("/feed"));
  }

  [Test]
  public void Toggle_ShouldRedirectToHome_WhenUserNotLoggedIn()
  {
    _controller.HttpContext.Session.Remove("user_id");

    var result = _controller.Toggle(_testPost.Id);

    var like = _context.Likes.FirstOrDefault(
        l => l.PostId == _testPost.Id && l.UserId == _testUser.Id
    );

    Assert.That(like, Is.Null,
                "No like should be created when not logged in");
    Assert.That(result, Is.TypeOf<RedirectResult>());
    Assert.That(((RedirectResult)result).Url, Is.EqualTo("/"),
                "Should redirect to Home/Sign In");
  }
}