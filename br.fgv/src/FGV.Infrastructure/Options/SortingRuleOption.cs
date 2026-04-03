namespace FGV.Infrastructure.Options;

public sealed class SortingRuleOption
{
    public string Attribute { get; set; } = string.Empty;
    public string Direction { get; set; } = string.Empty;
    public int Order { get; set; }
}
