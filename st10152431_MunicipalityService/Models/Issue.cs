using System;
using System.ComponentModel.DataAnnotations;

namespace st10152431_MunicipalityService.Models
{
    public class Issue
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Location { get; set; }

        [Required]
        public string Category { get; set; }

        [Required]
        public string Description { get; set; }

        public string? ImagePath { get; set; }

        public DateTime Timestamp { get; set; }

        // Nullable - for anonymous reports
        public string? UserId { get; set; }

        public Issue()
        {
            Timestamp = DateTime.Now;
        }
    }
}
