using FGV.Application.Books;
using FGV.Application.Books.GetAll;
using FGV.Application.Interfaces;
using FGV.SharedKernel;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FGV.Api.Controllers.Books;

/// <summary>
/// Gerenciamento de Livros
/// Controller seguindo princípios SOLID:
/// - SRP: Responsável apenas por receber requisições HTTP e delegar ao serviço
/// - DIP: Depende da abstração IBookService
/// </summary>
[Route("api/books")]
[ApiController]
[Produces("application/json")]
[SwaggerTag("Endpoints para CRUD de livros")]
public sealed class BooksController : ControllerBase
{
    private readonly IBookService _bookService;

    public BooksController(IBookService bookService)
    {
        _bookService = bookService;
    }
    /// <summary>
    /// Cria um novo livro
    /// </summary>
    /// <param name="request">Dados do livro a ser criado</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>ID do livro criado</returns>
    /// <response code="201">Livro criado com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    [HttpPost]
    [SwaggerOperation(
        Summary = "Cria um novo livro",
        Description = "Adiciona um novo livro ao catálogo com título, autor e edição",
        OperationId = "CreateBook",
        Tags = new[] { "Books" }
    )]
    [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreateBookRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _bookService.CreateAsync(
            request.Title,
            request.Author,
            request.Edition,
            cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Value },
            result.Value);
    }

    /// <summary>
    /// Busca um livro por ID
    /// </summary>
    /// <param name="id">ID do livro</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Dados do livro encontrado</returns>
    /// <response code="200">Livro encontrado</response>
    /// <response code="404">Livro não encontrado</response>
    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "Busca livro por ID",
        Description = "Retorna os dados de um livro específico pelo seu identificador único",
        OperationId = "GetBookById",
        Tags = new[] { "Books" }
    )]
    [ProducesResponseType(typeof(BookResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        [FromRoute] string id,
        CancellationToken cancellationToken)
    {
        var result = await _bookService.GetByIdAsync(id, cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(result.Error);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Lista todos os livros
    /// </summary>
    /// <param name="sortBy">Campos para ordenação separados por vírgula (ex: Title,Author,Edition)</param>
    /// <param name="sortOrder">Direções de ordenação separadas por vírgula (Asc ou Desc, ex: Asc,Desc)</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista de livros ativos ordenados</returns>
    /// <response code="200">Lista de livros retornada com sucesso</response>
    /// <response code="400">Parâmetros de ordenação inválidos</response>
    /// <remarks>
    /// Exemplos de uso:
    /// - GET /api/books (retorna todos os livros sem ordenação específica)
    /// - GET /api/books?sortBy=Title&amp;sortOrder=Asc (ordena por título ascendente)
    /// - GET /api/books?sortBy=Author,Title&amp;sortOrder=Desc,Asc (ordena por autor desc, depois título asc)
    /// - GET /api/books?sortBy=Edition,Author,Title&amp;sortOrder=Desc,Desc,Asc (ordenação por 3 campos)
    /// 
    /// Campos disponíveis: Title, Author, Edition
    /// Direções: Asc (ascendente) ou Desc (descendente)
    /// </remarks>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Lista todos os livros com ordenação customizável",
        Description = "Retorna uma lista com todos os livros ativos, com suporte a ordenação dinâmica por múltiplos campos",
        OperationId = "GetAllBooks",
        Tags = new[] { "Books" }
    )]
    [ProducesResponseType(typeof(IEnumerable<BookResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? sortBy,
        [FromQuery] string? sortOrder,
        CancellationToken cancellationToken)
    {
        List<SortRule>? sortRules = null;

        // Se foram fornecidos parâmetros de ordenação dinâmica
        if (!string.IsNullOrWhiteSpace(sortBy))
        {
            var fields = sortBy.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var directions = string.IsNullOrWhiteSpace(sortOrder)
                ? Enumerable.Repeat("Asc", fields.Length).ToArray()
                : sortOrder.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            // Se houver menos direções que campos, preenche com "Asc"
            if (directions.Length < fields.Length)
            {
                directions = directions
                    .Concat(Enumerable.Repeat("Asc", fields.Length - directions.Length))
                    .ToArray();
            }

            sortRules = fields
                .Select((field, index) => new SortRule
                {
                    Field = field,
                    Direction = index < directions.Length ? directions[index] : "Asc"
                })
                .ToList();
        }

        var result = await _bookService.GetAllAsync(sortRules, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Atualiza um livro existente
    /// </summary>
    /// <param name="id">ID do livro a ser atualizado</param>
    /// <param name="request">Novos dados do livro</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Sem conteúdo em caso de sucesso</returns>
    /// <response code="204">Livro atualizado com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    /// <response code="404">Livro não encontrado</response>
    [HttpPut("{id}")]
    [SwaggerOperation(
        Summary = "Atualiza um livro",
        Description = "Atualiza os dados de um livro existente (título, autor e edição)",
        OperationId = "UpdateBook",
        Tags = new[] { "Books" }
    )]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        [FromRoute] string id,
        [FromBody] UpdateBookRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _bookService.UpdateAsync(
            id,
            request.Title,
            request.Author,
            request.Edition,
            cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return NoContent();
    }

    /// <summary>
    /// Deleta um livro (soft delete)
    /// </summary>
    /// <param name="id">ID do livro a ser deletado</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Sem conteúdo em caso de sucesso</returns>
    /// <response code="204">Livro deletado com sucesso</response>
    /// <response code="404">Livro não encontrado</response>
    [HttpDelete("{id}")]
    [SwaggerOperation(
        Summary = "Deleta um livro",
        Description = "Remove logicamente um livro do catálogo (soft delete - marca como inativo)",
        OperationId = "DeleteBook",
        Tags = new[] { "Books" }
    )]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        [FromRoute] string id,
        CancellationToken cancellationToken)
    {
        var result = await _bookService.DeleteAsync(id, cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(result.Error);
        }

        return NoContent();
    }
}
