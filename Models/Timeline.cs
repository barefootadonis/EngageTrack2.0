using System.ComponentModel.DataAnnotations;
namespace EngageTrack.Models
{
    public class Timeline
    {
        public int TimelineId { get; set; }

        public int GroupId { get; set; }
        public Group Group { get; set; } = null!;

        public DateTime DateDue { get; set; }

        [Required]
        public string Content { get; set; } = string.Empty;
    }
}
