
namespace Acebook.Test.E2E;

public class UserManagement : PlaywrightTestBase
{
  [Test]
  public async Task SigningUp_RedirectsToSignInForm()
  {
    await Page.GotoAsync(BaseUrl);
    await Page.GetByRole(AriaRole.Button, new() { Name = "Create new account" }).ClickAsync();

    await Page.GetByPlaceholder("Name").FillAsync("New User");
    await Page.GetByPlaceholder("Email").FillAsync("new@user.com");
    await Page.GetByPlaceholder("Password").FillAsync("password");
    await Page.GetByLabel("Date of Birth").FillAsync("2000-01-01");
    await Page.GetByRole(AriaRole.Button, new() { Name = "Create account" }).ClickAsync();

    await Expect(Page).ToHaveURLAsync(new Regex(".*/$"));

    var alert = Page.Locator(".alert-success");
    await Expect(alert).ToContainTextAsync("Your profile has been created successfully. Log in to continue.");
  }

  [Test]
  public async Task SigningUp_WithMissingFields_ShowsErrorMessages()
  {
    await Page.GotoAsync(BaseUrl);
    await Page.GetByRole(AriaRole.Button, new() { Name = "Create new account" }).ClickAsync();

    await Page.GetByPlaceholder("Name").FillAsync("New User");
    await Page.GetByPlaceholder("Email").FillAsync("new@user.com");
    await Page.GetByPlaceholder("Password").FillAsync("password");
    // omits DOB
    await Page.GetByRole(AriaRole.Button, new() { Name = "Create account" }).ClickAsync();
    var alert = Page.Locator(".alert-danger");
    await Expect(alert).ToContainTextAsync("Missing Required Fields!");  
  }

  [Test]
  public async Task SigningIn_WithCorrectCredentials_RedirectsToPosts()
  {
    await Page.GotoAsync(BaseUrl);
    await Page.GetByPlaceholder("Email").FillAsync("test1@email.com");
    await Page.GetByPlaceholder("Password").FillAsync("password");
    await Page.GetByRole(AriaRole.Button, new() { Name = "Log In" }).ClickAsync();

    await Expect(Page).ToHaveURLAsync(new Regex(".*/feed", RegexOptions.IgnoreCase));
  }

  [Test]
  [Category("fliesh")]
  public async Task SigningIn_WithIncorrectCredentials_ShowsErrorMessage()
  {
    await Page.GotoAsync(BaseUrl);
    await Page.GetByPlaceholder("Email").FillAsync("test1@email.com");
    await Page.GetByPlaceholder("Password").FillAsync("wrongpassword");
    await Page.GetByRole(AriaRole.Button, new() { Name = "Log In" }).ClickAsync();
    await Expect(Page).ToHaveURLAsync(new Regex(".*/$"));
    var alert = Page.Locator(".alert-danger");
    await Expect(alert).ToContainTextAsync("Your email or password details are incorrect."); 
  }
}
