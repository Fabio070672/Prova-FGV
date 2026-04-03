namespace FGV.SharedKernel;

public abstract class Entity<TId>
{
    protected Entity()
    {
    }

    protected Entity(TId id)
    {
        Id = id;
    }

    public TId Id { get; protected set; } = default!;
    public DateTime CreatedAt { get; protected set; }
    public Guid? CreatedBy { get; protected set; }
    public DateTime? UpdatedAt { get; protected set; }
    public Guid? UpdatedBy { get; protected set; }
    public bool? Active { get; protected set; }
}
