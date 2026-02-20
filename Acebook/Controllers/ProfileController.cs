using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using acebook.Models;

public class ProfileController : Controller
{
  private readonly AcebookDbContext _context;

  public ProfileController(AcebookDbContext context)
  {
    _context = context;
  }

  [HttpGet]
  public IActionResult Index()
  {
    var userId = HttpContext.Session.GetInt32("user_id");
    if (userId == null)
    {
      return RedirectToAction("New", "Sessions");
    }

    var user = _context.Users
        .Include(u => u.Posts)
        .Include(u => u.Friends)
        .FirstOrDefault(u => u.Id == userId);

    if (user == null) return NotFound();

    var vm = new UserProfileViewModel
    {
      UserId = user.Id,
      Name = user.Name,
      DateOfBirth = user.DateOfBirth,
      RecentPosts = user.Posts
            .OrderByDescending(p => p.CreatedAt)
            .Take(10)
            .ToList(),
      Friends = user.Friends.ToList(),
      ProfileImg = user.ProfileImg
    };

    return View("Index", vm);   // ⭐ THIS IS THE FIX
  }

  [Route("/Profile/UploadImage")]
  [HttpPost]
  public RedirectResult UploadProfileImage(IFormFile? imageFile = null)
  {
    int currentUserId = HttpContext.Session.GetInt32("user_id").Value;

    if (imageFile != null && imageFile.Length > 0)
    {
      var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "profile");

      if (!Directory.Exists(uploadsFolder))
        Directory.CreateDirectory(uploadsFolder);

      var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
      var filePath = Path.Combine(uploadsFolder, fileName);

      using (var stream = new FileStream(filePath, FileMode.Create))
      {
        imageFile.CopyTo(stream);
      }

      var user = _context.Users.First(u => u.Id == currentUserId);
      user.ProfileImg = "/profile/" + fileName;

      _context.SaveChanges();
    }

    return new RedirectResult("/Profile");
  }

}
