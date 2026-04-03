using FGV.Domain.Books;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FGV.Infrastructure.Configurations.Books;

internal sealed class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.ToTable("books");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnName("id")
            .HasConversion(id => id.Value, value => new BookId(value))
            .HasMaxLength(36)
            .IsRequired();

        builder.Property(e => e.Title)
            .HasColumnName("title")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(e => e.Author)
            .HasColumnName("author")
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(e => e.Edition)
            .HasColumnName("edition")
            .IsRequired();

        builder.Property(e => e.CreatedBy)
            .HasColumnName("created_by");

        builder.Property(e => e.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(e => e.UpdatedAt)
            .HasColumnName("updated_at");

        builder.Property(e => e.UpdatedBy)
            .HasColumnName("updated_by");

        builder.Property(e => e.Active)
            .HasColumnName("active")
            .IsRequired();

        builder.HasIndex(e => e.Title)
            .HasDatabaseName("ix_books_title");

        builder.HasIndex(e => e.Author)
            .HasDatabaseName("ix_books_author");
    }
}
