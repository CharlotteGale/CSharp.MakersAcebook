namespace acebook.Models;
public class UserProfileViewModel
{
    public int UserId { get; set; }
    public string Name { get; set; } = "";
    public string? ProfilePhotoUrl { get; set; }
    public List<acebook.Models.Post> RecentPosts { get; set; } = new();
    public List<acebook.Models.User> Friends { get; set; } = new();
}