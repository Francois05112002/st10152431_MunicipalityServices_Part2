using System;

namespace st10152431_MunicipalityService.Models
{
    /// <summary>
    /// Node for AVL Tree Search
    /// Used to order issues by ID for fast lookup
    /// </summary>
    public class IssueSearchNode : IComparable<IssueSearchNode>
    {
        /// <summary>
        /// Issue ID (primary key for searching)
        /// </summary>
        public int IssueId { get; set; }

        /// <summary>
        /// Optional: Category for additional filtering
        /// </summary>
        public string? Category { get; set; }

        /// <summary>
        /// Compare to another node for tree ordering
        /// Ordered by IssueId
        /// </summary>
        public int CompareTo(IssueSearchNode? other)
        {
            if (other == null) return 1;

            // Order by IssueId (smaller IDs to the left)
            return IssueId.CompareTo(other.IssueId);
        }

        /// <summary>
        /// String representation for debugging
        /// </summary>
        public override string ToString()
        {
            return $"Issue #{IssueId}";
        }
    }
}
