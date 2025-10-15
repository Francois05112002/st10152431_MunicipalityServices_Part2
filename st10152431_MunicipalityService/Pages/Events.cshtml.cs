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

        // Filter properties
        [BindProperty(SupportsGet = true)]
        public string EventCategoryFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? EventDateFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public string AnnouncementCategoryFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? AnnouncementDateFilter { get; set; }

        // New item properties
        [BindProperty]
        public Event NewEvent { get; set; }

        [BindProperty]
        public Announcement NewAnnouncement { get; set; }

        // Display properties
        public List<Event> FilteredEvents { get; set; }
        public List<Announcement> FilteredAnnouncements { get; set; }
        public SelectList EventCategories { get; set; }
        public SelectList AnnouncementCategories { get; set; }
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

            // Load category dropdowns with "All Categories" option
            var eventCats = new List<string> { "All Categories" };
            eventCats.AddRange(_eventService.GetEventCategories());
            EventCategories = new SelectList(eventCats);

            var announcementCats = new List<string> { "All Categories" };
            announcementCats.AddRange(_eventService.GetAnnouncementCategories());
            AnnouncementCategories = new SelectList(announcementCats);

            // Apply filters using LIST filtering (O(n) but acceptable)
            FilteredEvents = _eventService.FilterEvents(EventCategoryFilter, EventDateFilter);
            FilteredAnnouncements = _eventService.FilterAnnouncements(
                AnnouncementCategoryFilter,
                AnnouncementDateFilter);
        }

        public IActionResult OnPostCreateEvent()
        {
            // Check if user is logged in FIRST
            var userPhone = HttpContext.Session.GetString("UserPhone");

            if (string.IsNullOrEmpty(userPhone))
            {
                ModelState.AddModelError(string.Empty, "You must be logged in to create events.");
                LoadPageData();
                return Page();
            }

            // Manual validation instead of ModelState.IsValid
            if (string.IsNullOrWhiteSpace(NewEvent?.Name))
            {
                ModelState.AddModelError(string.Empty, "Event name is required.");
                LoadPageData();
                return Page();
            }

            if (string.IsNullOrWhiteSpace(NewEvent?.Category) || NewEvent.Category == "All Categories")
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

            // Validate dates
            if (NewEvent.EndDate < NewEvent.StartDate)
            {
                ModelState.AddModelError(string.Empty, "End date must be after start date.");
                LoadPageData();
                return Page();
            }

            try
            {
                // Add event to LIST (O(1) append)
                int eventId = _eventService.AddEvent(
                    NewEvent.Name,
                    NewEvent.StartDate,
                    NewEvent.EndDate,
                    NewEvent.Category,
                    NewEvent.Location,
                    userPhone  // ← This gets set here!
                );

                TempData["SuccessMessage"] = $"Event '{NewEvent.Name}' created successfully!";

                // Redirect to clear form
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
            // Check if user is logged in FIRST
            var userPhone = HttpContext.Session.GetString("UserPhone");

            if (string.IsNullOrEmpty(userPhone))
            {
                ModelState.AddModelError(string.Empty, "You must be logged in to create announcements.");
                LoadPageData();
                return Page();
            }

            // Manual validation instead of ModelState.IsValid
            if (string.IsNullOrWhiteSpace(NewAnnouncement?.Name))
            {
                ModelState.AddModelError(string.Empty, "Announcement name is required.");
                LoadPageData();
                return Page();
            }

            if (string.IsNullOrWhiteSpace(NewAnnouncement?.Category) || NewAnnouncement.Category == "All Categories")
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

            // Validate dates
            if (NewAnnouncement.EndDate < NewAnnouncement.StartDate)
            {
                ModelState.AddModelError(string.Empty, "End date must be after start date.");
                LoadPageData();
                return Page();
            }

            try
            {
                // Add announcement to LIST (O(1) append)
                int announcementId = _eventService.AddAnnouncement(
                    NewAnnouncement.Name,
                    NewAnnouncement.StartDate,
                    NewAnnouncement.EndDate,
                    NewAnnouncement.Category,
                    NewAnnouncement.Location,
                    userPhone  // ← This gets set here!
                );

                TempData["SuccessMessage"] = $"Announcement '{NewAnnouncement.Name}' created successfully!";

                // Redirect to clear form
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
