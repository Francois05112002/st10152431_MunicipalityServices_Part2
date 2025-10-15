using System;
using System.ComponentModel.DataAnnotations;

namespace st10152431_MunicipalityService.Models
{
    public class PulseResponse
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Date { get; set; } // "yyyy-MM-dd" format

        [Required]
        public string UserId { get; set; } // Phone number

        [Required]
        public string Answer { get; set; }

        public DateTime CreatedAt { get; set; }

        public PulseResponse()
        {
            CreatedAt = DateTime.Now;
        }
    }
}