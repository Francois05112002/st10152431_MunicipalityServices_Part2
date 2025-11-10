using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using st10152431_MunicipalityService.Services;
using st10152431_MunicipalityService.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace st10152431_MunicipalityService.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly UserService _userService;

        [BindProperty]
        public InputModel Input { get; set; }

        public User UserProfile { get; set; }
        public string Message { get; set; }

        public RegisterModel(UserService userService)
        {
            _userService = userService;
        }

        public class InputModel
        {
            [Required(ErrorMessage = "Name is required")]
            [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Name must contain only letters and spaces")]
            public string Name { get; set; }

            [Required(ErrorMessage = "Cellphone number is required")]
            [RegularExpression(@"^\d{10}$", ErrorMessage = "Cellphone number must be exactly 10 digits")]
            public string CellphoneNumber { get; set; }
        }

        public void OnGet()
        {
            var loggedInPhone = HttpContext.Session.GetString("UserPhone");
            if (!string.IsNullOrEmpty(loggedInPhone))
            {
                UserProfile = _userService.GetUser(loggedInPhone);
            }
        }

        public IActionResult OnPost(string action)
        {
            if (action == "register")
                return HandleRegister();
            else if (action == "login")
                return HandleLogin();
            else if (action == "logout")
                return HandleLogout();

            return Page();
        }

        private IActionResult HandleRegister()
        {
            // Server-side validation
            if (!ModelState.IsValid)
            {
                Message = "Please correct the errors and try again.";
                return Page();
            }

            // Defensive validation (in case client-side is bypassed)
            if (!Regex.IsMatch(Input.Name ?? "", @"^[A-Za-z\s]+$"))
            {
                ModelState.AddModelError("Input.Name", "Name must contain only letters and spaces");
                Message = "Please correct the errors and try again.";
                return Page();
            }
            if (!Regex.IsMatch(Input.CellphoneNumber ?? "", @"^\d{10}$"))
            {
                ModelState.AddModelError("Input.CellphoneNumber", "Cellphone number must be exactly 10 digits");
                Message = "Please correct the errors and try again.";
                return Page();
            }

            bool success = _userService.RegisterUser(Input.Name.Trim(), Input.CellphoneNumber.Trim());

            if (success)
            {
                HttpContext.Session.SetString("UserPhone", Input.CellphoneNumber.Trim());
                UserProfile = _userService.GetUser(Input.CellphoneNumber.Trim());
                Message = "Registration successful! You are now logged in.";
            }
            else
            {
                Message = "User with this cellphone number already exists. Please log in instead.";
            }

            return Page();
        }

        private IActionResult HandleLogin()
        {
            // Only validate cellphone for login
            if (string.IsNullOrWhiteSpace(Input.CellphoneNumber))
            {
                ModelState.AddModelError("Input.CellphoneNumber", "Cellphone number is required");
                Message = "Please provide a cellphone number.";
                return Page();
            }
            if (!Regex.IsMatch(Input.CellphoneNumber ?? "", @"^\d{10}$"))
            {
                ModelState.AddModelError("Input.CellphoneNumber", "Cellphone number must be exactly 10 digits");
                Message = "Please enter a valid 10-digit cellphone number.";
                return Page();
            }

            User user = _userService.LoginUser(Input.CellphoneNumber.Trim());

            if (user != null)
            {
                HttpContext.Session.SetString("UserPhone", Input.CellphoneNumber.Trim());
                UserProfile = user;
                Message = $"Welcome back, {user.Name}!";
            }
            else
            {
                Message = "User not found. Please register first.";
            }

            return Page();
        }

        private IActionResult HandleLogout()
        {
            HttpContext.Session.Remove("UserPhone");
            UserProfile = null;
            Message = "You have been logged out successfully.";

            return Page();
        }
    }
}
