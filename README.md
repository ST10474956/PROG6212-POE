# RaceDay — Part 3: Automation of the Web Application

## 1. Project overview
RaceDay is a web platform for running, walking and cycling events. This Part 3 solution adds an ASP.NET Core MVC website to the API project supplied for Part 2. The MVC application does **not** connect to SQL Server and contains no database context. All data requests are sent to the REST API through `RaceDayApiClient`.

### Roles
- **Organiser:** dashboard, event creation/editing/deletion, category management, participant enrolments and results entry.
- **Participant:** browse events, view event details/categories, enter an event, review entries and results, and update profile information.
- **Visitor:** browse public events and register or log in.

## 2. Folder structure
```text
RaceDay_Part3_Submission/
├── RaceDay.Api/                 # Existing Part 2 API + Azure upload endpoints
│   ├── Controllers/             # REST endpoints
│   ├── Data/                    # EF Core context (API only)
│   ├── Models/                  # API entities
│   ├── Services/                # Password hashing and JWT
│   ├── Dockerfile
│   └── RaceDay.Api.csproj
├── RaceDay.Mvc/                 # New Part 3 MVC website
│   ├── Controllers/             # MVC actions and role-specific pages
│   ├── Models/                  # Form/view models (not database entities)
│   ├── Services/RaceDayApiClient.cs # API-only data access
│   ├── Views/                   # Razor pages
│   ├── wwwroot/css/site.css
│   └── RaceDay.Mvc.csproj
├── RaceDay.Tests/
├── .github/workflows/raceday-ci.yml
├── docs/
├── Dockerfile                  # MVC container
├── docker-compose.yml          # API + MVC orchestration
├── .dockerignore
├── RaceDay.sln
└── README.md
```

## 3. Requirements and implementation

### MVC consumes the API
`RaceDay.Mvc/Services/RaceDayApiClient.cs` is the single HTTP integration service. It sends GET/POST/PUT/DELETE requests to the API and attaches the logged-in user's bearer token. No MVC file references EF Core, SQL Server or `RaceDayDbContext`.

### Session authentication and role-aware navigation
The API login endpoint returns a JWT and user details. The MVC website stores the token, user ID, role and display name in ASP.NET Core session. The MVC role checks redirect users without a valid session to Login and show distinct Organiser and Participant navigation. Logout clears the session. This is session-based state management in MVC while the API retains the JWT contract from the supplied Part 2 implementation.

**Important alignment note:** The supplied Part 2 API uses JWT bearer authentication, not server-side API sessions. This Part 3 MVC layer maintains the authenticated state in ASP.NET Core session and forwards the API JWT on each protected API request. If the lecturer requires the API itself to use cookie/session authentication rather than JWT, that API contract must be changed in Part 2 as well.

### Organiser features
- Dashboard summary of events and upcoming events.
- Event create, edit and delete with name, description, date, location, distance and type.
- Category creation for an event.
- Enrolment list with participant, category, date and status.
- Results entry with finish time and finishing position.

### Participant features
- Upcoming event list and event detail/category view.
- Enrolment action sends the request to the API immediately.
- My Entries shows category and status.
- My Results shows event, date, category, finish time and position.
- Profile page updates name and profile image.

### Azure Blob Storage
The MVC site sends image files to `POST /api/uploads/event-banner` or `POST /api/uploads/profile-picture`. The API validates image type and size, stores the file in Azure Blob Storage and returns its URL. The MVC then saves that URL through the existing event/profile API endpoints. The MVC does not connect to Azure directly.

Supported file types: JPG, PNG and WEBP. Maximum file size: 5 MB. Configure Azure before testing image uploads. The current sample creates public-read image containers so the returned blob URL can be displayed by browsers; enable blob public access on the Storage account/container for this demonstration. For a production deployment, prefer private containers and short-lived read-only SAS URLs or an authenticated image proxy.

### Design
The website uses Bootstrap 5, a consistent dark/navy and race-red theme, responsive cards and tables, clear status badges and role-appropriate navigation.

## 4. Requirements
- .NET 8 SDK
- SQL Server / SQL Server Express / LocalDB
- Visual Studio 2022 with ASP.NET and web development workload, or VS Code
- Azure Storage account and Blob connection string for image uploads
- Docker Desktop for container execution

## 5. Run locally (Visual Studio / terminal)

### Step A — Configure the API
Edit `RaceDay.Api/appsettings.json` or use environment variables/user-secrets. Set:
- `ConnectionStrings:RaceDay` to your SQL Server connection string.
- `ConnectionStrings:AzureBlobStorage` to your Azure Storage connection string.
- `Jwt:Key` to a long random secret (at least 32 characters).

