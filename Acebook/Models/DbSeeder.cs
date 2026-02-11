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
        var testuser2 = new User { Name = "test2", Email = "test2@email.com", Password = "password1"};
        var testuser3 = new User { Name = "test3", Email = "test3@email.com", Password = "password1"};

        admin.AddFriend(testuser);
        admin.AddFriend(testuser2);
        admin.AddFriend(testuser3);
        testuser2.AddFriend(testuser3);

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