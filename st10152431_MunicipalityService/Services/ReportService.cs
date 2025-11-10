using System;
using System.Collections.Generic;
using System.Linq;
using st10152431_MunicipalityService.Data;
using st10152431_MunicipalityService.Models;

namespace st10152431_MunicipalityService.Services
{
    public class ReportService
    {
        private readonly AppDbContext _context;

        // LIST: Fixed category options for the dropdown
        private static readonly List<string> _categories = new List<string>
        {
            "Road",
            "Water",
            "Electricity",
            "Maintenance",
            "Finance",
            "Other"
        };

        // Current daily pulse question (could be made dynamic/database-driven later)
        private static readonly DailyPulse _currentPulse = new DailyPulse(
            DateTime.Now.ToString("yyyy-MM-dd"),
            "How satisfied are you with local road maintenance?",
            new List<string> { "Very satisfied", "Satisfied", "Neutral", "Dissatisfied" }
        );

        public ReportService(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all issue categories
        /// LIST: Returns fixed list of categories
        /// </summary>
        public List<string> GetCategories()
        {
            return _categories;
        }

        /// <summary>
        /// Add a new issue to the system
        /// Anyone can report (logged in or anonymous)
        /// LIST pattern: O(1) append operation
        /// </summary>
        public int AddIssue(string location, string category, string description,
                           string imagePath, string userId = null)
        {
            var issue = new Issue
            {
                Location = location,
                Category = category,
                Description = description,
                ImagePath = imagePath,
                UserId = userId,  // null if anonymous
                Timestamp = DateTime.Now
            };

            // LIST append is O(1) amortized
            _context.Issues.Add(issue);
            _context.SaveChanges();

            return issue.Id;
        }

        /// <summary>
        /// Get all issues (for admin/viewing purposes)
        /// Returns as LIST maintaining chronological order
        /// </summary>
        public List<Issue> GetAllIssues()
        {
            return _context.Issues.OrderBy(i => i.Timestamp).ToList();
        }

        /// <summary>
        /// Get issues by user
        /// LIST pattern: Filter and return as list
        /// </summary>
        public List<Issue> GetIssuesByUser(string userId)
        {
            return _context.Issues
                .Where(i => i.UserId == userId)
                .OrderBy(i => i.Timestamp)
                .ToList();
        }

        /// <summary>
        /// Get issues by category
        /// LIST pattern: Filter and return as list
        /// </summary>
        public List<Issue> GetIssuesByCategory(string category)
        {
            return _context.Issues
                .Where(i => i.Category == category)
                .OrderBy(i => i.Timestamp)
                .ToList();
        }

        /// <summary>
        /// Get total number of issues reported
        /// </summary>
        public int GetTotalIssuesCount()
        {
            return _context.Issues.Count();
        }

        // ===== DAILY PULSE METHODS =====

        /// <summary>
        /// Get today's pulse question
        /// </summary>
        public DailyPulse GetTodayPulse()
        {
            return _currentPulse;
        }

        /// <summary>
        /// Check if user has already answered today's pulse
        /// O(1) lookup using compound index (Date + UserId)
        /// </summary>
        public bool HasAnsweredToday(string userId, string date)
        {
            if (string.IsNullOrEmpty(userId))
                return false;

            // O(1) lookup using indexed columns
            return _context.PulseResponses
                .Any(p => p.Date == date && p.UserId == userId);
        }

        /// <summary>
        /// Get user's answer for today (if they answered)
        /// </summary>
        public string GetUserAnswerToday(string userId, string date)
        {
            var response = _context.PulseResponses
                .FirstOrDefault(p => p.Date == date && p.UserId == userId);

            return response?.Answer;
        }

        /// <summary>
        /// Submit pulse answer
        /// Returns true if successful, false if already answered
        /// Uses database constraint to prevent duplicates
        /// </summary>
        public bool SubmitPulseAnswer(string userId, string date, string answer)
        {
            if (string.IsNullOrEmpty(userId))
                return false;

            // Check if already answered today (O(1))
            if (HasAnsweredToday(userId, date))
                return false;

            var response = new PulseResponse
            {
                Date = date,
                UserId = userId,
                Answer = answer,
                CreatedAt = DateTime.Now
            };

            try
            {
                // O(1) insertion
                _context.PulseResponses.Add(response);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                // Unique constraint violation (user already answered)
                return false;
            }
        }

        /// <summary>
        /// Get all responses for a specific date (for analytics)
        /// Returns as DICTIONARY for O(1) lookups
        /// </summary>
        public Dictionary<string, string> GetResponsesForDate(string date)
        {
            return _context.PulseResponses
                .Where(p => p.Date == date)
                .ToDictionary(p => p.UserId, p => p.Answer);
        }

        /// <summary>
        /// Get pulse response statistics for today
        /// Returns count of each choice as DICTIONARY
        /// </summary>
        public Dictionary<string, int> GetTodayPulseStats(string date)
        {
            return _context.PulseResponses
                .Where(p => p.Date == date)
                .GroupBy(p => p.Answer)
                .ToDictionary(g => g.Key, g => g.Count());
        }
    }
}

