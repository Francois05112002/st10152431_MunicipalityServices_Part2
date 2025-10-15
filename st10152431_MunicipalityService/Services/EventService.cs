using System;
using System.Collections.Generic;
using System.Linq;
using st10152431_MunicipalityService.Data;
using st10152431_MunicipalityService.Models;

namespace st10152431_MunicipalityService.Services
{
    public class EventService
    {
        private readonly AppDbContext _context;

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

        public EventService(AppDbContext context)
        {
            _context = context;
        }

        // ===== EVENT METHODS =====

        /// <summary>
        /// Get all event categories
        /// LIST: Returns fixed list
        /// </summary>
        public List<string> GetEventCategories()
        {
            return _eventCategories;
        }

        /// <summary>
        /// Get all ACTIVE events (end date hasn't passed)
        /// LIST pattern: Returns filtered and sorted list
        /// Automatically filters out expired events
        /// </summary>
        public List<Event> GetAllEvents()
        {
            var today = DateTime.Today;

            return _context.Events
                .Where(e => e.EndDate.Date >= today)
                .OrderBy(e => e.StartDate)
                .ToList();
        }

        /// <summary>
        /// Add a new event (only logged-in users)
        /// LIST pattern: O(1) append operation
        /// Returns the new event ID
        /// </summary>
        public int AddEvent(string name, DateTime startDate, DateTime endDate,
                           string category, string location, string createdBy)
        {
            if (string.IsNullOrEmpty(createdBy))
            {
                throw new InvalidOperationException("Must be logged in to create events");
            }

            var newEvent = new Event
            {
                Name = name,
                StartDate = startDate,
                EndDate = endDate,
                Category = category,
                Location = location,
                CreatedBy = createdBy
            };

            // LIST append is O(1) amortized
            _context.Events.Add(newEvent);
            _context.SaveChanges();

            return newEvent.Id;
        }

        /// <summary>
        /// Filter events by category and/or date
        /// LIST pattern: Returns filtered list
        /// Only returns ACTIVE events (end date hasn't passed)
        /// </summary>
        public List<Event> FilterEvents(string category = null, DateTime? date = null)
        {
            var today = DateTime.Today;

            // Start with only active events (not expired)
            var query = _context.Events.Where(e => e.EndDate.Date >= today);

            // Filter by category if provided
            if (!string.IsNullOrEmpty(category) && category != "All Categories")
            {
                query = query.Where(e => e.Category == category);
            }

            // Filter by date if provided
            if (date.HasValue)
            {
                query = query.Where(e =>
                    e.StartDate.Date <= date.Value.Date &&
                    e.EndDate.Date >= date.Value.Date);
            }

            // Sort by start date (upcoming first)
            return query.OrderBy(e => e.StartDate).ToList();
        }

        /// <summary>
        /// Get events by user (who created them)
        /// LIST pattern: Returns filtered list
        /// Only returns ACTIVE events
        /// </summary>
        public List<Event> GetEventsByUser(string userId)
        {
            var today = DateTime.Today;

            return _context.Events
                .Where(e => e.CreatedBy == userId && e.EndDate.Date >= today)
                .OrderBy(e => e.StartDate)
                .ToList();
        }

        // ===== ANNOUNCEMENT METHODS =====

        /// <summary>
        /// Get all announcement categories
        /// LIST: Returns fixed list
        /// </summary>
        public List<string> GetAnnouncementCategories()
        {
            return _announcementCategories;
        }

        /// <summary>
        /// Get all ACTIVE announcements (end date hasn't passed)
        /// LIST pattern: Returns filtered and sorted list
        /// Automatically filters out expired announcements
        /// </summary>
        public List<Announcement> GetAllAnnouncements()
        {
            var today = DateTime.Today;

            return _context.Announcements
                .Where(a => a.EndDate.Date >= today)
                .OrderBy(a => a.StartDate)
                .ToList();
        }

        /// <summary>
        /// Add a new announcement (only logged-in users)
        /// LIST pattern: O(1) append operation
        /// Returns the new announcement ID
        /// </summary>
        public int AddAnnouncement(string name, DateTime startDate, DateTime endDate,
                                  string category, string location, string createdBy)
        {
            if (string.IsNullOrEmpty(createdBy))
            {
                throw new InvalidOperationException("Must be logged in to create announcements");
            }

            var newAnnouncement = new Announcement
            {
                Name = name,
                StartDate = startDate,
                EndDate = endDate,
                Category = category,
                Location = location,
                CreatedBy = createdBy
            };

            // LIST append is O(1) amortized
            _context.Announcements.Add(newAnnouncement);
            _context.SaveChanges();

            return newAnnouncement.Id;
        }

        /// <summary>
        /// Filter announcements by category and/or date
        /// LIST pattern: Returns filtered list
        /// Only returns ACTIVE announcements (end date hasn't passed)
        /// </summary>
        public List<Announcement> FilterAnnouncements(string category = null, DateTime? date = null)
        {
            var today = DateTime.Today;

            // Start with only active announcements (not expired)
            var query = _context.Announcements.Where(a => a.EndDate.Date >= today);

            // Filter by category if provided
            if (!string.IsNullOrEmpty(category) && category != "All Categories")
            {
                query = query.Where(a => a.Category == category);
            }

            // Filter by date if provided
            if (date.HasValue)
            {
                query = query.Where(a =>
                    a.StartDate.Date <= date.Value.Date &&
                    a.EndDate.Date >= date.Value.Date);
            }

            // Sort by start date
            return query.OrderBy(a => a.StartDate).ToList();
        }

        /// <summary>
        /// Get announcements by user (who created them)
        /// LIST pattern: Returns filtered list
        /// Only returns ACTIVE announcements
        /// </summary>
        public List<Announcement> GetAnnouncementsByUser(string userId)
        {
            var today = DateTime.Today;

            return _context.Announcements
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

            int activeEvents = _context.Events.Count(e => e.EndDate.Date >= today);
            int activeAnnouncements = _context.Announcements.Count(a => a.EndDate.Date >= today);

            return (activeEvents, activeAnnouncements);
        }

        /// <summary>
        /// Get upcoming events (starting from today onwards)
        /// LIST pattern: Returns filtered list
        /// </summary>
        public List<Event> GetUpcomingEvents()
        {
            var today = DateTime.Today;

            return _context.Events
                .Where(e => e.EndDate >= today)
                .OrderBy(e => e.StartDate)
                .ToList();
        }

        /// <summary>
        /// Get active announcements (currently in date range)
        /// LIST pattern: Returns filtered list
        /// </summary>
        public List<Announcement> GetActiveAnnouncements()
        {
            var today = DateTime.Today;

            return _context.Announcements
                .Where(a => a.StartDate <= today && a.EndDate >= today)
                .OrderBy(a => a.StartDate)
                .ToList();
        }
    }
}