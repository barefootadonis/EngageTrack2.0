using System.ComponentModel.DataAnnotations;

namespace EngageTrack.Models;

public class CreateGroupInput
{
    [Required]
    public string GroupName { get; set; } = string.Empty;

    [Required]
    public string ProjectName { get; set; } = string.Empty;

    [Required]
    public string ProjectSummary { get; set; } = string.Empty;

    [Required]
    public int? SupervisorId { get; set; }

    public List<CreateGroupMemberInput> Members { get; set; } = new();
}

public class CreateGroupMemberInput
{
    public int StudentId { get; set; }

    [Required(ErrorMessage = "Enter a project role for each student.")]
    public string StudentJob { get; set; } = string.Empty;
}