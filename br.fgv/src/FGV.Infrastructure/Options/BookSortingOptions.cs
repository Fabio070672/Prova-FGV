namespace FGV.Infrastructure.Options;

public sealed class BookSortingOptions
{
    public const string SectionName = "BookSorting";
    
    public string DefaultConfiguration { get; set; } = "Default";
    public List<SortingConfigurationOption> Configurations { get; set; } = new();
}
