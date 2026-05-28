using System.ComponentModel.DataAnnotations;

namespace GameStoreAPI.DTOs;

// ----- What clients GET back (list of games) -----
public record GameSummaryDto(
    int Id,
    string Name,
    string Genre,   // We return the genre NAME, not the ID
    decimal Price,
    DateOnly ReleaseDate
);

// ----- What clients GET back (single game detail) -----
public record GameDetailsDto(
    int Id,
    string Name,
    int GenreId,    // Detail view returns the ID so forms can pre-select it
    decimal Price,
    DateOnly ReleaseDate
);

// ----- What clients SEND when creating a game (POST) -----
public record CreateGameDto(
    [Required][StringLength(50)] string Name,
    [Range(1, 50)] int GenreId,
    [Range(1, 100)] decimal Price,
    DateOnly ReleaseDate
);

// ----- What clients SEND when updating a game (PUT) -----
public record UpdateGameDto(
    [Required][StringLength(50)] string Name,
    [Range(1, 50)] int GenreId,
    [Range(1, 100)] decimal Price,
    DateOnly ReleaseDate
);
