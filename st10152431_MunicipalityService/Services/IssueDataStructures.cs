using System;
using System.Collections.Generic;
using System.Linq;
using st10152431_MunicipalityService.Data;
using st10152431_MunicipalityService.Models;
using st10152431_MunicipalityService.Models.DataStructures;

namespace st10152431_MunicipalityService.Services
{
    /// <summary>
    /// Issue Data Structures Service
    /// 
    /// Manages advanced data structures for efficient issue management:
    /// 1. Min-Heap: Priority queue for urgent issues
    /// 2. AVL Tree: Fast search by issue ID
    /// 
    /// This service acts as a cache/index over the database:
    /// - Database (Entity Framework): Persistent storage
    /// - Data Structures: Fast in-memory access
    /// </summary>
    public class IssueDataStructures
    {
        // ===== DEPENDENCIES =====

        private readonly AppDbContext _context;

        // ===== DATA STRUCTURES =====

        /// <summary>
        /// Min-Heap: Stores urgent issues ordered by priority
        /// Root = most urgent issue (Priority 1, oldest date)
        /// </summary>
        private MinHeap<IssuePriorityNode> _urgentHeap;

        /// <summary>
        /// AVL Tree: Stores all issues ordered by ID for fast search
        /// Allows O(log n) search vs O(n) with List.Find()
        /// </summary>
        private AVLTree<IssueSearchNode> _searchTree;

        // ===== CACHE MANAGEMENT =====

        /// <summary>
        /// When structures were last refreshed from database
        /// </summary>
        private DateTime _lastRefresh;

        /// <summary>
        /// How long before we refresh (5 minutes)
        /// </summary>
        private const int CACHE_MINUTES = 5;

        // ===== CONSTRUCTOR =====

        /// <summary>
        /// Initialize data structures and load initial data
        /// </summary>
        public IssueDataStructures(AppDbContext context)
        {
            _context = context;

            // Initialize empty structures
            _urgentHeap = new MinHeap<IssuePriorityNode>();
            _searchTree = new AVLTree<IssueSearchNode>();

            // Load initial data from database
            RefreshStructures();
        }

        // ===== PUBLIC METHODS (What you'll use in your pages) =====

        /// <summary>
        /// Get top N most urgent issues using Min-Heap
        /// 
        /// WHY THIS IS FAST:
        /// - Min-Heap keeps root = most urgent
        /// - Peek = O(1), Extract = O(log n)
        /// - No need to sort entire list
        /// 
        /// COMPARISON:
        /// - List.OrderBy().Take(N) = O(n log n)
        /// - Min-Heap.PeekTop(N) = O(N log n) where N is small
        /// </summary>
        /// <param name="count">Number of urgent issues to return (default 5)</param>
        /// <returns>List of most urgent issues</returns>
        public List<Issue> GetTopUrgent(int count = 5)
        {
            // Refresh if cache is stale
            RefreshIfNeeded();

            // Check if heap is empty
            if (_urgentHeap.IsEmpty)
                return new List<Issue>();

            // Get top N nodes from heap (does NOT remove them)
            var urgentNodes = _urgentHeap.PeekTop(count);

            // Extract issue IDs
            var issueIds = urgentNodes.Select(n => n.IssueId).ToList();

            // Fetch actual Issue objects from database
            var issues = _context.Issues
                .Where(i => issueIds.Contains(i.Id))
                .ToList();

            // Return in heap order (most urgent first)
            var orderedIssues = new List<Issue>();
            foreach (var node in urgentNodes)
            {
                var issue = issues.FirstOrDefault(i => i.Id == node.IssueId);
                if (issue != null)
                    orderedIssues.Add(issue);
            }

            return orderedIssues;
        }

        /// <summary>
        /// Fast search for issue by ID using AVL Tree
        /// 
        /// WHY THIS IS FAST:
        /// - AVL Tree is self-balancing BST
        /// - Search = O(log n)
        /// - List.Find() = O(n)
        /// 
        /// EXAMPLE:
        /// - 1000 issues: List.Find() ~500 comparisons, AVL ~10 comparisons
        /// - 50x faster!
        /// </summary>
        /// <param name="issueId">ID of issue to find</param>
        /// <returns>Issue if found, null otherwise</returns>
        public Issue? FastSearch(int issueId)
        {
            // Refresh if cache is stale
            RefreshIfNeeded();

            // Create search node
            var searchNode = new IssueSearchNode { IssueId = issueId };

            // Search AVL Tree - O(log n)
            var foundNode = _searchTree.Search(searchNode);

            // If not found in tree, return null
            if (foundNode == null)
                return null;

            // Fetch actual Issue from database
            // (We only store IDs in tree, not full objects)
            return _context.Issues.Find(issueId);
        }

        /// <summary>
        /// Get the single most urgent issue
        /// </summary>
        /// <returns>Most urgent issue or null if none</returns>
        public Issue? GetMostUrgent()
        {
            RefreshIfNeeded();

            if (_urgentHeap.IsEmpty)
                return null;

            var mostUrgentNode = _urgentHeap.Peek();
            return _context.Issues.Find(mostUrgentNode.IssueId);
        }

