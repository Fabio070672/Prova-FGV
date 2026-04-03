namespace FGV.Domain.Sorting;

public interface ISortingConfigurationRepository
{
    Task<SortingConfiguration?> GetByIdAsync(SortingConfigurationId id, CancellationToken cancellationToken = default);
    Task<SortingConfiguration?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IEnumerable<SortingConfiguration>> GetAllAsync(CancellationToken cancellationToken = default);
    void Add(SortingConfiguration configuration);
    void Remove(SortingConfiguration configuration);
}
