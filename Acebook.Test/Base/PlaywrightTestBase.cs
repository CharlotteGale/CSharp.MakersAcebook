using acebook.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;

namespace Acebook.Test.Base;


public class PlaywrightTestBase : PageTest
{
    protected AcebookDbContext _context;

    [SetUp]
    public async Task BaseSetUp()
    {
        var configuration = TestConfiguration.GetConfiguration();
        var connectionString = TestConfiguration.GetConnectionString();

        var options = new DbContextOptionsBuilder<AcebookDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        _context = new AcebookDbContext(options, configuration);
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();

        DbSeeder.Seed(_context);

        _context.SaveChanges();
    }

    [TearDown]
    public void BaseTearDown()
    {
        _context?.Dispose();
    }
}