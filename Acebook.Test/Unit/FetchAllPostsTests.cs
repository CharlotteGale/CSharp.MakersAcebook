namespace Acebook.Test.Unit;


public class FetchAllPostsTests : NUnitTestBase
{
    [Test]
    public void Test()
    {
        List<Post> posts = _context.Posts.ToList();

        Assert.That(posts[0].Content, Is.EquivalentTo("This is a post"));
        Assert.That(posts[0].User.Name, Is.EquivalentTo("Admin"));
    }
}

