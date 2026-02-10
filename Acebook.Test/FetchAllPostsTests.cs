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


    // [Test]
    // public void Test()
    // {
    //     List<Post> posts = _context.Posts.ToList();
    //     List<Post> test = new List<Post>();
    //     Post post1 = new Post(Id: 18, Content: "soafeir;hjgo", UserId: 11);
    //     test.Add(post1);
    //     Post post2 = new Post(Id: 17, Content: "SD;VOIUH", UserId: 10);
    //     test.Add(post2);
    //     Post post3 = new Post(Id: 15, Content: "apWEIFRGHOU", UserId: 10);
    //     test.Add(post3);
    //     Post post4 = new Post(Id: 16, Content: ";focwe hi", UserId: 10);
    //     test.Add(post4);
    //     Assert.That(posts, Is.EquivalentTo(test));
    // }

}

