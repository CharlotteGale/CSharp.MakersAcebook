namespace acebook.Models;

using System.ComponentModel.DataAnnotations;
using Models.Validation;

public class User
{
  [Key]
  public int Id { get; set; }
  [Required]
  public required string Name { get; set; }

  [Required(ErrorMessage = "Email is required.")]
  [EmailAddress(ErrorMessage = "Enter a valid email address.")]
  public required string Email { get; set; }

  [Required(ErrorMessage = "Password is required.")]
  [StrongPassword(8)]
  public required string Password { get; set; }

  [Required (ErrorMessage = "The Date of Birth field is required.")]
  public DateTime? DateOfBirth { get; set; }
  public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
  public virtual ICollection<User> Friends { get; set; } = new List<User>();
  public ICollection<Comment> Comments { get; set; } = new List<Comment>();
  public ICollection<Like> Likes { get; set; } = new List<Like>();

  public User() { }

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
  public void RemoveFriend(User friend)
  {
    this.Friends.Remove(friend);
    friend.Friends.Remove(this);
  }

  public int GetAge()
  {
    var today = DateTime.UtcNow;
    int age = today.Year - DateOfBirth!.Value.Year;

    // Subtracts a year if birthday hasn't occurred yet this year
    if (DateOfBirth!.Value.Date > today.AddYears(-age))
    {
      age--;
    }

    return age;
  }
}

