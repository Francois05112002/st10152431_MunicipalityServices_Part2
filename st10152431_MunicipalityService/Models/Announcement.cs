using System;
using System.ComponentModel.DataAnnotations;

namespace st10152431_MunicipalityService.Models
{
    public class Announcement : IComparable<Announcement>
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Announcement name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Start date is required")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End date is required")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public string Category { get; set; }

        [Required(ErrorMessage = "Location is required")]
        public string Location { get; set; }

        // NO [Required] attribute here - set programmatically
        public string CreatedBy { get; set; }

        public Announcement()
        {
            StartDate = DateTime.Today;
            EndDate = DateTime.Today;
        }

        public int CompareTo(Announcement? other)
        {
            if (other == null) return 1;
            return this.StartDate.CompareTo(other.StartDate);
        }
    }
}
