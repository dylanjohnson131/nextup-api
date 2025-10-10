# NextUp-API

This is a minimal ASP.NET Core Web API using EF Core with PostgreSQL (Npgsql).

Prerequisites
- .NET SDK 8.0 or 9.0 (dotnet)
- PostgreSQL locally or accessible endpoint

Quick setup
1. Update the connection string in `NextUp.Api/appsettings.json` (DefaultConnection).
2. Create the database (example using psql):

```powershell
# create database
psql -U postgres -c "CREATE DATABASE nextup;"
```

3. From the `NextUp.Api` folder, create and apply migrations:

```powershell
cd NextUp.Api
dotnet ef migrations add InitialCreate
dotnet ef database update
dotnet run
```

API endpoints
- GET /users - list users
- POST /users - create user (JSON body: { "username": "x", "email": "y" })

Notes
- If `dotnet ef` isn't available, install the EF tools: `dotnet tool install --global dotnet-ef`.
