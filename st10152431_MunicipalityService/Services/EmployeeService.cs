using System;
using System.Collections.Generic;
using System.Linq;
using st10152431_MunicipalityService.Data;
using st10152431_MunicipalityService.Models;

namespace st10152431_MunicipalityService.Services
{
    /// <summary>
    /// Employee Service - Manages employee authentication and issue management
    /// </summary>
    public class EmployeeService
    {
        private readonly AppDbContext _context;

        // Employee credentials (in production, use proper authentication)
        private static readonly List<string> _employeeCellphones = new List<string>
        {
            "1111111111"  // Default employee login
        };

        public EmployeeService(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Check if cellphone number belongs to an employee
        /// </summary>
        public bool IsEmployee(string cellphone)
        {
            if (string.IsNullOrEmpty(cellphone))
                return false;

            return _employeeCellphones.Contains(cellphone);
        }

        /// <summary>
        /// Get all issues for employee review
        /// Ordered by: Not reviewed first, then by priority, then by date
        /// </summary>
        public List<Issue> GetAllIssuesForReview()
        {
            return _context.Issues
                .OrderBy(i => i.Priority.HasValue ? 1 : 0)  // Not reviewed first
                .ThenBy(i => i.Priority ?? 99)              // Then by priority
                .ThenBy(i => i.Timestamp)                   // Then by date (oldest first)
                .ToList();
        }

        /// <summary>
        /// Get issues that haven't been reviewed yet (no priority assigned)
        /// </summary>
        public List<Issue> GetUnreviewedIssues()
        {
            return _context.Issues
                .Where(i => !i.Priority.HasValue)
                .OrderBy(i => i.Timestamp)
                .ToList();
        }

        /// <summary>
        /// Get issues by priority level
        /// </summary>
        public List<Issue> GetIssuesByPriority(int priority)
        {
            return _context.Issues
                .Where(i => i.Priority == priority)
                .OrderBy(i => i.Timestamp)
                .ToList();
        }

        /// <summary>
        /// Get issues by status
        /// </summary>
        public List<Issue> GetIssuesByStatus(string status)
        {
            return _context.Issues
                .Where(i => i.Status == status)
                .OrderBy(i => i.Timestamp)
                .ToList();
        }

        /// <summary>
        /// Get overdue issues
        /// </summary>
        public List<Issue> GetOverdueIssues()
        {
            var today = DateTime.Now;
            return _context.Issues
                .Where(i => i.DueDate.HasValue &&
                           i.DueDate.Value < today &&
                           i.Status != "Completed" &&
                           i.Status != "Cancelled")
                .OrderBy(i => i.DueDate)
                .ToList();
        }

        /// <summary>
        /// Update issue details (priority, status, due date)
        /// </summary>
        public bool UpdateIssue(int issueId, int? priority, string status,
                               DateTime? dueDate, string employeeCellphone)
        {
            var issue = _context.Issues.Find(issueId);

            if (issue == null)
                return false;

            // Update fields
            if (priority.HasValue)
                issue.Priority = priority.Value;

            if (!string.IsNullOrEmpty(status))
                issue.Status = status;

            if (dueDate.HasValue)
                issue.DueDate = dueDate.Value;

            // Track who reviewed and when
            issue.LastReviewedDate = DateTime.Now;
            issue.ReviewedBy = employeeCellphone;

            _context.SaveChanges();
            return true;
        }

        /// <summary>
        /// Bulk update status for multiple issues
        /// </summary>
        public int BulkUpdateStatus(List<int> issueIds, string newStatus, string employeeCellphone)
        {
            int updated = 0;

            foreach (int id in issueIds)
            {
                var issue = _context.Issues.Find(id);
                if (issue != null)
                {
                    issue.Status = newStatus;
                    issue.LastReviewedDate = DateTime.Now;
                    issue.ReviewedBy = employeeCellphone;
                    updated++;
                }
            }

            _context.SaveChanges();
            return updated;
        }

        /// <summary>
        /// Get statistics for employee dashboard
        /// </summary>
        public (int total, int unreviewed, int pending, int inProgress, int completed, int overdue)
            GetStatistics()
        {
            var total = _context.Issues.Count();
            var unreviewed = _context.Issues.Count(i => !i.Priority.HasValue);
            var pending = _context.Issues.Count(i => i.Status == "Pending");
            var inProgress = _context.Issues.Count(i => i.Status == "In Progress");
            var completed = _context.Issues.Count(i => i.Status == "Completed");

            var today = DateTime.Now;
            var overdue = _context.Issues.Count(i =>
                i.DueDate.HasValue &&
                i.DueDate.Value < today &&
                i.Status != "Completed" &&
                i.Status != "Cancelled"
            );

            return (total, unreviewed, pending, inProgress, completed, overdue);
        }

        /// <summary>
        /// Get available statuses for dropdown
        /// </summary>
        public List<string> GetStatuses()
        {
            return new List<string>
            {
                "Pending",
                "Assigned",
                "In Progress",
                "Completed",
                "Cancelled"
            };
        }

        /// <summary>
        /// Get priority levels with labels
        /// </summary>
        public Dictionary<int, string> GetPriorityLevels()
        {
            return new Dictionary<int, string>
            {
                { 1, "🔴 Critical" },
                { 2, "🟠 High" },
                { 3, "🟡 Medium" },
                { 4, "🟢 Low" },
                { 5, "⚪ Very Low" }
            };
        }
    }
}