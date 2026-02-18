
namespace Acebook.Test.E2E;

public class LandingPageTests : PlaywrightTestBase
{
    [Test]
    public async Task Index_ShowsWelcomeMessage()
    {
        await Page.GotoAsync(BaseUrl);

        await Expect(Page).ToHaveTitleAsync("- Acebook");
    }


    [Test]
    public async Task LandingPage_ShowsLoginForm()
    {
        await Page.GotoAsync(BaseUrl);

        var form = Page.Locator("form.login-card");
        await Expect(form).ToBeVisibleAsync();
    }


    [Test]
    public async Task LandingPage_UserCanNavigateToSignUpPage()
    {

        await Page.GotoAsync(BaseUrl);

        await Page.ClickAsync(".new-acc-btn");

        await Expect(Page).ToHaveURLAsync(new Regex(".*/signup", RegexOptions.IgnoreCase));

        await Expect(Page.Locator("#email")).ToBeVisibleAsync();
        await Expect(Page.Locator("#password")).ToBeVisibleAsync();
        await Expect(Page.Locator("#name")).ToBeVisibleAsync();
    }

    [Test]
    public async Task Footer_PrivacyLink_NavigatesToPrivacyPage()
    {
        await Page.GotoAsync(BaseUrl);

        var privacyLink = Page.GetByRole(AriaRole.Link, new() { Name = "Privacy" });
        await privacyLink.ClickAsync();

        await Expect(Page).ToHaveURLAsync(new Regex(".*/privacy", RegexOptions.IgnoreCase));

        await Expect(Page.GetByRole(AriaRole.Heading)).ToContainTextAsync("Privacy Policy");
    }

}
