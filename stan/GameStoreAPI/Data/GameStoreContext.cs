using GameStoreAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace GameStoreAPI.Data;

// DbContext = a "session" with the database.
// Think of it as a smart gateway that translates C# ↔ SQL.
public class GameStoreContext(DbContextOptions<GameStoreContext> options)
    : DbContext(options)
{
    // These two properties represent the database tables.
    // EF Core will generate SQL queries when you use them.
    public DbSet<Game> Games => Set<Game>();
    public DbSet<Genre> Genres => Set<Genre>();
}
