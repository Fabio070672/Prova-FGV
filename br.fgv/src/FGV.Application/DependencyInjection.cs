using FGV.Application.Abstractions.Messaging;
using FGV.Application.Books;
using FGV.Application.Books.Create;
using FGV.Application.Books.Delete;
using FGV.Application.Books.GetAll;
using FGV.Application.Books.GetById;
using FGV.Application.Books.Update;
using Microsoft.Extensions.DependencyInjection;

namespace FGV.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register Book handlers
        services.AddScoped<ICommandHandler<CreateBookCommand, string>, CreateBookHandler>();
        services.AddScoped<ICommandHandler<UpdateBookCommand>, UpdateBookHandler>();
        services.AddScoped<ICommandHandler<DeleteBookCommand>, DeleteBookHandler>();
        services.AddScoped<IQueryHandler<GetBookByIdQuery, BookResponse>, GetBookByIdHandler>();
        services.AddScoped<IQueryHandler<GetAllBooksQuery, IEnumerable<BookResponse>>, GetAllBooksHandler>();

        return services;
    }
}
