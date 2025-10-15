using System.Collections.Generic;

namespace st10152431_MunicipalityService.Models
{
    /// <summary>
    /// User model representing a registered user
    /// Uses DICTIONARY for fast lookup by phone number in UserService
    /// </summary>
    public class User
    {
        public string Name { get; set; }
        public string CellphoneNumber { get; set; }

        // LIST: Preserves chronological order of issues reported by this user
        public List<Issue> Issues { get; set; }

        // HASHSET: Prevents duplicate pulse participation per day, O(1) lookup
        // Stores dates in "yyyy-MM-dd" format
        public HashSet<string> PulseDates { get; set; }

        // Computed properties for easy display
        public int IssuesReported => Issues.Count;
        public int DailyPulsesCompleted => PulseDates.Count;

        public User()
        {
            Issues = new List<Issue>();
            PulseDates = new HashSet<string>();
        }

        public User(string name, string cellphoneNumber)
        {
            Name = name;
            CellphoneNumber = cellphoneNumber;
            Issues = new List<Issue>();
            PulseDates = new HashSet<string>();
        }
    }
}
