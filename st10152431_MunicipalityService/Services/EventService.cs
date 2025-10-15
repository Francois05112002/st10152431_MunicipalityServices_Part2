using System;
using System.Collections.Generic;
using System.Linq;
using st10152431_MunicipalityService.Models;

namespace st10152431_MunicipalityService.Services
{
    public class EventService
    {
        // LIST: Stores all events chronologically, easy to filter
        // O(1) append, O(n) filtering (acceptable for reasonable dataset sizes)
        private static List<Event> _allEvents = new List<Event>();

        // LIST: Stores all announcements chronologically
        private static List<Announcement> _allAnnouncements = new List<Announcement>();

        // Counters for unique IDs
        private static int _eventCounter = 0;
        private static int _announcementCounter = 0;

        // LIST: Fixed category options for events
        private static readonly List<string> _eventCategories = new List<string>
        {
            "Community",
            "Sports",
            "Education",
            "Culture",
            "Other"
        };

        // LIST: Fixed category options for announcements
        private static readonly List<string> _announcementCategories = new List<string>
        {
            "General",
            "Alert",
            "Maintenance",
            "Event",
            "Other"
        };

        // ===== EVENT METHODS =====

        /// <summary>
        /// Get all event categories
        /// </summary>
        public List<string> GetEventCategories()
        {
            return _eventCategories;
        }

        /// <summary>
        /// Get all ACTIVE events (end date hasn't passed)
        /// Automatically filters out expired events
        /// </summary>
        public List<Event> GetAllEvents()
        {
            var today = DateTime.Today;

            // Only return events where EndDate >= Today (not expired)
            return _allEvents
                .Where(e => e.EndDate.Date >= today)
                .OrderBy(e => e.StartDate)
                .ToList();
        }

        /// <summary>
        /// Add a new event (only logged-in users)
        /// Returns the new event ID
        /// </summary>
        public int AddEvent(string name, DateTime startDate, DateTime endDate,
                           string category, string location, string createdBy)
        {
            if (string.IsNullOrEmpty(createdBy))
            {
                throw new InvalidOperationException("Must be logged in to create events");
            }

            _eventCounter++;

            var newEvent = new Event
            {
                Id = _eventCounter,
                Name = name,
                StartDate = startDate,
                EndDate = endDate,
                Category = category,
                Location = location,
                CreatedBy = createdBy
            };

            // LIST append is O(1) amortized
            _allEvents.Add(newEvent);

            return newEvent.Id;
        }

        /// <summary>
        /// Filter events by category and/or date
        /// Only returns ACTIVE events (end date hasn't passed)
        /// </summary>
        public List<Event> FilterEvents(string category = null, DateTime? date = null)
        {
            var today = DateTime.Today;

            // Start with only active events (not expired)
            var filtered = _allEvents.Where(e => e.EndDate.Date >= today);

            // Filter by category if provided
            if (!string.IsNullOrEmpty(category) && category != "All Categories")
            {
                filtered = filtered.Where(e => e.Category == category);
            }

            // Filter by date if provided
            // Show events that overlap with the selected date
            if (date.HasValue)
            {
                filtered = filtered.Where(e =>
                    e.StartDate.Date <= date.Value.Date &&
                    e.EndDate.Date >= date.Value.Date);
            }

            // Sort by start date (upcoming first)
            return filtered.OrderBy(e => e.StartDate).ToList();
        }

        /// <summary>
        /// Get events by user (who created them)
        /// Only returns ACTIVE events
        /// </summary>
        public List<Event> GetEventsByUser(string userId)
        {
            var today = DateTime.Today;

            return _allEvents
                .Where(e => e.CreatedBy == userId && e.EndDate.Date >= today)
                .OrderBy(e => e.StartDate)
                .ToList();
        }

        // ===== ANNOUNCEMENT METHODS =====

        /// <summary>
        /// Get all announcement categories
        /// </summary>
        public List<string> GetAnnouncementCategories()
        {
            return _announcementCategories;
        }

