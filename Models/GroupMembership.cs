using System.ComponentModel.DataAnnotations;
namespace EngageTrack.Models
{
    public class GroupMembership
    {
        public int GroupId { get; set; }
        public Group Group { get; set; } = null!;

        public int StudentId { get; set; }
        public User Student { get; set; } = null!;

        public string? StudentJob { get; set; }
    }
}
