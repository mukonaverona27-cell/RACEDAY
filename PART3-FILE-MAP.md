# RaceDay Part 3 — File map

Open `RACEDAY.sln` from the `RaceDayPart2` folder. The MVC website is now inside the same folder as the API so it is included in the GitHub repository and CI build.

## MVC project: `RaceDay.Web`

### Controllers
- `Controllers/ApiControllerBase.cs` — creates API clients and forwards the API session cookie.
- `Controllers/AccountController.cs` — registration, login and logout.
- `Controllers/OrganiserController.cs` — organiser dashboard.
- `Controllers/EventsController.cs` — organiser event CRUD and event banner upload.
- `Controllers/CategoriesController.cs` — event category create, read, update and delete.
- `Controllers/EnrolmentsController.cs` — organiser view of event enrolments.
- `Controllers/ResultsController.cs` — organiser result entry and deletion.
- `Controllers/ParticipantController.cs` — participant dashboard, event browsing, event details, enrolments and results.
- `Controllers/ProfileController.cs` — participant profile view, update and picture upload.

### Models
- `Models/LoginViewModels.cs` and `Models/RegisterViewModels.cs` — account forms.
- `Models/EventViewModels.cs` — event and event category forms.
- `Models/ParticipantFeatureViewModels.cs` — enrolment, result and profile forms.

### Views
- `Views/Account` — Login and Register.
- `Views/Organiser` — organiser dashboard.
- `Views/Events` — event list, create and edit.
- `Views/Categories` — category list, create and edit.
- `Views/Enrolments` — enrolment list for an event.
- `Views/Results` — event results and result entry.
- `Views/Participant` — participant dashboard, browse events, event details, my events and my results.
- `Views/Profile` — profile display and edit form.
- `Views/Shared/_Layout.cshtml` — role-based navigation.

### Static files
- `wwwroot/css/site.css` — RaceDay styling.
- `wwwroot/images/profiles` — uploaded participant profile pictures.
- `wwwroot/images/events` — uploaded event banner images.

## API project: `RaceDay.Api`

- `Models/Event.cs` — adds `Description` and `BannerImagePath`.
- `Models/Participant.cs` — adds `ProfileImagePath`.
- `Controllers/EventsController.cs` — saves event description and banner path.
- `Controllers/CategoriesController.cs` — category update and delete endpoints.
- `Controllers/Participant.cs` — saves the participant profile image path.
- `Controllers/ProfileController.cs` — returns participant profile fields and image path.
- `Controllers/ResultsController.cs` — includes event category details in result responses.
- `Data/RaceDayContext.cs` — defines decimal precision for category fee and distance.

## Required database update

Because the API models gained new database columns, run the migration commands in `README.md` before testing profile pictures or event banners.
