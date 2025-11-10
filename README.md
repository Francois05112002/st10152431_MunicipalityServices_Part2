# Municipality Service Web Application

*Student:* ST10152431 - Louis Francois Fourie - Group 2  
*Course:* PROG7312 - Programming 3B - Advanced Application Development  
*Portfolio Of Evidence - Part 2*

---

## Project Description

A comprehensive web application for managing municipal services, including:
- Issue reporting (with optional image uploads)
- Event management with category/date filtering
- Announcements for residents
- Community engagement via daily pulse surveys
- User registration and authentication using cellphone number only (privacy-focused)

---

## Data Structures Implemented

### 1. Dictionary<string, User> -- O(1) Lookup
- *Purpose:* Fast user authentication and profile management, keyed by cellphone number
- *Implementation:* Each registered user is stored in a dictionary, enabling constant-time lookups during login and session management
- *Logic:* Used extensively in UserService for sign-in, registration, and profile retrieval

### 2. List<Issue> -- O(1) Append, O(n) Filter
- *Purpose:* Chronological storage of reported issues (each issue is appended)
- *Implementation:* Issues are stored in a list; new reports are added to the end, ensuring quick insertion and iteration in order
- *Logic:* Used in ReportService for adding, searching, and filtering issues linked to users

### 3. HashSet<string> -- O(1) Unique Value Storage
- *Purpose:* Tracks which users have participated in daily pulse surveys; prevents duplicate responses
- *Implementation:* Each user has a HashSet of pulse dates; O(1) add and lookup guarantees no duplicate participation per day
- *Logic:* Managed in both UserService and ReportService for pulse tracking

### 4. *Nested Structures*
- *User* contains a List<Issue> (one-to-many: a user can report many issues)
- *User* contains a HashSet<string> for daily pulse participation dates
- *Event* and *Announcement* linked to users by creator
- *Programming logic* leverages these relationships to enforce business rules (e.g., one pulse per user per day, personal issue history, event creation permissions)

---

## Technologies Used

- ASP.NET Core 8.0 (Razor Pages)
- Entity Framework Core 8.0 (Code-First, automatic migrations)
- SQL Server LocalDB
- C# 12
- HTML5, CSS3, Bootstrap 5

---

## Prerequisites
- Visual Studio 2022 (Community or higher)
- .NET 8.0 SDK
- SQL Server LocalDB (included with Visual Studio)

---

## Setup Instructions

### Step 1: Clone the Repository
bash
git clone https://github.com/Francois05112002/st10152431_MunicipalityServices_Part2.git
cd st10152431_MunicipalityServices_Part2


### Step 2: Open in Visual Studio
- Open st10152431_MunicipalityService.sln in Visual Studio 2022
- NuGet packages will restore automatically

### Step 3: Create the Database
Open *Package Manager Console* (Tools → NuGet Package Manager → Package Manager Console) and run:
powershell
Update-Database

This will:
- Create the MunicipalityDB database
- Create all required tables (Users, Issues, Events, Announcements, PulseResponses)
- *Seed sample data*, including test users, issues, events, and announcements

### Step 4: Run the Application
- Press *F5* or click the *Play* button in Visual Studio
- The app will open in your default browser
- Navigate to https://localhost:7205 or http://localhost:5010

---

## Test User Credentials (Seeded Values)

The database is automatically seeded with these test accounts:

| Phone Number | Name         | Purpose                          |
|--------------|--------------|----------------------------------|
| 0817246624   | Test User    | Primary test account             |
| 0123456789   | John Doe     | Secondary test account           |
| 0987654321   | Jane Smith   | Additional test account          |

*How to test:*
1. Click "Login" or "Register"
2. Enter one of the phone numbers above
3. Explore all features with pre-populated data

---

## Features

### For All Users (Anonymous)
- View events and announcements
- Filter events by category and date
- Report municipal issues (no login required)
- See event details

