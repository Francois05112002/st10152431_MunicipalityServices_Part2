using System;
using System.ComponentModel.DataAnnotations;

namespace st10152431_MunicipalityService.Models
{
    /// <summary>
    /// Issue model representing a reported problem
    /// Stored in LIST in ReportService for chronological order
    /// </summary>
    public class Issue
    {
        public int Id { get; set; }

        [Required]
        public string Location { get; set; }

        [Required]
        public string Category { get; set; }

        [Required]
        public string Description { get; set; }

        public string ImagePath { get; set; }

        public DateTime Timestamp { get; set; }

        // Cellphone number of user who reported, or null if anonymous
        public string UserId { get; set; }

        public Issue()
        {
            Timestamp = DateTime.Now;
        }

        public Issue(int id, string location, string category, string description,
                    string imagePath = null, string userId = null)
        {
            Id = id;
            Location = location;
            Category = category;
            Description = description;
            ImagePath = imagePath;
            UserId = userId;
            Timestamp = DateTime.Now;
        }
    }
}
