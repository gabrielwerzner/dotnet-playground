using GameStoreAPI.Data;
using GameStoreAPI.DTOs;
using GameStoreAPI.Models;
using Microsoft.EntityFrameworkCore;

// This static class groups all /games API endpoints together.
// Extension method pattern keeps Program.cs clean.
public static class GamesEndpoints
{
    const string GetGameEndpointName = "GetGame";

    public static void MapGamesEndpoints(this WebApplication app)
    {
        // All routes below share the /games base path
        var group = app.MapGroup("/games");

        // ---------------------------------------------------------------
        // GET /games  →  returns list of all games
        // ---------------------------------------------------------------
        group.MapGet("/", async (GameStoreContext db) =>
        {
            return await db.Games
                .Include(g => g.Genre)          // loads the related Genre row
                .AsNoTracking()                  // read-only, faster
                .Select(g => new GameSummaryDto(
                    g.Id,
                    g.Name,
                    g.Genre!.Name,               // ! tells compiler Genre is never null here
                    g.Price,
                    g.ReleaseDate
                ))
                .ToListAsync();
        });

        // ---------------------------------------------------------------
        // GET /games/{id}  →  returns one game by ID
        // ---------------------------------------------------------------
        group.MapGet("/{id}", async (int id, GameStoreContext db) =>
        {
            var game = await db.Games.FindAsync(id);

            return game is null
                ? Results.NotFound()
                : Results.Ok(new GameDetailsDto(
                    game.Id, game.Name, game.GenreId, game.Price, game.ReleaseDate));
        })
        .WithName(GetGameEndpointName); // Named so POST can link to it

        // ---------------------------------------------------------------
        // POST /games  →  creates a new game
        // Body: { "name": "...", "genreId": 1, "price": 29.99, "releaseDate": "2024-01-01" }
        // ---------------------------------------------------------------
        group.MapPost("/", async (CreateGameDto newGame, GameStoreContext db) =>
        {
            var game = new Game
            {
                Name        = newGame.Name,
                GenreId     = newGame.GenreId,
                Price       = newGame.Price,
                ReleaseDate = newGame.ReleaseDate
            };

            db.Games.Add(game);
            await db.SaveChangesAsync();

            var dto = new GameDetailsDto(
                game.Id, game.Name, game.GenreId, game.Price, game.ReleaseDate);

            // 201 Created + Location header pointing to GET /games/{id}
            return Results.CreatedAtRoute(GetGameEndpointName, new { id = game.Id }, dto);
        });

        // ---------------------------------------------------------------
        // PUT /games/{id}  →  updates an existing game
        // ---------------------------------------------------------------
        group.MapPut("/{id}", async (int id, UpdateGameDto updatedGame, GameStoreContext db) =>
        {
            var existing = await db.Games.FindAsync(id);
            if (existing is null) return Results.NotFound();

            existing.Name        = updatedGame.Name;
            existing.GenreId     = updatedGame.GenreId;
            existing.Price       = updatedGame.Price;
            existing.ReleaseDate = updatedGame.ReleaseDate;

            await db.SaveChangesAsync();
            return Results.NoContent(); // 204 — success, no body needed
        });

        // ---------------------------------------------------------------
        // DELETE /games/{id}  →  deletes a game
        // ---------------------------------------------------------------
        group.MapDelete("/{id}", async (int id, GameStoreContext db) =>
        {
            await db.Games
                .Where(g => g.Id == id)
                .ExecuteDeleteAsync(); // Bulk delete — no SaveChanges needed

            return Results.NoContent();
        });
    }
}