Do not commit real connection strings or production secrets to GitHub. Use .NET user-secrets or environment variables.

Run `database/RaceDayDB.sql` in SQL Server Management Studio to create the database and the six tables. This script follows the Users/Roles structure used by the supplied Part 2 API and Part 1 SQL section; it also seeds the two roles. Register your own Organiser and Participant accounts through the MVC Register page because placeholder password hashes in planning examples are not usable login credentials. Then start the API:
```bash
dotnet restore RaceDay.sln
dotnet build RaceDay.sln
dotnet run --project RaceDay.Api --urls http://localhost:5080
```
Open `http://localhost:5080/swagger` to confirm the API is running.

### Step B — Configure and start MVC
`RaceDay.Mvc/appsettings.json` defaults to `http://localhost:5080/`. If your API uses another address, change `Api:BaseUrl`.

In a second terminal:
```bash
dotnet run --project RaceDay.Mvc --urls http://localhost:5081
```
Open `http://localhost:5081`.

### Step C — Create test accounts
Use the Register page to create one account with role **Organiser** and another with role **Participant**. Log in separately to test each role. Use realistic test data only.

## 6. Docker — single command
The root Dockerfile builds the MVC project. First start the API on a reachable host/port (or use the included Compose file to run both services). To run the MVC container against an API already running on the host:

```bash
docker build -t raceday-mvc -f Dockerfile .
docker run --rm -p 8080:8080 -e Api__BaseUrl=http://host.docker.internal:5080/ raceday-mvc
```
Then open `http://localhost:8080`.

### Run API and MVC together with Docker Compose
Create a `.env` file in the repository root (do not commit it):
```env
RACEDAY_SQL_CONNECTION=Server=host.docker.internal,1433;Database=RaceDayDB;User Id=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=True
AZURE_BLOB_CONNECTION=YOUR_AZURE_STORAGE_CONNECTION_STRING
JWT_KEY=REPLACE_WITH_A_LONG_RANDOM_SECRET_KEY_32_CHARS_MINIMUM
```
Run:
```bash
docker compose up --build
```
MVC: `http://localhost:8080` · API Swagger: `http://localhost:5080/swagger`.
The API container expects the database to be reachable using the SQL connection string above. Ensure SQL Server accepts TCP connections and that the database exists/migrations have been applied. `host.docker.internal` is used to reach a database on the host machine from Docker Desktop.

## 7. GitHub Actions / CI
`.github/workflows/raceday-ci.yml` checks required project files, restores the solution, builds API + MVC + tests in Release mode and runs tests on pushes and pull requests.

After pushing your work:
1. Open the GitHub repository.
2. Select **Actions**.
3. Open the latest RaceDay Part 3 CI run.
4. Confirm all steps are green.
5. Add your own screenshot to `docs/ci-green-build.png` and include it in the submission. A screenshot is intentionally not fabricated in this package.

## 8. Suggested Git commit sequence
Make real commits as you complete the work. Do not create empty commits or claim commits that were not made. Suggested milestones:
1. Add Part 3 MVC project to solution.
2. Configure MVC services and session middleware.
3. Add API HTTP client service.
4. Add login and registration view models.
5. Implement API-backed registration.
6. Implement login and session creation.
7. Implement logout and session clearing.
8. Add role-aware shared navigation.
9. Add public event browsing.
10. Add event filtering.
11. Add Organiser dashboard.
12. Add Organiser event listing.
13. Add event creation form.
14. Add event editing and deletion.
15. Add category management.
16. Add enrolment listing.
17. Add results entry and publishing.
18. Add Participant event details and entry.
19. Add My Entries and My Results pages.
20. Add profile update and image upload.
21. Add Azure Blob Storage API integration.
22. Add responsive RaceDay styling.
23. Add Dockerfile and Compose configuration.
24. Extend GitHub Actions to build MVC.
25. Update README and test fixes.

## 9. Video presentation checklist
Record an unlisted YouTube video and paste its URL in the submission form and below:
1. Introduce RaceDay and explain the user roles.
2. Show registration and login for Organiser and Participant.
3. Demonstrate the different navigation menus and logout.
4. As Organiser, create/edit an event, add a category, view enrolments and publish a result.
5. As Participant, browse an event, enter a category, view My Entries, My Results and update Profile.
6. Explain `RaceDay.Mvc/Controllers`, `Views`, `Models` and `Services/RaceDayApiClient.cs`.
7. Explain how MVC sends the image to the API and how the API stores it in Azure Blob Storage.
8. Show Docker build/run and open the running container.
9. Show the successful GitHub Actions workflow.

