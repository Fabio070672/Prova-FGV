using FGV.Domain.Books;
using FGV.Domain.Sorting;
using FGV.Infrastructure.Options;
using FGV.Infrastructure.Repositories;
using FGV.Infrastructure.Services;
using FGV.SharedKernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FGV.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Use InMemory Database
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseInMemoryDatabase("FGV_BookSorting_DB"));

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Register repositories
        services.AddScoped<IBookRepository, BookRepository>();

        // Configure book sorting options
        services.Configure<BookSortingOptions>(
            configuration.GetSection(BookSortingOptions.SectionName));

        // Register sorting service
        services.AddScoped<IBookSortingService, BookSortingService>();

        return services;
    }
}

