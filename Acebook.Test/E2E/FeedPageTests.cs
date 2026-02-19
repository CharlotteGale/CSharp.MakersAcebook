
namespace Acebook.Test.E2E;

public class FeedPageTests : PlaywrightTestBase
{
    [Test]
    public async Task Index_UserCanPostOnFeed()
    {
        await Page.GotoAsync(BaseUrl);
        await AuthHelper.LoginAsync(Page);

        await Page.GetByPlaceholder("What's on your mind?").FillAsync("lorem ipsum");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Submit"}).ClickAsync();

        var postCard = Page.Locator(".recent-post-card")
                        .Filter(new() { HasText = "lorem ipsum"})
                        .First;
        await Expect(postCard).ToBeVisibleAsync();

        await Expect(postCard.Locator(".post-header")).ToContainTextAsync("test1");
    }

    [Test]
    public async Task Index_UserCanEditOwnPost()
    {
        await Page.GotoAsync(BaseUrl);
        await AuthHelper.LoginAsync(Page);
        await PostHelper.CreatePostAsync(Page);

        var postCard = Page.Locator(".recent-post-card").Filter(new() { HasText = "Lorem ipsum" });
        await postCard.GetByRole(AriaRole.Link, new() { Name = "Edit" }).ClickAsync();

        await postCard.Locator("textarea[name='content']").FillAsync("A proper post");
        await postCard.GetByRole(AriaRole.Button, new() { Name = "Save" }).ClickAsync();

        await Expect(postCard.Locator(".card-text")).ToHaveTextAsync("A proper post");
    }
}