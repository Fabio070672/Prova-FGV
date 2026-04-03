using FGV.Application.Abstractions.Messaging;

namespace FGV.Application.Books.Delete;

public sealed record DeleteBookCommand(string Id) : ICommand;
