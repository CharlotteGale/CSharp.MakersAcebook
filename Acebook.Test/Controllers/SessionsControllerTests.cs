

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
    public void Create_ShouldVerifyHashedPassword_AndLoginUser()
    {
        var hasher = new PasswordHasher<User>();
        var user = new User
        {
            Name = "Test User",
            Email = "test@example.com",
            Password = hasher.HashPassword(null, "Password1!")
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
        var user = new User
        {
            Name = "Test User",
            Email = "test@example.com",
            Password = hasher.HashPassword(null, "correctPassword1!")
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

    /// DEVELOPMENT ENVIRON ONLY TESTS ///
    [Test]
    public void Create_ShouldLoginWithPlainTextPassword_InDevelopmentMode()
    {
        var user = new User
        {
            Name = "Test User",
            Email = "test@example.com",
            Password = "plainPassword1!"
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
        var user = new User
        {
            Name = "Test User",
            Email = "test@example.com",
            Password = "correctPlainPW1!"
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

public class MockHttpSession : ISession
{
    private readonly Dictionary<string, byte[]> _sessionStorage = new Dictionary<string, byte[]>();

    public bool IsAvailable => true;
    public string Id => Guid.NewGuid().ToString();
    public IEnumerable<string> Keys => _sessionStorage.Keys;

    public void Clear() => _sessionStorage.Clear();
    public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    public void Remove(string key) => _sessionStorage.Remove(key);
    public void Set(string key, byte[] value) => _sessionStorage[key] = value;
    public bool TryGetValue(string key, out byte[] value) => _sessionStorage.TryGetValue(key, out value);
}