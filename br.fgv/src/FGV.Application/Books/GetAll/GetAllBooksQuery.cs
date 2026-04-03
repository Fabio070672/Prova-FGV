using FGV.Application.Abstractions.Messaging;

namespace FGV.Application.Books.GetAll;

public sealed record GetAllBooksQuery(List<SortRule>? SortRules = null) : IQuery<IEnumerable<BookResponse>>;


