using FGV.Domain.Books;

namespace FGV.Domain.Sorting;

/// <summary>
/// Interface do serviço de ordenação de livros
/// </summary>
public interface IBookSortingService
{
    /// <summary>
    /// Ordena uma coleção de livros conforme a configuração especificada
    /// </summary>
    /// <param name="books">Coleção de livros a ser ordenada</param>
    /// <param name="configurationName">Nome da configuração de ordenação (opcional)</param>
    /// <returns>Coleção ordenada de livros</returns>
    /// <exception cref="OrdenacaoException">Lançada quando a coleção é nula ou quando há erro na configuração</exception>
    IEnumerable<Book> Sort(IEnumerable<Book>? books, string? configurationName = null);

    /// <summary>
    /// Ordena uma coleção de livros com regras dinâmicas
    /// </summary>
    /// <param name="books">Coleção de livros a ser ordenada</param>
    /// <param name="sortRules">Lista de regras de ordenação (campo e direção)</param>
    /// <returns>Coleção ordenada de livros</returns>
    /// <exception cref="OrdenacaoException">Lançada quando a coleção é nula ou quando há erro nas regras</exception>
    IEnumerable<Book> SortDynamic(IEnumerable<Book>? books, IEnumerable<(string Field, string Direction)> sortRules);
}

