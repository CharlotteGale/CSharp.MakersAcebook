namespace acebook.Models;

public static class DbSeeder
{
    public static void Seed(AcebookDbContext context)
    {
        context.Users.RemoveRange(context.Users);
        context.Posts.RemoveRange(context.Posts);
    
        context.SaveChanges();

        context.Users.AddRange(
            new User { Name = "Admin", Email = "admin@email.com", Password = "password" },
            new User { Name = "Testy McTesterson", Email = "test@email.com", Password = "password1"}
        );

        context.Posts.AddRange(
            new Post { Content = "apWEIFRGHOU", UserId = 1 },
            new Post { Content = ";focwe hi", UserId = 1 },
            new Post { Content = "SD;VOIUH", UserId = 1 },
            new Post { Content = "soafeir;hjgo", UserId = 1 }
        );  
    }

}