using FGV.Domain.Books;
using FGV.SharedKernel;

namespace FGV.Domain.Sorting;

/// <summary>
/// Serviço de domínio para ordenação de livros
/// </summary>
public interface IBookSortingService
{
    /// <summary>
    /// Ordena uma coleção de livros de acordo com as regras de configuração
    /// </summary>
    /// <param name="books">Coleção de livros a ser ordenada</param>
    /// <param name="configuration">Configuração de ordenação</param>
    /// <returns>Result contendo a coleção ordenada ou erro</returns>
    Result<IEnumerable<Book>> SortBooks(
        IEnumerable<Book>? books,
        SortingConfiguration configuration);

    /// <summary>
    /// Ordena uma coleção de livros de acordo com o nome da configuração
    /// </summary>
    /// <param name="books">Coleção de livros a ser ordenada</param>
    /// <param name="configurationName">Nome da configuração</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Result contendo a coleção ordenada ou erro</returns>
    Task<Result<IEnumerable<Book>>> SortBooksAsync(
        IEnumerable<Book>? books,
        string configurationName,
        CancellationToken cancellationToken = default);
}
