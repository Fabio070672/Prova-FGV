using FGV.Application.Abstractions.Messaging;

namespace FGV.Application.Books.GetAll;

public sealed record GetAllBooksQuery(
    string? Title = null,
    string? Author = null,
    string? SortBy = null,
    string? SortOrder = "asc") : IQuery<IEnumerable<BookResponse>>;
