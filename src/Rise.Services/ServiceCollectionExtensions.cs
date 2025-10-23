using Microsoft.Extensions.DependencyInjection;
using Rise.Persistence;
using Rise.Services.Products;
using Rise.Services.Projects;
using Rise.Services.Schedule;
using Rise.Services.News;
using Rise.Shared.News;
// duplicate using removed
using Rise.Shared.Products;
using Rise.Shared.Projects;
using Rise.Shared.Campus;
using Rise.Services.Campus;
using Rise.Services.Resto;
using Rise.Shared.Resto;
using Rise.Shared.CampusInfo;
using Rise.Services.CampusInfo;
using Rise.Shared.Schedule;

namespace Rise.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<ICampusService, CampusService>();
        services.AddTransient<DbSeeder>();

        services.AddScoped<IScheduleService, MockScheduleService>();
        services.AddScoped<IRestoService, MockRestoService>();
        services.AddScoped<INewsService, NewsService>();
        services.AddScoped<ICampusInfoService, CampusInfoService>();
        services.AddTransient<DbSeeder>();

        // Add other application services here.
        return services;
    }
}