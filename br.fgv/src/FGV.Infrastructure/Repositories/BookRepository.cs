using FGV.Domain.Books;
using Microsoft.EntityFrameworkCore;

namespace FGV.Infrastructure.Repositories;

internal sealed class BookRepository : Repository<Book, BookId>, IBookRepository
{
    public BookRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IEnumerable<Book>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbContext.Set<Book>()
            .Where(b => b.Active == true)
            .OrderBy(b => b.Title)
            .ToListAsync(cancellationToken);
    }
}
