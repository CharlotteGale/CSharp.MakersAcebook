using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using acebook.Models;
using acebook.ActionFilters;

namespace acebook.Controllers;

public class CommentsController : Controller
{ 
    private readonly AcebookDbContext _context;
    private readonly ILogger<CommentsController> _logger;

    public CommentsController(AcebookDbContext context, ILogger<CommentsController> logger)
    {
        _context = context;
        _logger = logger;
    }

}