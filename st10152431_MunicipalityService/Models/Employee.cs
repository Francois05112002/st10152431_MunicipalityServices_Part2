using System;
using System.ComponentModel.DataAnnotations;

namespace st10152431_MunicipalityService.Models
{
    /// <summary>
    /// Employee Model
    /// Represents a municipality employee who can review and manage issues
    /// </summary>
    public class Employee
    {
        /// <summary>
        /// Employee's cellphone number (used as login ID)
        /// Primary key for employee authentication
        /// </summary>
        [Key]
        [Required]
        [StringLength(10, MinimumLength = 10)]
        public string CellphoneNumber { get; set; }

        /// <summary>
        /// Employee's full name
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Employee's role/title
        /// </summary>
        public string Role { get; set; }

        /// <summary>
        /// When employee account was created
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Number of issues reviewed by this employee
        /// </summary>
        public int IssuesReviewed { get; set; }

        /// <summary>
        /// Is this employee account active?
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Constructor: Initialize with default values
        /// </summary>
        public Employee()
        {
            CreatedDate = DateTime.Now;
            IsActive = true;
            IssuesReviewed = 0;
            Role = "Issue Reviewer";
        }

        /// <summary>
        /// Check if cellphone number is valid employee
        /// Static method for quick validation
        /// </summary>
        public static bool IsEmployeeNumber(string cellphone)
        {
            // Currently hardcoded check
            // In production, would check database
            return cellphone == "1111111111";
        }
    }
}