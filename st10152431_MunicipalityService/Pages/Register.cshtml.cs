using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using st10152431_MunicipalityService.Services;
using st10152431_MunicipalityService.Models;

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
            public string Name { get; set; }
            public string CellphoneNumber { get; set; }
        }

        public void OnGet()
        {
            // Always reload user from service for fresh data
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
            if (string.IsNullOrWhiteSpace(Input.Name) || string.IsNullOrWhiteSpace(Input.CellphoneNumber))
            {
                Message = "Please provide both name and cellphone number.";
                return Page();
            }

            bool success = _userService.RegisterUser(Input.Name, Input.CellphoneNumber);

            if (success)
            {
                HttpContext.Session.SetString("UserPhone", Input.CellphoneNumber);
                UserProfile = _userService.GetUser(Input.CellphoneNumber);
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
            if (string.IsNullOrWhiteSpace(Input.CellphoneNumber))
            {
                Message = "Please provide a cellphone number.";
                return Page();
            }

            User user = _userService.LoginUser(Input.CellphoneNumber);

            if (user != null)
            {
                HttpContext.Session.SetString("UserPhone", Input.CellphoneNumber);
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
