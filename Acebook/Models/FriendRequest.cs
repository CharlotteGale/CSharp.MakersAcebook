namespace acebook.Models;
using System.ComponentModel.DataAnnotations;

public class FriendRequest
{
    [Key]
    public int Id {get; set;}
    public int UserId {get; set;}
    // This is Id of the user placing the request
    public User? User {get; set;}
    public int FriendId {get; set;}
    // This is the Id of the user that is receving the request
    public User? Friend {get;set;}
    public bool Pending {get; set;} = true;
    // Starts as pending, if accepted or rejected, turns to false then deletes 
    public DateTime CreatedAt {get; set;} = DateTime.UtcNow;


}