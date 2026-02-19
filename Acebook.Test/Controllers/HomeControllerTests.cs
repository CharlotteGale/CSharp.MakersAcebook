
namespace Acebook.Test.Controllers;

public class HomeControllerTests : NUnitTestBase
{
    private HomeController _controller;
    private ILogger<HomeController> _logger;

    [SetUp]
    public void SetUp()
    {
        _logger = Mock.Of<ILogger<HomeController>>();
        _controller = new HomeController(_logger);

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
    public void Index_ShouldReturnView()
    {
        var result = _controller.Index();

        Assert.That(result, Is.TypeOf<ViewResult>());
    }

    [Test]
    public void Privacy_ShouldReturnView()
    {
        var result = _controller.Privacy();

        Assert.That(result, Is.TypeOf<ViewResult>());
    }
}