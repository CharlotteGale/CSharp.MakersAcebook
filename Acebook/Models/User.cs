namespace acebook.Models;
using System.ComponentModel.DataAnnotations;

public class User
{
  [Key]
  public int Id {get; set;}
  [Required]
  public required string Name {get; set;}
  [Required]
  public required string Email {get; set;}
  [Required]
  public required string Password {get; set;}
  public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
  public virtual ICollection<User> Friends { get; set; } = new List<User>();

  public User(){}

  public void AddFriend(User friend)
    {
      if (!this.Friends.Contains(friend))
      {
        this.Friends.Add(friend);
      }
      if (!friend.Friends.Contains(this))
      {
        friend.Friends.Add(this);
      }
    }
}