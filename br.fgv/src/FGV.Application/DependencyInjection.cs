using FGV.Application.Interfaces;
using FGV.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FGV.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register services following SOLID principles
        // - Dependency Inversion Principle: Depend on abstractions (IBookService)
        // - Single Responsibility: Each service has a single, well-defined purpose
        services.AddScoped<IBookService, BookService>();

        return services;
    }
}

