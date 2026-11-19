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
                    UserEmail = "amara.okafor@gmail.com",
                    UserRole = "Student"
                },
                new User
                {
                    UserId = 1002,
                    UserName = "Jamie Patel",
                    UserEmail = "jamie.patel@gmail.com",
                    UserRole = "Student"
                },
                new User
                {
                    UserId = 1003,
                    UserName = "Leila Morgan",
                    UserEmail = "leila.morgan@gmail.com",
                    UserRole = "Student"
                },
                new User
                {
                    UserId = 1004,
                    UserName = "Theo Williams",
                    UserEmail = "theo.williams@gmail.com",
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
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupMembership> GroupMemberships { get; set; }
        public DbSet<GroupChangeRequest> GroupChangeRequests { get; set; }
        public DbSet<Timeline> Timelines { get; set; }
    }
}
