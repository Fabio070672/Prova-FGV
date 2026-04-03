using FGV.Application.Abstractions.Messaging;
using FGV.Domain.Books;
using FGV.Domain.Sorting;
using FGV.SharedKernel;

namespace FGV.Application.Books.GetAll;

internal sealed class GetAllBooksHandler : IQueryHandler<GetAllBooksQuery, IEnumerable<BookResponse>>
{
    private readonly IBookRepository _bookRepository;
    private readonly IBookSortingService _sortingService;

    public GetAllBooksHandler(
        IBookRepository bookRepository,
        IBookSortingService sortingService)
    {
        _bookRepository = bookRepository;
        _sortingService = sortingService;
    }

    public async Task<Result<IEnumerable<BookResponse>>> HandleAsync(
        GetAllBooksQuery query,
        CancellationToken cancellationToken)
    {
        try
        {
            IEnumerable<Book> books = await _bookRepository.GetAllAsync(cancellationToken);

            IEnumerable<Book> sortedBooks;

            // Se foram fornecidas regras dinâmicas de ordenação, usa-as
            if (query.SortRules != null && query.SortRules.Any())
            {
                var dynamicRules = query.SortRules
                    .Select(r => (r.Field, r.Direction))
                    .ToList();
                
                sortedBooks = _sortingService.SortDynamic(books, dynamicRules);
            }
            else
            {
                // Sem regras específicas, retorna sem ordenação
                sortedBooks = books;
            }

            var response = sortedBooks.Select(book => new BookResponse(
                book.Id.Value,
                book.Title,
                book.Author,
                book.Edition,
                book.Active ?? false,
                book.CreatedAt,
                book.UpdatedAt));

            return Result.Success(response);
        }
        catch (OrdenacaoException ex)
        {
            return Result.Failure<IEnumerable<BookResponse>>(
                new Error("BookSorting.Error", ex.Message));
        }
    }
}


        

