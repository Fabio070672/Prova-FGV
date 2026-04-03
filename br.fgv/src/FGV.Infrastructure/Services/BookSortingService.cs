using FGV.Domain.Books;
using FGV.Domain.Sorting;
using FGV.Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace FGV.Infrastructure.Services;

/// <summary>
/// Serviço de ordenação de livros baseado em configuração
/// </summary>
public sealed class BookSortingService : IBookSortingService
{
    private readonly BookSortingOptions _options;

    public BookSortingService(IOptions<BookSortingOptions> options)
    {
        _options = options.Value;
    }

    public IEnumerable<Book> Sort(IEnumerable<Book>? books, string? configurationName = null)
    {
        if (books is null)
        {
            throw new OrdenacaoException("A coleção de livros não pode ser nula");
        }

        var booksList = books.ToList();
        
        if (!booksList.Any())
        {
            return booksList;
        }

        var config = GetConfiguration(configurationName);
        
        if (!config.Rules.Any())
        {
            return booksList;
        }

        IOrderedEnumerable<Book>? orderedBooks = null;

        foreach (var rule in config.Rules.OrderBy(r => r.Order))
        {
            if (!Enum.TryParse<BookSortAttribute>(rule.Attribute, true, out var attribute))
            {
                throw new OrdenacaoException($"Atributo de ordenação inválido: {rule.Attribute}");
            }

            if (!Enum.TryParse<SortDirection>(rule.Direction, true, out var direction))
            {
                throw new OrdenacaoException($"Direção de ordenação inválida: {rule.Direction}");
            }

            if (orderedBooks == null)
            {
                orderedBooks = direction == SortDirection.Ascending
                    ? OrderBy(booksList, attribute)
                    : OrderByDescending(booksList, attribute);
            }
            else
            {
                orderedBooks = direction == SortDirection.Ascending
                    ? ThenBy(orderedBooks, attribute)
                    : ThenByDescending(orderedBooks, attribute);
            }
        }

        return orderedBooks?.ToList() ?? booksList;
    }

    private SortingConfigurationOption GetConfiguration(string? configurationName)
    {
        var name = string.IsNullOrWhiteSpace(configurationName)
            ? _options.DefaultConfiguration
            : configurationName;

        var config = _options.Configurations.FirstOrDefault(c => 
            c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

        if (config == null)
        {
            throw new OrdenacaoException($"Configuração de ordenação não encontrada: {name}");
        }

        return config;
    }

    private static IOrderedEnumerable<Book> OrderBy(IEnumerable<Book> books, BookSortAttribute attribute)
    {
        return attribute switch
        {
            BookSortAttribute.Title => books.OrderBy(b => b.Title),
            BookSortAttribute.Author => books.OrderBy(b => b.Author),
            BookSortAttribute.Edition => books.OrderBy(b => b.Edition),
            _ => throw new OrdenacaoException($"Atributo de ordenação não suportado: {attribute}")
        };
    }

    private static IOrderedEnumerable<Book> OrderByDescending(IEnumerable<Book> books, BookSortAttribute attribute)
    {
        return attribute switch
        {
            BookSortAttribute.Title => books.OrderByDescending(b => b.Title),
            BookSortAttribute.Author => books.OrderByDescending(b => b.Author),
            BookSortAttribute.Edition => books.OrderByDescending(b => b.Edition),
            _ => throw new OrdenacaoException($"Atributo de ordenação não suportado: {attribute}")
        };
    }

    private static IOrderedEnumerable<Book> ThenBy(IOrderedEnumerable<Book> books, BookSortAttribute attribute)
    {
        return attribute switch
        {
            BookSortAttribute.Title => books.ThenBy(b => b.Title),
            BookSortAttribute.Author => books.ThenBy(b => b.Author),
            BookSortAttribute.Edition => books.ThenBy(b => b.Edition),
            _ => throw new OrdenacaoException($"Atributo de ordenação não suportado: {attribute}")
        };
    }

    private static IOrderedEnumerable<Book> ThenByDescending(IOrderedEnumerable<Book> books, BookSortAttribute attribute)
    {
        return attribute switch
        {
            BookSortAttribute.Title => books.ThenByDescending(b => b.Title),
            BookSortAttribute.Author => books.ThenByDescending(b => b.Author),
            BookSortAttribute.Edition => books.ThenByDescending(b => b.Edition),
            _ => throw new OrdenacaoException($"Atributo de ordenação não suportado: {attribute}")
        };
    }

    public IEnumerable<Book> SortDynamic(IEnumerable<Book>? books, IEnumerable<(string Field, string Direction)> sortRules)
    {
        if (books is null)
        {
            throw new OrdenacaoException("A coleção de livros não pode ser nula");
        }

        var booksList = books.ToList();
        
        if (!booksList.Any())
        {
            return booksList;
        }

        var rulesList = sortRules?.ToList();
        
        if (rulesList == null || !rulesList.Any())
        {
            return booksList;
        }

        IOrderedEnumerable<Book>? orderedBooks = null;

        foreach (var (field, direction) in rulesList)
        {
            if (!Enum.TryParse<BookSortAttribute>(field, true, out var attribute))
            {
                throw new OrdenacaoException($"Campo de ordenação inválido: {field}. Valores válidos: Title, Author, Edition");
            }

            var sortDirection = direction.Equals("Desc", StringComparison.OrdinalIgnoreCase) 
                ? SortDirection.Descending 
                : SortDirection.Ascending;

            if (orderedBooks == null)
            {
                orderedBooks = sortDirection == SortDirection.Ascending
                    ? OrderBy(booksList, attribute)
                    : OrderByDescending(booksList, attribute);
            }
            else
            {
                orderedBooks = sortDirection == SortDirection.Ascending
                    ? ThenBy(orderedBooks, attribute)
                    : ThenByDescending(orderedBooks, attribute);
            }
        }

        return orderedBooks?.ToList() ?? booksList;
    }
}
