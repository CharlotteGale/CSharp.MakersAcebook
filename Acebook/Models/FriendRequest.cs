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
    // This is the Id of the user that is receiving the request
    public User? Friend {get;set;}
    // public bool Pending {get; set;} = true;
    // Starts as pending(t), if accepted or rejected, turns to false then deletes 
    public DateTime CreatedAt {get; set;} = DateTime.UtcNow;

    // potentially adapt this to control all relationships chaning the pending key to more versatile flag i.e. friends blocked pending ignore.


}