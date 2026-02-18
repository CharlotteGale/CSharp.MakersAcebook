namespace Acebook.Test;


public class FetchAllPostsTests : NUnitTestBase
{
    [Test]
    public void Test()
    {
        List<Post> posts = _context.Posts.ToList();

        Assert.That(posts[0].Content, Is.EquivalentTo("Hello world!"));
        Assert.That(posts[0].User.Name, Is.EquivalentTo("test2"));
    }
}