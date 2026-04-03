using FGV.SharedKernel;

namespace FGV.Domain.Books;

public sealed class Book : Entity<BookId>
{
    private Book() { }

    private Book(
        BookId id,
        string title,
        string author,
        int edition,
        Guid? createdBy)
    {
        Id = id;
        Title = title;
        Author = author;
        Edition = edition;
        CreatedBy = createdBy;
        CreatedAt = DateTime.UtcNow;
        Active = true;
    }

    public string Title { get; private set; } = default!;
    public string Author { get; private set; } = default!;
    public int Edition { get; private set; }

    public static Book Create(
        string title,
        string author,
        int edition,
        Guid? createdBy = null)
    {
        return new Book(
            BookId.NewId(),
            title,
            author,
            edition,
            createdBy);
    }

    public void Update(
        string title,
        string author,
        int edition,
        Guid? updatedBy = null)
    {
        Title = title;
        Author = author;
        Edition = edition;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    public void Deactivate()
    {
        Active = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        Active = true;
        UpdatedAt = DateTime.UtcNow;
    }
}
