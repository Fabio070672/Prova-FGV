using FGV.Application.Abstractions.Messaging;
using FGV.Domain.Books;
using FGV.SharedKernel;

namespace FGV.Application.Books.Update;

internal sealed class UpdateBookHandler : ICommandHandler<UpdateBookCommand>
{
    private readonly IBookRepository _bookRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateBookHandler(
        IBookRepository bookRepository,
        IUnitOfWork unitOfWork)
    {
        _bookRepository = bookRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> HandleAsync(
        UpdateBookCommand command,
        CancellationToken cancellationToken)
    {
        Book? book = await _bookRepository.GetByIdAsync(
            new BookId(command.Id),
            cancellationToken);

        if (book is null)
        {
            return Result.Failure(BookErrors.NotFound(command.Id));
        }

        book.Update(
            command.Title,
            command.Author,
            command.Edition);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
