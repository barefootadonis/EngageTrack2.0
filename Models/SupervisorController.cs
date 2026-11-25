using EngageTrack.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EngageTrack.Models
{
    public class SupervisorController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SupervisorController(ApplicationDbContext context)
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
    }
}
