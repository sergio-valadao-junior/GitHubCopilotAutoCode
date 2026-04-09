# Copilot Workspace Instructions

## About this project

This is an ASP.NET Core Web API project created to demonstrate how to use GitHub Copilot for code generation and assistance. The project is intentionally minimal to allow for easy experimentation and learning.

The application consists in an ecommerce for an IT Store that sellds GitHub Copilot Skills. It accepts payments though Stripe and
has a simple product catalog.

The API exposes endpoints for managing categories, managing products, processing orders, and handling payments.

## Architecture Overview

This is a minimal ASP.NET Core Web API project targeting .NET 10.0.
It uses the default WebApplication builder and minimal APIs pattern with Entity Framework Core InMemory for data persistence.

**Models:**

- `Category`: Represents product categories with Id (GUID), Title (string), Description (string), and CreatedAtUtc (DateTime) properties. Has a one-to-many relationship with Products.
- `Product`: Represents products in the catalog with Id (GUID), Name (string), Description (string), Price (decimal), CategoryId (foreign key), CreatedAtUtc, and UpdatedAtUtc properties. Belongs to a single Category.

## Build & Run

- **Build:** `dotnet build`
- **Run (Development with HTTPS):** `dotnet run --launch-profile https`
- **Run (Production with HTTPS):** `dotnet run --configuration Release --launch-profile https`
- **Clean build artifacts:** `dotnet clean`
- **Restore dependencies:** `dotnet restore`
- The app will be available at the URLs specified in `Properties/launchSettings.json` (default: http://localhost:5256, https://localhost:7026).

## Project Structure

- `Program.cs`: Main entry point, configures and runs the web app, sets up DbContext and minimal APIs.
- `Models/`: Data models including `Category.cs` and `Product.cs` with navigation properties for one-to-many relationships.
- `Data/`: Data access layer containing `ApplicationDbContext.cs` for Entity Framework Core configuration.
  - `Mapping/`: Entity configuration files using EF Core's IEntityTypeConfiguration<T> pattern (e.g., `CategoryConfiguration.cs`, `ProductConfiguration.cs`).
- `Endpoints/`: REST API endpoint definitions for each entity.
  - `CategoryEndpoints.cs`: Extension method `MapCategoryEndpoints()` for Category CRUD operations.
  - `ProductEndpoints.cs`: Extension method `MapProductEndpoints()` for Product CRUD operations.
  - Request/Response records defined inline for each endpoint.
- `appsettings.json`, `appsettings.Development.json`: Configuration files for logging and environment settings.
- `GitHubCopilotAutoCode.csproj`: Project file, targets .NET 10.0, includes Entity Framework Core InMemory package.
- `Properties/launchSettings.json`: Launch profiles for development.

## Conventions

- Follows standard ASP.NET Core minimal API patterns.
- Nullable reference types enabled.
- Implicit usings enabled.
- Sensitive data (e.g., connection strings) should be stored in user secrets or environment variables when available, otherwise use configuration files like 'appsettings.json' and 'appsettings.Development.json'.
- Uses RESTful conventions for API endpoints (e.g., `/api/categories`, `/api/products`).
- Uses primary constructors for dependency injection when possible.
- All classes are sealed by default to prevent unexpected inheritance.

## Data Access with Entity Framework Core

- Uses Entity Framework Core InMemory for data access.
- The InMemory provider is suitable for testing and development but not for production.
- Uses `DbContext` for managing data with InMemory database provider.
- Entity configurations are separated using the `IEntityTypeConfiguration<T>` pattern in the `Data/Mapping/` folder.
- Configure DbContext in Program.cs with `services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("GitHubCopilotAutoCodeDb"))`.
- Use `DbSet<T>` for each entity to query and manipulate data.
- Use `DbSet<T>` should be initialized with 'null!' to satisfy non-nullable reference type requirements.
- The `ApplicationDbContext.OnModelCreating()` method uses `ApplyConfigurationsFromAssembly()` to automatically discover and apply all entity configurations.
- Ensure navigation properties are properly configured in DbContext (navigations and foreign key relationships).

## REST API Endpoints

- Each entity has its own endpoint mapping file in the `Endpoints/` folder (e.g., `CategoryEndpoints.cs`, `ProductEndpoints.cs`).
- Each endpoint file contains a public static extension method named `MapXyzEndpoints()` to register all routes for that entity.
- Request and response records (DTOs) are defined as sealed records within each endpoint file.
- Endpoints use `MapGroup()` to organize routes with metadata using `WithName()` and `WithSummary()`.
- All endpoint handlers are `private static` methods that receive `ApplicationDbContext` via dependency injection.
- CRUD Operations follow RESTful conventions:
  - `GET /api/{entities}` - Get all
  - `GET /api/{entities}/{id}` - Get by ID
  - `POST /api/{entities}` - Create new
  - `PUT /api/{entities}/{id}` - Update
  - `DELETE /api/{entities}/{id}` - Delete
- Extension methods are called in `Program.cs` to register all endpoints.

## Potential Pitfalls

- Ensure you are using .NET 10.0 SDK or later.
- If ports are in use, update `applicationUrl` in `launchSettings.json`.
- InMemory database is ephemeral; data is lost when the application restarts.

## Documentation

- No additional documentation or contributing guidelines are present.

---

## Example Prompts

- "Create a DbContext for managing Categories and Products."
- "Add a new GET endpoint at `/api/categories` that returns all categories with their products."
- "Add a POST endpoint to create a new product with a category."
- "Configure Entity Framework Core InMemory in Program.cs."
- "Add Swagger/OpenAPI support."
- "Change the default port to 5000."
- "Add seed data to the InMemory database on startup."

---

## Next Steps

- Add error handling and comprehensive validation to API endpoints.
- Consider adding Swagger/OpenAPI documentation.
- Consider adding seed data to the InMemory database on startup.
- Add service/business logic layer for complex operations.
- Implement pagination and filtering for list endpoints.
- Add authorization/authentication middleware.
- For more advanced customization, create agent instructions for API conventions, testing, or deployment workflows.
