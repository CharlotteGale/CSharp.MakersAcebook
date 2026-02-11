namespace acebook.Models;
using Microsoft.EntityFrameworkCore;

public class AcebookDbContext : DbContext
{
    private readonly IConfiguration _configuration;

    public DbSet<Post>? Posts { get; set; }
    public DbSet<User>? Users { get; set; }

    public string? DbPath { get; }

    public AcebookDbContext(DbContextOptions<AcebookDbContext> options, IConfiguration configuration)
      : base(options)
    {
      _configuration = configuration;
    }

    public AcebookDbContext(){}

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
    var connectionString = _configuration.GetConnectionString("DefaultConnection");
        optionsBuilder.UseNpgsql(connectionString);
  }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<Post>()
        .Navigation(post => post.User)
        .AutoInclude();

      modelBuilder.Entity<User>()
        .HasMany(u => u.Friends)
        .WithMany()
        .UsingEntity<Dictionary<string, object>>(
          "UserFriends",
          j => j.HasOne<User>().WithMany().HasForeignKey("FriendId"),
          j => j.HasOne<User>().WithMany().HasForeignKey("UserId")
          );
    }
}
