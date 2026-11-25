using System.ComponentModel.DataAnnotations;
namespace EngageTrack.Models
{
    public class Group
    {
        public int GroupId { get; set; }

        [Required]
        public string GroupName { get; set; } = string.Empty;

        [Required]
        public string ProjectName { get; set; } = string.Empty;

        public string? ProjectSummary { get; set; }

        public int MaxMembers { get; set; } = 5;

        public int? SupervisorId { get; set; }
        public User? Supervisor { get; set; }

        public bool GroupIsActive { get; set; } = true;
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    }
}
