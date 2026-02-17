namespace acebook.Models;
using System.ComponentModel.DataAnnotations;

public class Post
{
  [Key]
  public int Id {get; set;}
  [Required]
  public string Content {get; set;} = null!;
  public int UserId {get; set;}
  public User User {get; set;} = null!;
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  public DateTime? UpdatedAt { get; set; }
  public ICollection<Comment> Comments {get; set;} = new List<Comment>();
  public ICollection<Like> Likes {get; set;} = new List<Like>();
  
  }
