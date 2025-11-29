# Library.Api (Updated)

ASP.NET Core 8 Web API for Library Management with JWT authentication and role-based access (Admin/User).

## Requirements
- .NET 8 SDK
- SQL Server / LocalDB
- (Optional) dotnet-ef tool: `dotnet tool install --global dotnet-ef`

## Setup
1. Open `Library.Api/appsettings.json` and adjust connection string & JWT key.
2. From the repository root run:
   ```bash
   dotnet restore
   dotnet ef migrations add InitialCreate -p Library.Api -s Library.Api
   dotnet ef database update -p Library.Api -s Library.Api
   dotnet run --project Library.Api
   ```
3. Swagger UI available at the URL shown in console (development environment).

## Default admin
- Email: `admin@library.local`
- Password: `Admin@123` (change immediately)

## Endpoints (high level)
- `POST /api/auth/register` : register user (creates Student role by default)
- `POST /api/auth/login` : login -> returns { token, role, userId }
- `GET /api/users` : (Admin) list all users
- `GET /api/users/me` : (Authenticated) get your profile
- `GET /api/loans` : (Admin) list all loans
- `GET /api/loans/my-loans` : (Authenticated) get loans for current user
- `POST /api/loans/issue` : (Admin) issue book
- `POST /api/loans/return/{id}` : (Admin) return book
