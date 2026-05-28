namespace GameStoreAPI.Models;

// Maps to a "Genres" table (Fighting, RPG, Platformer, etc.)
public class Genre
{
    public int Id { get; set; }
    public required string Name { get; set; }
}
