using System;
using System.Diagnostics;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using st10152431_MunicipalityService.Models;
using st10152431_MunicipalityService.Services;

namespace st10152431_MunicipalityService.Pages
{
    public class ReportModel : PageModel
    {
        private readonly ReportService _reportService;
        private readonly UserService _userService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        [BindProperty]
        public ReportInput Input { get; set; }

        public List<string> Categories { get; set; }
        public User CurrentUser { get; set; }
        public DailyPulse TodayPulse { get; set; }
        public bool ShowPulseForm { get; set; }
        public string PulseAnswer { get; set; }
        public string Message { get; set; }

        public ReportModel(ReportService reportService, UserService userService, IWebHostEnvironment webHostEnvironment)
        {
            _reportService = reportService;
            _userService = userService;
            _webHostEnvironment = webHostEnvironment;
        }

        public void OnGet()
        {
            Categories = _reportService.GetCategories();

            var userPhone = HttpContext.Session.GetString("UserPhone");
            if (!string.IsNullOrEmpty(userPhone))
            {
                CurrentUser = _userService.GetUser(userPhone);

                TodayPulse = _reportService.GetTodayPulse();
                string today = DateTime.Now.ToString("yyyy-MM-dd");
                bool hasAnswered = _reportService.HasAnsweredToday(userPhone, today);

                ShowPulseForm = !hasAnswered;
                PulseAnswer = hasAnswered ? _reportService.GetUserAnswerToday(userPhone, today) : null;
            }
            else
            {
                CurrentUser = null;
                ShowPulseForm = false;
                PulseAnswer = null;
            }
        }

        public IActionResult OnPost()
        {
            Console.WriteLine("***********ONPOST WAS CALLED************");

            // ===== DEBUG LOGGING AT THE VERY START =====
            Console.WriteLine("========== STARTING ISSUE REPORT ==========");
            Console.WriteLine($"Input object is null: {Input == null}");

            if (Input != null)
            {
                Console.WriteLine($"Location: '{Input.Location ?? "NULL"}'");
                Console.WriteLine($"Category: '{Input.Category ?? "NULL"}'");
                Console.WriteLine($"Description: '{Input.Description ?? "NULL"}'");
            }

            // Validation with debug output
            if (Input == null ||
                string.IsNullOrWhiteSpace(Input.Location) ||
                string.IsNullOrWhiteSpace(Input.Category) ||
                string.IsNullOrWhiteSpace(Input.Description))
            {
                Console.WriteLine("VALIDATION FAILED!");
                Message = "Please fill in all required fields.";
                OnGet();
                return Page();
            }

            Console.WriteLine("VALIDATION PASSED!");

            // Handle image
            string imagePath = null;
            if (Input.Image != null)
            {
                imagePath = SaveImage(Input.Image);
                Console.WriteLine($"Image saved: {imagePath ?? "NULL"}");
            }

            // Get user phone from session
            var userPhone = HttpContext.Session.GetString("UserPhone");
            Console.WriteLine($"UserPhone from session: '{userPhone ?? "NULL"}'");
            Console.WriteLine($"Is user logged in: {!string.IsNullOrEmpty(userPhone)}");

            // Add to global list
            int issueId = _reportService.AddIssue(
                Input.Location,
                Input.Category,
                Input.Description,
                imagePath,
                userPhone
            );

            Console.WriteLine($"Issue ID created: {issueId}");
            Console.WriteLine($"Total issues in system: {_reportService.GetAllIssues().Count}");

            // If logged in, add to user's personal list
            if (!string.IsNullOrEmpty(userPhone))
            {
                Console.WriteLine("Attempting to add issue to user's list...");

                var user = _userService.GetUser(userPhone);

                Console.WriteLine($"User found: {user != null}");

                if (user != null)
                {
                    Console.WriteLine($"User name: {user.Name}");
                    Console.WriteLine($"User issues count BEFORE: {user.Issues.Count}");

                    var newIssue = new Issue
                    {
                        Id = issueId,
                        Location = Input.Location,
                        Category = Input.Category,
                        Description = Input.Description,
                        ImagePath = imagePath,
                        Timestamp = DateTime.Now,
                        UserId = userPhone
                    };

                    // Use the service method
                    _userService.AddIssueToUser(userPhone, newIssue);

                    Console.WriteLine($"User issues count AFTER: {user.Issues.Count}");
                    Console.WriteLine("Issue added to user's list successfully!");
                }
                else
                {
                    Console.WriteLine("ERROR: User not found in UserService!");
                }
            }
            else
            {
                Console.WriteLine("Anonymous report - not adding to user list");
            }

            Console.WriteLine("==========================================");

            Message = $"Issue #{issueId} reported successfully! Thank you for helping improve our community.";

            ModelState.Clear();
            Input = new ReportInput();
            OnGet();

            return Page();
        }

        public IActionResult OnPostPulse(string pulseChoice)
        {
            var userPhone = HttpContext.Session.GetString("UserPhone");
            if (string.IsNullOrEmpty(userPhone))
            {
                Message = "You must be logged in to participate in the daily pulse.";
                OnGet();
                return Page();
            }

            if (string.IsNullOrWhiteSpace(pulseChoice))
            {
                Message = "Please select an answer.";
                OnGet();
                return Page();
            }

            string today = DateTime.Now.ToString("yyyy-MM-dd");
            bool success = _reportService.SubmitPulseAnswer(userPhone, today, pulseChoice);

            if (success)
            {
                var user = _userService.GetUser(userPhone);
                if (user != null)
                    user.PulseDates.Add(today);

                Message = "Thank you for participating in today's community pulse!";
            }
            else
            {
                Message = "You have already answered today's pulse.";
            }

            OnGet();
            return Page();
        }

        private string SaveImage(IFormFile image)
        {
            try
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = Guid.NewGuid() + "_" + image.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                    image.CopyTo(fileStream);

                return "/uploads/" + uniqueFileName;
            }
            catch
            {
                return null;
            }
        }
    }
}

