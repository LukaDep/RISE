using Microsoft.Extensions.DependencyInjection;
using Rise.Persistence;
using Rise.Services.Products;
using Rise.Services.Projects;
using Rise.Services.TimeEdit;
using Rise.Shared.Products;
using Rise.Shared.Projects;
using Rise.Shared.TimeEdit;

namespace Rise.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<ITimeEditService, MockTimeEditService>();
        services.AddTransient<DbSeeder>();

        return services;
    }
}