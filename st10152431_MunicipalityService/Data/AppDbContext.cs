using Microsoft.EntityFrameworkCore;
using st10152431_MunicipalityService.Models;

namespace st10152431_MunicipalityService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Issue> Issues { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Announcement> Announcements { get; set; }
        public DbSet<PulseResponse> PulseResponses { get; set; }
        public DbSet<ServiceRequest> ServiceRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.CellphoneNumber);
                entity.Property(u => u.Name).IsRequired();

                entity.HasMany(u => u.Issues)
                    .WithOne()
                    .HasForeignKey(i => i.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(u => u.PulseDatesString).HasColumnName("PulseDates");
            });

            // Configure Issue entity
            modelBuilder.Entity<Issue>(entity =>
            {
                entity.HasKey(i => i.Id);
                entity.Property(i => i.Location).IsRequired();
                entity.Property(i => i.Category).IsRequired();
                entity.Property(i => i.Description).IsRequired();
            });

            // Configure Event entity
            modelBuilder.Entity<Event>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.Category).IsRequired();
                entity.Property(e => e.Location).IsRequired();
            });

            // Configure Announcement entity
            modelBuilder.Entity<Announcement>(entity =>
            {
                entity.HasKey(a => a.Id);
                entity.Property(a => a.Name).IsRequired();
                entity.Property(a => a.Category).IsRequired();
                entity.Property(a => a.Location).IsRequired();
            });

            // Configure PulseResponse entity
            modelBuilder.Entity<PulseResponse>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.HasIndex(p => new { p.Date, p.UserId }).IsUnique();
            });

            // Configure ServiceRequest entity
            modelBuilder.Entity<ServiceRequest>(entity =>
            {
                entity.HasKey(sr => sr.RequestId);
                entity.Property(sr => sr.Title).IsRequired();
                entity.Property(sr => sr.Description).IsRequired();
                entity.Property(sr => sr.Category).IsRequired();
                entity.Property(sr => sr.Location).IsRequired();
                entity.Property(sr => sr.Priority).IsRequired();
                entity.Property(sr => sr.Status).IsRequired();
            });

            // ===== SEED TEST DATA =====
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Users (including an employee)
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    CellphoneNumber = "0817246624",
                    Name = "Test User",
                    IssuesReported = 1,
                    DailyPulsesCompleted = 1,
                    TotalDataPoints = 3,
                    PulseDatesString = "2025-10-15"
                },
                new User
                {
                    CellphoneNumber = "0123456789",
                    Name = "John Doe",
                    IssuesReported = 1,
                    DailyPulsesCompleted = 1,
                    TotalDataPoints = 2,
                    PulseDatesString = "2025-10-15"
                },
                new User
                {
                    CellphoneNumber = "0987654321",
                    Name = "Jane Smith",
                    IssuesReported = 0,
                    DailyPulsesCompleted = 0,
                    TotalDataPoints = 0,
                    PulseDatesString = ""
                },
                new User
                {
                    CellphoneNumber = "1111111111",
                    Name = "Employee User",
                    IssuesReported = 0,
                    DailyPulsesCompleted = 0,
                    TotalDataPoints = 0,
                    PulseDatesString = ""
                }
            );

            // Seed Issues (with various statuses and priorities)
            modelBuilder.Entity<Issue>().HasData(
                new Issue
                {
                    Id = 1,
                    Location = "123 Main Street, Cape Town",
                    Category = "Road",
                    Description = "Large pothole causing traffic issues",
                    ImagePath = null,
                    UserId = "0817246624",
                    Timestamp = new DateTime(2025, 10, 10, 10, 0, 0),
                    Priority = 2,
                    Status = "In Progress",
                    DueDate = new DateTime(2025, 10, 20),
                    LastReviewedDate = new DateTime(2025, 10, 12),
                    ReviewedBy = "1111111111"
                },
                new Issue
                {
                    Id = 2,
                    Location = "45 Beach Road, Sea Point",
                    Category = "Water",
                    Description = "Water pipe burst on sidewalk",
                    ImagePath = null,
                    UserId = "0123456789",
                    Timestamp = new DateTime(2025, 10, 12, 14, 30, 0),
                    Priority = 1,
                    Status = "Pending",
                    DueDate = new DateTime(2025, 10, 22),
                    LastReviewedDate = null,
                    ReviewedBy = null
                },
                new Issue
                {
                    Id = 3,
                    Location = "78 Park Avenue, Gardens",
                    Category = "Electricity",
                    Description = "Streetlight not working for 2 weeks",
                    ImagePath = null,
                    UserId = null, // Anonymous report
                    Timestamp = new DateTime(2025, 10, 14, 9, 15, 0),
                    Priority = null,
                    Status = "Pending",
                    DueDate = null,
                    LastReviewedDate = null,
                    ReviewedBy = null
                }
            );

            // Seed Events
            modelBuilder.Entity<Event>().HasData(
                new Event
                {
                    Id = 1,
                    Name = "Community Clean-Up Day",
                    StartDate = new DateTime(2025, 10, 22, 9, 0, 0),
                    EndDate = new DateTime(2025, 10, 22, 17, 0, 0),
                    Category = "Community",
                    Location = "Central Park, Cape Town",
                    CreatedBy = "0817246624"
                },
                new Event
                {
                    Id = 2,
                    Name = "Local Football Tournament",
                    StartDate = new DateTime(2025, 10, 29, 8, 0, 0),
                    EndDate = new DateTime(2025, 10, 31, 18, 0, 0),
                    Category = "Sports",
                    Location = "Sports Stadium, Cape Town",
                    CreatedBy = "0123456789"
                }
            );

            // Seed Announcements
            modelBuilder.Entity<Announcement>().HasData(
                new Announcement
                {
                    Id = 1,
                    Name = "Water Supply Maintenance",
                    StartDate = new DateTime(2025, 10, 13, 0, 0, 0),
                    EndDate = new DateTime(2025, 10, 20, 23, 59, 59),
                    Category = "Maintenance",
                    Location = "Northern Suburbs",
                    CreatedBy = "0817246624"
                },
                new Announcement
                {
                    Id = 2,
                    Name = "New Recycling Schedule",
                    StartDate = new DateTime(2025, 10, 15, 0, 0, 0),
                    EndDate = new DateTime(2025, 11, 15, 23, 59, 59),
                    Category = "General",
                    Location = "All areas",
                    CreatedBy = "0987654321"
                }
            );

            // Seed PulseResponses
            modelBuilder.Entity<PulseResponse>().HasData(
                new PulseResponse
                {
                    Id = 1,
                    Date = "2025-10-15",
                    UserId = "0123456789",
                    Answer = "Satisfied",
                    CreatedAt = new DateTime(2025, 10, 15, 10, 30, 0)
                },
                new PulseResponse
                {
                    Id = 2,
                    Date = "2025-10-15",
                    UserId = "0817246624",
                    Answer = "Very satisfied",
                    CreatedAt = new DateTime(2025, 10, 15, 11, 0, 0)
                }
            );

            // Seed ServiceRequests (Dependencies as serialized string)
            modelBuilder.Entity<ServiceRequest>().HasData(
                new
                {
                    RequestId = "REQ-2025-001",
                    Title = "Fix burst water pipe",
                    Description = "Major water leak at 45 Beach Road.",
                    Category = "Water",
                    Location = "45 Beach Road, Sea Point",
                    Latitude = -33.9123,
                    Longitude = 18.3876,
                    Priority = 1,
                    Status = "Pending",
                    UserId = "0123456789",
                    SubmittedDate = new DateTime(2025, 10, 12, 14, 35, 0),
                    LastUpdated = new DateTime(2025, 10, 12, 14, 35, 0),
                    EstimatedCompletion = new DateTime(2025, 10, 20),
                    Dependencies = "[\"\"]", // No dependencies
                    Notes = "Urgent: school nearby affected"
                },
                new
                {
                    RequestId = "REQ-2025-002",
                    Title = "Repair pothole",
                    Description = "Large pothole at 123 Main Street.",
                    Category = "Road",
                    Location = "123 Main Street, Cape Town",
                    Latitude = -33.9249,
                    Longitude = 18.4241,
                    Priority = 2,
                    Status = "In Progress",
                    UserId = "0817246624",
                    SubmittedDate = new DateTime(2025, 10, 10, 10, 5, 0),
                    LastUpdated = new DateTime(2025, 10, 12, 12, 0, 0),
                    EstimatedCompletion = new DateTime(2025, 10, 22),
                    Dependencies = "[\"REQ-2025-001\"]", // Depends on first request
                    Notes = string.Empty
                }
            );
        }
    }
}
