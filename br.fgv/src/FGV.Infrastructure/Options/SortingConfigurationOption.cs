namespace FGV.Infrastructure.Options;

public sealed class SortingConfigurationOption
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<SortingRuleOption> Rules { get; set; } = new();
}
