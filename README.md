# RaceDay Event Management — Part 3

## Project structure

The solution contains three projects:

- `RaceDay.Api` — ASP.NET Core Web API and Entity Framework Core database access.
- `RaceDay.Web` — ASP.NET Core MVC website. It calls the API using `HttpClient`; it does not connect directly to the database.
- `RaceDay.Api.Tests` — automated API tests.

Open `RACEDAY.sln` from this folder in Visual Studio 2022.

## Technologies

- C# and .NET 8
- ASP.NET Core Web API
- ASP.NET Core MVC
- Entity Framework Core
- SQL Server LocalDB
- Bootstrap
- xUnit and GitHub Actions

## First-time database update

The Part 3 MVC features add the following API database fields:

- `Event.Description`
- `Event.BannerImagePath`
- `Participant.ProfileImagePath`

After opening the solution and restoring NuGet packages, open **Tools → NuGet Package Manager → Package Manager Console**.

Set the Default project to `RaceDay.Api`, then run:

```powershell
Add-Migration AddPart3ImageFields
Update-Database
```

Do not create a second `InitialCreate` migration. The project already has an initial migration.

If the migration was created already, only run `Update-Database`.

## Configure and run

1. Make sure SQL Server LocalDB is installed.
2. Check the connection string in `RaceDay.Api/appsettings.json`.
3. In Visual Studio, set multiple startup projects:
   - `RaceDay.Api` — Start
   - `RaceDay.Web` — Start
4. Run the solution.
5. The API uses `https://localhost:52913/`.
6. The MVC website uses the HTTPS URL shown by Visual Studio (currently `https://localhost:7256/`).
7. Register an Organiser account and create an event. Register a Participant account and enrol in the event.

The MVC application forwards the API session cookie with protected API requests. This is needed because the API checks the logged-in role and user ID for organiser and participant operations.

## Implemented MVC features

### Accounts
- Participant and Organiser registration through the API.
- Login and logout through the API.
- MVC session state and role-specific navigation.
- Protected dashboard and feature pages.

### Organiser
- Organiser dashboard.
- Create, list, edit and delete events.
- Upload event banner images.
- Add, edit and delete event categories.
- View enrolments for an event.
- Enter and delete race results.

### Participant
- Participant dashboard.
- Browse and filter events by event name, venue, city and event type.
- View event details and categories.
- Enrol in an event category.
- View own event enrolments and status.
- View own results, event date, category, finish time and positions.
- View and update participant profile.

## Image storage

Profile pictures and event banners are uploaded by the MVC website into its `wwwroot` folder:

- Profile pictures: `RaceDay.Web/wwwroot/images/profiles`
- Event banners: `RaceDay.Web/wwwroot/images/events`

The relative image path is saved through the API in the database and used to display the image. Image uploads are limited by file extension and size. This implementation uses local `wwwroot` storage, not Azure Blob Storage, following the lecturer's instruction shared for this project.

The files are stored on the web server. For deployment to multiple web servers or ephemeral hosting, a shared persistent file store would be needed.

## Build and test

From the folder containing `RACEDAY.sln`, run:

```bash
dotnet restore RACEDAY.sln
dotnet build RACEDAY.sln
dotnet test RaceDay.Api.Tests/RaceDay.Api.Tests.csproj
```

GitHub Actions is configured to restore and build the full solution and run the API test project on the `master` and `ST10483747` branches.

## Important assignment note

The earlier Part 3 brief supplied for this project mentioned Azure Blob Storage and Docker. The current image implementation uses `wwwroot` based on the lecturer's updated instruction. Docker has not been added because the student requested that it be left out. Confirm any changed marking requirements with the lecturer before submission.

## Submission checklist

- Push the full `RaceDayPart2` repository folder, including `RaceDay.Web`, to GitHub.
- Make regular, meaningful commits while completing and testing each feature.
- Confirm GitHub Actions passes.
- Update this README with the final run instructions and any lecturer-specific changes.
- Record the required unlisted demonstration video and add its link here.
