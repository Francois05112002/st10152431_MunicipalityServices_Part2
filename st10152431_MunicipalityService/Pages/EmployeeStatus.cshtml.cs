using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using st10152431_MunicipalityService.Models;
using st10152431_MunicipalityService.Services;

namespace st10152431_MunicipalityService.Pages
{
    /// <summary>
    /// Employee Status Page Model
    /// Allows employees to view and manage all reported issues
    /// Can assign priority, status, and due date
    /// NOW WITH: Data structures integration (Min-Heap + AVL Tree)
    /// </summary>
    public class EmployeeStatusModel : PageModel
    {
        private readonly EmployeeService _employeeService;
        private readonly ReportService _reportService;
        private readonly IssueDataStructures _dataStructures;  // ✅ Field declaration

        // ✅ CORRECTED CONSTRUCTOR: Added IssueDataStructures parameter
        public EmployeeStatusModel(EmployeeService employeeService,
                                  ReportService reportService,
                                  IssueDataStructures dataStructures)
        {
            _employeeService = employeeService;
            _reportService = reportService;
            _dataStructures = dataStructures;  // ✅ Initialize the field
        }

        // ===== EXISTING PROPERTIES =====

        /// <summary>
        /// Employee phone number
        /// </summary>
        public string EmployeePhone { get; set; }

        /// <summary>
        /// All issues (before filtering)
        /// </summary>
        public List<Issue> AllIssues { get; set; } = new List<Issue>();

        /// <summary>
        /// Filtered issues to display
        /// </summary>
        public List<Issue> FilteredIssues { get; set; } = new List<Issue>();

        /// <summary>
        /// Current filter type
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public string FilterType { get; set; }

        /// <summary>
        /// Success message after update
        /// </summary>
        [TempData]
        public string SuccessMessage { get; set; }

        // ===== EXISTING STATISTICS =====

        public int TotalIssues { get; set; }
        public int UnreviewedIssues { get; set; }
        public int PendingIssues { get; set; }
        public int InProgressIssues { get; set; }
        public int CompletedIssues { get; set; }
        public int OverdueIssues { get; set; }

        // ===== NEW PROPERTIES FOR DATA STRUCTURES =====

        /// <summary>
        /// Top urgent issues from Min-Heap
        /// </summary>
        public List<Issue> TopUrgentIssues { get; set; } = new List<Issue>();

        /// <summary>
        /// Result from AVL Tree fast search
        /// </summary>
        public Issue? FastSearchResult { get; set; }

        /// <summary>
        /// Search ID for binding
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public int? SearchId { get; set; }

        /// <summary>
        /// Performance statistics from data structures
        /// </summary>
        public DataStructureStats? Stats { get; set; }

        // ===== HANDLERS =====

        /// <summary>
        /// OnGet: Load all issues for employee review
        /// </summary>
        public IActionResult OnGet()
        {
            // Get current employee phone from session
            EmployeePhone = HttpContext.Session.GetString("EmployeePhone");

            if (string.IsNullOrEmpty(EmployeePhone))
            {
                // Not logged in as employee
                return RedirectToPage("/Register");
            }

            // Check if user is actually an employee
            if (!_employeeService.IsEmployee(EmployeePhone))
            {
                // Not a valid employee, redirect to user status or register
                return RedirectToPage("/Register");
            }

            // Load issues and statistics
            LoadIssuesAndStats();

            // ✅ Load data structure features
            LoadDataStructureFeatures();

            return Page();
        }

        /// <summary>
        /// OnPostUpdateIssue: Update a single issue's priority, status, and due date
        /// </summary>
        public IActionResult OnPostUpdateIssue(int issueId, int? priority,
                                              string status, DateTime? dueDate)
        {
            // Get current employee phone from session
            EmployeePhone = HttpContext.Session.GetString("EmployeePhone");

            if (string.IsNullOrEmpty(EmployeePhone) ||
                !_employeeService.IsEmployee(EmployeePhone))
            {
                return RedirectToPage("/Register");
            }

            // Validate inputs
            if (!priority.HasValue || string.IsNullOrEmpty(status))
            {
                TempData["ErrorMessage"] = "Priority and Status are required fields.";
                return RedirectToPage();
            }

            if (priority.Value < 1 || priority.Value > 5)
            {
                TempData["ErrorMessage"] = "Priority must be between 1 and 5.";
                return RedirectToPage();
            }

            // Update issue
            bool success = _employeeService.UpdateIssue(
                issueId,
                priority.Value,
                status,
                dueDate,
                EmployeePhone
            );

            if (success)
            {
                SuccessMessage = $"Issue #{issueId} has been updated successfully!";

                // ✅ Refresh data structures after update
                _dataStructures.ForceRefresh();
            }
            else
            {
                TempData["ErrorMessage"] = $"Failed to update Issue #{issueId}.";
            }

            // Reload page with same filter
            return RedirectToPage(new { filter = FilterType });
        }

