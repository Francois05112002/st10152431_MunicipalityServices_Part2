using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using st10152431_MunicipalityService.Models;
using st10152431_MunicipalityService.Services;

namespace st10152431_MunicipalityService.Pages
{
    public class EventsModel : PageModel
    {
        private readonly EventService _eventService;
        private readonly UserService _userService;

        public List<SelectListItem> EventCategories { get; set; }
        public List<SelectListItem> AnnouncementCategories { get; set; }


        // Unified filter properties
        [BindProperty(SupportsGet = true)]
        public string CategoryFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? DateFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public string TypeFilter { get; set; }

        // New item properties
        [BindProperty]
        public Event NewEvent { get; set; }

        [BindProperty]
        public Announcement NewAnnouncement { get; set; }

        // Display properties
        public List<Event> FilteredEvents { get; set; } = new();
        public List<Announcement> FilteredAnnouncements { get; set; } = new();
        public List<string> AllCategories { get; set; } = new();

        public bool IsLoggedIn { get; set; }
        public string Message { get; set; }

        public EventsModel(EventService eventService, UserService userService)
        {
            _eventService = eventService;
            _userService = userService;
        }

        public void OnGet()
        {
            LoadPageData();
        }

        private void LoadPageData()
        {
            // Check if user is logged in
            var userPhone = HttpContext.Session.GetString("UserPhone");
            IsLoggedIn = !string.IsNullOrEmpty(userPhone);

            EventCategories = _eventService.GetEventCategories()
    .Select(c => new SelectListItem { Value = c, Text = c }).ToList();

            AnnouncementCategories = _eventService.GetAnnouncementCategories()
                .Select(c => new SelectListItem { Value = c, Text = c }).ToList();


            // Get all events and announcements (active only)
            var allEvents = _eventService.GetAllEvents();
            var allAnnouncements = _eventService.GetAllAnnouncements();

            // Build category list for filter dropdown
            AllCategories = allEvents.Select(e => e.Category)
                .Concat(allAnnouncements.Select(a => a.Category))
                .Distinct()
                .OrderBy(c => c)
                .ToList();

            // Filtering
            if (!string.IsNullOrEmpty(CategoryFilter))
            {
                allEvents = allEvents.Where(e => e.Category == CategoryFilter).ToList();
                allAnnouncements = allAnnouncements.Where(a => a.Category == CategoryFilter).ToList();
            }

            if (DateFilter.HasValue)
            {
                allEvents = allEvents.Where(e => e.EndDate >= DateFilter.Value).ToList();
                allAnnouncements = allAnnouncements.Where(a => a.EndDate >= DateFilter.Value).ToList();
            }
            else
            {
                // Default: only show future or current items
                allEvents = allEvents.Where(e => e.EndDate >= DateTime.Today).ToList();
                allAnnouncements = allAnnouncements.Where(a => a.EndDate >= DateTime.Today).ToList();
            }

            if (TypeFilter == "Event")
            {
                allAnnouncements.Clear();
            }
            else if (TypeFilter == "Announcement")
            {
                allEvents.Clear();
            }

            FilteredEvents = allEvents;
            FilteredAnnouncements = allAnnouncements;
        }

        public IActionResult OnPostCreateEvent()
        {
            var userPhone = HttpContext.Session.GetString("UserPhone");
            if (string.IsNullOrEmpty(userPhone))
            {
                ModelState.AddModelError(string.Empty, "You must be logged in to create events.");
                LoadPageData();
                return Page();
            }

            if (string.IsNullOrWhiteSpace(NewEvent?.Name))
            {
                ModelState.AddModelError(string.Empty, "Event name is required.");
                LoadPageData();
                return Page();
            }

            if (string.IsNullOrWhiteSpace(NewEvent?.Category))
            {
                ModelState.AddModelError(string.Empty, "Please select a valid category.");
                LoadPageData();
                return Page();
            }

            if (string.IsNullOrWhiteSpace(NewEvent?.Location))
            {
                ModelState.AddModelError(string.Empty, "Location is required.");
                LoadPageData();
                return Page();
            }

            if (NewEvent.EndDate < NewEvent.StartDate)
            {
                ModelState.AddModelError(string.Empty, "End date must be after start date.");
                LoadPageData();
                return Page();
            }

            try
            {
                int eventId = _eventService.AddEvent(
                    NewEvent.Name,
                    NewEvent.StartDate,
                    NewEvent.EndDate,
                    NewEvent.Category,
                    NewEvent.Location,
                    userPhone
                );

                TempData["SuccessMessage"] = $"Event '{NewEvent.Name}' created successfully!";
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error creating event: {ex.Message}");
                LoadPageData();
                return Page();
            }
        }

        public IActionResult OnPostCreateAnnouncement()
        {
            var userPhone = HttpContext.Session.GetString("UserPhone");
            if (string.IsNullOrEmpty(userPhone))
            {
                ModelState.AddModelError(string.Empty, "You must be logged in to create announcements.");
                LoadPageData();
                return Page();
            }

            if (string.IsNullOrWhiteSpace(NewAnnouncement?.Name))
            {
                ModelState.AddModelError(string.Empty, "Announcement name is required.");
                LoadPageData();
                return Page();
            }

            if (string.IsNullOrWhiteSpace(NewAnnouncement?.Category))
            {
                ModelState.AddModelError(string.Empty, "Please select a valid category.");
                LoadPageData();
                return Page();
            }

            if (string.IsNullOrWhiteSpace(NewAnnouncement?.Location))
            {
                ModelState.AddModelError(string.Empty, "Location is required.");
                LoadPageData();
                return Page();
            }

            if (NewAnnouncement.EndDate < NewAnnouncement.StartDate)
            {
                ModelState.AddModelError(string.Empty, "End date must be after start date.");
                LoadPageData();
                return Page();
            }

            try
            {
                int announcementId = _eventService.AddAnnouncement(
                    NewAnnouncement.Name,
                    NewAnnouncement.StartDate,
                    NewAnnouncement.EndDate,
                    NewAnnouncement.Category,
                    NewAnnouncement.Location,
                    userPhone
                );

                TempData["SuccessMessage"] = $"Announcement '{NewAnnouncement.Name}' created successfully!";
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error creating announcement: {ex.Message}");
                LoadPageData();
                return Page();
            }
        }
    }
}

