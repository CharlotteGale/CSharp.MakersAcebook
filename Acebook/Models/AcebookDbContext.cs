namespace acebook.Models;
using Microsoft.EntityFrameworkCore;

public class AcebookDbContext : DbContext
{
    private readonly IConfiguration _configuration;

    public DbSet<Post>? Posts { get; set; }
    public DbSet<User>? Users { get; set; }
    public DbSet<Comment>? Comments { get; set; }
    public DbSet<FriendRequest>? FriendRequests {get; set;}
    public DbSet<Like>? Likes { get; set; }

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

      modelBuilder.Entity<FriendRequest>()
        .HasOne(fr => fr.User)
        .WithMany() 
        .HasForeignKey(fr => fr.UserId)
        .OnDelete(DeleteBehavior.Restrict); 

      modelBuilder.Entity<FriendRequest>()
        .HasOne(fr => fr.Friend)
        .WithMany()
        .HasForeignKey(fr => fr.FriendId)
        .OnDelete(DeleteBehavior.Restrict);
    }
}
