# GitHubCopilotAutoCode

Developer-focused ASP.NET Core 10.0 Minimal Web API sample for managing a small
product catalog (Categories and Products). Designed for contributors and engineers
exploring Minimal APIs, EF Core InMemory usage, and clean architecture patterns.

## Table of Contents
- Quickstart
- Features
- Architecture & Tech
- API Endpoints
- Development Notes
- Contributing
- Assets & Design

## Quickstart
Build and run locally (requires .NET 10 SDK):

```bash
dotnet build
dotnet run --launch-profile https
```

Default dev URLs (from project launch settings):
- http://localhost:5256
- https://localhost:7026

After startup, try a sample request:

```bash
curl -sS http://localhost:5256/api/categories
```

## Features
- Minimal APIs with clear route grouping and DTOs
- Entity Framework Core InMemory provider for development/testing
- RESTful CRUD for `Category` and `Product`
- Service layer (`Services/*`) to separate concerns
- Configuration and mappings in `Data/` and `Data/Mapping/`

## Architecture & Tech
- Target framework: .NET 10.0 ([GitHubCopilotAutoCode.csproj](GitHubCopilotAutoCode.csproj))
- Hosting: ASP.NET Core Minimal APIs entry point ([Program.cs](Program.cs))
- InMemory EF provider configured in [Data/ApplicationDbContext.cs](Data/ApplicationDbContext.cs)
- Launch profiles and URLs: [Properties/launchSettings.json](Properties/launchSettings.json)

## API Endpoints
Core endpoint mappings:
- [Endpoints/CategoryEndpoints.cs](Endpoints/CategoryEndpoints.cs)
- [Endpoints/ProductEndpoints.cs](Endpoints/ProductEndpoints.cs)

Common data & services:
- [Data/ApplicationDbContext.cs](Data/ApplicationDbContext.cs)
- [Data/Mapping/CategoryConfiguration.cs](Data/Mapping/CategoryConfiguration.cs)
- [Data/Mapping/ProductConfiguration.cs](Data/Mapping/ProductConfiguration.cs)
- [Services/CategoryService.cs](Services/CategoryService.cs)
- [Services/ProductService.cs](Services/ProductService.cs)

For implementation details and DTOs, explore the `Models/` and `Endpoints/` folders.

## Development Notes
- The InMemory database is ephemeral — data is lost when the app restarts.
- Use `dotnet build` and `dotnet run --launch-profile https` to run locally.
- If you change ports, update `Properties/launchSettings.json` and the Quickstart above.

## Contributing
- Open issues and PRs against this repository. 
- Follow existing code style and the project's `.github` guidance (see [.github/copilot-instructions.md](.github/copilot-instructions.md)).

