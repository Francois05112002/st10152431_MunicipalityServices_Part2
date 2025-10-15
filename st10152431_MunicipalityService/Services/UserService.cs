using System;
using System.Collections.Generic;
using System.Linq;
using st10152431_MunicipalityService.Models;

namespace st10152431_MunicipalityService.Services
{
    public class UserService
    {
        // DICTIONARY: O(1) lookup by phone number - perfect for login/authentication
        private static Dictionary<string, User> _users = new Dictionary<string, User>();

        /// <summary>
        /// Register a new user
        /// </summary>
        public bool RegisterUser(string name, string cellphoneNumber)
        {
            // Check if user already exists (O(1) lookup)
            if (_users.ContainsKey(cellphoneNumber))
            {
                return false; // User already exists
            }

            // Create new user and add to dictionary (O(1) insertion)
            var newUser = new User(name, cellphoneNumber);
            _users[cellphoneNumber] = newUser;

            return true;
        }

        /// <summary>
        /// Login user - returns user if exists, null otherwise
        /// </summary>
        public User LoginUser(string cellphoneNumber)
        {
            // O(1) lookup in dictionary
            if (_users.TryGetValue(cellphoneNumber, out User user))
            {
                return user;
            }

            return null; // User not found
        }

        /// <summary>
        /// Get user by phone number
        /// </summary>
        public User GetUser(string cellphoneNumber)
        {
            // O(1) lookup
            _users.TryGetValue(cellphoneNumber, out User user);
            return user;
        }

        /// <summary>
        /// Check if user exists
        /// </summary>
        public bool UserExists(string cellphoneNumber)
        {
            // O(1) lookup
            return _users.ContainsKey(cellphoneNumber);
        }

        /// <summary>
        /// Add an issue to a user's profile (if logged in)
        /// </summary>
        public void AddIssueToUser(string cellphoneNumber, Issue issue)
        {
            if (_users.TryGetValue(cellphoneNumber, out User user))
            {
                // LIST append is O(1) amortized
                user.Issues.Add(issue);
            }
        }

        /// <summary>
        /// Record daily pulse participation for a user
        /// Returns true if successfully added, false if already participated today
        /// </summary>
        public bool RecordPulseParticipation(string cellphoneNumber, DateTime date)
        {
            if (_users.TryGetValue(cellphoneNumber, out User user))
            {
                string dateString = date.ToString("yyyy-MM-dd");

                // HASHSET Add returns false if element already exists (O(1) operation)
                // This automatically prevents duplicate participation
                return user.PulseDates.Add(dateString);
            }

            return false;
        }

        /// <summary>
        /// Check if user has participated in pulse today
        /// </summary>
        public bool HasParticipatedToday(string cellphoneNumber, DateTime date)
        {
            if (_users.TryGetValue(cellphoneNumber, out User user))
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
            return _users.Count;
        }

        /// <summary>
        /// Get all users (for admin purposes if needed)
        /// </summary>
        public IEnumerable<User> GetAllUsers()
        {
            return _users.Values;
        }
    }
}