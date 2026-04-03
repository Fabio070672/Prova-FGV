using FGV.Application.Abstractions.Messaging;

namespace FGV.Application.Books.Create;

public sealed record CreateBookCommand(
    string Title,
    string Author,
    int Edition) : ICommand<string>;
