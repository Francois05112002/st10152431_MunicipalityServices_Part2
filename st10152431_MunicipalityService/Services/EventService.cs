using System;
using System.Collections.Generic;
using System.Linq;
using st10152431_MunicipalityService.Data;
using st10152431_MunicipalityService.Models;
using st10152431_MunicipalityService.Models.DataStructures;

namespace st10152431_MunicipalityService.Services
{
    public class EventService
    {
        private readonly AppDbContext _context;
        private readonly AVLTree<Event> _eventTree = new();
        private readonly AVLTree<Announcement> _announcementTree = new();

        public EventService(AppDbContext context)
        {
            _context = context;

            // Populate AVL trees from database
            foreach (var evt in _context.Events)
                _eventTree.Insert(evt);

            foreach (var ann in _context.Announcements)
                _announcementTree.Insert(ann);
        }

        // Fixed category options for events
        private static readonly List<string> _eventCategories = new List<string>
        {
            "Community",
            "Sports",
            "Education",
            "Culture",
            "Other"
        };

        // Fixed category options for announcements
        private static readonly List<string> _announcementCategories = new List<string>
        {
            "General",
            "Alert",
            "Maintenance",
            "Event",
            "Other"
        };

        // ===== EVENT METHODS =====

        public List<string> GetEventCategories() => _eventCategories;

        /// <summary>
        /// Get all ACTIVE events (end date hasn't passed), sorted by StartDate
        /// </summary>
        public List<Event> GetAllEvents()
        {
            var today = DateTime.Today;
            return _eventTree.InOrderTraversal()
                .Where(e => e.EndDate.Date >= today)
                .ToList();
        }

        /// <summary>
        /// Add a new event (only logged-in users)
        /// </summary>
        public int AddEvent(string name, DateTime startDate, DateTime endDate,
                            string category, string location, string createdBy)
        {
            if (string.IsNullOrEmpty(createdBy))
                throw new InvalidOperationException("Must be logged in to create events");

            var newEvent = new Event
            {
                Name = name,
                StartDate = startDate,
                EndDate = endDate,
                Category = category,
                Location = location,
                CreatedBy = createdBy
            };

            _context.Events.Add(newEvent);
            _context.SaveChanges();
            _eventTree.Insert(newEvent);

            return newEvent.Id;
        }

        /// <summary>
        /// Update an event (by Id)
        /// </summary>
        public bool UpdateEvent(Event updatedEvent)
        {
            var existing = _context.Events.Find(updatedEvent.Id);
            if (existing == null)
                return false;

            _eventTree.Remove(existing);

            existing.Name = updatedEvent.Name;
            existing.StartDate = updatedEvent.StartDate;
            existing.EndDate = updatedEvent.EndDate;
            existing.Category = updatedEvent.Category;
            existing.Location = updatedEvent.Location;
            existing.CreatedBy = updatedEvent.CreatedBy;

            _context.SaveChanges();
            _eventTree.Insert(existing);

            return true;
        }

        /// <summary>
        /// Delete an event (by Id)
        /// </summary>
        public bool DeleteEvent(int eventId)
        {
            var evt = _context.Events.Find(eventId);
            if (evt == null)
                return false;

            _context.Events.Remove(evt);
            _context.SaveChanges();
            _eventTree.Remove(evt);

            return true;
        }

        /// <summary>
        /// Filter events by category and/or date (only ACTIVE events)
        /// </summary>
        public List<Event> FilterEvents(string category = null, DateTime? date = null)
        {
            var today = DateTime.Today;
            var events = _eventTree.InOrderTraversal()
                .Where(e => e.EndDate.Date >= today);

            if (!string.IsNullOrEmpty(category) && category != "All Categories")
                events = events.Where(e => e.Category == category);

            if (date.HasValue)
                events = events.Where(e =>
                    e.StartDate.Date <= date.Value.Date &&
                    e.EndDate.Date >= date.Value.Date);

            return events.ToList();
        }

        /// <summary>
        /// Get events by user (who created them), only ACTIVE events
        /// </summary>
        public List<Event> GetEventsByUser(string userId)
        {
            var today = DateTime.Today;
            return _eventTree.InOrderTraversal()
                .Where(e => e.CreatedBy == userId && e.EndDate.Date >= today)
                .ToList();
        }

