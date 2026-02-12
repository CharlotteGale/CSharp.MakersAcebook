using Acebook.Test;

namespace Acebook.Tests;

public class UserManagement : PlaywrightTestBase
{
  [Test]
  public async Task SigningUpRedirectsToSignInForm()
  {
    await Page.GotoAsync("http://127.0.0.1:5287/");
    await Page.GetByRole(AriaRole.Button, new() { Name = "Create new account" }).ClickAsync();

    await Page.GetByPlaceholder("Name").FillAsync("New User");
    await Page.GetByPlaceholder("Email").FillAsync("new@user.com");
    await Page.GetByPlaceholder("Password").FillAsync("password");
    await Page.GetByRole(AriaRole.Button, new() { Name = "Create account" }).ClickAsync();

    await Expect(Page).ToHaveURLAsync("http://127.0.0.1:5287/");

    var alert = Page.Locator(".alert-success");
    await Expect(alert).ToContainTextAsync("Your profile has been created successfully. Log in to continue.");
  }

  [Test]
  public async Task SigningUpWithMissingFieldsShowsErrorMessages()
  {
    await Page.GotoAsync("http://127.0.0.1:5287/");
    await Page.GetByRole(AriaRole.Button, new() { Name = "Create new account" }).ClickAsync();

    await Page.GetByPlaceholder("Name").FillAsync("");
    await Page.GetByPlaceholder("Email").FillAsync("new@user.com");
    await Page.GetByPlaceholder("Password").FillAsync("password");
    await Page.GetByRole(AriaRole.Button, new() { Name = "Create account" }).ClickAsync();
  }

  [Test]
  public async Task SigningInWithCorrectCredentialsRedirectsToPosts()
  {
    await Page.GotoAsync("http://127.0.0.1:5287/");
    await Page.GetByPlaceholder("Email").FillAsync("admin@email.com");
    await Page.GetByPlaceholder("Password").FillAsync("password");
    await Page.GetByRole(AriaRole.Button, new() { Name = "Log In" }).ClickAsync();

    await Expect(Page).ToHaveURLAsync("http://127.0.0.1:5287/Feed");
  }


  
}
