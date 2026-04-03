using FGV.Application.Abstractions.Messaging;
using FGV.Domain.Books;
using FGV.SharedKernel;

namespace FGV.Application.Books.GetAll;

internal sealed class GetAllBooksHandler : IQueryHandler<GetAllBooksQuery, IEnumerable<BookResponse>>
{
    private readonly IBookRepository _bookRepository;

    public GetAllBooksHandler(
        IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public async Task<Result<IEnumerable<BookResponse>>> HandleAsync(
        GetAllBooksQuery query,
        CancellationToken cancellationToken)
    {
        IEnumerable<Book> books = await _bookRepository.GetAllAsync(cancellationToken);

        // Apply filters
        if (!string.IsNullOrWhiteSpace(query.Title))
        {
            books = books.Where(b => b.Title.Contains(query.Title, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(query.Author))
        {
            books = books.Where(b => b.Author.Contains(query.Author, StringComparison.OrdinalIgnoreCase));
        }

        // Apply sorting
        var sortOrder = query.SortOrder?.ToLowerInvariant() ?? "asc";
        var sortBy = query.SortBy?.ToLowerInvariant() ?? "title";

        books = sortBy switch
        {
            "author" => sortOrder == "desc" 
                ? books.OrderByDescending(b => b.Author)
                : books.OrderBy(b => b.Author),
            "edition" => sortOrder == "desc"
                ? books.OrderByDescending(b => b.Edition)
                : books.OrderBy(b => b.Edition),
            _ => sortOrder == "desc"
                ? books.OrderByDescending(b => b.Title)
                : books.OrderBy(b => b.Title)
        };

        var response = books.Select(book => new BookResponse(
            book.Id.Value,
            book.Title,
            book.Author,
            book.Edition,
            book.Active ?? false,
            book.CreatedAt,
            book.UpdatedAt));

        return Result.Success(response);
    }
}
        