        /// <summary>
        /// Get upcoming events (starting from today onwards)
        /// </summary>
        public List<Event> GetUpcomingEvents()
        {
            var today = DateTime.Today;
            return _eventTree.InOrderTraversal()
                .Where(e => e.EndDate >= today)
                .ToList();
        }

        // ===== ANNOUNCEMENT METHODS =====

        public List<string> GetAnnouncementCategories() => _announcementCategories;

        /// <summary>
        /// Get all ACTIVE announcements (end date hasn't passed), sorted by StartDate
        /// </summary>
        public List<Announcement> GetAllAnnouncements()
        {
            var today = DateTime.Today;
            return _announcementTree.InOrderTraversal()
                .Where(a => a.EndDate.Date >= today)
                .ToList();
        }

        /// <summary>
        /// Add a new announcement (only logged-in users)
        /// </summary>
        public int AddAnnouncement(string name, DateTime startDate, DateTime endDate,
                                   string category, string location, string createdBy)
        {
            if (string.IsNullOrEmpty(createdBy))
                throw new InvalidOperationException("Must be logged in to create announcements");

            var newAnnouncement = new Announcement
            {
                Name = name,
                StartDate = startDate,
                EndDate = endDate,
                Category = category,
                Location = location,
                CreatedBy = createdBy
            };

            _context.Announcements.Add(newAnnouncement);
            _context.SaveChanges();
            _announcementTree.Insert(newAnnouncement);

            return newAnnouncement.Id;
        }

        /// <summary>
        /// Update an announcement (by Id)
        /// </summary>
        public bool UpdateAnnouncement(Announcement updatedAnnouncement)
        {
            var existing = _context.Announcements.Find(updatedAnnouncement.Id);
            if (existing == null)
                return false;

            _announcementTree.Remove(existing);

            existing.Name = updatedAnnouncement.Name;
            existing.StartDate = updatedAnnouncement.StartDate;
            existing.EndDate = updatedAnnouncement.EndDate;
            existing.Category = updatedAnnouncement.Category;
            existing.Location = updatedAnnouncement.Location;
            existing.CreatedBy = updatedAnnouncement.CreatedBy;

            _context.SaveChanges();
            _announcementTree.Insert(existing);

            return true;
        }

        /// <summary>
        /// Delete an announcement (by Id)
        /// </summary>
        public bool DeleteAnnouncement(int announcementId)
        {
            var ann = _context.Announcements.Find(announcementId);
            if (ann == null)
                return false;

            _context.Announcements.Remove(ann);
            _context.SaveChanges();
            _announcementTree.Remove(ann);

            return true;
        }

        /// <summary>
        /// Filter announcements by category and/or date (only ACTIVE announcements)
        /// </summary>
        public List<Announcement> FilterAnnouncements(string category = null, DateTime? date = null)
        {
            var today = DateTime.Today;
            var announcements = _announcementTree.InOrderTraversal()
                .Where(a => a.EndDate.Date >= today);

            if (!string.IsNullOrEmpty(category) && category != "All Categories")
                announcements = announcements.Where(a => a.Category == category);

            if (date.HasValue)
                announcements = announcements.Where(a =>
                    a.StartDate.Date <= date.Value.Date &&
                    a.EndDate.Date >= date.Value.Date);

            return announcements.ToList();
        }

        /// <summary>
        /// Get announcements by user (who created them), only ACTIVE announcements
        /// </summary>
        public List<Announcement> GetAnnouncementsByUser(string userId)
        {
            var today = DateTime.Today;
            return _announcementTree.InOrderTraversal()
                .Where(a => a.CreatedBy == userId && a.EndDate.Date >= today)
                .ToList();
        }

        /// <summary>
        /// Get active announcements (currently in date range)
        /// </summary>
        public List<Announcement> GetActiveAnnouncements()
        {
            var today = DateTime.Today;
            return _announcementTree.InOrderTraversal()
                .Where(a => a.StartDate <= today && a.EndDate >= today)
                .ToList();
        }

        /// <summary>
        /// Get total counts for statistics (ACTIVE items only)
        /// </summary>
        public (int eventCount, int announcementCount) GetTotalCounts()
        {
            var today = DateTime.Today;
            int activeEvents = _eventTree.InOrderTraversal().Count(e => e.EndDate.Date >= today);
            int activeAnnouncements = _announcementTree.InOrderTraversal().Count(a => a.EndDate.Date >= today);
            return (activeEvents, activeAnnouncements);
        }
    }
}
