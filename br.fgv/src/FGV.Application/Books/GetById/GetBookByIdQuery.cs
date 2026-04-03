using FGV.Application.Abstractions.Messaging;

namespace FGV.Application.Books.GetById;

public sealed record GetBookByIdQuery(string Id) : IQuery<BookResponse>;
