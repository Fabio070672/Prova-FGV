using FGV.SharedKernel;

namespace FGV.Domain.Sorting;

/// <summary>
/// Regra de ordenação que define um atributo e sua direção
/// </summary>
public sealed class SortingRule : Entity<SortingRuleId>
{
    private SortingRule() { }

    private SortingRule(
        SortingRuleId id,
        BookSortAttribute attribute,
        SortDirection direction,
        int order,
        Guid? createdBy)
    {
        Id = id;
        Attribute = attribute;
        Direction = direction;
        Order = order;
        CreatedBy = createdBy;
        CreatedAt = DateTime.UtcNow;
        Active = true;
    }

    public BookSortAttribute Attribute { get; private set; }
    public SortDirection Direction { get; private set; }
    public int Order { get; private set; }

    public static SortingRule Create(
        BookSortAttribute attribute,
        SortDirection direction,
        int order,
        Guid? createdBy = null)
    {
        return new SortingRule(
            SortingRuleId.NewId(),
            attribute,
            direction,
            order,
            createdBy);
    }

    public void Update(
        BookSortAttribute attribute,
        SortDirection direction,
        int order,
        Guid? updatedBy = null)
    {
        Attribute = attribute;
        Direction = direction;
        Order = order;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    public void Deactivate()
    {
        Active = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        Active = true;
        UpdatedAt = DateTime.UtcNow;
    }
}
