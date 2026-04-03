namespace FGV.Application.Sorting;

public sealed record SortingRuleResponse(
    string Attribute,
    string Direction,
    int Order);
