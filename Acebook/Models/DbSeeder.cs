using acebook.Models;

public static class DbSeeder
{
    public static void Seed(AcebookDbContext context)
    {
        // Clear tables in correct order
        context.Comments.RemoveRange(context.Comments);
        context.FriendRequests.RemoveRange(context.FriendRequests);
        context.Posts.RemoveRange(context.Posts);
        context.Users.RemoveRange(context.Users);
        context.SaveChanges();

        // -----------------------------
        // 1. USERS
        // -----------------------------
        DateTime Dob(int year, int month, int day) => DateTime.SpecifyKind(new DateTime(year, month, day), DateTimeKind.Utc);
        var users = Enumerable.Range(1, 20)
            .Select(i => new User
            {
                Name = $"test{i}",
                Email = $"test{i}@email.com",
                Password = "password",
                DateOfBirth = Dob(2000, 01, 01)
            })
            .ToList();

        context.Users.AddRange(users);
        context.SaveChanges();

        var u = users; 

        // -----------------------------
        // 2. FRIENDSHIPS
        // -----------------------------
        void Friend(int a, int b) => u[a].AddFriend(u[b]);

        // test1 (u[0]) gets 12+ friends
        Friend(0, 1);
        Friend(0, 2);
        Friend(0, 3);
        Friend(0, 4);
        Friend(0, 5);
        Friend(0, 6);
        Friend(0, 7);
        Friend(0, 8);
        Friend(0, 9);
        Friend(0, 10);
        Friend(0, 11);
        Friend(0, 12);

        // Additional friendships 
        Friend(1, 2);
        Friend(3, 4);
        Friend(5, 6);
        Friend(7, 8);
        Friend(9, 10);
        Friend(11, 12);
        Friend(13, 14);
        Friend(15, 16);
        Friend(17, 18);
        Friend(18, 19);

        // -----------------------------
        // 3. FRIEND REQUESTS
        // -----------------------------
        var friendRequests = new List<FriendRequest>
        {
            new FriendRequest { User = u[16], Friend = u[0] },
            new FriendRequest { User = u[3], Friend = u[2] },
            new FriendRequest { User = u[5], Friend = u[4] },
            new FriendRequest { User = u[7], Friend = u[6] },
            new FriendRequest { User = u[9], Friend = u[8] },
            new FriendRequest { User = u[11], Friend = u[10] },
            new FriendRequest { User = u[13], Friend = u[12] },
            new FriendRequest { User = u[15], Friend = u[14] },
            new FriendRequest { User = u[17], Friend = u[16] },
            new FriendRequest { User = u[19], Friend = u[18] }
        };

        context.FriendRequests.AddRange(friendRequests);

        // -----------------------------
        // 4. POSTS — test1's friends all post
        // -----------------------------
        var posts = new List<Post>
        {
            new Post { Content = "Hello world!", User = u[1] },
            new Post { Content = "Just had a great coffee.", User = u[2] },
            new Post { Content = "Working on my coding skills.", User = u[3] },
            new Post { Content = "This is obviously AI LinkedIn rubbish.", User = u[4] },
            new Post { Content = "I love programming!", User = u[5] },
            new Post { Content = "Coffee is life.", User = u[6] },
            new Post { Content = "Just finished a 5k run!", User = u[7] },
            new Post { Content = "Look at this cute cat!", User = u[8] },
            new Post { Content = "Anyone up for a game tonight?", User = u[9] },
            new Post { Content = "Trying out a new recipe.", User = u[10] },
            new Post { Content = "Reading a great book.", User = u[11] },
            new Post { Content = "What a beautiful day!", User = u[12] },

            // test1 posts too
            new Post { Content = "test1 checking in!", User = u[0] },
            new Post { Content = "Happy to be here!", User = u[0] },
            new Post { Content = "Another day, another post.", User = u[0] }
        };

        context.Posts.AddRange(posts);
        context.SaveChanges();

        // -----------------------------
        // 5. COMMENTS
        // -----------------------------
        bool AreFriends(User a, User b)
        {
            return a.Friends.Any(f => f.Id == b.Id) || b.Friends.Any(f => f.Id == a.Id);
        }

        var comments = new List<Comment>();

        // add a comment only if friends
        void AddComment(int commenterIndex, int postIndex, string content)
        {
            var commenter = u[commenterIndex];
            var post = posts[postIndex];

            if (commenter.Id == post.User.Id || AreFriends(commenter, post.User))
            {
                comments.Add(new Comment
                {
                    Content = content,
                    User = commenter,
                    Post = post
                });
            }
        }

        AddComment(0, 0, "Welcome to the feed!");
        AddComment(2, 0, "Nice to see you posting.");
        AddComment(3, 0, "Classic first post.");

        AddComment(0, 1, "Coffee is essential.");
        AddComment(1, 1, "Where did you get it?");
        AddComment(3, 1, "Now I want one too.");


        AddComment(0, 2, "Love to see it!");
        AddComment(1, 2, "What language?");
        AddComment(2, 2, "Proud of you.");


        AddComment(0, 3, "Haha this is too real.");
        AddComment(1, 3, "I’ve seen so many of these.");
        AddComment(2, 3, "LinkedIn is wild.");


        AddComment(0, 4, "Same here!");
        AddComment(6, 4, "What are you working on?");
        AddComment(7, 4, "Keep it up!");

        AddComment(0, 5, "Agreed.");
        AddComment(5, 5, "I need one right now.");
        AddComment(7, 5, "Best drink ever.");


        AddComment(0, 6, "Nice work!");
        AddComment(6, 6, "Proud of you!");
        AddComment(8, 6, "Keep going!");


        AddComment(0, 7, "Awwww!");
        AddComment(7, 7, "So cute!");
        AddComment(9, 7, "I love cats.");


        AddComment(0, 8, "I'm in!");
        AddComment(8, 8, "What game?");
        AddComment(10, 8, "Count me in.");


        AddComment(0, 9, "Looks tasty!");
        AddComment(9, 9, "Share the recipe?");
        AddComment(11, 9, "Yum!");


        AddComment(0, 10, "Which book?");
        AddComment(10, 10, "Sounds interesting.");
        AddComment(12, 10, "I love reading.");


        AddComment(0, 11, "It really is.");
        AddComment(11, 11, "Perfect weather.");
        AddComment(10, 11, "Enjoy it!");


        AddComment(1, 12, "Welcome!");
        AddComment(2, 12, "Glad you're here.");

        AddComment(3, 13, "Nice post!");
        AddComment(4, 13, "Good vibes.");

        AddComment(5, 14, "Keep posting!");
        AddComment(6, 14, "Love it.");

        context.Comments.AddRange(comments);
        context.SaveChanges();
    }
}
