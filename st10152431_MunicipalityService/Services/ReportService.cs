using System;
using System.Collections.Generic;
using System.Linq;
using st10152431_MunicipalityService.Models;

namespace st10152431_MunicipalityService.Services
{
    public class ReportService
    {
        // LIST: Stores all issues chronologically (anyone can report, logged in or not)
        // O(1) append operation
        private static List<Issue> _allIssues = new List<Issue>();

        // Counter for unique issue IDs
        private static int _issueCounter = 0;

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

        // DICTIONARY: Stores pulse responses organized by date, then by user phone
        // Structure: { "2025-10-14" -> { "0817246624" -> "Satisfied", "0823456789" -> "Neutral" } }
        // O(1) lookup to check if user answered today
        private static Dictionary<string, Dictionary<string, string>> _pulseResponses =
            new Dictionary<string, Dictionary<string, string>>();

        // Current daily pulse question
        private static DailyPulse _currentPulse = new DailyPulse(
            DateTime.Now.ToString("yyyy-MM-dd"),
            "How satisfied are you with local road maintenance?",
            new List<string> { "Very satisfied", "Satisfied", "Neutral", "Dissatisfied" }
        );

        /// <summary>
        /// Get all issue categories
        /// </summary>
        public List<string> GetCategories()
        {
            return _categories;
        }

        /// <summary>
        /// Add a new issue to the system
        /// Anyone can report (logged in or anonymous)
        /// </summary>
        public int AddIssue(string location, string category, string description,
                           string imagePath, string userId = null)
        {
            _issueCounter++;

            var issue = new Issue
            {
                Id = _issueCounter,
                Location = location,
                Category = category,
                Description = description,
                ImagePath = imagePath,
                UserId = userId,  // null if anonymous
                Timestamp = DateTime.Now
            };

            // LIST append is O(1) amortized
            _allIssues.Add(issue);

            return issue.Id;
        }

        /// <summary>
        /// Get all issues (for admin/viewing purposes)
        /// </summary>
        public List<Issue> GetAllIssues()
        {
            return _allIssues;
        }

        /// <summary>
        /// Get issues by user
        /// </summary>
        public List<Issue> GetIssuesByUser(string userId)
        {
            // Filter the list to get user's issues
            return _allIssues.Where(i => i.UserId == userId).ToList();
        }

        /// <summary>
        /// Get issues by category
        /// </summary>
        public List<Issue> GetIssuesByCategory(string category)
        {
            return _allIssues.Where(i => i.Category == category).ToList();
        }

        /// <summary>
        /// Get total number of issues reported
        /// </summary>
        public int GetTotalIssuesCount()
        {
            return _allIssues.Count;
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
        /// O(1) lookup using nested dictionaries
        /// </summary>
        public bool HasAnsweredToday(string userId, string date)
        {
            if (string.IsNullOrEmpty(userId))
                return false;

            // Check if date exists in responses (O(1))
            if (_pulseResponses.ContainsKey(date))
            {
                // Check if user answered on this date (O(1))
                return _pulseResponses[date].ContainsKey(userId);
            }

            return false;
        }

        /// <summary>
        /// Get user's answer for today (if they answered)
        /// </summary>
        public string GetUserAnswerToday(string userId, string date)
        {
            if (_pulseResponses.ContainsKey(date) &&
                _pulseResponses[date].ContainsKey(userId))
            {
                return _pulseResponses[date][userId];
            }

            return null;
        }

        /// <summary>
        /// Submit pulse answer
        /// Returns true if successful, false if already answered
        /// </summary>
        public bool SubmitPulseAnswer(string userId, string date, string answer)
        {
            if (string.IsNullOrEmpty(userId))
                return false;

            // Check if already answered today (O(1))
            if (HasAnsweredToday(userId, date))
                return false;

            // Initialize date dictionary if it doesn't exist
            if (!_pulseResponses.ContainsKey(date))
            {
                _pulseResponses[date] = new Dictionary<string, string>();
            }

            // Store the answer (O(1) insertion)
            _pulseResponses[date][userId] = answer;

            return true;
        }

        /// <summary>
        /// Get all responses for a specific date (for analytics)
        /// </summary>
        public Dictionary<string, string> GetResponsesForDate(string date)
        {
            if (_pulseResponses.ContainsKey(date))
            {
                return _pulseResponses[date];
            }

            return new Dictionary<string, string>();
        }

        /// <summary>
        /// Get pulse response statistics for today
        /// Returns count of each choice
        /// </summary>
        public Dictionary<string, int> GetTodayPulseStats(string date)
        {
            var stats = new Dictionary<string, int>();

            if (_pulseResponses.ContainsKey(date))
            {
                var responses = _pulseResponses[date];

                // Count each answer choice
                foreach (var response in responses.Values)
                {
                    if (stats.ContainsKey(response))
                        stats[response]++;
                    else
                        stats[response] = 1;
                }
            }

            return stats;
        }

        /// <summary>
        /// Update daily pulse question (for admin)
        /// </summary>
        public void UpdateDailyPulse(string question, List<string> choices)
        {
            string today = DateTime.Now.ToString("yyyy-MM-dd");
            _currentPulse = new DailyPulse(today, question, choices);
        }
    }
}

