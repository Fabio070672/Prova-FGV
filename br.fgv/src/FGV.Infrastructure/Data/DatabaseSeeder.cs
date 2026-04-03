using FGV.Domain.Books;

namespace FGV.Infrastructure.Data;

/// <summary>
/// Seed para popular o banco InMemory com dados de teste da documentação FGV
/// </summary>
public static class DatabaseSeeder
{
    public static void SeedTestData(ApplicationDbContext context)
    {
        // Verifica se já existem dados
        if (context.Books.Any())
        {
            return;
        }

        // Dados de teste conforme documentação FGV
        var books = new List<Book>
        {
            // Livro 1
            Book.Create(
                title: "Java How to Program",
                author: "Deitel & Deitel",
                edition: 2007
            ),

            // Livro 2
            Book.Create(
                title: "Patterns of Enterprise Application Architecture",
                author: "Martin Fowler",
                edition: 2002
            ),

            // Livro 3
            Book.Create(
                title: "Head First Design Patterns",
                author: "Elisabeth Freeman",
                edition: 2004
            ),

            // Livro 4
            Book.Create(
                title: "Internet & World Wide Web: How to Program",
                author: "Deitel & Deitel",
                edition: 2007
            )
        };

        context.Books.AddRange(books);
        context.SaveChanges();
    }
}
