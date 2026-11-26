
using EngageTrack.Data;
using EngageTrack.Slices.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EngageTrack.Slices.Controllers
{
    public class GitHubActivityController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const int DemoUserId = 1001;

        public GitHubActivityController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var activities = await _context.GitHubActivities
                .Where(a => a.UserId == DemoUserId)
                .Include(a => a.Author)
                .OrderByDescending(a => a.OccurredAt)
                .ToListAsync();

            var syncStatus = await _context.GitHubSyncStatuses
                .FirstOrDefaultAsync(s => s.UserId == DemoUserId);

            var viewModel = new GitHubActivityViewModel
            {
                Activities = activities,
                SyncStatus = syncStatus,
                CommitCount = activities.Count(a =>
                    a.ActivityType.Equals("Commit", StringComparison.OrdinalIgnoreCase)),
                PullRequestCount = activities.Count(a =>
                    a.ActivityType.Equals("Pull Request", StringComparison.OrdinalIgnoreCase))
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Connect()
        {
            var status = await _context.GitHubSyncStatuses
                .FirstOrDefaultAsync(s => s.UserId == DemoUserId);

            if (status == null)
            {
                status = new GitHubSyncStatus
                {
                    UserId = DemoUserId
                };

                _context.GitHubSyncStatuses.Add(status);
            }

            status.IsConnected = true;
            status.ErrorMessage = string.Empty;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Synchronise()
        {
            var status = await _context.GitHubSyncStatuses
                .FirstOrDefaultAsync(s => s.UserId == DemoUserId);

            if (status == null || !status.IsConnected)
            {
                if (status != null)
                {
                    status.LastSyncSuccessful = false;
                    status.ErrorMessage = "Connect the demo GitHub account before synchronising.";
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }

            status.LastSyncSuccessful = true;
            status.LastSuccessfulSync = DateTime.Now;
            status.ErrorMessage = string.Empty;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Disconnect()
        {
            var status = await _context.GitHubSyncStatuses
                .FirstOrDefaultAsync(s => s.UserId == DemoUserId);

            if (status != null)
            {
                status.IsConnected = false;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> SyncStatus()
        {
            var syncStatuses = await _context.GitHubSyncStatuses
                .Where(s => s.UserId == DemoUserId)
                .ToListAsync();

            return View(syncStatuses);
        }
    }
}

