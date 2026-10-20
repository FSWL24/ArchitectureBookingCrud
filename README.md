# Architecture Booking CRUD

A .NET 9 ASP.NET Core API for managing hotel bookings.

## Run

Requires the .NET 9 SDK.

```sh
dotnet run --project ArchitectureBookingCrud/ArchitectureBookingCrud.Api/ArchitectureBookingCrud.Api.csproj
```

Swagger UI: `http://localhost:5282/api`

## Endpoints

All endpoints use `/api/bookings`.

| Method | Route | Description |
| --- | --- | --- |
| `POST` | `/` | Create a booking (`201 Created`). |
| `GET` | `/{id}` | Get a booking (`404` if missing). |
| `GET` | `/between?startDate={date}&endDate={date}` | Find bookings overlapping a date range. |
| `PUT` | `/{id}` | Update a booking (`404` if missing). |
| `DELETE` | `/{id}` | Cancel a booking without deleting it. |

Dates use ISO 8601 with an offset. Check-out must be later than check-in.

## Architecture Notes

- **Solution structure:** `ArchitectureBookingCrud.sln` contains the ASP.NET Core API project and the xUnit test project.
- **Controller → service → repository:** `BookingsController` handles HTTP, `BookingService` applies booking rules, and `BookingRepository` accesses storage through `InMemoryDbContext`.
- **In-memory persistence:** Storage operations are synchronized and return copies. Data is lost when the app stops.
- **Validation and responses:** Request models validate input; API responses use dedicated DTOs. Missing bookings return `404`, and invalid input returns validation problem details.

## Test

```sh
dotnet test ArchitectureBookingCrud/ArchitectureBookingCrud.sln
```

The xUnit tests cover controller HTTP results and booking service behavior, including validation, date-range search, updates, and cancellation.
