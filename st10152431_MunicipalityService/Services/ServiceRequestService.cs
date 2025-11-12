using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using st10152431_MunicipalityService.Data;
using st10152431_MunicipalityService.Models;

namespace st10152431_MunicipalityService.Services
{
    public class ServiceRequestService
    {
        private readonly AppDbContext _context;

        public ServiceRequestService(AppDbContext context)
        {
            _context = context;
        }

        // Create a new service request
        public void CreateRequest(ServiceRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            request.SubmittedDate = DateTime.Now;
            request.LastUpdated = DateTime.Now;
            request.Status = "Pending";
            _context.ServiceRequests.Add(request);
            _context.SaveChanges();
        }

        // Get a service request by RequestId (string)
        public ServiceRequest GetRequestById(string requestId)
        {
            return _context.ServiceRequests
                .AsNoTracking()
                .FirstOrDefault(r => r.RequestId == requestId);
        }

        // Get all service requests (for employees/admin)
        public List<ServiceRequest> GetAllRequests()
        {
            return _context.ServiceRequests
                .OrderByDescending(r => r.SubmittedDate)
                .ToList();
        }

        // Get all requests for a specific user
        public List<ServiceRequest> GetRequestsByUser(string userId)
        {
            if (string.IsNullOrEmpty(userId)) return new List<ServiceRequest>();
            return _context.ServiceRequests
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.SubmittedDate)
                .ToList();
        }

        // Update a service request (status, priority, etc.)
        public bool UpdateRequest(ServiceRequest updated)
        {
            var existing = _context.ServiceRequests.FirstOrDefault(r => r.RequestId == updated.RequestId);
            if (existing == null) return false;

            existing.Status = updated.Status;
            existing.Priority = updated.Priority;
            existing.Category = updated.Category;
            existing.Title = updated.Title;
            existing.Description = updated.Description;
            existing.Location = updated.Location;
            existing.Latitude = updated.Latitude;
            existing.Longitude = updated.Longitude;
            existing.EstimatedCompletion = updated.EstimatedCompletion;
            existing.Notes = updated.Notes;
            existing.Dependencies = updated.Dependencies ?? new List<string>();
            existing.LastUpdated = DateTime.Now;

            _context.SaveChanges();
            return true;
        }

        // Delete/cancel a service request
        public bool DeleteRequest(string requestId)
        {
            var req = _context.ServiceRequests.FirstOrDefault(r => r.RequestId == requestId);
            if (req == null) return false;
            _context.ServiceRequests.Remove(req);
            _context.SaveChanges();
            return true;
        }

        // Get requests by status (for filtering)
        public List<ServiceRequest> GetRequestsByStatus(string status)
        {
            return _context.ServiceRequests
                .Where(r => r.Status == status)
                .OrderByDescending(r => r.SubmittedDate)
                .ToList();
        }

        // Get statistics (counts by status)
        public Dictionary<string, int> GetStatusCounts()
        {
            return _context.ServiceRequests
                .GroupBy(r => r.Status)
                .ToDictionary(g => g.Key, g => g.Count());
        }
    }
}


