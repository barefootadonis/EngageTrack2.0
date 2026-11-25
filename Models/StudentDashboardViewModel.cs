using System.Collections.Generic;

namespace EngageTrack.Models;

public sealed record StudentDashboardViewModel(
    string StudentName,
    StudentGroupDetails? ActiveGroup);

public sealed record StudentGroupDetails(
    int GroupId,
    string GroupName,
    string ProjectName,
    string ProjectSummary,
    IReadOnlyList<StudentGroupMember> Members,
    string SupervisorName,
    string SupervisorEmail,
    IReadOnlyList<string> Timeline);

public sealed record StudentGroupMember(
    string Name,
    string Role,
    string Email);