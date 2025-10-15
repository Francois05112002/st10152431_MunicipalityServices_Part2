using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using st10152431_MunicipalityService.Data;
using st10152431_MunicipalityService.Models;

namespace st10152431_MunicipalityService.Services
{
    public class UserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Register a new user
        /// Uses DICTIONARY pattern: O(1) lookup to check if user exists
        /// </summary>
        public bool RegisterUser(string name, string cellphoneNumber)
        {
            // O(1) lookup in database using primary key
            if (_context.Users.Any(u => u.CellphoneNumber == cellphoneNumber))
            {
                return false; // User already exists
            }

            // Create new user with empty LIST and HASHSET
            var newUser = new User(name, cellphoneNumber);

            _context.Users.Add(newUser);
            _context.SaveChanges();

            return true;
        }

        /// <summary>
        /// Login user - returns user if exists, null otherwise
        /// DICTIONARY pattern: O(1) lookup by phone number (primary key)
        /// </summary>
        public User LoginUser(string cellphoneNumber)
        {
            // O(1) lookup using primary key, eagerly load Issues collection
            var user = _context.Users
                .Include(u => u.Issues) // Load the LIST of issues
                .FirstOrDefault(u => u.CellphoneNumber == cellphoneNumber);

            return user;
        }

        /// <summary>
        /// Get user by phone number
        /// DICTIONARY pattern: O(1) lookup
        /// </summary>
        public User GetUser(string cellphoneNumber)
        {
            // O(1) lookup using primary key
            return _context.Users
                .Include(u => u.Issues) 
                .FirstOrDefault(u => u.CellphoneNumber == cellphoneNumber);
        }

        /// <summary>
        /// Check if user exists
        /// DICTIONARY pattern: O(1) lookup
        /// </summary>
        public bool UserExists(string cellphoneNumber)
        {
            // O(1) lookup using primary key
            return _context.Users.Any(u => u.CellphoneNumber == cellphoneNumber);
        }

        /// <summary>
        /// Add an issue to a user's profile (if logged in)
        /// LIST pattern: O(1) append operation
        /// </summary>
        //public void AddIssueToUser(string cellphoneNumber, Issue issue)
        //{
        //    var user = _context.Users
        //        .Include(u => u.Issues)
        //        .FirstOrDefault(u => u.CellphoneNumber == cellphoneNumber);

        //    if (user != null)
        //    {
        //        // LIST append - O(1) amortized
        //        user.Issues.Add(issue);
        //        _context.SaveChanges();
        //    }
        //}

        /// <summary>
        /// Record daily pulse participation for a user
        /// HASHSET pattern: O(1) add operation with automatic duplicate prevention
        /// Returns true if successfully added, false if already participated today
        /// </summary>
        public bool RecordPulseParticipation(string cellphoneNumber, DateTime date)
        {
            var user = _context.Users.Find(cellphoneNumber);

            if (user != null)
            {
                string dateString = date.ToString("yyyy-MM-dd");

                // HASHSET Add returns false if element already exists (O(1) operation)
                // This automatically prevents duplicate participation
                bool added = user.PulseDates.Add(dateString);

                if (added)
                {
                    _context.SaveChanges();
                }

                return added;
            }

            return false;
        }

        /// <summary>
        /// Check if user has participated in pulse today
        /// HASHSET pattern: O(1) contains check
        /// </summary>
        public bool HasParticipatedToday(string cellphoneNumber, DateTime date)
        {
            var user = _context.Users.Find(cellphoneNumber);

            if (user != null)
            {
                string dateString = date.ToString("yyyy-MM-dd");

                // HASHSET Contains is O(1)
                return user.PulseDates.Contains(dateString);
            }

            return false;
        }

        /// <summary>
        /// Get total number of registered users
        /// </summary>
        public int GetTotalUsers()
        {
            return _context.Users.Count();
        }

        /// <summary>
        /// Get all users (for admin purposes if needed)
        /// </summary>
        public IEnumerable<User> GetAllUsers()
        {
            return _context.Users.Include(u => u.Issues).ToList();
        }
    }
}