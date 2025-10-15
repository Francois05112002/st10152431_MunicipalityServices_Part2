using System.Collections.Generic;

namespace st10152431_MunicipalityService.Models
{
    /// <summary>
    /// Daily pulse question model
    /// Responses stored in DICTIONARY (by date, then by user) in ReportService
    /// </summary>
    public class DailyPulse
    {
        public string Date { get; set; }
        public string Question { get; set; }

        // LIST: Maintains order of choices for display
        public List<string> Choices { get; set; }

        public DailyPulse()
        {
            Choices = new List<string>();
        }

        public DailyPulse(string date, string question, List<string> choices)
        {
            Date = date;
            Question = question;
            Choices = choices;
        }
    }
}
