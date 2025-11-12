using System;
using System.ComponentModel.DataAnnotations;

namespace st10152431_MunicipalityService.Models
{
    /// <summary>
    /// Issue Model - Updated for Part 3
    /// Now includes Priority, Status, and DueDate that employees can manage
    /// </summary>
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

        // ===== NEW FIELDS FOR EMPLOYEE MANAGEMENT =====

        /// <summary>
        /// Priority level (1-5) assigned by employee
        /// 1 = Critical, 2 = High, 3 = Medium, 4 = Low, 5 = Very Low
        /// Default: null (not yet reviewed by employee)
        /// </summary>
        public int? Priority { get; set; }

        /// <summary>
        /// Current status of the issue
        /// Options: "Pending", "Assigned", "In Progress", "Completed", "Cancelled"
        /// Default: "Pending"
        /// </summary>
        [Required]
        public string Status { get; set; }

        /// <summary>
        /// Due date set by employee - when issue should be resolved
        /// Default: null (not yet assigned)
        /// </summary>
        public DateTime? DueDate { get; set; }

        /// <summary>
        /// When employee last reviewed/updated this issue
        /// </summary>
        public DateTime? LastReviewedDate { get; set; }

        /// <summary>
        /// Which employee reviewed this (cellphone number)
        /// </summary>
        public string? ReviewedBy { get; set; }

        /// <summary>
        /// Constructor: Initialize with default values
        /// </summary>
        public Issue()
        {
            Timestamp = DateTime.Now;
            Status = "Pending";  // Default status for new issues
            Priority = null;     // Not yet reviewed
            DueDate = null;      // Not yet assigned
        }

        /// <summary>
        /// Get priority label with color/emoji
        /// </summary>
        public string GetPriorityLabel()
        {
            if (!Priority.HasValue)
                return "⚪ Not Reviewed";

            return Priority.Value switch
            {
                1 => "🔴 Critical",
                2 => "🟠 High",
                3 => "🟡 Medium",
                4 => "🟢 Low",
                5 => "⚪ Very Low",
                _ => "❓ Unknown"
            };
        }

        /// <summary>
        /// Get status with emoji
        /// </summary>
        public string GetStatusLabel()
        {
            return Status switch
            {
                "Pending" => "⏳ Pending",
                "Assigned" => "👷 Assigned",
                "In Progress" => "🔧 In Progress",
                "Completed" => "✅ Completed",
                "Cancelled" => "❌ Cancelled",
                _ => Status
            };
        }

        /// <summary>
        /// Check if issue is overdue based on due date
        /// </summary>
        public bool IsOverdue()
        {
            if (!DueDate.HasValue || Status == "Completed" || Status == "Cancelled")
                return false;

            return DateTime.Now > DueDate.Value;
        }

        /// <summary>
        /// Get days until due (negative if overdue)
        /// </summary>
        public int? GetDaysUntilDue()
        {
            if (!DueDate.HasValue)
                return null;

            return (DueDate.Value - DateTime.Now).Days;
        }

        /// <summary>
        /// Get CSS class for priority badge
        /// </summary>
        public string GetPriorityBadgeClass()
        {
            if (!Priority.HasValue)
                return "badge-secondary";

            return Priority.Value switch
            {
                1 => "badge-critical",
                2 => "badge-high",
                3 => "badge-medium",
                4 => "badge-low",
                5 => "badge-verylow",
                _ => "badge-secondary"
            };
        }

        /// <summary>
        /// Check if issue needs review (no priority assigned yet)
        /// </summary>
        public bool NeedsReview()
        {
            return !Priority.HasValue;
        }
    }
}
