namespace acebook.Models;

public static class DbSeeder
{
    public static void Seed(AcebookDbContext context)
    {
        context.Users.RemoveRange(context.Users);
        context.Posts.RemoveRange(context.Posts);
    
        context.SaveChanges();

        var admin = new User { Name = "Admin", Email = "admin@email.com", Password = "password" };
        var testuser = new User { Name = "Testy McTesterson", Email = "test@email.com", Password = "password1"};

        context.Users.AddRange( admin, testuser );

        context.Posts.AddRange(
            new Post { Content = "apWEIFRGHOU", User = admin },
            new Post { Content = ";focwe hi", User = admin},
            new Post { Content = "SD;VOIUH", User = admin},
            new Post { Content = "soafeir;hjgo", User = testuser}
        );  

        context.SaveChanges();
    }

}