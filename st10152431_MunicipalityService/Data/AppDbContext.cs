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

            // ===== SEED TEST DATA =====
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed test users
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    CellphoneNumber = "0817246624",
                    Name = "Test User",
                    PulseDatesString = "" // Empty at start
                },
                new User
                {
                    CellphoneNumber = "0123456789",
                    Name = "John Doe",
                    PulseDatesString = ""
                },
                new User
                {
                    CellphoneNumber = "0987654321",
                    Name = "Jane Smith",
                    PulseDatesString = ""
                }
            );

            // Seed some sample issues - USE FIXED DATES
            modelBuilder.Entity<Issue>().HasData(
                new Issue
                {
                    Id = 1,
                    Location = "123 Main Street, Cape Town",
                    Category = "Road",
                    Description = "Large pothole causing traffic issues",
                    ImagePath = null,
                    UserId = "0817246624",
                    Timestamp = new DateTime(2025, 10, 10, 10, 0, 0) // Fixed date
                },
                new Issue
                {
                    Id = 2,
                    Location = "45 Beach Road, Sea Point",
                    Category = "Water",
                    Description = "Water pipe burst on sidewalk",
                    ImagePath = null,
                    UserId = "0123456789",
                    Timestamp = new DateTime(2025, 10, 12, 14, 30, 0) // Fixed date
                },
                new Issue
                {
                    Id = 3,
                    Location = "78 Park Avenue, Gardens",
                    Category = "Electricity",
                    Description = "Streetlight not working for 2 weeks",
                    ImagePath = null,
                    UserId = null, // Anonymous report
                    Timestamp = new DateTime(2025, 10, 14, 9, 15, 0) // Fixed date
                }
            );

            // Seed sample events - USE FIXED DATES
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

            // Seed sample announcements - USE FIXED DATES
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

            // Seed sample pulse responses - USE FIXED DATE
            modelBuilder.Entity<PulseResponse>().HasData(
                new PulseResponse
                {
                    Id = 1,
                    Date = "2025-10-15",
                    UserId = "0123456789",
                    Answer = "Satisfied",
                    CreatedAt = new DateTime(2025, 10, 15, 10, 30, 0) // Fixed date
                }
            );
        }
    }
}