namespace Acebook.Test;
using System.Text.RegularExpressions;

public class SignIn_SignUp_Tests : PageTest
{
    [Test]
public async Task User_Can_Log_In()
{
    await Page.GotoAsync("http://127.0.0.1:5287");

    await Page.FillAsync("#email", "admin@email.com");
    await Page.FillAsync("#password", "password");

    await Page.ClickAsync("input[type=submit]");

    await Expect(Page).ToHaveURLAsync(new Regex(".*/feed", RegexOptions.IgnoreCase));

    await Expect(Page.Locator("text=My Feed")).ToBeVisibleAsync();
}


[Test]
public async Task User_Can_Sign_Up()
{
    await Page.GotoAsync("http://127.0.0.1:5287/signup");

    await Page.FillAsync("#name", "Test User");
    await Page.FillAsync("#email", "newuser@example.com");
    await Page.FillAsync("#password", "password123");

    await Page.ClickAsync("input[type=submit]");

    await Expect(Page).ToHaveURLAsync("http://127.0.0.1:5287");

    var form = Page.Locator("form.login-card");
    await Expect(form).ToBeVisibleAsync();
}

}
