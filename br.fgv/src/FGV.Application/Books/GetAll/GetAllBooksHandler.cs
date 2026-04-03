using FGV.Application.Abstractions.Messaging;
using FGV.Domain.Books;
using FGV.SharedKernel;

namespace FGV.Application.Books.GetAll;

internal sealed class GetAllBooksHandler : IQueryHandler<GetAllBooksQuery, IEnumerable<BookResponse>>
{
    private readonly IBookRepository _bookRepository;

    public GetAllBooksHandler(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public async Task<Result<IEnumerable<BookResponse>>> HandleAsync(
        GetAllBooksQuery query,
        CancellationToken cancellationToken)
    {
        IEnumerable<Book> books = await _bookRepository.GetAllAsync(cancellationToken);

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
