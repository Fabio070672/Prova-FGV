using FGV.Application.Abstractions.Messaging;

namespace FGV.Application.Books.GetAll;

public sealed record GetAllBooksQuery(string? ConfigurationName = null) : IQuery<IEnumerable<BookResponse>>;
