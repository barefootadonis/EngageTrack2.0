using System.ComponentModel.DataAnnotations;
namespace EngageTrack.Models
{
    public class GroupChangeRequest
    {
        [Key]
        public int RequestId { get; set; }

        public int StudentId { get; set; }
        public User Student { get; set; } = null!;

        public int GroupId { get; set; }
        public Group Group { get; set; } = null!;

        [Required]
        public string Reason { get; set; } = string.Empty;

        [Required]
        public string Status { get; set; } = "Pending";

        public DateTime DateSubmitted { get; set; } = DateTime.UtcNow;

        public string? Notes { get; set; }
    }
}
