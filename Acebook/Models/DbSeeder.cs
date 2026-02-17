namespace acebook.Models;

public static class DbSeeder
{
    public static void Seed(AcebookDbContext context)
    {
        context.FriendRequests.RemoveRange(context.FriendRequests);
        context.Posts.RemoveRange(context.Posts);
        context.Users.RemoveRange(context.Users);

        context.SaveChanges();

        DateTime Dob(int year, int month, int day) => DateTime.SpecifyKind(new DateTime(year, month, day), DateTimeKind.Utc);

        var admin = new User { Name = "Admin", Email = "admin@email.com", Password = "password", DateOfBirth = Dob(1990, 1, 1) };
        var testuser = new User { Name = "Testy McTesterson", Email = "test@email.com", Password = "password1", DateOfBirth = Dob(1995, 6, 15) };
        var testuser2 = new User { Name = "test2", Email = "test2@email.com", Password = "password1", DateOfBirth = Dob(1998, 3, 22) };
        var testuser3 = new User { Name = "test3", Email = "test3@email.com", Password = "password1", DateOfBirth = Dob(2000, 11, 5) };
        //We need an extra test user for visability

        var request1 = new FriendRequest { User = testuser2, Friend = testuser };

        admin.AddFriend(testuser);
        admin.AddFriend(testuser2);
        admin.AddFriend(testuser3);
        testuser2.AddFriend(testuser3);

        context.Users.AddRange(admin, testuser);

        context.Posts.AddRange(
            new Post { Content = "This is a post", User = admin },
            new Post { Content = "This is a funnier post", User = admin },
            new Post { Content = "This is obviously AI Linkdin rubbish", User = admin },
            new Post { Content = "This is a cute picture of a cat", User = testuser }
        );

        context.FriendRequests.AddRange(request1);

        context.SaveChanges();
    }

}