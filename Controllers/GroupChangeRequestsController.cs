using System.Data;
using EngageTrack.Data;
using EngageTrack.Models;
using EngageTrack.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class GroupChangeRequestsController : Controller
{
    private const string Pending = "Pending";
    private const string Accepted = "Accepted";
    private const string Rejected = "Rejected";
    private const string Withdrawn = "Withdrawn";

    private readonly ApplicationDbContext _context;
    private readonly CurrentStudent _currentStudent;

    public GroupChangeRequestsController(
        ApplicationDbContext context,
        CurrentStudent currentStudent)
    {
        _context = context;
        _currentStudent = currentStudent;
    }

    private IQueryable<User> Students()
    {
        return _context.Users.Where(user => user.UserRole == "Student");
    }

    private IQueryable<Group> ActiveGroupsFor(int studentId)
    {
        return _context.Groups.Where(group =>
            group.GroupIsActive &&
            _context.GroupMemberships.Any(membership =>
                membership.GroupId == group.GroupId &&
                membership.StudentId == studentId));
    }

    private IQueryable<GroupChangeRequest> RequestsForStaff(int staffId)
    {
        return _context.GroupChangeRequests.Where(request =>
            _context.Users.Any(user => user.UserId == staffId &&
                (user.UserRole == "ModuleLeader" ||
                 (user.UserRole == "Supervisor" &&
                  _context.Groups.Any(group =>
                      group.GroupId == request.GroupId &&
                      group.SupervisorId == staffId)))));
    }

    private Task<List<GroupChangeRequestRow>> LoadRows(
        IQueryable<GroupChangeRequest> requests)
    {
        return (
            from request in requests
            join student in _context.Users on request.StudentId equals student.UserId
            join currentGroup in _context.Groups on request.GroupId equals currentGroup.GroupId
            orderby request.DateSubmitted descending, request.RequestId descending
            select new GroupChangeRequestRow(
                request.RequestId,
                student.UserName,
                student.UserEmail,
                currentGroup.GroupName,
                request.Reason,
                request.Status,
                request.DateSubmitted,
                request.Notes)
        ).ToListAsync();
    }

    private async Task<IActionResult> ShowStudentPage(int studentId)
    {
        var student = await Students().AsNoTracking()
            .SingleOrDefaultAsync(user => user.UserId == studentId);

        if (student is null)
        {
            return Forbid();
        }

        var activeGroups = await ActiveGroupsFor(studentId)
            .Select(group => group.GroupName)
            .Take(2)
            .ToListAsync();

        if (activeGroups.Count > 1)
        {
            return Conflict("Your account has more than one active group.");
        }

        var history = await LoadRows(_context.GroupChangeRequests
            .Where(request => request.StudentId == studentId));

        return View("Index", new StudentGroupChangePage(
            student.UserName,
            activeGroups.SingleOrDefault(),
            history));
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        if (_currentStudent.GetUserId(User) is not int studentId)
        {
            return Unauthorized();
        }

        return await ShowStudentPage(studentId);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Submit(string? reason)
    {
        if (_currentStudent.GetUserId(User) is not int studentId)
        {
            return Unauthorized();
        }

        if (!await Students().AnyAsync(user => user.UserId == studentId))
        {
            return Forbid();
        }

        reason = reason?.Trim();

        if (string.IsNullOrWhiteSpace(reason) || reason.Length > 1000)
        {
            ModelState.AddModelError("", "Enter a reason of 1 to 1,000 characters.");
            ViewData["Reason"] = reason;
            return await ShowStudentPage(studentId);
        }

        await using var transaction = await _context.Database
            .BeginTransactionAsync(IsolationLevel.Serializable);

        var groupIds = await ActiveGroupsFor(studentId)
            .Select(group => group.GroupId)
            .Take(2)
            .ToListAsync();

        if (groupIds.Count != 1)
        {
            return Conflict("You need exactly one active group to submit a request.");
        }

        var hasPending = await _context.GroupChangeRequests.AnyAsync(request =>
            request.StudentId == studentId && request.Status == Pending);

        if (hasPending)
        {
            TempData["GroupChangeMessage"] = "You already have a pending request.";
            return RedirectToAction(nameof(Index));
        }

        _context.GroupChangeRequests.Add(new GroupChangeRequest
        {
            StudentId = studentId,
            GroupId = groupIds[0],
            Reason = reason,
            Status = Pending,
            DateSubmitted = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
        await transaction.CommitAsync();

        TempData["GroupChangeMessage"] = "Group change request submitted.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Withdraw(int id)
    {
        if (_currentStudent.GetUserId(User) is not int studentId)
        {
            return Unauthorized();
        }

        var changed = await _context.GroupChangeRequests
            .Where(request => request.RequestId == id &&
                request.StudentId == studentId && request.Status == Pending)
            .ExecuteUpdateAsync(update => update
                .SetProperty(request => request.Status, Withdrawn));

        TempData["GroupChangeMessage"] = changed == 1
            ? "Request withdrawn. You can now send another."
            : "That request is no longer available to withdraw.";

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Review()
    {
        if (_currentStudent.GetStaffUserId(User) is not int staffId)
        {
            return Unauthorized();
        }

        if (!await _context.Users.AnyAsync(user => user.UserId == staffId &&
            (user.UserRole == "Supervisor" || user.UserRole == "ModuleLeader")))
        {
            return Forbid();
        }

        var requests = await LoadRows(RequestsForStaff(staffId)
            .Where(request => request.Status == Pending));

        return View(requests);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Decide(int id, string decision, string? notes)
    {
        if (_currentStudent.GetStaffUserId(User) is not int staffId)
        {
            return Unauthorized();
        }

        if (decision != Accepted && decision != Rejected)
        {
            return BadRequest("Choose Accept or Reject.");
        }

        notes = notes?.Trim();
        if (notes?.Length > 1000)
        {
            TempData["GroupChangeMessage"] = "Notes must be 1,000 characters or fewer.";
            return RedirectToAction(nameof(Review));
        }

        var changed = await RequestsForStaff(staffId)
            .Where(request => request.RequestId == id && request.Status == Pending)
            .ExecuteUpdateAsync(update => update
                .SetProperty(request => request.Status, decision)
                .SetProperty(request => request.Notes, notes));

        TempData["GroupChangeMessage"] = changed == 1
            ? $"Request {decision.ToLowerInvariant()}."
            : "That request is no longer available to review.";

        return RedirectToAction(nameof(Review));
    }
}