        /// <summary>
        /// Process (extract and mark as in progress) the most urgent issue
        /// This REMOVES it from the heap
        /// </summary>
        /// <returns>The processed issue</returns>
        public Issue? ProcessMostUrgent()
        {
            RefreshIfNeeded();

            if (_urgentHeap.IsEmpty)
                return null;

            // Extract from heap (removes it)
            var urgentNode = _urgentHeap.ExtractMin();

            // Get issue from database
            var issue = _context.Issues.Find(urgentNode.IssueId);

            if (issue != null)
            {
                // Mark as in progress
                issue.Status = "In Progress";
                _context.SaveChanges();

                // Refresh structures to reflect change
                RefreshStructures();
            }

            return issue;
        }

        /// <summary>
        /// Get statistics about data structures
        /// Useful for showing performance metrics
        /// </summary>
        public DataStructureStats GetStatistics()
        {
            RefreshIfNeeded();

            return new DataStructureStats
            {
                TotalIssuesIndexed = _searchTree.Count,
                UrgentIssuesQueued = _urgentHeap.Count,
                AVLTreeHeight = _searchTree.GetHeight(),
                LastRefreshTime = _lastRefresh
            };
        }

        /// <summary>
        /// Force refresh of structures from database
        /// Call this after adding/updating issues
        /// </summary>
        public void ForceRefresh()
        {
            RefreshStructures();
        }

        // ===== PRIVATE METHODS (Internal workings) =====

        /// <summary>
        /// Refresh data structures if cache is stale
        /// </summary>
        private void RefreshIfNeeded()
        {
            var minutesSinceRefresh = (DateTime.Now - _lastRefresh).TotalMinutes;

            if (minutesSinceRefresh > CACHE_MINUTES)
            {
                RefreshStructures();
            }
        }

        /// <summary>
        /// Rebuild all data structures from database
        /// This is called:
        /// 1. On initialization
        /// 2. Every 5 minutes (cache expiry)
        /// 3. When ForceRefresh() is called
        /// </summary>
        private void RefreshStructures()
        {
            // Clear existing structures
            _urgentHeap.Clear();
            _searchTree.Clear();

            // Load ALL issues from database
            var allIssues = _context.Issues.ToList();

            // ===== BUILD MIN-HEAP =====
            // Only include pending/assigned issues that have priority set
            var urgentIssues = allIssues
                .Where(i => (i.Status == "Pending" || i.Status == "Assigned") &&
                           i.Priority.HasValue)
                .ToList();

            foreach (var issue in urgentIssues)
            {
                _urgentHeap.Insert(new IssuePriorityNode
                {
                    IssueId = issue.Id,
                    Priority = issue.Priority.Value,
                    SubmittedDate = issue.Timestamp
                });
            }

            // ===== BUILD AVL TREE =====
            // Include ALL issues for searching
            foreach (var issue in allIssues)
            {
                _searchTree.Insert(new IssueSearchNode
                {
                    IssueId = issue.Id,
                    Category = issue.Category
                });
            }

            // Update timestamp
            _lastRefresh = DateTime.Now;

            // Log refresh (optional, for debugging)
            Console.WriteLine($"[IssueDataStructures] Refreshed: {allIssues.Count} issues, " +
                            $"{urgentIssues.Count} in heap, Height: {_searchTree.GetHeight()}");
        }
    }

    // ===== HELPER CLASS: Statistics =====

    /// <summary>
    /// Statistics about data structure performance
    /// Used for displaying metrics to user
    /// </summary>
    public class DataStructureStats
    {
        /// <summary>
        /// Total issues indexed in AVL Tree
        /// </summary>
        public int TotalIssuesIndexed { get; set; }

        /// <summary>
        /// Urgent issues in Min-Heap
        /// </summary>
        public int UrgentIssuesQueued { get; set; }

        /// <summary>
        /// Current height of AVL Tree
        /// Should be close to log2(TotalIssuesIndexed)
        /// </summary>
        public int AVLTreeHeight { get; set; }

        /// <summary>
        /// When structures were last refreshed
        /// </summary>
        public DateTime LastRefreshTime { get; set; }

        /// <summary>
        /// Calculate theoretical minimum height for perfect balance
        /// </summary>
        public int TheoreticalHeight =>
            (int)Math.Ceiling(Math.Log2(TotalIssuesIndexed + 1));

        /// <summary>
        /// Check if tree is well-balanced
        /// (Actual height should be close to theoretical)
        /// </summary>
        public bool IsWellBalanced =>
            AVLTreeHeight <= TheoreticalHeight + 1;

        /// <summary>
        /// Calculate efficiency gain vs linear search
        /// </summary>
        public double SearchEfficiencyGain()
        {
            if (TotalIssuesIndexed == 0) return 1.0;

            // Average linear search: n/2 comparisons
            double linearSearchOps = TotalIssuesIndexed / 2.0;

            // AVL tree search: log2(n) comparisons
            double treeSearchOps = AVLTreeHeight;

            if (treeSearchOps == 0) return 1.0;

            return linearSearchOps / treeSearchOps;
        }
    }
}
