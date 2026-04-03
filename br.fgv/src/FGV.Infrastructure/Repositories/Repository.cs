using FGV.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace FGV.Infrastructure.Repositories;

internal abstract class Repository<TEntity, TId>
    where TEntity : Entity<TId>
{
    protected readonly ApplicationDbContext DbContext;

    protected Repository(ApplicationDbContext dbContext)
    {
        DbContext = dbContext;
    }

    public virtual async Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        return await DbContext.Set<TEntity>()
            .FirstOrDefaultAsync(e => e.Id!.Equals(id), cancellationToken);
    }

    public virtual void Add(TEntity entity)
    {
        DbContext.Set<TEntity>().Add(entity);
    }

    public virtual void Remove(TEntity entity)
    {
        DbContext.Set<TEntity>().Remove(entity);
    }
}
