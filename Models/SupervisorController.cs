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

        public async Task<IActionResult> Index(string? searchTerm, bool noActivity = false, bool crf = false)
        {
            var groupsQuery = _context.Groups.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.Trim();
                groupsQuery = groupsQuery.Where(group =>
                    group.GroupName.Contains(searchTerm) ||
                    group.ProjectName.Contains(searchTerm) ||
                    (group.ProjectSummary != null && group.ProjectSummary.Contains(searchTerm)));
            }

            if (noActivity)
            {
                groupsQuery = groupsQuery.Where(group => !group.GroupIsActive);
            }

            if (crf)
            {
                groupsQuery = groupsQuery.Where(group => group.GroupChangeRequest);
            }

            ViewData["SearchTerm"] = searchTerm;
            ViewData["NoActivity"] = noActivity;
            ViewData["Crf"] = crf;

            var groups = await groupsQuery.ToListAsync();
            return View(groups);
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
