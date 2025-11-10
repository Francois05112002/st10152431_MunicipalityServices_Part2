using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace st10152431_MunicipalityService.Models
{
    public class User
    {
        public string Name { get; set; }
        public string CellphoneNumber { get; set; }

        // LIST: Issues collection (EF Core will create a foreign key relationship)
        public List<Issue> Issues { get; set; }

        // HASHSET: Pulse dates - stored as string in database, converted to HashSet in code
        [NotMapped] // This property won't be stored directly
        public HashSet<string> PulseDates { get; set; }

        // This property stores the HashSet as a delimited string in the database
        public string PulseDatesString
        {
            get => PulseDates != null ? string.Join(",", PulseDates) : "";
            set => PulseDates = !string.IsNullOrEmpty(value)
                ? new HashSet<string>(value.Split(',', StringSplitOptions.RemoveEmptyEntries))
                : new HashSet<string>();
        }

        // Computed properties
        [NotMapped]
        public int IssuesReported => Issues?.Count ?? 0;

        [NotMapped]
        public int DailyPulsesCompleted => PulseDates?.Count ?? 0;

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
