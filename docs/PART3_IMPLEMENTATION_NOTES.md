# Part 3 implementation notes

## Request flow
Browser → MVC controller → RaceDayApiClient → Part 2 REST API → EF Core/SQL Server.
For images: Browser → MVC multipart request → API upload endpoint → Azure Blob Storage → returned blob URL → MVC sends URL to API profile/event endpoint.

## Security decisions
- MVC stores login state in server-side ASP.NET Core session.
- API JWT is held in the server-side session and attached to API requests; it is not rendered into the browser.
- Role checks are performed before Organiser/Participant actions.
- API authorization remains the final authority for role and ownership checks.
- Image upload validates extension-independent MIME type and file size; use private containers plus a controlled delivery mechanism if public images are not appropriate for production.
- Production deployment should use HTTPS, secure cookie settings, secrets management, rate limiting and stronger CSRF protections.

## Integration checks
Confirm the Part 2 API is available at the configured base URL. API response JSON uses camelCase. MVC expects the existing endpoint routes in the Part 2 README. If routes or response field names are changed in Part 2, update `RaceDayApiClient` and the MVC JSON property access in one place.
