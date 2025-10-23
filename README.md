# NextUp-API

# NextUp.Api

Backend API for NextUp - a comprehensive youth football team management platform built with ASP.NET Core and PostgreSQL.

## Overview

NextUp.Api provides a robust REST API for managing youth football teams, tracking player performance, scheduling games, and delivering data-driven insights for coaches and athletic directors. The API handles complex football statistics, role-based permissions, and team analytics.

## Tech Stack

- **Framework**: ASP.NET Core
- **Database**: PostgreSQL
- **ORM**: Entity Framework Core
- **Authentication**: JWT (JSON Web Tokens)
- **Language**: C# (.NET 8)

## Key Features

### User Authentication & Authorization: 
- Supports JWT-based authentication with role-based access for Players, Coaches, and Athletic Directors.

### Team Management: 
- Endpoints for creating, updating, and managing teams, including assigning coaches and players.

### Player Management: 
- CRUD operations for player profiles, tracking player stats, goals, and notes.

### Coach & Athletic Director Management: 
- Endpoints for managing coach and athletic director profiles and their associated teams.

### Game Scheduling & Management: 
- Create, update, and retrieve games, including scheduling, scores, and game notes.

### Player Stats & Analytics: 
- Endpoints for recording and retrieving detailed player game stats, supporting football-specific metrics.

### Role-Based Dashboards: 
- Provides tailored data and endpoints for each user role to support their workflows.

### Data Seeding & Migration: 
- Includes scripts and tools for database seeding and schema migrations using Entity Framework Core.

### Secure API Design: 
- Implements best practices for security, including JWT authentication and validation constraints.
## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [PostgreSQL](https://www.postgresql.org/download/) (v12 or higher)
- IDE: Visual Studio 2022, VS Code, or Rider

## Getting Started

### 1. Clone the Repository

```bash
git clone <repository-url>
cd NextUp.Api
```

### 2. Configure Database Connection

Create an `appsettings.Development.json` file in the project root:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=nextup_db;Username=your_username;Password=your_password"
  },
  "Jwt": {
    "Key": "your-secret-key-here-min-32-characters",
    "Issuer": "NextUpApi",
    "Audience": "NextUpClient",
    "ExpirationMinutes": 60
  }
}
```

### 3. Set Up the Database

Run migrations to create the database schema:

```bash
dotnet ef database update
```

### 4. Run the Application

```bash
dotnet run
```

The API will be available at `https://localhost:5001` (or the port specified in `launchSettings.json`).

### 5. Access API Documentation

Navigate to `https://localhost:5001/swagger` to view the interactive API documentation.

## Project Structure

```
NextUp-API/
├── .gitignore
├── global.json
├── NextUp-API.sln
├── README.md
├── .vscode/
│   ├── launch.json
│   ├── settings.json
│   └── tasks.json
├── NextUp.Api/
│   ├── appsettings.Development.json
│   ├── appsettings.json
│   ├── migration.sql
│   ├── NextUp.Api.csproj
│   ├── NextUp.Api.http
│   ├── NextUp.Api.sln
│   ├── NextUp.session.sql
│   ├── players.sql
│   ├── Program.cs
│   ├── update-coaches.sql
│   ├── update-players.sql
│   ├── update-users.sql
│   ├── users.sql
│   ├── .vscode/
│   │   └── settings.json
│   ├── bin/
│   ├── Data/
│   │   ├── DataSeeder.cs
│   │   └── NextUpDbContext.cs
│   ├── DTOs/
│   │   ├── AthleticDirectorDTOs.cs
│   │   ├── AuthDTOs.cs
│   │   ├── CoachDTOs.cs
│   │   ├── GameDTOs.cs
│   │   ├── PlayerDTOs.cs
│   │   ├── StatsDTOs.cs
│   │   ├── TeamDTOs.cs
│   │   └── UserDTOs.cs
│   ├── Endpoints/
│   │   ├── AthleticDirectorEndpoints.cs
│   │   ├── AuthEndpoints.cs
│   │   ├── CoachEndpoints.cs
│   │   ├── GameEndpoints.cs
│   │   ├── GameNotesEndpoints.cs
│   │   ├── PlayerEndpoints.cs
│   │   ├── PlayerGoalsEndpoints.cs
│   │   ├── PlayerNotesEndpoints.cs
│   │   ├── StatsEndpoints.cs
│   │   ├── TeamEndpoints.cs
│   │   └── UserEndpoints.cs
│   ├── Helpers/
│   ├── Migrations/
│   │   ├── 20251012205104_InitialCreate.cs
│   │   ├── 20251012205104_InitialCreate.Designer.cs
│   │   ├── 20251016183423_AddAthleticDirector.cs
│   │   ├── 20251016183423_AddAthleticDirector.Designer.cs
│   │   ├── 20251016225652_AddTeamFields.cs
│   │   ├── 20251016225652_AddTeamFields.Designer.cs
│   │   ├── 20251017011355_MakeTeamCoachIdOptional.cs
│   │   ├── 20251017011355_MakeTeamCoachIdOptional.Designer.cs
│   │   ├── 20251017144516_FixMissingCoachRecords.cs
│   │   ├── 20251017144516_FixMissingCoachRecords.Designer.cs
│   │   ├── 20251017150832_AddRoleValidationConstraint.cs
│   │   ├── 20251017150832_AddRoleValidationConstraint.Designer.cs
│   │   ├── 20251023001838_AddWeekToGame.cs
│   │   ├── 20251023001838_AddWeekToGame.Designer.cs
│   │   ├── 20251023172410_ExpandPlayerGameStatsFields.cs
│   │   ├── 20251023172410_ExpandPlayerGameStatsFields.Designer.cs
│   │   ├── 20251023172550_ExpandPlayerGameStatsFieldsV2.cs
│   │   ├── 20251023172550_ExpandPlayerGameStatsFieldsV2.Designer.cs
│   │   ├── 20251023180150_ExpandPlayerGameStatsFieldsV3.cs
│   │   ├── 20251023180150_ExpandPlayerGameStatsFieldsV3.Designer.cs
│   │   └── NextUpDbContextModelSnapshot.cs
│   ├── Models/
│   │   ├── AthleticDirector.cs
│   │   ├── Coach.cs
│   │   ├── Game.cs
│   │   ├── GameNote.cs
│   │   ├── Player.cs
│   │   ├── PlayerGameStats.cs
│   │   ├── PlayerGoal.cs
│   │   └── ...
│   ├── obj/
│   ├── Properties/
│   ├── Services/
├── scripts/
│   └── smoke-tests.ps1

```


This project was developed as part of a school capstone project. If you'd like to contribute or have suggestions:

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Submit a pull request

## Future Enhancements

- Real-time notifications for game updates
- Video highlight integration
- Advanced analytics dashboard
- Mobile app support
- Multi-league management
- Parent portal for viewing player progress


**Note**: This project was developed as a demonstration of full-stack development capabilities, focusing on complex domain modeling, API design, and data-driven insights for youth sports management.