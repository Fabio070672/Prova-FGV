using FGV.Application.Abstractions.Messaging;
using FGV.Domain.Books;
using FGV.SharedKernel;

namespace FGV.Application.Books.Create;

internal sealed class CreateBookHandler : ICommandHandler<CreateBookCommand, string>
{
    private readonly IBookRepository _bookRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateBookHandler(
        IBookRepository bookRepository,
        IUnitOfWork unitOfWork)
    {
        _bookRepository = bookRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<string>> HandleAsync(
        CreateBookCommand command,
        CancellationToken cancellationToken)
    {
        var book = Book.Create(
            command.Title,
            command.Author,
            command.Edition);

        _bookRepository.Add(book);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(book.Id.Value);
    }
}
