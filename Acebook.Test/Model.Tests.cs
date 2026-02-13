namespace Acebook.Test;

public class ModelTests : NUnitTestBase
{
    [Test]
    [Category("charlotte")]
    public void UserObjectConstructs()
    {
        User testUser = new User()
        {
            Name = "Test User",
            Email = "testuser@email.com",
            Password = "password"
        };

        Assert.That(testUser.Name, Is.EqualTo("Test User"));
        Assert.That(testUser.Email, Is.EqualTo("testuser@email.com"));
    }

    [Test]
    [Category("charlotte")]
    public void PostObjectConstructs()
    {
        Post testPost = new Post()
        {
            Content = "Lorem ipsum"
        };

        Assert.That(testPost.Content, Is.EqualTo("Lorem ipsum"));
    }
}