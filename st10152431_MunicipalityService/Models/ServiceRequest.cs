using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace st10152431_MunicipalityService.Models
{
    /// <summary>
    /// Service Request Model for Part 3
    /// Represents a service request with priority, status, and dependencies
    /// 
    /// Implements IComparable for use with:
    /// - AVL Tree: Sorted by RequestId
    /// - Min-Heap: Sorted by Priority (1 = most urgent)
    /// </summary>
    public class ServiceRequest : IComparable<ServiceRequest>
    {
        /// <summary>
        /// Unique identifier for the request
        /// Format: "REQ-2024-001", "REQ-2024-002", etc.
        /// </summary>
        [Key]
        public string RequestId { get; set; }

        /// <summary>
        /// Request title/summary
        /// </summary>
        [Required]
        public string Title { get; set; }

        /// <summary>
        /// Detailed description of the issue
        /// </summary>
        [Required]
        public string Description { get; set; }

        /// <summary>
        /// Category: Water, Roads, Electricity, etc.
        /// </summary>
        [Required]
        public string Category { get; set; }

        /// <summary>
        /// Location/address where service is needed
        /// </summary>
        [Required]
        public string Location { get; set; }

        /// <summary>
        /// GPS Latitude for distance calculations
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// GPS Longitude for distance calculations
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// Priority level (1-5)
        /// 1 = Critical (immediate danger)
        /// 2 = High (major inconvenience)
        /// 3 = Medium (moderate issue)
        /// 4 = Low (minor issue)
        /// 5 = Very Low (cosmetic/suggestions)
        /// </summary>
        [Required]
        [Range(1, 5)]
        public int Priority { get; set; }

        /// <summary>
        /// Current status of the request
        /// </summary>
        [Required]
        public string Status { get; set; }

        /// <summary>
        /// User who created the request (nullable for anonymous)
        /// </summary>
        public string? UserId { get; set; }

        /// <summary>
        /// When the request was submitted
        /// </summary>
        public DateTime SubmittedDate { get; set; }

        /// <summary>
        /// When the request was last updated
        /// </summary>
        public DateTime LastUpdated { get; set; }

        /// <summary>
        /// Estimated completion date
        /// </summary>
        public DateTime? EstimatedCompletion { get; set; }

        /// <summary>
        /// Dependencies: List of RequestIds that must be completed first
        /// Used for Graph dependency tracking
        /// Example: ["REQ-2024-001"] means this request depends on REQ-2024-001
        /// </summary>
        public List<string> Dependencies { get; set; }

        /// <summary>
        /// Notes/comments from staff
        /// </summary>
        public string? Notes { get; set; }

        /// <summary>
        /// Constructor: Initialize with default values
        /// </summary>
        public ServiceRequest()
        {
            SubmittedDate = DateTime.Now;
            LastUpdated = DateTime.Now;
            Status = "Pending";
            Priority = 3; // Default to medium priority
            Dependencies = new List<string>();
        }

        /// <summary>
        /// Compare service requests for ordering
        /// 
        /// Comparison logic:
        /// 1. For AVL Tree: Compare by RequestId (alphabetical)
        /// 2. For Min-Heap: Compare by Priority (1 is highest/most urgent)
        /// 3. If priorities equal, compare by SubmittedDate (older first)
        /// 
        /// This method is used by both AVL Tree and Min-Heap
        /// </summary>
        public int CompareTo(ServiceRequest other)
        {
            if (other == null)
                return 1;

            // Primary sort: Priority (ascending: 1, 2, 3, 4, 5)
            // Lower number = higher priority = should come first
            int priorityComparison = this.Priority.CompareTo(other.Priority);

            if (priorityComparison != 0)
                return priorityComparison;

            // Secondary sort: SubmittedDate (ascending: older requests first)
            return this.SubmittedDate.CompareTo(other.SubmittedDate);
        }

        /// <summary>
        /// Alternative comparison for AVL Tree (by RequestId)
        /// Used when we want to search by ID instead of priority
        /// </summary>
        public int CompareById(ServiceRequest other)
        {
            if (other == null)
                return 1;

            return string.Compare(this.RequestId, other.RequestId, StringComparison.Ordinal);
        }

        /// <summary>
        /// Get priority label with emoji
        /// </summary>
        public string GetPriorityLabel()
        {
            return Priority switch
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
                "On Hold" => "⏸️ On Hold",
                _ => Status
            };
        }

        /// <summary>
        /// String representation for debugging
        /// </summary>
        public override string ToString()
        {
            return $"{RequestId}: {Title} [{GetPriorityLabel()}] - {Status}";
        }

        /// <summary>
        /// Get days since submission
        /// </summary>
        public int DaysSinceSubmission()
        {
            return (DateTime.Now - SubmittedDate).Days;
        }

        /// <summary>
        /// Check if request is overdue (based on priority)
        /// </summary>
        public bool IsOverdue()
        {
            if (Status == "Completed" || Status == "Cancelled")
                return false;

            int maxDays = Priority switch
            {
                1 => 1,   // Critical: 1 day
                2 => 3,   // High: 3 days
                3 => 7,   // Medium: 1 week
                4 => 14,  // Low: 2 weeks
                5 => 30,  // Very Low: 1 month
                _ => 7
            };

            return DaysSinceSubmission() > maxDays;
        }

        /// <summary>
        /// Clone method for creating copies
        /// </summary>
        public ServiceRequest Clone()
        {
            return new ServiceRequest
            {
                RequestId = this.RequestId,
                Title = this.Title,
                Description = this.Description,
                Category = this.Category,
                Location = this.Location,
                Latitude = this.Latitude,
                Longitude = this.Longitude,
                Priority = this.Priority,
                Status = this.Status,
                UserId = this.UserId,
                SubmittedDate = this.SubmittedDate,
                LastUpdated = this.LastUpdated,
                EstimatedCompletion = this.EstimatedCompletion,
                Dependencies = new List<string>(this.Dependencies),
                Notes = this.Notes
            };
        }
    }

    /// <summary>
    /// Custom comparer for AVL Tree (by RequestId)
    /// </summary>
    public class ServiceRequestIdComparer : IComparer<ServiceRequest>
    {
        public int Compare(ServiceRequest x, ServiceRequest y)
        {
            if (x == null && y == null) return 0;
            if (x == null) return -1;
            if (y == null) return 1;

            return string.Compare(x.RequestId, y.RequestId, StringComparison.Ordinal);
        }
    }
}