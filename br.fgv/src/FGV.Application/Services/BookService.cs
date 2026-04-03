using FGV.Application.Books;
using FGV.Application.Books.GetAll;
using FGV.Application.Interfaces;
using FGV.Domain.Books;
using FGV.Domain.Sorting;
using FGV.SharedKernel;

namespace FGV.Application.Services;

/// <summary>
/// Implementação do serviço de gerenciamento de livros
/// Segue os princípios SOLID:
/// - SRP: Responsabilidade única de gerenciar operações de livros
/// - OCP: Aberto para extensão através de interfaces
/// - LSP: Implementa IBookService de forma substituível
/// - ISP: Interface segregada com métodos específicos
/// - DIP: Depende de abstrações (IBookRepository, IBookSortingService)
/// </summary>
public sealed class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;
    private readonly IBookSortingService _sortingService;
    private readonly IUnitOfWork _unitOfWork;

    public BookService(
        IBookRepository bookRepository,
        IBookSortingService sortingService,
        IUnitOfWork unitOfWork)
    {
        _bookRepository = bookRepository;
        _sortingService = sortingService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IEnumerable<BookResponse>>> GetAllAsync(
        List<SortRule>? sortRules = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var books = await _bookRepository.GetAllAsync(cancellationToken);

            IEnumerable<Book> sortedBooks;

            // Se foram fornecidas regras dinâmicas de ordenação, usa-as
            if (sortRules != null && sortRules.Any())
            {
                var dynamicRules = sortRules
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

    public async Task<Result<BookResponse>> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id) || !Guid.TryParse(id, out _))
        {
            return Result.Failure<BookResponse>(BookErrors.InvalidId);
        }

        var bookId = new BookId(id);
        var book = await _bookRepository.GetByIdAsync(bookId, cancellationToken);

        if (book is null)
        {
            return Result.Failure<BookResponse>(BookErrors.NotFound(id));
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

    public async Task<Result<string>> CreateAsync(
        string title,
        string author,
        int edition,
        CancellationToken cancellationToken = default)
    {
        // Validações básicas
        if (string.IsNullOrWhiteSpace(title))
        {
            return Result.Failure<string>(new Error("Book.TitleRequired", "O título é obrigatório"));
        }

        if (string.IsNullOrWhiteSpace(author))
        {
            return Result.Failure<string>(new Error("Book.AuthorRequired", "O autor é obrigatório"));
        }

        if (edition <= 0)
        {
            return Result.Failure<string>(new Error("Book.InvalidEdition", "A edição deve ser maior que zero"));
        }

        var book = Book.Create(title, author, edition);

        _bookRepository.Add(book);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(book.Id.Value);
    }

    public async Task<Result> UpdateAsync(
        string id,
        string title,
        string author,
        int edition,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id) || !Guid.TryParse(id, out _))
        {
            return Result.Failure(BookErrors.InvalidId);
        }

        // Validações básicas
        if (string.IsNullOrWhiteSpace(title))
        {
            return Result.Failure(new Error("Book.TitleRequired", "O título é obrigatório"));
        }

        if (string.IsNullOrWhiteSpace(author))
        {
            return Result.Failure(new Error("Book.AuthorRequired", "O autor é obrigatório"));
        }

        if (edition <= 0)
        {
            return Result.Failure(new Error("Book.InvalidEdition", "A edição deve ser maior que zero"));
        }

        var bookId = new BookId(id);
        var book = await _bookRepository.GetByIdAsync(bookId, cancellationToken);

        if (book is null)
        {
            return Result.Failure(BookErrors.NotFound(id));
        }

        book.Update(title, author, edition);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id) || !Guid.TryParse(id, out _))
        {
            return Result.Failure(BookErrors.InvalidId);
        }

        var bookId = new BookId(id);
        var book = await _bookRepository.GetByIdAsync(bookId, cancellationToken);

        if (book is null)
        {
            return Result.Failure(BookErrors.NotFound(id));
        }

        book.Deactivate();

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
