using EngageTrack.Data;
using EngageTrack.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using EngageTrack.Services;

public class StudentsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly CurrentStudent _currentStudent;


    public StudentsController(
        ApplicationDbContext context,
        CurrentStudent currentStudent)
    {
        _context = context;
        _currentStudent = currentStudent;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        if (_currentStudent.GetUserId(User) is not int studentId)
        {
            return Unauthorized();
        }

        var student = await _context.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(user =>
                user.UserId == studentId && user.UserRole == "Student");

        if (student is null)
        {
            return NotFound("The seeded student with UserId 1004 was not found.");
        }

        var activeGroups = await _context.Groups
            .AsNoTracking()
            .Where(group => group.GroupIsActive &&
                _context.GroupMemberships.Any(membership =>
                    membership.GroupId == group.GroupId &&
                    membership.StudentId == studentId))
            .Take(2)
            .ToListAsync();

        if (activeGroups.Count == 0)
        {
            return View(new StudentDashboardViewModel(student.UserName, null));
        }

        if (activeGroups.Count > 1)
        {
            return Conflict("This student belongs to more than one active group.");
        }

        var group = activeGroups[0];

        var activeGroupId = group.GroupId;

        var members = await (
            from membership in _context.GroupMemberships
            join user in _context.Users on membership.StudentId equals user.UserId
            where membership.GroupId == activeGroupId
            orderby user.UserName
            select new StudentGroupMember(
                user.UserName,
                membership.StudentJob ?? "",
                user.UserEmail)
        ).ToListAsync();

        var supervisor = await _context.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(user => user.UserId == group.SupervisorId);

        var timelineEntries = await _context.Timelines
            .AsNoTracking()
            .Where(item => item.GroupId == group.GroupId)
            .OrderBy(item => item.DateDue)
            .ToListAsync();

        var timeline = timelineEntries
            .Select(item => $"{item.DateDue:d MMMM} — {item.Content}")
            .ToList();

        var details = new StudentGroupDetails(
            GroupId: group.GroupId,
            GroupName: group.GroupName,
            ProjectName: group.ProjectName,
            ProjectSummary: group.ProjectSummary ?? "",
            Members: members,
            SupervisorName: supervisor?.UserName ?? "No supervisor assigned",
            SupervisorEmail: supervisor?.UserEmail ?? "",
            Timeline: timeline);

        return View(new StudentDashboardViewModel(student.UserName, details));
    }
}