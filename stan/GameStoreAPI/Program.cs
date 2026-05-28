using GameStoreAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. Register the database (reads connection string from appsettings.json)
builder.AddGameStoreDb();

var app = builder.Build();

// 3. Apply any pending database migrations on startup
app.MigrateDb();

// 4. Register all /games endpoints
app.MapGamesEndpoints();

app.Run();