namespace Acebook.Test;
using System.Text.RegularExpressions;

public class LandingPageTests : PlaywrightTestBase
{
  [Test]
  public async Task LandingPage_ShowsWelcomeMessage()
  {
    await Page.GotoAsync("http://127.0.0.1:5287");

    await Expect(Page).ToHaveTitleAsync("- Acebook");
  }


  [Test]
  public async Task LandingPage_Show_Login_Form()
  {
    await Page.GotoAsync("http://127.0.0.1:5287");

    var form = Page.Locator("form.login-card");
    await Expect(form).ToBeVisibleAsync();
  }


[Test]
public async Task User_Can_Navigate_To_SignUp_Page()
{

    await Page.GotoAsync("http://127.0.0.1:5287");

    await Page.ClickAsync(".new-acc-btn");

    await Expect(Page).ToHaveURLAsync(new Regex(".*/signup", RegexOptions.IgnoreCase));

    await Expect(Page.Locator("#Email")).ToBeVisibleAsync();
    await Expect(Page.Locator("#Password")).ToBeVisibleAsync();
    await Expect(Page.Locator("#Name")).ToBeVisibleAsync();
}

}
