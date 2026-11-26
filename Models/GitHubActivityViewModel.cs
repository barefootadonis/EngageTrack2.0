using System.Collections.Generic;

namespace EngageTrack.Slices.Models
{
    public class GitHubActivityViewModel
    {
        public IEnumerable<GitHubActivity> Activities { get; set; } = new List<GitHubActivity>();

        public GitHubSyncStatus? SyncStatus { get; set; }

        public int CommitCount { get; set; }

        public int PullRequestCount { get; set; }
    }
}
