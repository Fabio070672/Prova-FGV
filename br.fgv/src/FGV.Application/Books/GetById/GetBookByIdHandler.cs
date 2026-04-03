using FGV.Application.Abstractions.Messaging;
using FGV.Domain.Books;
using FGV.SharedKernel;

namespace FGV.Application.Books.GetById;

internal sealed class GetBookByIdHandler : IQueryHandler<GetBookByIdQuery, BookResponse>
{
    private readonly IBookRepository _bookRepository;

    public GetBookByIdHandler(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public async Task<Result<BookResponse>> HandleAsync(
        GetBookByIdQuery query,
        CancellationToken cancellationToken)
    {
        Book? book = await _bookRepository.GetByIdAsync(
            new BookId(query.Id),
            cancellationToken);

        if (book is null)
        {
            return Result.Failure<BookResponse>(
                BookErrors.NotFound(query.Id));
        }

        var response = new BookResponse(
            book.Id.Value,
            book.Title,
            book.Author,
            book.Edition,
            book.Active ?? false,
            book.CreatedAt,
            book.UpdatedAt);

        return Result.Success(response);
    }
}
