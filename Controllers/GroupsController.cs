using System.Data;
using System.Globalization;
using EngageTrack.Data;
using EngageTrack.Models;
using EngageTrack.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class GroupsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly CurrentStudent _currentStudent;

    public GroupsController(ApplicationDbContext context, CurrentStudent currentStudent)
    {
        _context = context;
        _currentStudent = currentStudent;
    }

    private IQueryable<User> AvailableStudents()
    {
        return _context.Users
            .Where(user => user.UserRole == "Student")
            .Where(user => !_context.GroupMemberships.Any(membership =>
                membership.StudentId == user.UserId &&
                _context.Groups.Any(group =>
                    group.GroupId == membership.GroupId && group.GroupIsActive)));
    }

    private IQueryable<User> Supervisors()
    {
        return _context.Users.Where(user => user.UserRole == "Supervisor");
    }

    private async Task LoadCreateOptions()
    {
        ViewBag.Students = await AvailableStudents()
            .AsNoTracking()
            .OrderBy(user => user.UserName)
            .ToListAsync();

        ViewBag.Supervisors = await Supervisors()
            .AsNoTracking()
            .OrderBy(user => user.UserName)
            .ToListAsync();
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        if (_currentStudent.GetUserId(User) is not int studentId)
        {
            return Unauthorized();
        }

        if (!await AvailableStudents().AnyAsync(user => user.UserId == studentId))
        {
            return Conflict("You must be an available student to create a group.");
        }

        await LoadCreateOptions();
        ViewBag.CurrentStudentId = studentId;

        return View(new CreateGroupInput
        {
            Members = new() { new CreateGroupMemberInput { StudentId = studentId } }
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateGroupInput input)
    {
        if (_currentStudent.GetUserId(User) is not int studentId)
        {
            return Unauthorized();
        }

        ViewBag.CurrentStudentId = studentId;
        var studentIds = input.Members.Select(member => member.StudentId).ToList();

        if (!studentIds.Contains(studentId))
        {
            ModelState.AddModelError("", "You must be a member of the group you create.");
        }

        if (studentIds.Count is < 2 or > 5)
        {
            ModelState.AddModelError("", "Select between 2 and 5 students.");
        }

        if (studentIds.Distinct().Count() != studentIds.Count)
        {
            ModelState.AddModelError("", "Select each student only once.");
        }

        if (ModelState.IsValid)
        {
            await using var transaction = await _context.Database
                .BeginTransactionAsync(IsolationLevel.Serializable);

            var availableCount = await AvailableStudents()
                .CountAsync(user => studentIds.Contains(user.UserId));

            if (availableCount != studentIds.Count)
            {
                ModelState.AddModelError("", "One or more selected students are unavailable.");
            }

            if (!await Supervisors().AnyAsync(user => user.UserId == input.SupervisorId))
            {
                ModelState.AddModelError(nameof(input.SupervisorId), "Select a valid supervisor.");
            }

            if (ModelState.IsValid)
            {
                var group = new Group
                {
                    GroupName = input.GroupName.Trim(),
                    ProjectName = input.ProjectName.Trim(),
                    ProjectSummary = input.ProjectSummary.Trim(),
                    SupervisorId = input.SupervisorId!.Value,
                    GroupIsActive = true,
                    DateCreated = DateTime.UtcNow
                };

                _context.Groups.Add(group);

                foreach (var member in input.Members)
                {
                    _context.GroupMemberships.Add(new GroupMembership
                    {
                        Group = group,
                        StudentId = member.StudentId,
                        StudentJob = member.StudentJob.Trim()
                    });
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["GroupCreated"] = $"Group '{group.GroupName}' was created.";
                return RedirectToAction("Index", "Students");
            }
        }

        await LoadCreateOptions();
        return View(input);
    }

    private IQueryable<Group> StaffAccessibleGroups(int staffId)
    {
        return _context.Groups.Where(group => _context.Users.Any(user =>
            user.UserId == staffId &&
            (user.UserRole == "ModuleLeader" ||
             (user.UserRole == "Supervisor" && group.SupervisorId == user.UserId))));
    }

    private IQueryable<Group> StudentEditableGroups(int studentId)
    {
        return _context.Groups.Where(group =>
            group.GroupIsActive &&
            _context.Users.Any(user => user.UserId == studentId && user.UserRole == "Student") &&
            _context.GroupMemberships.Any(membership =>
                membership.GroupId == group.GroupId &&
                membership.StudentId == studentId));
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id = 1)
    {
        if (_currentStudent.GetStaffUserId(User) is not int staffId)
        {
            return Unauthorized();
        }

        var group = await StaffAccessibleGroups(staffId)
            .AsNoTracking()
            .SingleOrDefaultAsync(item => item.GroupId == id);

        return group is null
            ? NotFound()
            : View("GroupView", await LoadGroupModel(group));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id = 1)
    {
        if (_currentStudent.GetStaffUserId(User) is not int staffId)
        {
            return Unauthorized();
        }

        return await OpenEditor(id, StaffAccessibleGroups(staffId), studentEditing: false);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, GroupPreviewInput input)
    {
        if (_currentStudent.GetStaffUserId(User) is not int staffId)
        {
            return Unauthorized();
        }

        return await SaveGroupEdits(id, input, StaffAccessibleGroups(staffId), studentEditing: false);
    }

    [HttpGet]
    public async Task<IActionResult> EditAsStudent(int id)
    {
        if (_currentStudent.GetUserId(User) is not int studentId)
        {
            return Unauthorized();
        }

        return await OpenEditor(id, StudentEditableGroups(studentId), studentEditing: true);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditAsStudent(int id, GroupPreviewInput input)
    {
        if (_currentStudent.GetUserId(User) is not int studentId)
        {
            return Unauthorized();
        }

        return await SaveGroupEdits(id, input, StudentEditableGroups(studentId), studentEditing: true);
    }

    private async Task<IActionResult> OpenEditor(
        int id, IQueryable<Group> accessibleGroups, bool studentEditing)
    {
        var group = await accessibleGroups.AsNoTracking()
            .SingleOrDefaultAsync(item => item.GroupId == id);

        if (group is null)
        {
            return NotFound();
        }

        return await ShowEditor(await LoadGroupModel(group), studentEditing);
    }

    private async Task<IActionResult> ShowEditor(GroupPreviewInput input, bool studentEditing)
    {
        await LoadCreateOptions();
        ViewData["StudentEditing"] = studentEditing;
        return View("GroupEdit", input);
    }

    private async Task<GroupPreviewInput> LoadGroupModel(Group activeGroup)
    {
        var id = activeGroup.GroupId;

        var members = await (
            from membership in _context.GroupMemberships
            join user in _context.Users on membership.StudentId equals user.UserId
            where membership.GroupId == id
            orderby user.UserName
            select new GroupPreviewMember
            {
                Name = user.UserName,
                Email = user.UserEmail,
                Role = membership.StudentJob ?? ""
            }
        ).ToListAsync();

        var supervisor = await Supervisors().AsNoTracking()
            .SingleOrDefaultAsync(user => user.UserId == activeGroup.SupervisorId);

        var timeline = await _context.Timelines.AsNoTracking()
            .Where(item => item.GroupId == id)
            .OrderBy(item => item.DateDue)
            .ToListAsync();

        return new GroupPreviewInput
        {
            Id = id,
            GroupName = activeGroup.GroupName,
            ProjectName = activeGroup.ProjectName,
            ProjectSummary = activeGroup.ProjectSummary ?? "",
            Members = members,
            SupervisorName = supervisor?.UserName,
            SupervisorEmail = supervisor?.UserEmail,
            Timeline = timeline.Select(item => new GroupPreviewTimelineItem
            {
                TimelineId = item.TimelineId,
                Date = item.DateDue.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                Description = item.Content
            }).ToList()
        };
    }

    private async Task<IActionResult> SaveGroupEdits(
        int id, GroupPreviewInput input, IQueryable<Group> accessibleGroups, bool studentEditing)
    {
        input.Id = id;

        await using (var transaction = await _context.Database
            .BeginTransactionAsync(IsolationLevel.Serializable))
        {
            var group = await accessibleGroups.SingleOrDefaultAsync(item => item.GroupId == id);

            if (group is null)
            {
                return NotFound();
            }

            var existingMembers = await _context.GroupMemberships
                .Where(member => member.GroupId == id)
                .ToDictionaryAsync(member => member.StudentId);

            var existingTimeline = await _context.Timelines
                .Where(item => item.GroupId == id)
                .ToDictionaryAsync(item => item.TimelineId);

            var existingStudentIds = existingMembers.Keys.ToList();
            var availableStudentIds = AvailableStudents().Select(user => user.UserId);

            var eligibleStudents = await _context.Users.AsNoTracking()
                .Where(user => user.UserRole == "Student" &&
                    (existingStudentIds.Contains(user.UserId) ||
                     availableStudentIds.Contains(user.UserId)))
                .ToListAsync();

            var selectedStudents = new Dictionary<int, string>();

            foreach (var member in input.Members)
            {
                var matchingUsers = eligibleStudents.Where(user =>
                    string.Equals(user.UserEmail, member.Email?.Trim(),
                        StringComparison.OrdinalIgnoreCase)).Take(2).ToList();

                if (matchingUsers.Count != 1 ||
                    !selectedStudents.TryAdd(matchingUsers[0].UserId, member.Role))
                {
                    ModelState.AddModelError("", "Select distinct students who are available.");
                    break;
                }
            }

            if (input.Members.Count is < 2 or > 5)
            {
                ModelState.AddModelError("", "A group must have between 2 and 5 students.");
            }

            if (studentEditing && existingStudentIds.Any(id => !selectedStudents.ContainsKey(id)))
            {
                ModelState.AddModelError("", "Students must keep all existing group members.");
            }

            var postedTimelineIds = input.Timeline
                .Where(item => item.TimelineId != 0)
                .Select(item => item.TimelineId)
                .ToHashSet();

            if (postedTimelineIds.Count != input.Timeline.Count(item => item.TimelineId != 0) ||
                postedTimelineIds.Any(id => !existingTimeline.ContainsKey(id)) ||
                (studentEditing && postedTimelineIds.Count != existingTimeline.Count))
            {
                ModelState.AddModelError("", "The timeline has changed. Reopen the editor and try again.");
            }

            var dates = new List<DateTime>();

            foreach (var item in input.Timeline)
            {
                if (!DateTime.TryParseExact(item.Date, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                        DateTimeStyles.None, out var date))
                {
                    ModelState.AddModelError("", "Enter a valid date for each timeline item.");
                    break;
                }

                dates.Add(date);
            }

            var supervisorQuery = Supervisors().AsNoTracking();
            var supervisor = studentEditing && group.SupervisorId is int assignedId && assignedId > 0
                ? await supervisorQuery.SingleOrDefaultAsync(user => user.UserId == assignedId)
                : await supervisorQuery.SingleOrDefaultAsync(user => user.UserEmail == input.SupervisorEmail);

            if (supervisor is null)
            {
                ModelState.AddModelError("", "Select a valid supervisor.");
            }

            if (ModelState.IsValid)
            {
                group.GroupName = input.GroupName.Trim();
                group.ProjectName = input.ProjectName.Trim();
                group.ProjectSummary = input.ProjectSummary.Trim();
                group.SupervisorId = supervisor!.UserId;

                foreach (var selection in selectedStudents)
                {
                    if (existingMembers.TryGetValue(selection.Key, out var member))
                    {
                        member.StudentJob = selection.Value.Trim();
                    }
                    else
                    {
                        _context.GroupMemberships.Add(new GroupMembership
                        {
                            GroupId = id,
                            StudentId = selection.Key,
                            StudentJob = selection.Value.Trim()
                        });
                    }
                }

                if (!studentEditing)
                {
                    _context.GroupMemberships.RemoveRange(existingMembers.Values.Where(member =>
                        !selectedStudents.ContainsKey(member.StudentId)));

                    _context.Timelines.RemoveRange(existingTimeline.Values.Where(item =>
                        !postedTimelineIds.Contains(item.TimelineId)));
                }

                for (var i = 0; i < input.Timeline.Count; i++)
                {
                    var entry = input.Timeline[i];

                    if (entry.TimelineId == 0)
                    {
                        _context.Timelines.Add(new Timeline
                        {
                            GroupId = id,
                            DateDue = dates[i],
                            Content = entry.Description.Trim()
                        });
                    }
                    else
                    {
                        var existing = existingTimeline[entry.TimelineId];
                        existing.DateDue = dates[i];
                        existing.Content = entry.Description.Trim();
                    }
                }

                await _context.SaveChangesAsync();

                var canReturnToDetails = !studentEditing &&
                    await accessibleGroups.AnyAsync(item => item.GroupId == id);

                await transaction.CommitAsync();

                if (studentEditing)
                {
                    TempData["GroupCreated"] = $"Changes to '{group.GroupName}' were saved.";
                    return RedirectToAction("Index", "Students");
                }

                if (canReturnToDetails)
                {
                    TempData["GroupEditMessage"] = "Group changes saved.";
                    return RedirectToAction(nameof(Details), new { id });
                }

                // The staff dashboard can receive this redirect once its route is available.
                return Content("Group changes saved. The group is now assigned to another supervisor.");
            }

            if (studentEditing && supervisor is not null)
            {
                ModelState.Remove(nameof(input.SupervisorName));
                ModelState.Remove(nameof(input.SupervisorEmail));
                input.SupervisorName = supervisor.UserName;
                input.SupervisorEmail = supervisor.UserEmail;
            }
        }

        return await ShowEditor(input, studentEditing);
    }
}