using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using acebook.Models;
using acebook.ActionFilters;
using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace acebook.Tests;


public class FetchAllPostsTests
{
    private User _testuser;
    private AcebookDbContext _context;

    [SetUp]

    public void SetUp()
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.Development.json")
            .Build();
        var options = new DbContextOptionsBuilder<AcebookDbContext>()
            .Options;
        
        _context = new AcebookDbContext(options, configuration);
        _context.Database.EnsureCreated();
        _context.SaveChanges();

    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }



[Test]
    public void Test()
    {
        List<Post> posts = _context.Posts.ToList();
        Assert.That(posts[0].Content, Is.EquivalentTo("apWEIFRGHOU"));
        Assert.That(posts[0].User.Name, Is.EquivalentTo("Admin"));
    }
}

