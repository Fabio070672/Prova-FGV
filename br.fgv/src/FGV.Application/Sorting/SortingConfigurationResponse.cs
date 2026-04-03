namespace FGV.Application.Sorting;

public sealed record SortingConfigurationResponse(
    string Name,
    string? Description,
    IEnumerable<SortingRuleResponse> Rules);
