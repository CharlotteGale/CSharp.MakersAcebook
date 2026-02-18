
namespace Acebook.Test.Helpers;

public static class PostHelper
{
    public static async Task CreatePostAsync(IPage page, string content="Lorem ipsum")
    {
        var postForm = page.Locator("form[action='/posts']");

        await postForm.Locator("input[name='content']").FillAsync(content);
        await page.GetByRole(AriaRole.Button, new() { Name = "Submit"}).ClickAsync();

        await Assertions.Expect(page.Locator(".recent-post-card").First).ToBeVisibleAsync();
    }
}