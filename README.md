# RaceDay — Part 2: RESTful API (PROG6212)

This is the Part 2 API project built from the Part 1 RaceDay ERD, SQL design and endpoint plan. The API uses ASP.NET Core Web API, EF Core Code First, SQL Server, JWT bearer authentication, role checks, Swagger and a GitHub Actions build/test workflow.

## Folder map
- `RaceDay.Api/Models` — entity classes (Users, Roles, Events, Categories, Enrolments, Results)
- `RaceDay.Api/Data` — EF Core database context and relationships
- `RaceDay.Api/DTOs` — request validation models
- `RaceDay.Api/Services` — password hashing and token creation
- `RaceDay.Api/Controllers` — endpoint logic and access rules
- `RaceDay.Tests` — automated smoke test project
- `.github/workflows/raceday-ci.yml` — build and test on pushes/PRs
- `docs/` — Part 1 documents supplied with this project

## Roles in plain language
- **Organiser:** creates and manages their own events and event categories, views event enrolments, and records/corrects results for participants in their events.
- **Participant:** browses events/categories, enrols in a category, cancels their own enrolment, and views their own profile and results.
- Public browsing endpoints do not require a login. Private endpoints require a valid bearer token. Role attributes stop a Participant from using Organiser actions and vice versa. Ownership is checked as well as role.

## Part 1 alignment / implementation notes
The EF entities follow the Part 1 SQL tables and foreign-key relationships: `Roles`, `Users`, `Events`, `Categories`, `Enrolments`, and `Results`. `Weather` and `Routes` were shown in the ERD but are not in the supplied Part 1 SQL table script or endpoint list; they are not implemented in this minimum API.
- Part 1's register plan uses `firstName`/`lastName`; the submitted SQL schema stores `FullName`. This API accepts `FullName` to keep the database aligned with the SQL script.
- Part 1's category endpoint includes `entryFee`, but the Part 1 Categories table has no `EntryFee` column. This implementation omits it rather than silently changing the schema.
- Part 1's result plan names `positionOverall` and `positionCategory`; its SQL table has one `Position` column. The implementation maps the supplied schema's `Position`.
- Part 1 describes JWT role claims. The API follows that approved endpoint design (bearer JWT), rather than ASP.NET session cookies. If your lecturer explicitly insists on server-side session storage, confirm this design difference and adapt it before final submission.

## Requirements
- .NET 8 SDK
- Visual Studio 2022 (ASP.NET and web development workload) or VS Code
- SQL Server / SQL Server Express / LocalDB
- Git and GitHub account

## Run locally (Windows / Visual Studio)
1. Extract this folder and open `RaceDay.sln`.
2. In `RaceDay.Api/appsettings.json`, change the connection string to your SQL Server instance. The default uses LocalDB.
3. Open **Tools → NuGet Package Manager → Package Manager Console**, set Default project to `RaceDay.Api`, then run:
   ```powershell
   Add-Migration InitialCreate
   Update-Database
   ```
   Alternatively:
   ```bash
   dotnet tool install --global dotnet-ef
   dotnet ef migrations add InitialCreate --project RaceDay.Api
   dotnet ef database update --project RaceDay.Api
   ```
4. Run the API (`RaceDay.Api`) with Visual Studio or:
   ```bash
   dotnet restore RaceDay.sln
   dotnet build RaceDay.sln
   dotnet run --project RaceDay.Api
   ```
5. Open the Swagger URL shown in the terminal (normally `https://localhost:xxxx/swagger`).

**Do not commit production secrets.** Replace the development JWT key with a long random secret in local user-secrets/environment variables before deployment.

## Swagger and login walkthrough
1. `POST /api/auth/register` — register an Organiser and a Participant. Role must be exactly `Organiser` or `Participant`.
2. `POST /api/auth/login` — copy the returned `token`.
3. Click **Authorize** in Swagger and enter `Bearer YOUR_TOKEN` (include `Bearer` and a space).
4. Test endpoints using the correct role token. Expected responses include 200, 201, 204, 400, 401, 403, 404 and 409 as applicable.

## Endpoint checklist (matches Part 1 plan)
| Method | Route | Access |
|---|---|---|
| POST | `/api/auth/register` | Public |
| POST | `/api/auth/login` | Public |
| GET / PUT / DELETE | `/api/users/me` | Authenticated |
| GET | `/api/events`, `/api/events/{id}` | Public |
| POST | `/api/events` | Organiser |
| PUT / DELETE | `/api/events/{id}` | Event owner Organiser |
| GET | `/api/events/{eventId}/categories` | Public |
| POST | `/api/events/{eventId}/categories` | Event owner Organiser |
| PUT / DELETE | `/api/categories/{id}` | Event owner Organiser |
| POST | `/api/categories/{categoryId}/enrol` | Participant |
| GET | `/api/enrolments/me` | Participant |
| GET | `/api/events/{eventId}/enrolments` | Event owner Organiser |
| DELETE | `/api/enrolments/{id}` | Enrolment owner Participant |
| POST | `/api/enrolments/{enrolmentId}/results` | Event owner Organiser |
| GET | `/api/results/me` | Participant |
| GET | `/api/events/{eventId}/results` | Public |
| PUT | `/api/results/{id}` | Event owner Organiser |

## Run tests
```bash
dotnet test RaceDay.sln
```
The included smoke test checks that the Development Swagger document responds. Add endpoint tests for register/login, invalid credentials, authenticated profile, Organiser event CRUD, Participant forbidden responses, enrolment duplicate/full capacity and result ownership. A smoke test alone does not meet the full 15-mark testing rubric.

## GitHub Actions / commits
The workflow at `.github/workflows/raceday-ci.yml` restores, builds and runs tests on every push and pull request. Push this project to a clearly named Part 2 branch (example `part-2-api`), then check **GitHub → Actions**. Fix failures until the latest workflow run is green.
- Insert your own screenshot of a successful green Actions run below. Do not use a fabricated screenshot.
- Make at least 20 meaningful commits, each showing a genuine step (model, context, auth, each controller, validation, tests, docs, CI, fixes). Do not create empty or misleading commits.

**CI screenshot:** `docs/ci-green-build.png`

