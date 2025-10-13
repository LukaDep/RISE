using Microsoft.Extensions.DependencyInjection;
using Rise.Persistence;
using Rise.Services.Products;
using Rise.Services.Projects;
using Rise.Services.News;
using Rise.Shared.News;
using Rise.Shared.Products;
using Rise.Shared.Projects;

namespace Rise.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IProductService, ProductService>();        
        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<INewsService, NewsService>();
        services.AddTransient<DbSeeder>();       
        
        // Add other application services here.
        return services;
    }
}