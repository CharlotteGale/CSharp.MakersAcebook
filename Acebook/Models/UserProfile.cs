namespace acebook.Models;
public class UserProfileViewModel
{
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ProfileImg { get; set; } = null;
    public DateTime? DateOfBirth { get; set; }   // added to UserProfileViewModel so that profile page can pull users posts
    public List<Post> RecentPosts { get; set; } = new();
    public List<User> Friends { get; set; } = new();
}
