namespace FGV.Domain.Books;

public record BookId(string Value)
{
    public static BookId NewId() => new(Guid.NewGuid().ToString());
}
