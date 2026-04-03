using FGV.Application.Abstractions.Messaging;
using FGV.Application.Books;
using FGV.Application.Books.Create;
using FGV.Application.Books.Delete;
using FGV.Application.Books.GetAll;
using FGV.Application.Books.GetById;
using FGV.Application.Books.Update;
using FGV.SharedKernel;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FGV.Api.Controllers.Books;

/// <summary>
/// Gerenciamento de Livros
/// </summary>
[Route("api/books")]
[ApiController]
[Produces("application/json")]
[SwaggerTag("Endpoints para CRUD de livros")]
public sealed class BooksController : ControllerBase
{
    /// <summary>
    /// Cria um novo livro
    /// </summary>
    /// <param name="request">Dados do livro a ser criado</param>
    /// <param name="handler">Handler do comando</param>
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
        ICommandHandler<CreateBookCommand, string> handler,
        CancellationToken cancellationToken)
    {
        var command = new CreateBookCommand(
            request.Title,
            request.Author,
            request.Edition);

        Result<string> result = await handler.HandleAsync(command, cancellationToken);

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
    /// <param name="handler">Handler da query</param>
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
        IQueryHandler<GetBookByIdQuery, BookResponse> handler,
        CancellationToken cancellationToken)
    {
        var query = new GetBookByIdQuery(id);

        Result<BookResponse> result = await handler.HandleAsync(query, cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(result.Error);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Lista todos os livros
    /// </summary>
    /// <param name="title">Filtro por título (opcional)</param>
    /// <param name="author">Filtro por autor (opcional)</param>
    /// <param name="sortBy">Campo para ordenação: title, author, edition (padrão: title)</param>
    /// <param name="sortOrder">Ordem de ordenação: asc, desc (padrão: asc)</param>
    /// <param name="handler">Handler da query</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista de livros ativos filtrados e ordenados</returns>
    /// <response code="200">Lista de livros retornada com sucesso</response>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Lista todos os livros",
        Description = "Retorna uma lista com todos os livros ativos no catálogo, com opções de filtragem por título e autor, e ordenação personalizável",
        OperationId = "GetAllBooks",
        Tags = new[] { "Books" }
    )]
    [ProducesResponseType(typeof(IEnumerable<BookResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? title,
        [FromQuery] string? author,
        [FromQuery] string? sortBy,
        [FromQuery] string? sortOrder,
        IQueryHandler<GetAllBooksQuery, IEnumerable<BookResponse>> handler,
        CancellationToken cancellationToken)
    {
        var query = new GetAllBooksQuery(title, author, sortBy, sortOrder);

        Result<IEnumerable<BookResponse>> result = await handler.HandleAsync(query, cancellationToken);

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
    /// <param name="handler">Handler do comando</param>
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
        ICommandHandler<UpdateBookCommand> handler,
        CancellationToken cancellationToken)
    {
        var command = new UpdateBookCommand(
            id,
            request.Title,
            request.Author,
            request.Edition);

        Result result = await handler.HandleAsync(command, cancellationToken);

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
    /// <param name="handler">Handler do comando</param>
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
        ICommandHandler<DeleteBookCommand> handler,
        CancellationToken cancellationToken)
    {
        var command = new DeleteBookCommand(id);

        Result result = await handler.HandleAsync(command, cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(result.Error);
        }

        return NoContent();
    }
}
