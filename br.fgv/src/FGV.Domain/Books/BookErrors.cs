using FGV.SharedKernel;

namespace FGV.Domain.Books;

public static class BookErrors
{
    public static Error NotFound(string bookId) => new(
        "Books.NotFound",
        $"The book with Id '{bookId}' was not found");

    public static Error InvalidTitle => new(
        "Books.InvalidTitle",
        "The book title cannot be empty");

    public static Error InvalidAuthor => new(
        "Books.InvalidAuthor",
        "The book author cannot be empty");

    public static Error InvalidEdition => new(
        "Books.InvalidEdition",
        "The book edition must be a positive number");
}
