using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EngageTrack.Models;

public class GroupPreviewInput
{
    public int Id { get; set; }

    [Required]
    public string GroupName { get; set; } = "";

    [Required]
    public string ProjectName { get; set; } = "";

    [Required]
    public string ProjectSummary { get; set; } = "";

    public List<GroupPreviewMember> Members { get; set; } = new();

    public string? SupervisorName { get; set; }
    public string? SupervisorEmail { get; set; }

    public List<GroupPreviewTimelineItem> Timeline { get; set; } = new();
}

public class GroupPreviewMember
{
    public string Name { get; set; } = "";

    [Required]
    public string Role { get; set; } = "";

    public string Email { get; set; } = "";
}

public class GroupPreviewTimelineItem
{
    public int TimelineId { get; set; }
    [Required]
    public string Date { get; set; } = "";

    [Required]
    public string Description { get; set; } = "";
}