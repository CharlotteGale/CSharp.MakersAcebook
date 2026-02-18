using Microsoft.Extensions.Options;

namespace Acebook.Test.Controllers;

public class SessionsControllerTests : NUnitTestBase
{
    private SessionsController _controller;
    private ILogger<SessionsController> _logger;

    [SetUp]
    public void SetUp()
    {
        _logger = Mock.Of<ILogger<SessionsController>>();

        _controller = new SessionsController(_context, _logger);

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

    }

    [TearDown]
    public void TearDown()
    {
        _controller?.Dispose();
    }

    [Test]
    public void New_ShouldReturnView()
    {
        var result = _controller.New();

        Assert.That(result, Is.TypeOf<ViewResult>());
    }

    [Test]
    public void Create_ShouldVerifyHashedPassword_AndLoginUser()
    {
        var hasher = new PasswordHasher<User>();
        DateTime Dob(int year, int month, int day) => DateTime.SpecifyKind(new DateTime(year, month, day), DateTimeKind.Utc);
        var user = new User
        {
            Name = "Test User",
            Email = "test@example.com",
            Password = hasher.HashPassword(null, "Password1!"),
            DateOfBirth = Dob(2000, 01, 01)
        };

        _context.Users.Add(user);
        _context.SaveChanges();

        var result = _controller.Create("test@example.com", "Password1!");

        Assert.That(result, Is.TypeOf<RedirectResult>());
        Assert.That(((RedirectResult)result).Url, Is.EqualTo("/Feed"));

        var sessionUserId = _controller.HttpContext.Session.GetInt32("user_id");
        Assert.That(sessionUserId, Is.EqualTo(user.Id));
    }

    [Test]
    public void Create_ShouldRehashPassword_AndLoginUser_WhenRehashNeeded()
    {
        var hasher = new PasswordHasher<User>();

        // set the hash to an older, weaker version
        var hasherOptions = new PasswordHasherOptions { CompatibilityMode = PasswordHasherCompatibilityMode.IdentityV2 };
        var v2Hasher = new PasswordHasher<User>(Options.Create(hasherOptions));

        DateTime Dob(int year, int month, int day) => DateTime.SpecifyKind(new DateTime(year, month, day), DateTimeKind.Utc);
        var user = new User
        {
            Name = "Test User",
            Email = "test@example.com",
            Password = v2Hasher.HashPassword(null, "Password1!"),
            DateOfBirth = Dob(2000, 01, 01)
        };
        _context.Users.Add(user);
        _context.SaveChanges();

        var oldPasswordHash = user.Password;
        var result = _controller.Create("test@example.com", "Password1!");

        Assert.That(result, Is.TypeOf<RedirectResult>());
        Assert.That(((RedirectResult)result).Url, Is.EqualTo("/Feed"));

        var sessionUserId = _controller.HttpContext.Session.GetInt32("user_id");
        Assert.That(sessionUserId, Is.EqualTo(user.Id));

        var updatedUser = _context.Users.Find(user.Id);
        Assert.That(updatedUser.Password, Is.Not.EqualTo(oldPasswordHash),
                    "Password should have been rehashed with v3 stronger algo");
    }

    [Test]
    public void Create_ShouldShowError_WhenUserDoesNotExist()
    {
        var result = _controller.Create("fake_user@example.com", "Password1!");

        Assert.That(result, Is.TypeOf<RedirectResult>());
        Assert.That(((RedirectResult)result).Url, Is.EqualTo("/"), 
                    "Should redirect to Home/Sign In");

        Assert.That(_controller.TempData["ErrorMessage"],
                    Is.EqualTo("Your email or password details are incorrect."),
                    "Should set error message in TempData");
        
        var sessionUserId = _controller.HttpContext.Session.GetInt32("user_id");
        Assert.That(sessionUserId, Is.Null,
                    "Should not set session when login fails");
    }

        [Test]
    public void Create_ShouldShowError_WhenPasswordIsIncorrect()
    {
        var hasher = new PasswordHasher<User>();

        DateTime Dob(int year, int month, int day) => DateTime.SpecifyKind(new DateTime(year, month, day), DateTimeKind.Utc);
        var user = new User
        {
            Name = "Test User",
            Email = "test@example.com",
            Password = hasher.HashPassword(null, "correctPassword1!"),
            DateOfBirth = Dob(2000, 01, 01)
        };

        _context.Users.Add(user);
        _context.SaveChanges();

        var result = _controller.Create("test@example.com", "wrongPassword1!");

        Assert.That(result, Is.TypeOf<RedirectResult>());
        Assert.That(((RedirectResult)result).Url, Is.EqualTo("/"),
                    "Should redirect to Home/Sign In");

        Assert.That(_controller.TempData["ErrorMessage"],
                    Is.EqualTo("Your email or password details are incorrect."),
                    "Should set error message in TempData");

        var sessionUserId = _controller.HttpContext.Session.GetInt32("user_id");
        Assert.That(sessionUserId, Is.Null,
                    "Should not set session when login fails");
    }

        [Test]
    public void Logout_ShouldClearSession_AndRedirectToHome()
    {
        _controller.HttpContext.Session.SetInt32("user_id", 1);

        var result = _controller.Logout();

        var sessionUserId = _controller.HttpContext.Session.GetInt32("user_id");
        Assert.That(sessionUserId, Is.Null,
                    "Session should be destroyed after logout");

        Assert.That(result, Is.TypeOf<RedirectToActionResult>());

        var redirect = (RedirectToActionResult)result;
        Assert.That(redirect.ActionName, Is.EqualTo("New"));
        Assert.That(redirect.ControllerName, Is.EqualTo("Sessions"));
    }

    /// DEVELOPMENT ENVIRON ONLY TESTS ///
    [Test]
    public void Create_ShouldLoginWithPlainTextPassword_InDevelopmentMode()
    {
        DateTime Dob(int year, int month, int day) => DateTime.SpecifyKind(new DateTime(year, month, day), DateTimeKind.Utc);
        var user = new User
        {
            Name = "Test User",
            Email = "test@example.com",
            Password = "plainPassword1!",
            DateOfBirth = Dob(2000, 01, 01)
        };
        _context.Users.Add(user);
        _context.SaveChanges();

        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");

        var result = _controller.Create("test@example.com", "plainPassword1!");

        Assert.That(result, Is.TypeOf<RedirectResult>());
        Assert.That(((RedirectResult)result).Url, Is.EqualTo("/Feed"),
                    "Should login successfully with seed data and plain text password in development");
        
        var sessionUserId = _controller.HttpContext.Session.GetInt32("user_id");
        Assert.That(sessionUserId, Is.EqualTo(user.Id));

        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", null);
    }

    [Test]
    public void Create_ShouldFailWithWrongPlainTextPassword_InDevelopmentMode()
    {
        DateTime Dob(int year, int month, int day) => DateTime.SpecifyKind(new DateTime(year, month, day), DateTimeKind.Utc);
        var user = new User
        {
            Name = "Test User",
            Email = "test@example.com",
            Password = "correctPlainPW1!",
            DateOfBirth = Dob(2000, 01, 01)
        };
        _context.Users.Add(user);
        _context.SaveChanges();

        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");

        var result = _controller.Create("test@example.com", "wrongPlainPW1!");

        Assert.That(result, Is.TypeOf<RedirectResult>());
        Assert.That(((RedirectResult)result).Url, Is.EqualTo("/"),
                    "Should fail with wrong plain text password");
        Assert.That(_controller.TempData["ErrorMessage"], 
        Is.EqualTo("Your email or password details are incorrect."));

        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", null);
    }
}
