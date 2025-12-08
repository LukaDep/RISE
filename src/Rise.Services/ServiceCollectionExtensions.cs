using Microsoft.Extensions.DependencyInjection;
using Rise.Persistence;
using Rise.Services.Absences;
using Rise.Services.Campus;
using Rise.Services.Contact;
using Rise.Services.Deadlines;
using Rise.Services.Grades;
using Rise.Services.Menu;
using Rise.Services.News;
using Rise.Shared.News;
using Rise.Shared.Campus;
using Rise.Services.Resto;
using Rise.Services.Schedule;
using Rise.Services.StudentCards;
using Rise.Shared.Resto;
using Rise.Shared.Absences;
using Rise.Shared.Contact;
using Rise.Shared.Grades;
using Rise.Shared.Menu;
using Rise.Services.Notifications;
using Rise.Shared.Deadlines;
using Rise.Services.Widgets;
using Rise.Shared.Notifications;
using Rise.Shared.Schedule;
using Rise.Shared.StudentCards;
using Rise.Shared.Widgets;
using Rise.Shared.Events;
using Rise.Services.Events;
using System.ComponentModel.Design;

namespace Rise.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ICampusService, CampusService>();
        services.AddScoped<IMenuService, MenuService>();
        services.AddScoped<IScheduleService, MockScheduleService>();
        services.AddScoped<IRestoService, RestoService>();
        services.AddScoped<INewsService, NewsService>();
        services.AddScoped<IContactService, ContactService>();
        services.AddScoped<IAbsencesService, AbsencesService>();
        services.AddScoped<IGradesService, GradesService>();
        services.AddScoped<INotificationPreferencesService, NotificationPreferencesService>();
        services.AddScoped<ISentNotificationService, SentNotificationService>();
        services.AddScoped<IStudentCardService, StudentCardService>();
        services.AddScoped<IDeadlineService, DeadlineService>();
        services.AddScoped<IWidgetService, WidgetService>();
        services.AddScoped<IEventService, EventService>();
        services.AddTransient<DbSeeder>();
        return services;
    }
}