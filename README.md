# Management Solution

A modular ASP.NET Core solution with MVC + API for managing users and products. Includes layered architecture, EF Core, JWT authentication for API, and shared services for image handling.

## Overview
- __Web App__: `Management.Admin/` (MVC + API endpoints)
- __Domain Models__: `Entity/`
- __DTOs__: `Model/`
- __Data + Repositories__: `DatabaseLayer/`
- __Business Services__: `ServiceLayer/`
- __Shared Product Image Service__: `Management.Admin/Services/`

## Architecture
- Clean layering: Controller -> ServiceLayer -> DatabaseLayer (Repositories) -> EF Core DbContext
- DTOs decouple web layer from entities
- Dependency Injection used throughout
- API secured via JWT (`Bearer`)

## Key Features
- __Products & Users__ CRUD
- __Sortable tables__ on Products/Users index views (Bootstrap Icons)
- __Product description__ clamped/justified for cleaner UI
- __Shared image handling__: `IProductImageService` for save/delete to `/wwwroot/uploads/products`

- __Client-side pagination & search__: Listing pages for Users and Products handle pagination and search on the client.

## Roles & Permissions
- __Admin__: Full access across the system (Users and Products modules).
- __Manager__: Full access to Products module (create, update, delete, view). No access to Users API.

## Default Login Accounts
- __Admin__
  - Email: `admin@example.com`
  - Password: `Admin@12345`
- __Manager__
  - Email: `manager@example.com`
  - Password: `Manager@12345`

## Project Structure
```
Management.sln
├── Management.Admin/               # ASP.NET Core MVC + API
│   ├── Controllers/
│   │   ├── ProductsController.cs
│   │   └── Api/ProductsController.cs
│   ├── Services/
│   │   ├── IProductImageService.cs
│   │   └── ProductImageService.cs
│   ├── Views/
│   │   ├── Products/Index.cshtml   # Sort icons
│   │   └── Users/Index.cshtml      # Sort icons
│   └── wwwroot/css/site.css        # UI styles
├── ServiceLayer/
├── DatabaseLayer/
├── Entity/
└── Model/
```

## Prerequisites
- .NET SDK 8.0 (recommended)
- SQLite (embedded; no separate install needed)

## Configuration
- App settings in `Management.Admin/appsettings.json`
  - Connection string: `DefaultConnection` (SQLite)
  - JWT under `Jwt:Key`, `Jwt:Issuer`, `Jwt:Audience`
- Image upload path: `wwwroot/uploads/products`

## Setup
```bash
# From repository root
dotnet restore

# Apply EF migrations if needed
# Example (adjust project paths if your default startup is Management.Admin)
dotnet ef database update --project Management.Admin --startup-project Management.Admin

# Or update DatabaseLayer context if required
# dotnet ef database update --project DatabaseLayer --startup-project Management.Admin
```

## Run
```bash
# Web (MVC + API)
dotnet run --project Management.Admin
```
- MVC UI at http://localhost:5000 (or as shown in console)
- API base: `http://localhost:{port}/api`
  - Products: `GET /api/products`, `GET /api/products/{id}`
  - Requires `Authorization: Bearer <token>` for protected endpoints

## Development Notes
- Register services in `Management.Admin/Program.cs`:
  - `builder.Services.AddServiceLayer();`
  - `builder.Services.AddDatabaseLayerRepositories();`
  - `builder.Services.AddScoped<IProductImageService, ProductImageService>();`
- Controllers only depend on `IProductService` and `IProductImageService` (no `IWebHostEnvironment`).
- Sorting icons toggle states via JS; styles in `wwwroot/css/site.css`.

## Common Tasks
- __Upload product image__: handled by `ProductImageService`, returns relative path `/uploads/products/{file}`
- __Replace product image__: service deletes old file then saves new
- __Remove image__: sets `ImageUrl = null` and deletes physical file

## Testing API with JWT
1. Configure `Jwt` section in `appsettings.json`.
2. Obtain a token from your auth endpoint (not shown here).
3. Include `Authorization: Bearer <token>` in requests.

## Troubleshooting
- If images don’t appear, ensure folder `wwwroot/uploads/products` exists and app has write permissions.
- For migration issues, verify the correct startup and target projects when running `dotnet ef`.

## Contributing
- Use feature branches and PRs.
- Add/Update unit tests in ServiceLayer where applicable.
- Keep controllers thin; push logic into services.

## License
Proprietary/Private (update as appropriate).
