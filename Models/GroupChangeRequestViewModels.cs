namespace EngageTrack.Models;

public sealed record GroupChangeRequestRow(
    int RequestId,
    string StudentName,
    string StudentEmail,
    string GroupName,
    string Reason,
    string Status,
    DateTime DateSubmitted,
    string? Notes);

public sealed record StudentGroupChangePage(
    string StudentName,
    string? ActiveGroupName,
    IReadOnlyList<GroupChangeRequestRow> Requests)
{
    public bool HasPendingRequest =>
        Requests.Any(request => request.Status == "Pending");
}