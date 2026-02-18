namespace Acebook.Tests.E2E;

public class UserManagement : PlaywrightTestBase
{
[Test]
public async Task SigningUpRedirectsToSignInForm()
{
  await Page.GotoAsync("http://127.0.0.1:5287/");
  await Page.GetByRole(AriaRole.Button, new() { Name = "Create new account" }).ClickAsync();

  await Page.GetByPlaceholder("Name").FillAsync("New User");
  await Page.GetByPlaceholder("Email").FillAsync("new@user.com");
  await Page.GetByPlaceholder("Password").FillAsync("Password1!");
  await Page.GetByLabel("Date of Birth").FillAsync("2000-01-01");

  await Page.Locator("#signup-submit").ClickAsync();

  await Expect(Page).ToHaveURLAsync("http://127.0.0.1:5287/");

  var alert = Page.Locator(".alert-success");
  await Expect(alert).ToContainTextAsync("Your profile has been created successfully. Log in to continue.");
}

[Test]
public async Task SigningUpWithMissingFieldsShowsErrorMessages()
{
  await Page.GotoAsync("http://127.0.0.1:5287/");
  await Page.GetByRole(AriaRole.Button, new() { Name = "Create new account" }).ClickAsync();

  // Missing Name + DOB (and use a valid strong password so we only test missing fields)
  await Page.GetByPlaceholder("Name").FillAsync("");
  await Page.GetByPlaceholder("Email").FillAsync("new@user.com");
  await Page.GetByPlaceholder("Password").FillAsync("Password1!");

  await Page.Locator("#signup-submit").ClickAsync();

  await Expect(Page).ToHaveURLAsync("http://127.0.0.1:5287/users");

  await Expect(Page.Locator("text=The Name field is required.")).ToBeVisibleAsync();
  await Expect(Page.Locator("text=The Date of Birth field is required.")).ToBeVisibleAsync();
}

  [Test]
  public async Task SigningInWithCorrectCredentialsRedirectsToPosts()
  {
    await Page.GotoAsync("http://127.0.0.1:5287/");
    await Page.GetByPlaceholder("Email").FillAsync("test1@email.com");
    await Page.GetByPlaceholder("Password").FillAsync("password");
    await Page.GetByRole(AriaRole.Button, new() { Name = "Log In" }).ClickAsync();

    await Expect(Page).ToHaveURLAsync("http://127.0.0.1:5287/Feed");
  }

  [Test]
  [Category("fliesh")]
  public async Task SigningInWithIncorrectCredentialsShowsErrorMessage()
  {
    await Page.GotoAsync("http://127.0.0.1:5287/");
    await Page.GetByPlaceholder("Email").FillAsync("admin@email.com");
    await Page.GetByPlaceholder("Password").FillAsync("wrongpassword");
    await Page.GetByRole(AriaRole.Button, new() { Name = "Log In" }).ClickAsync();
    await Expect(Page).ToHaveURLAsync("http://127.0.0.1:5287/");
    var alert = Page.Locator(".alert-danger");
    await Expect(alert).ToContainTextAsync("Your email or password details are incorrect."); 
  }

  [Test]
public async Task WeakPasswordIsRejectedWithCorrectValidationMessage()
{
  await Page.GotoAsync("http://127.0.0.1:5287/");
  await Page.GetByRole(AriaRole.Button, new() { Name = "Create new account" }).ClickAsync();

  await Page.GetByPlaceholder("Name").FillAsync("Weak User");
  await Page.GetByPlaceholder("Email").FillAsync("weak@user.com");
  await Page.GetByPlaceholder("Password").FillAsync("password"); 
  await Page.GetByLabel("Date of Birth").FillAsync("2000-01-01");

  var submit = Page.Locator("#signup-submit");

  await Expect(submit).ToBeDisabledAsync();
  await Expect(Page.Locator("#password-help"))
    .ToHaveTextAsync("Password must be at least 8 characters and include uppercase, lowercase, a number, and a symbol.");
}

[Test]
public async Task ValidPasswordIsAcceptedAndAllowsFormToSubmit()
{
  await Page.GotoAsync("http://127.0.0.1:5287/");
  await Page.GetByRole(AriaRole.Button, new() { Name = "Create new account" }).ClickAsync();

  await Page.GetByPlaceholder("Name").FillAsync("Strong User");
  await Page.GetByPlaceholder("Email").FillAsync("strong@user.com");
  await Page.GetByPlaceholder("Password").FillAsync("Password1!");
  await Page.GetByLabel("Date of Birth").FillAsync("2000-01-01");

  var submit = Page.Locator("#signup-submit");
  await Expect(submit).ToBeEnabledAsync();

  await submit.ClickAsync();

  await Expect(Page).ToHaveURLAsync("http://127.0.0.1:5287/");
  await Expect(Page.Locator(".alert-success"))
    .ToContainTextAsync("Your profile has been created successfully. Log in to continue.");
}
[Test]
public async Task DuplicateEmailIsRejectedWithInlineError()
{
  await Page.GotoAsync("http://127.0.0.1:5287/");
  await Page.GetByRole(AriaRole.Button, new() { Name = "Create new account" }).ClickAsync();

  // First signup
  await Page.GetByPlaceholder("Name").FillAsync("User One");
  await Page.GetByPlaceholder("Email").FillAsync("dupe@user.com");
  await Page.GetByPlaceholder("Password").FillAsync("Password1!");
  await Page.GetByLabel("Date of Birth").FillAsync("2000-01-01");
  await Page.Locator("#signup-submit").ClickAsync();

  await Expect(Page).ToHaveURLAsync("http://127.0.0.1:5287/");

  // Go back to signup again
  await Page.GetByRole(AriaRole.Button, new() { Name = "Create new account" }).ClickAsync();

  // Second signup with same email
  await Page.GetByPlaceholder("Name").FillAsync("User Two");
  await Page.GetByPlaceholder("Email").FillAsync("dupe@user.com");
  await Page.GetByPlaceholder("Password").FillAsync("Password1!");
  await Page.GetByLabel("Date of Birth").FillAsync("2000-01-01");
  await Page.Locator("#signup-submit").ClickAsync();

  // Should stay on /users (re-render form) and show inline email error
  await Expect(Page).ToHaveURLAsync("http://127.0.0.1:5287/users");

  await Expect(Page.Locator("span[data-valmsg-for='Email']"))
    .ToContainTextAsync("An account with this email already exists.");
}

[Test]
public async Task InvalidEmailFormatIsRejectedWithInlineMessage()
{
  await Page.GotoAsync("http://127.0.0.1:5287/");
  await Page.GetByRole(AriaRole.Button, new() { Name = "Create new account" }).ClickAsync();

  await Page.GetByPlaceholder("Name").FillAsync("Test User");
  await Page.GetByPlaceholder("Email").FillAsync("notanemail");
  await Page.GetByPlaceholder("Password").FillAsync("Password1!");
  await Page.GetByLabel("Date of Birth").FillAsync("2000-01-01");

  await Page.Locator("#signup-submit").ClickAsync();

  await Expect(Page).ToHaveURLAsync("http://127.0.0.1:5287/users");

  var emailError = Page.Locator("span[data-valmsg-for='Email']");
  await Expect(emailError).ToContainTextAsync("Enter a valid email address.");
}


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