        /// <summary>
        /// ✅ NEW HANDLER: Fast search using AVL Tree
        /// </summary>
        public IActionResult OnPostFastSearch(int searchId)
        {
            // Auth check
            EmployeePhone = HttpContext.Session.GetString("EmployeePhone");

            if (string.IsNullOrEmpty(EmployeePhone) ||
                !_employeeService.IsEmployee(EmployeePhone))
            {
                return RedirectToPage("/Register");
            }

            // Store search ID
            SearchId = searchId;

            // ✨ Fast search using AVL Tree - O(log n)
            FastSearchResult = _dataStructures.FastSearch(searchId);

            if (FastSearchResult != null)
            {
                SuccessMessage = $"✅ Found Issue #{searchId} in O(log n) time using AVL Tree!";
            }
            else
            {
                SuccessMessage = $"⚠️ Issue #{searchId} not found.";
            }

            // Load other data
            LoadIssuesAndStats();
            LoadDataStructureFeatures();

            return Page();
        }

        /// <summary>
        /// ✅ NEW HANDLER: Process most urgent issue from Min-Heap
        /// </summary>
        public IActionResult OnPostProcessUrgent()
        {
            // Auth check
            EmployeePhone = HttpContext.Session.GetString("EmployeePhone");

            if (string.IsNullOrEmpty(EmployeePhone) ||
                !_employeeService.IsEmployee(EmployeePhone))
            {
                return RedirectToPage("/Register");
            }

            // ✨ Extract most urgent from Min-Heap - O(log n)
            var urgentIssue = _dataStructures.ProcessMostUrgent();

            if (urgentIssue != null)
            {
                SuccessMessage = $"✅ Processing Issue #{urgentIssue.Id} (extracted from Min-Heap as most urgent)";
            }
            else
            {
                SuccessMessage = "No urgent issues in queue.";
            }

            return RedirectToPage();
        }

        // ===== HELPER METHODS =====

        /// <summary>
        /// Load all issues and calculate statistics
        /// </summary>
        private void LoadIssuesAndStats()
        {
            // Get statistics
            var stats = _employeeService.GetStatistics();
            TotalIssues = stats.total;
            UnreviewedIssues = stats.unreviewed;
            PendingIssues = stats.pending;
            InProgressIssues = stats.inProgress;
            CompletedIssues = stats.completed;
            OverdueIssues = stats.overdue;

            // Load issues based on filter
            switch (FilterType?.ToLower())
            {
                case "unreviewed":
                    FilteredIssues = _employeeService.GetUnreviewedIssues();
                    break;

                case "pending":
                    FilteredIssues = _employeeService.GetIssuesByStatus("Pending");
                    break;

                case "inprogress":
                    FilteredIssues = _employeeService.GetIssuesByStatus("In Progress");
                    break;

                case "completed":
                    FilteredIssues = _employeeService.GetIssuesByStatus("Completed");
                    break;

                case "overdue":
                    FilteredIssues = _employeeService.GetOverdueIssues();
                    break;

                default:
                    // No filter - show all issues
                    FilteredIssues = _employeeService.GetAllIssuesForReview();
                    break;
            }

            AllIssues = _employeeService.GetAllIssuesForReview();
        }

        /// <summary>
        /// ✅ NEW METHOD: Load data structure features
        /// </summary>
        private void LoadDataStructureFeatures()
        {
            // Get top 5 urgent issues from Min-Heap
            TopUrgentIssues = _dataStructures.GetTopUrgent(5);

            // Get performance statistics
            Stats = _dataStructures.GetStatistics();
        }
    }
}
