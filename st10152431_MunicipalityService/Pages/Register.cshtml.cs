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

        [TempData]
        public string SuccessMessage { get; set; }

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
            // If already logged in as user, show profile
            var userPhone = HttpContext.Session.GetString("UserPhone");
            var employeePhone = HttpContext.Session.GetString("EmployeePhone");
            if (!string.IsNullOrEmpty(userPhone))
            {
                UserProfile = _userService.GetUser(userPhone);
            }
        }

        public IActionResult OnPost()
        {
            var action = Request.Form["action"];
            if (action == "register")
            {
                // Only validate for registration
                if (!ModelState.IsValid)
                {
                    Message = "Please correct the errors and try again.";
                    return Page();
                }
                if (_userService.RegisterUser(Input.Name.Trim(), Input.CellphoneNumber.Trim()))
                {
                    HttpContext.Session.SetString("UserPhone", Input.CellphoneNumber.Trim());
                    HttpContext.Session.Remove("EmployeePhone");
                    return RedirectToPage("/Index");
                }
                else
                {
                    Message = "User already exists.";
                    return Page();
                }
            }
            else if (action == "login")
            {
                // Remove Name validation error for login
                ModelState.Remove("Input.Name");

                var phone = Input.CellphoneNumber?.Trim();
                if (string.IsNullOrEmpty(phone) || !Regex.IsMatch(phone, @"^\d{10}$"))
                {
                    Message = "Please enter a valid 10-digit cellphone number.";
                    return Page();
                }

                if (phone == "1111111111")
                {
                    // Employee login
                    HttpContext.Session.SetString("EmployeePhone", phone);
                    HttpContext.Session.Remove("UserPhone");
                    SuccessMessage = "Employee login successful!";
                    return Page(); // Stay on Register page, show message
                }
                else if (_userService.UserExists(phone))
                {
                    // Regular user login
                    HttpContext.Session.SetString("UserPhone", phone);
                    HttpContext.Session.Remove("EmployeePhone");
                    return RedirectToPage("/Index");
                }
                else
                {
                    Message = "Invalid login.";
                    return Page();
                }
            }
            else if (action == "logout")
            {
                HttpContext.Session.Remove("UserPhone");
                HttpContext.Session.Remove("EmployeePhone");
                UserProfile = null;
                SuccessMessage = null;
                Message = "You have been logged out successfully.";
                return RedirectToPage("/Register");
            }
            return Page();
        }
    }
}

