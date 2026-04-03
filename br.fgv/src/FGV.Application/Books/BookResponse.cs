namespace FGV.Application.Books;

public sealed record BookResponse(
    string Id,
    string Title,
    string Author,
    int Edition,
    bool Active,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
