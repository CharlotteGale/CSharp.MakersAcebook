using Microsoft.AspNetCore.Identity;

namespace Acebook.Test.Unit;

public class PasswordHashingTests : NUnitTestBase
{
    [Test]
    [Category("password")]
    public void HashPassword_ShouldReturnHashedPassword()
    {
        var hasher = new PasswordHasher<User>();
        var user = new User 
        { 
            Name = "Test Subject", 
            Email = "test@example.com", 
            Password = "testPassword" 
        };
        string hashedPassword = hasher.HashPassword(user, user.Password);

        Assert.That(hashedPassword, Is.Not.Null);
        Assert.That(hashedPassword, Is.Not.EqualTo(user.Password));
        Assert.That(hashedPassword.Length, Is.GreaterThan(20));
        Assert.That(hashedPassword.Length, Is.Not.EqualTo(user.Password.Length));
    }

    [Test]
    [Category("password")]
    public void VerifyHashedPassword_WithCorrectPassword_ShouldReturnSuccess()
    {
        var hasher = new PasswordHasher<User>();
        var user = new User 
        { 
            Name = "Test Subject", 
            Email = "test@example.com", 
            Password = "testPassword" 
        };
        string plainTextPassword = "testPassword";
        string hashedPassword = hasher.HashPassword(user, user.Password);
        var result = hasher.VerifyHashedPassword(user, hashedPassword, plainTextPassword);

        Assert.That(result, Is.EqualTo(PasswordVerificationResult.Success)
                            .Or.EqualTo(PasswordVerificationResult.SuccessRehashNeeded));
    }

    [Test]
    [Category("password")]
    public void VerifyHashedPassword_WithIncorrectPassword_ShouldReturnFailed()
    {
        var hasher = new PasswordHasher<User>();
        var user = new User 
        { 
            Name = "Test Subject", 
            Email = "test@example.com", 
            Password = "testPassword" 
        };
        string plainTextPassword = "wrongPassword";
        string hashedPassword = hasher.HashPassword(user, user.Password);
        var result = hasher.VerifyHashedPassword(user, hashedPassword, plainTextPassword);

        Assert.That(result, Is.EqualTo(PasswordVerificationResult.Failed));
    }
}