using FGV.Application.Abstractions.Messaging;

namespace FGV.Application.Books.GetAll;

public sealed record GetAllBooksQuery : IQuery<IEnumerable<BookResponse>>;
