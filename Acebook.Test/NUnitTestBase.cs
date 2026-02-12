namespace Acebook.Test;

public class NUnitTestBase
{
    protected AcebookDbContext _context;

    [SetUp]
    public void BaseSetUp()
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
    public void TearDown()
    {
        _context.Dispose();
    }
}