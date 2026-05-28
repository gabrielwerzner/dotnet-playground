# GameStoreAPI — Your First .NET REST API

A minimal Game Store API built with ASP.NET Core + SQLite.
This project covers everything from the beginner course: REST, CRUD, EF Core, DTOs, Dependency Injection, and async code.

---

## Folder structure

```
GameStoreAPI/
│
├── Program.cs              ← App startup (3 lines of setup)
│
├── Models/
│   ├── Game.cs             ← Database table: Games
│   └── Genre.cs            ← Database table: Genres
│
├── DTOs/
│   └── GameDtos.cs         ← What the API sends/receives (NOT the raw DB model)
│
├── Data/
│   ├── GameStoreContext.cs ← Bridge between C# and SQLite (the "session")
│   └── DataExtensions.cs  ← DB registration + auto-migration + seed data
│
├── Endpoints/
│   └── GamesEndpoints.cs  ← All 5 API routes (GET all, GET one, POST, PUT, DELETE)
│
├── Properties/
│   └── launchSettings.json ← Local dev settings (port, environment)
│
├── appsettings.json        ← Connection string lives here (NOT in code)
├── games.http              ← Test all endpoints directly in VS Code
└── GameStoreAPI.csproj     ← Project config + NuGet packages
```

---

## Setup (do this once)

**1. Install .NET SDK** — https://dotnet.microsoft.com/download (pick .NET 9)

**2. Install VS Code extensions:**
- `C# Dev Kit` (IntelliSense, debugger)
- `REST Client` (to run the .http file)
- `SQLite` (to peek inside the database)

**3. Install EF Core CLI tool** (run once in any terminal):
```
dotnet tool install --global dotnet-ef
```

**4. Open this folder in VS Code:**
```
code GameStoreAPI
```

---

## Run the project

```bash
cd GameStoreAPI
dotnet run
```

The database file `gamestore.db` is created automatically on first run.
Genres are seeded automatically (Fighting, RPG, Platformer, Racing, Sports).

---

## Generate the database migration (do this once, or after changing Models)

```bash
dotnet ef migrations add InitialCreate --output-dir Data/Migrations
dotnet ef database update
```

> Tip: Since `MigrateDb()` is called in Program.cs, `dotnet run` will apply
> migrations automatically. You only need `database update` manually if you
> want to inspect things.

---

## Test the API

Open `games.http` in VS Code and click **Send Request** above each block.

| Method | URL              | What it does              |
|--------|------------------|---------------------------|
| GET    | /games           | List all games            |
| GET    | /games/1         | Get one game by ID        |
| POST   | /games           | Create a new game         |
| PUT    | /games/1         | Update an existing game   |
| DELETE | /games/1         | Delete a game             |

---

## Key concepts covered

| Concept                  | Where to look                            |
|--------------------------|------------------------------------------|
| REST + CRUD              | `Endpoints/GamesEndpoints.cs`            |
| DTOs (data contracts)    | `DTOs/GameDtos.cs`                       |
| Entity Framework Core    | `Data/GameStoreContext.cs`               |
| Dependency Injection     | `db` parameter in each endpoint handler |
| Async programming        | `async/await` in every endpoint          |
| Configuration system     | `appsettings.json` + `GetConnectionString` |
| Input validation         | `[Required]`, `[Range]` in DTOs          |
| Auto migrations + seeding| `Data/DataExtensions.cs`                 |
