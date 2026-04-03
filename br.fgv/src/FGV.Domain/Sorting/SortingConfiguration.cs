using FGV.SharedKernel;

namespace FGV.Domain.Sorting;

/// <summary>
/// Configuração de ordenação contendo múltiplas regras
/// </summary>
public sealed class SortingConfiguration : Entity<SortingConfigurationId>
{
    private readonly List<SortingRule> _rules = new();

    private SortingConfiguration() { }

    private SortingConfiguration(
        SortingConfigurationId id,
        string name,
        string? description,
        Guid? createdBy)
    {
        Id = id;
        Name = name;
        Description = description;
        CreatedBy = createdBy;
        CreatedAt = DateTime.UtcNow;
        Active = true;
    }

    public string Name { get; private set; } = default!;
    public string? Description { get; private set; }
    public IReadOnlyCollection<SortingRule> Rules => _rules.AsReadOnly();

    public static SortingConfiguration Create(
        string name,
        string? description = null,
        Guid? createdBy = null)
    {
        return new SortingConfiguration(
            SortingConfigurationId.NewId(),
            name,
            description,
            createdBy);
    }

    public void AddRule(SortingRule rule)
    {
        _rules.Add(rule);
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveRule(SortingRuleId ruleId)
    {
        var rule = _rules.FirstOrDefault(r => r.Id.Equals(ruleId));
        if (rule != null)
        {
            _rules.Remove(rule);
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void ClearRules()
    {
        _rules.Clear();
        UpdatedAt = DateTime.UtcNow;
    }

    public void Update(
        string name,
        string? description,
        Guid? updatedBy = null)
    {
        Name = name;
        Description = description;
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
