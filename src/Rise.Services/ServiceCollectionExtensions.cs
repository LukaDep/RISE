using Microsoft.Extensions.DependencyInjection;
using Rise.Persistence;
using Rise.Services.Absences;
using Rise.Services.Products;
using Rise.Services.Projects;
using Rise.Services.Schedule;
using Rise.Services.News;
using Rise.Services.Campus;
using Rise.Services.CampusInfo;
using Rise.Services.Grades;
using Rise.Shared.News;
using Rise.Shared.Products;
using Rise.Shared.Projects;
using Rise.Shared.Campus;
using Rise.Services.Campus;
using Rise.Services.Resto;
using Rise.Shared.Resto;
using Rise.Shared.CampusInfo;
using Rise.Services.CampusInfo;
using Rise.Shared.Absences;
using Rise.Shared.Schedule;
using Rise.Shared.Grades;

namespace Rise.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<ICampusService, CampusService>();
        services.AddScoped<IScheduleService, MockScheduleService>();
        services.AddScoped<IRestoService, MockRestoService>();
        services.AddScoped<INewsService, NewsService>();
        services.AddScoped<ICampusInfoService, CampusInfoService>();
        services.AddScoped<IAbsencesService, AbsencesService>();
        services.AddScoped<IGradesService, GradesService>();
        services.AddTransient<DbSeeder>();

        return services;
    }
}