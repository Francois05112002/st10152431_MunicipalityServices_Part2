using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using st10152431_MunicipalityService.Models;
using st10152431_MunicipalityService.Services;

namespace st10152431_MunicipalityService.Pages
{
    /// <summary>
    /// User Status Page Model
    /// Shows only issues reported by the logged-in user
    /// Allows searching by Issue ID using a Dictionary for O(1) lookup
    /// </summary>
    public class UserStatusModel : PageModel
    {
        private readonly ReportService _reportService;
        private readonly UserService _userService;
        private readonly EmployeeService _employeeService;

        public UserStatusModel(ReportService reportService,
                              UserService userService,
                              EmployeeService employeeService)
        {
            _reportService = reportService;
            _userService = userService;
            _employeeService = employeeService;
        }

        // ===== PROPERTIES =====

        /// <summary>
        /// Current user's phone number
        /// </summary>
        public string UserPhone { get; set; }

        /// <summary>
        /// All issues reported by this user
        /// </summary>
        public List<Issue> UserIssues { get; set; } = new List<Issue>();

        /// <summary>
        /// Dictionary for fast lookup by Issue ID
        /// </summary>
        public Dictionary<int, Issue> IssueDictionary { get; set; } = new Dictionary<int, Issue>();

        /// <summary>
        /// Issues to display (filtered by search if applicable)
        /// </summary>
        public List<Issue> DisplayIssues { get; set; } = new List<Issue>();

        /// <summary>
        /// Search ID entered by user
        /// </summary>
        [BindProperty]
        public int? SearchId { get; set; }

        /// <summary>
        /// Search result (if found)
        /// </summary>
        public Issue SearchResult { get; set; }

        // ===== HANDLERS =====

        /// <summary>
        /// OnGet: Load user's issues and build dictionary
        /// </summary>
        public IActionResult OnGet()
        {
            UserPhone = HttpContext.Session.GetString("UserPhone");

            if (string.IsNullOrEmpty(UserPhone))
            {
                // Not logged in, redirect to register/login
                return RedirectToPage("/Register");
            }

            // Check if employee trying to access user page
            if (_employeeService.IsEmployee(UserPhone))
            {
                // Redirect employees to their dedicated page
                return RedirectToPage("/EmployeeStatus");
            }

            // Load user's issues
            UserIssues = _reportService.GetIssuesByUser(UserPhone);

            // Build dictionary for fast lookup
            IssueDictionary = UserIssues.ToDictionary(i => i.Id);

            // Ensure DisplayIssues is set for the initial page load
            DisplayIssues = UserIssues.OrderByDescending(i => i.Timestamp).ToList();

            return Page();
        }

        /// <summary>
        /// OnPostSearch: Search for specific issue by ID using dictionary
        /// </summary>
        public IActionResult OnPostSearch(int? searchId)
        {
            UserPhone = HttpContext.Session.GetString("UserPhone");

            if (string.IsNullOrEmpty(UserPhone))
            {
                return RedirectToPage("/Register");
            }

            if (_employeeService.IsEmployee(UserPhone))
            {
                return RedirectToPage("/EmployeeStatus");
            }

            SearchId = searchId;

            // Load user's issues and build dictionary
            UserIssues = _reportService.GetIssuesByUser(UserPhone);
            IssueDictionary = UserIssues.ToDictionary(i => i.Id);

            // Search within user's issues using dictionary
            if (SearchId.HasValue && IssueDictionary.TryGetValue(SearchId.Value, out var foundIssue))
            {
                SearchResult = foundIssue;
                DisplayIssues = new List<Issue> { foundIssue };
            }
            else if (SearchId.HasValue)
            {
                SearchResult = null;
                DisplayIssues = new List<Issue>();
            }
            else
            {
                // If no search, show all issues
                DisplayIssues = UserIssues.OrderByDescending(i => i.Timestamp).ToList();
            }

            return Page();
        }
    }
}

