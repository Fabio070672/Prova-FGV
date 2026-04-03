using FGV.Application.Books;
using FGV.Application.Books.GetAll;
using FGV.SharedKernel;

namespace FGV.Application.Interfaces;

/// <summary>
/// Interface do serviço de gerenciamento de livros
/// Segue o princípio de Interface Segregation (ISP) e Dependency Inversion (DIP) do SOLID
/// </summary>
public interface IBookService
{
    /// <summary>
    /// Obtém todos os livros com ordenação configurável
    /// </summary>
    Task<Result<IEnumerable<BookResponse>>> GetAllAsync(
        List<SortRule>? sortRules = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém um livro por ID
    /// </summary>
    Task<Result<BookResponse>> GetByIdAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cria um novo livro
    /// </summary>
    Task<Result<string>> CreateAsync(string title, string author, int edition, CancellationToken cancellationToken = default);

    /// <summary>
    /// Atualiza um livro existente
    /// </summary>
    Task<Result> UpdateAsync(string id, string title, string author, int edition, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove um livro (soft delete)
    /// </summary>
    Task<Result> DeleteAsync(string id, CancellationToken cancellationToken = default);
}

