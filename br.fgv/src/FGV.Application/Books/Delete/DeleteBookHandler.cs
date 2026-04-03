using FGV.Application.Abstractions.Messaging;
using FGV.Domain.Books;
using FGV.SharedKernel;

namespace FGV.Application.Books.Delete;

internal sealed class DeleteBookHandler : ICommandHandler<DeleteBookCommand>
{
    private readonly IBookRepository _bookRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteBookHandler(
        IBookRepository bookRepository,
        IUnitOfWork unitOfWork)
    {
        _bookRepository = bookRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> HandleAsync(
        DeleteBookCommand command,
        CancellationToken cancellationToken)
    {
        Book? book = await _bookRepository.GetByIdAsync(
            new BookId(command.Id),
            cancellationToken);

        if (book is null)
        {
            return Result.Failure(BookErrors.NotFound(command.Id));
        }

        book.Deactivate();

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
