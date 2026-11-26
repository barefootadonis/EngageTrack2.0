using EngageTrack.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EngageTrack.Slices.Controllers
{
    public class GitHubActivityController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GitHubActivityController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var activities = await _context.GitHubActivities
                .Include(a => a.Author)
                .OrderByDescending(a => a.OccurredAt)
                .ToListAsync();

            return View(activities);
        }

        public async Task<IActionResult> SyncStatus()
        {
            var syncStatus = await _context.GitHubSyncStatuses
                .FirstOrDefaultAsync(s => s.UserId == 1001);

            return View(syncStatus);
        }
    }
}
