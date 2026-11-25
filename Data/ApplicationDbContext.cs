using EngageTrack.Models;
using Microsoft.EntityFrameworkCore;

namespace EngageTrack.Data
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<GroupMembership>()
                .HasKey(membership => new
                {
                    membership.GroupId,
                    membership.StudentId
                });
            builder.Entity<User>().HasData(
                new User
                {
                    UserId = 1001,
                    UserName = "Amara Okafor",
                    UserEmail = "amara.okafor.student@mail.shu.ac.uk",
                    UserRole = "Student"
                },
                new User
                {
                    UserId = 1002,
                    UserName = "Jamie Patel",
                    UserEmail = "jamie.patel.student@mail.shu.ac.uk",
                    UserRole = "Student"
                },
                new User
                {
                    UserId = 1003,
                    UserName = "Leila Morgan",
                    UserEmail = "leila.morgan.student@mail.shu.ac.uk",
                    UserRole = "Student"
                },
                new User
                {
                    UserId = 1004,
                    UserName = "Theo Williams",
                    UserEmail = "theo.williams.student@mail.shu.ac.uk",
                    UserRole = "Student"
                },
                new User
                {
                    UserId = 1005,
                    UserName = "Aisha Khan",
                    UserEmail = "aisha.khan.student@mail.shu.ac.uk",
                    UserRole = "Student"
                },
                new User
                {
                    UserId = 1006,
                    UserName = "Oliver James",
                    UserEmail = "oliver.james.student@mail.shu.ac.uk",
                    UserRole = "Student"
                },
                new User
                {
                    UserId = 2001,
                    UserName = "Hannah Reed",
                    UserEmail = "hannah.reed@gmail.com",
                    UserRole = "Supervisor"
                },
                new User
                {
                    UserId = 2002,
                    UserName = "Samir Ahmed",
                    UserEmail = "samir.ahmed@gmail.com",
                    UserRole = "Supervisor"
                },
                new User
                {
                    UserId = 3001,
                    UserName = "Maya Chen",
                    UserEmail = "maya.chen@gmail.com",
                    UserRole = "ModuleLeader"
                }
            );

            builder.Entity<Group>().HasData(
                new Group
                {
                    GroupId = 4001,
                    GroupName = "Green Energy Team",
                    ProjectName = "Campus Solar Tracker",
                    ProjectSummary = "Designing a dashboard to monitor campus solar energy production.",
                    MaxMembers = 5,
                    CurrentMembers = 3,
                    SupervisorId = 2001,
                    GroupIsActive = true,
                    GroupChangeRequest = false,
                    DateCreated = new DateTime(2026, 1, 15, 0, 0, 0, DateTimeKind.Utc)
                },
                new Group
                {
                    GroupId = 4002,
                    GroupName = "Access for All",
                    ProjectName = "Accessible Campus Map",
                    ProjectSummary = "Creating an accessible map of campus facilities and routes.",
                    MaxMembers = 5,
                    CurrentMembers = 3,
                    SupervisorId = 2002,
                    GroupIsActive = true,
                    GroupChangeRequest = true,
                    DateCreated = new DateTime(2026, 2, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );

            builder.Entity<GroupMembership>().HasData(
                new GroupMembership { GroupId = 4001, StudentId = 1001 },
                new GroupMembership { GroupId = 4001, StudentId = 1002 },
                new GroupMembership { GroupId = 4001, StudentId = 1003 },
                new GroupMembership { GroupId = 4002, StudentId = 1004 },
                new GroupMembership { GroupId = 4002, StudentId = 1005 },
                new GroupMembership { GroupId = 4002, StudentId = 1006 }
            );
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupMembership> GroupMemberships { get; set; }
        public DbSet<GroupChangeRequest> GroupChangeRequests { get; set; }
        public DbSet<Timeline> Timelines { get; set; }
    }
}
