namespace FGV.Domain.Sorting;

public record SortingConfigurationId(string Value)
{
    public static SortingConfigurationId NewId() => new(Guid.NewGuid().ToString());
}
