using System.ComponentModel.DataAnnotations;

namespace FGV.Api.Controllers.Books;

/// <summary>
/// Request para atualização de um livro existente
/// </summary>
public sealed record UpdateBookRequest(
    /// <summary>
    /// Título do livro
    /// </summary>
    /// <example>Java How to Program - 8th Edition</example>
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
    /// <example>2010</example>
    [Required]
    int Edition);
