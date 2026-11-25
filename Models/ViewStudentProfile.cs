namespace EngageTrack.Models
{
    public class ViewStudentProfile
    {
        public int StudentId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Skills { get; set; } = string.Empty;
    }
}
