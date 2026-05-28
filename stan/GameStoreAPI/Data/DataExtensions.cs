using GameStoreAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace GameStoreAPI.Data;

public static class DataExtensions
{
    // Called from Program.cs as: builder.AddGameStoreDb()
    // Reads the connection string from appsettings.json and registers the DB.
    public static void AddGameStoreDb(this WebApplicationBuilder builder)
    {
        var connString = builder.Configuration.GetConnectionString("GameStore");

        builder.Services.AddSqlite<GameStoreContext>(connString);
    }

    // Called from Program.cs as: app.MigrateDb()
    // Automatically applies any pending DB migrations when the app starts.
    public static void MigrateDb(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<GameStoreContext>();
        dbContext.Database.Migrate();

        // Seed genres if the table is empty
        if (!dbContext.Genres.Any())
        {
            dbContext.Genres.AddRange(
                new Genre { Name = "Fighting" },
                new Genre { Name = "RPG" },
                new Genre { Name = "Platformer" },
                new Genre { Name = "Racing" },
                new Genre { Name = "Sports" }
            );
            dbContext.SaveChanges();
        }
    }
}