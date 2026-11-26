using System;

namespace EngageTrack.Slices.Models
{
    public class GitHubSyncStatus
    {
        public int GitHubSyncStatusId { get; set; }

        public int UserId { get; set; }

        public bool IsConnected { get; set; }

        public bool LastSyncSuccessful { get; set; }

        public DateTime? LastSuccessfulSync { get; set; }

        public string ErrorMessage { get; set; } = string.Empty;
    }
}
