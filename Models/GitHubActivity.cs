using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EngageTrack.Models;

namespace EngageTrack.Slices.Models
{
    public class GitHubActivity
    {
        public int GitHubActivityId { get; set; }
        public string ActivityType { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public int UserId {  get; set; }
        public User Author { get; set; } = null!;

        public DateTime OccurredAt { get; set; }

        public string Url { get; set; } = string.Empty;
    }
}
