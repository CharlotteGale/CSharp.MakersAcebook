
namespace Acebook.Test.Controllers;

public class UsersControllerTests : NUnitTestBase
{
    private UsersController _controller;
    private ILogger<UsersController> _logger;

    [SetUp]
    public void SetUp()
    {
        _logger = Mock.Of<ILogger<UsersController>>();

        _controller = new UsersController(_context, _logger);

        _controller.TempData = new Microsoft.AspNetCore.Mvc.ViewFeatures.TempDataDictionary(
            new DefaultHttpContext(),
            Mock.Of<Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataProvider>()
        );
    }

    [TearDown]
    public void TearDown()
    {
        _controller?.Dispose();
    }

    [Test]
    public void Create_ShouldHashPassword_WhenModelStateIsValid()
    {
        var user = new User
        {
            Name = "Test User",
            Email = "test@example.com",
            Password = "plainPassword1!"
        };

        var result = _controller.Create(user);

        var savedUser = _context.Users.FirstOrDefault(user => user.Email == "test@example.com");
        Assert.That(savedUser, Is.Not.Null);
        Assert.That(savedUser.Password, Is.Not.EqualTo("plainPassword1!"),
                        "Password should be hashed");
        Assert.That(savedUser.Password.Length, Is.GreaterThan(20),
                        "Hashed password should be longer than 20 characters");

        Assert.That(result, Is.TypeOf<RedirectResult>());
        Assert.That(((RedirectResult)result).Url, Is.EqualTo("/"));
    }

    [Test]
    public void Create_ShouldReturnView_WhenEmailIsInvalid()
    {
        var user = new User
        {
            Name = "Test User",
            Email = "not-an-email",
            Password = "validPassword1!"
        };

        _controller.ModelState.AddModelError("Email", "Invalid Email");

        var result = _controller.Create(user);

        var savedUser = _context.Users.FirstOrDefault(user => user.Email == "not-an-email");

        Assert.That(savedUser, Is.Null, "User should not be saved");
        Assert.That(result, Is.TypeOf<ViewResult>());
    }

    [Test]
    public void Create_ShouldReturnView_WhenNameIsMissing()
    {
        var user = new User
        {
            Name = "",
            Email = "test@example.com",
            Password = "validPassword1!"
        };

        _controller.ModelState.AddModelError("Name", "Name is required");

        var result = _controller.Create(user);

        var savedUser = _context.Users.FirstOrDefault(user => user.Email == "test@example");

        Assert.That(savedUser, Is.Null, "User should not be saved");
        Assert.That(result, Is.TypeOf<ViewResult>());
    }
    
}