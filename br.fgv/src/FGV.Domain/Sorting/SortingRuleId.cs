namespace FGV.Domain.Sorting;

public record SortingRuleId(string Value)
{
    public static SortingRuleId NewId() => new(Guid.NewGuid().ToString());
}
