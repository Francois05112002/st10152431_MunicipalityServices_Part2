using System;

namespace st10152431_MunicipalityService.Models
{
    /// <summary>
    /// Node for Min-Heap Priority Queue
    /// Used to order issues by priority and date
    /// </summary>
    public class IssuePriorityNode : IComparable<IssuePriorityNode>
    {
        /// <summary>
        /// Issue ID reference
        /// </summary>
        public int IssueId { get; set; }

        /// <summary>
        /// Priority level (1 = most urgent, 5 = least urgent)
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// When issue was submitted
        /// </summary>
        public DateTime SubmittedDate { get; set; }

        /// <summary>
        /// Compare to another node for heap ordering
        /// PRIMARY: Priority (lower number = higher priority)
        /// SECONDARY: Date (older = higher priority)
        /// </summary>
        public int CompareTo(IssuePriorityNode? other)
        {
            if (other == null) return 1;

            // First compare by priority (1 is most urgent)
            int priorityComparison = Priority.CompareTo(other.Priority);
            if (priorityComparison != 0)
                return priorityComparison;

            // If same priority, older issues first
            return SubmittedDate.CompareTo(other.SubmittedDate);
        }

        /// <summary>
        /// String representation for debugging
        /// </summary>
        public override string ToString()
        {
            return $"Issue #{IssueId} - Priority {Priority}";
        }
    }
}
