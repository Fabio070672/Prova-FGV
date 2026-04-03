namespace FGV.Domain.Books;

public interface IBookRepository
{
    Task<Book?> GetByIdAsync(BookId id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Book>> GetAllAsync(CancellationToken cancellationToken = default);
    void Add(Book book);
    void Remove(Book book);
}