### For Logged-In Users
- All anonymous features, PLUS:
- Personal issue tracking (reported issues per user)
- Create events and announcements
- Participate in daily community pulse
- View personal profile with statistics (issues reported, pulse participation)

---

## Project Structure


st10152431_MunicipalityService/
├── Data/
│   └── AppDbContext.cs          # EF Core DbContext, data seeding
├── Migrations/                  # EF Core migration history
├── Models/                      # Data models: User, Issue, Event, Announcement, PulseResponse
│   ├── User.cs                  # Contains List<Issue>, HashSet<string>
│   ├── Issue.cs
│   ├── Event.cs
│   ├── Announcement.cs
│   └── PulseResponse.cs
├── Pages/                       # Razor Pages (UI & logic)
│   ├── Index.cshtml             # Home page
│   ├── Register.cshtml          # Registration/Login/Profile
│   ├── Report.cshtml            # Issue reporting
│   ├── Events.cshtml            # Events & announcements
│   └── Shared/                  # Layout, partials, validation scripts
├── Services/                    # Business logic, data manipulation
│   ├── UserService.cs           # User dictionary, pulse tracking
│   ├── ReportService.cs         # Issue list, validation
│   └── EventService.cs          # Event list, filtering
├── wwwroot/                     # Static files (CSS, JS, images)
└── Program.cs                   # App startup configuration


---

## Data Structure Analysis

| Operation              | Data Structure           | Time Complexity | Location        |
|------------------------|-------------------------|-----------------|-----------------|
| User Lookup            | Dictionary (PK)         | O(1)            | UserService     |
| User Registration      | Dictionary Insert       | O(1)            | UserService     |
| Add Issue              | List Append             | O(1)            | ReportService   |
| Check Pulse            | HashSet Contains        | O(1)            | UserService     |
| Add Pulse Date         | HashSet Add             | O(1)            | UserService     |
| Filter Issues/Events   | List Filter             | O(n)            | Report/EventSvc |

### Space Complexity
- User storage: O(n), where n = users
- Issue/Event storage: O(m), where m = issues/events
- Nested structures: O(n × k), where k = average issues per user

---

## Database Schema

*Tables Created:*
- *Users:* PK = CellphoneNumber
- *Issues:* FK = UserId (nullable for anonymous)
- *Events:* FK = CreatedBy
- *Announcements:* FK = CreatedBy
- *PulseResponses:* Unique index = (Date, UserId)

---

## Troubleshooting

*Database Connection Issues*
- Verify SQL Server LocalDB is installed ((localdb)\MSSQLLocalDB)
- Check via SQL Server Object Explorer in Visual Studio
- If problems persist, run Update-Database again

*Migration Errors*
- Remove all migrations and start fresh:
    powershell
    Remove-Migration
    Add-Migration InitialCreate
    Update-Database
    

*Port Conflicts*
- Edit Properties/launchSettings.json and change port numbers if needed

---

## Notes for Lecturer

- *No database file required:* DB is created automatically on first run via EF Core migrations
- *Test data is seeded:* Sample users, issues, events, announcements available on initialization
- *Data structures showcased throughout:* Dictionary for users, List for issues/events, HashSet for pulse tracking
- *Expired events/announcements hidden:* Date filtering logic using O(n) complexity

---

## Learning Outcomes Demonstrated

- ✅ Implementation of complex data structures (Dictionary, List, HashSet)
- ✅ Time complexity analysis and practical application (O(1) lookup/insert, O(n) filtering)
- ✅ Database persistence with Entity Framework Core
- ✅ CRUD operations and robust validation
- ✅ One-to-many relationships (User → Issues)
- ✅ Session management for authentication
- ✅ Responsive web design using Bootstrap
- ✅ Clean, modular architecture (Models, Services, Razor Pages)

---

*For lecturer sign-in, use any of the following seeded phone numbers:*  
- 0817246624 (Test User)  
- 0123456789 (John Doe)  
- 0987654321 (Jane Smith)  

These accounts have sample data and can be used to demonstrate all features.
