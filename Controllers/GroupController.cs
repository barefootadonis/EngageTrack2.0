using Microsoft.AspNetCore.Mvc;
using EngageTrack.Data;
using EngageTrack.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

public class GroupsController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }
    [HttpGet]
    public IActionResult Details(int id)
    {
        return View();
    }
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }
    
    [HttpPost]
    public IActionResult Create(View_Groups model)
    {
        return View();
    }
    private readonly ApplicationDbContext _context;

    public GroupsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Details(int id)
    {
        var group = await _context.Groups
            .Include(g => g.Supervisor)
            .FirstOrDefaultAsync(g => g.GroupId == id);

        if (group == null)
            return NotFound();

        return View(group);
    }