namespace Acebook.Test;

public class LandingPageTests : PageTest
{
  [Test]
  public async Task LandingPage_ShowsWelcomeMessage()
  {
    await Page.GotoAsync("http://127.0.0.1:5287");

    await Expect(Page).ToHaveTitleAsync("- acebook");
  }
}
