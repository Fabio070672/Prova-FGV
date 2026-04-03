using FGV.Application.Abstractions.Messaging;

namespace FGV.Application.Books.Update;

public sealed record UpdateBookCommand(
    string Id,
    string Title,
    string Author,
    int Edition) : ICommand;
