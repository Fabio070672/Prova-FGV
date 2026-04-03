namespace FGV.Application.Books.GetAll;

/// <summary>
/// DTO para definir regras de ordenação dinâmicas via query string
/// </summary>
public sealed class SortRule
{
    /// <summary>
    /// Campo a ser ordenado: Title, Author, ou Edition
    /// </summary>
    public string Field { get; set; } = string.Empty;

    /// <summary>
    /// Direção da ordenação: Asc ou Desc
    /// </summary>
    public string Direction { get; set; } = "Asc";
}
