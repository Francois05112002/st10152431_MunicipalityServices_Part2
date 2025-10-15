using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace st10152431_MunicipalityService.Models
{
    /// <summary>
    /// Input model for the Report form
    /// Used for binding form data in Report.cshtml
    /// </summary>
    public class ReportInput
    {
        [Required(ErrorMessage = "Location is required")]
        public string Location { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public string Category { get; set; }

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }

        // Optional image upload
        public IFormFile Image { get; set; }
    }
}