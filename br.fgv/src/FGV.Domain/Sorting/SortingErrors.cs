using FGV.SharedKernel;

namespace FGV.Domain.Sorting;

public static class SortingErrors
{
    public static Error ConfigurationNotFound(string configId) => new(
        "Sorting.ConfigurationNotFound",
        $"The sorting configuration with Id '{configId}' was not found");

    public static Error ConfigurationNameNotFound(string name) => new(
        "Sorting.ConfigurationNameNotFound",
        $"The sorting configuration with name '{name}' was not found");

    public static Error InvalidConfigurationName => new(
        "Sorting.InvalidConfigurationName",
        "The configuration name cannot be empty");

    public static Error NullRulesCollection => new(
        "Sorting.NullRulesCollection",
        "The sorting rules collection cannot be null");

    public static Error EmptyRulesCollection => new(
        "Sorting.EmptyRulesCollection",
        "The sorting configuration must have at least one rule");

    public static Error RuleNotFound(string ruleId) => new(
        "Sorting.RuleNotFound",
        $"The sorting rule with Id '{ruleId}' was not found");
}
