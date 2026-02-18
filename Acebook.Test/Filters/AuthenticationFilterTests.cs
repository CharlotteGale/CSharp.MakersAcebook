
namespace Acebook.Test.Filters;

public class AuthenticationFilterTests : NUnitTestBase
{
    private AuthenticationFilter _filter;
    private ActionExecutingContext _context;

    [SetUp]
    public void SetUp()
    {
        _filter = new AuthenticationFilter();

        var httpContext = new DefaultHttpContext();
        httpContext.Session = new MockHttpSession();

        var actionContext = new ActionContext(
            httpContext,                            // request/session info mocked
            new RouteData(),                        // empty map of the URL
            new ActionDescriptor()                  // blank description of method being called
        );

        _context = new ActionExecutingContext(
            actionContext,                          // the context created ^^
            new List<IFilterMetadata>(),            // scoping that allows for additional filters (eventually)
            new Dictionary<string, object>(),       // handles any arguments being passed to the method
            Mock.Of<Controller>()                   // a fake Controller
        );
    }

    [Test]
    public void OnActionExecuting_ShouldRedirect_WhenUserIsNotLoggedIn()
    {
        _context.HttpContext.Session.Remove("user_id");
        
        _filter.OnActionExecuting(_context);
        Assert.That(_context.Result, Is.TypeOf<RedirectResult>());

        var redirect = _context.Result as RedirectResult;
        Assert.That(redirect.Url, Is.EqualTo("/"));
    }

    [Test]
    public void OnActionExecuting_ShouldDoNothing_WhenUserIsLoggedIn()
    {
        _context.HttpContext.Session.SetInt32("user_id", 1);

        _filter.OnActionExecuting(_context);

        Assert.That(_context.Result, Is.Null,
                    "The filter should not set a result if the user is logged in");
    }
}