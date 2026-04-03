using System.ComponentModel.DataAnnotations;

namespace FGV.Api.Controllers.Books;

/// <summary>
/// Request para criação de um novo livro
/// </summary>
public sealed record CreateBookRequest(
    /// <summary>
    /// Título do livro
    /// </summary>
    /// <example>Java How to Program</example>
    [Required]
    string Title,
    
    /// <summary>
    /// Nome do autor
    /// </summary>
    /// <example>Deitel &amp; Deitel</example>
    [Required]
    string Author,
    
    /// <summary>
    /// Ano de edição
    /// </summary>
    /// <example>2007</example>
    [Required]
    int Edition);
