namespace GameStoreAPI.Models;

// This class maps to a "Games" table in the database.
// Entity Framework Core uses it to create/query the table.
public class Game
{
    public int Id { get; set; }

    // "required" forces anyone creating a Game to provide a Name
    public required string Name { get; set; }

    // Foreign key — links this game to a Genre row
    public int GenreId { get; set; }

    // Navigation property — EF Core fills this when we use .Include()
    public Genre? Genre { get; set; }

    public decimal Price { get; set; }
    public DateOnly ReleaseDate { get; set; }
}