        /// <summary>
        /// Get all ACTIVE announcements (end date hasn't passed)
        /// Automatically filters out expired announcements
        /// </summary>
        public List<Announcement> GetAllAnnouncements()
        {
            var today = DateTime.Today;

            // Only return announcements where EndDate >= Today (not expired)
            return _allAnnouncements
                .Where(a => a.EndDate.Date >= today)
                .OrderBy(a => a.StartDate)
                .ToList();
        }

        /// <summary>
        /// Add a new announcement (only logged-in users)
        /// Returns the new announcement ID
        /// </summary>
        public int AddAnnouncement(string name, DateTime startDate, DateTime endDate,
                                  string category, string location, string createdBy)
        {
            if (string.IsNullOrEmpty(createdBy))
            {
                throw new InvalidOperationException("Must be logged in to create announcements");
            }

            _announcementCounter++;

            var newAnnouncement = new Announcement
            {
                Id = _announcementCounter,
                Name = name,
                StartDate = startDate,
                EndDate = endDate,
                Category = category,
                Location = location,
                CreatedBy = createdBy
            };

            // LIST append is O(1) amortized
            _allAnnouncements.Add(newAnnouncement);

            return newAnnouncement.Id;
        }

        /// <summary>
        /// Filter announcements by category and/or date
        /// Only returns ACTIVE announcements (end date hasn't passed)
        /// </summary>
        public List<Announcement> FilterAnnouncements(string category = null, DateTime? date = null)
        {
            var today = DateTime.Today;

            // Start with only active announcements (not expired)
            var filtered = _allAnnouncements.Where(a => a.EndDate.Date >= today);

            // Filter by category if provided
            if (!string.IsNullOrEmpty(category) && category != "All Categories")
            {
                filtered = filtered.Where(a => a.Category == category);
            }

            // Filter by date if provided
            // Show announcements that overlap with the selected date
            if (date.HasValue)
            {
                filtered = filtered.Where(a =>
                    a.StartDate.Date <= date.Value.Date &&
                    a.EndDate.Date >= date.Value.Date);
            }

            // Sort by start date
            return filtered.OrderBy(a => a.StartDate).ToList();
        }

        /// <summary>
        /// Get announcements by user (who created them)
        /// Only returns ACTIVE announcements
        /// </summary>
        public List<Announcement> GetAnnouncementsByUser(string userId)
        {
            var today = DateTime.Today;

            return _allAnnouncements
                .Where(a => a.CreatedBy == userId && a.EndDate.Date >= today)
                .OrderBy(a => a.StartDate)
                .ToList();
        }

        /// <summary>
        /// Get total counts for statistics (ACTIVE items only)
        /// </summary>
        public (int eventCount, int announcementCount) GetTotalCounts()
        {
            var today = DateTime.Today;

            int activeEvents = _allEvents.Count(e => e.EndDate.Date >= today);
            int activeAnnouncements = _allAnnouncements.Count(a => a.EndDate.Date >= today);

            return (activeEvents, activeAnnouncements);
        }

        /// <summary>
        /// Get upcoming events (starting from today onwards)
        /// </summary>
        public List<Event> GetUpcomingEvents()
        {
            var today = DateTime.Today;

            return _allEvents
                .Where(e => e.EndDate >= today)
                .OrderBy(e => e.StartDate)
                .ToList();
        }

        /// <summary>
        /// Get active announcements (currently in date range)
        /// </summary>
        public List<Announcement> GetActiveAnnouncements()
        {
            var today = DateTime.Today;

            return _allAnnouncements
                .Where(a => a.StartDate <= today && a.EndDate >= today)
                .OrderBy(a => a.StartDate)
                .ToList();
        }

        // ===== OPTIONAL: ADMIN METHODS (if you want to see expired items) =====

        /// <summary>
        /// Get ALL events including expired ones (for admin/history purposes)
        /// </summary>
        public List<Event> GetAllEventsIncludingExpired()
        {
            return _allEvents.OrderBy(e => e.StartDate).ToList();
        }

        /// <summary>
        /// Get ALL announcements including expired ones (for admin/history purposes)
        /// </summary>
        public List<Announcement> GetAllAnnouncementsIncludingExpired()
        {
            return _allAnnouncements.OrderBy(a => a.StartDate).ToList();
        }
    }
}