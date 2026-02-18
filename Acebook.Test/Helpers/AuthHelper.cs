
namespace Acebook.Test.Helpers;

public static class AuthHelper
{
    public static async Task LoginAsync(IPage page, string email, string password)
    {
        await page.GetByPlaceholder("Email").FillAsync(email);
        await page.GetByPlaceholder("Password").FillAsync(password);
        await page.GetByRole(AriaRole.Button, new() { Name = "Log In" }).ClickAsync();

        await Assertions.Expect(page).ToHaveURLAsync(new Regex(".*/feed", RegexOptions.IgnoreCase));
    }
}