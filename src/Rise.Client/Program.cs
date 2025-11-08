using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Rise.Client;
using Rise.Client.Identity;
using Rise.Client.News;
using Rise.Client.Products;
using Rise.Client.Campus;
using Rise.Client.Resto;
using Rise.Client.CampusInfo;
using Rise.Client.Schedule;
using Rise.Shared.CampusInfo;
using Rise.Shared.News;
using Rise.Shared.Campus;
using Rise.Shared.Products;
using Rise.Shared.Schedule;
using Rise.Client.Menu;
using Rise.Shared.Menu;
using Rise.Shared.Resto;
using Rise.Shared.Contact;
using Rise.Client.Contact;
using Rise.Client.Grades;
using Rise.Shared.Grades;

try
{
    var builder = WebAssemblyHostBuilder.CreateDefault(args);

    builder.RootComponents.Add<App>("#app");
    builder.RootComponents.Add<HeadOutlet>("head::after");
    builder.Services.AddLocalization();

    // Determine the backend URL once. If BackendUrl is not configured,
    // fall back to the current host origin (same origin). This avoids
    // mixed-protocol or wrong-host network errors when running in dev.
    var backend = builder.Configuration["BackendUrl"] ?? builder.HostEnvironment.BaseAddress;
    Log.Information("Configured backend URL: {Backend}", backend);

    Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Information()
        .WriteTo.BrowserConsole(outputTemplate: "[{Timestamp:HH:mm:ss}{Level:u3}]{Message:lj} {NewLine}{Exception}")
        .CreateLogger();

    Log.Information("Starting web application");

    // register the cookie handler
    builder.Services.AddTransient<CookieHandler>();

    // set up authorization
    builder.Services.AddAuthorizationCore();

    // register the custom state provider
    builder.Services.AddScoped<AuthenticationStateProvider, CookieAuthenticationStateProvider>();
    // register the account management interface
    builder.Services.AddScoped(sp => (IAccountManager)sp.GetRequiredService<AuthenticationStateProvider>());

    // configure client for auth interactions
    builder.Services.AddHttpClient("SecureApi", opt => opt.BaseAddress = new Uri(builder.Configuration["BackendUrl"] ?? "https://localhost:5001"))
        .AddHttpMessageHandler<CookieHandler>();

    builder.Services.AddHttpClient<IProductService, ProductService>(client =>
    {
        client.BaseAddress = new Uri(builder.Configuration["BackendUrl"] ?? "https://localhost:5001");
    });
    builder.Services.AddHttpClient<ICampusService, CampusClientService>(client =>
    {
        client.BaseAddress = new Uri(builder.Configuration["BackendUrl"] ?? "https://localhost:5001");

    });

    builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

    builder.Services.AddHttpClient<INewsService, NewsService>(client =>
    {
        client.BaseAddress = new Uri(builder.Configuration["BackendUrl"] ?? "https://localhost:5001");
    });

    builder.Services.AddHttpClient<ICampusInfoService, CampusInfoService>(client =>
        {
            client.BaseAddress = new Uri(builder.Configuration["BackendUrl"] ?? "https://localhost:5001");
        });

    builder.Services.AddHttpClient<IScheduleService, ScheduleClientService>(client =>
    {
        client.BaseAddress = new Uri(builder.Configuration["BackendUrl"] ?? "https://localhost:5001");
    });

    builder.Services.AddHttpClient<IGradesService, GradesClientService>(client =>
    {
        client.BaseAddress = new Uri(builder.Configuration["BackendUrl"] ?? "https://localhost:5001");
    });

    builder.Services.AddHttpClient<IMenuService, MenuClientService>(client =>
    {
        client.BaseAddress = new Uri(builder.Configuration["BackendUrl"] ?? "https://localhost:5001");
    });

    builder.Services.AddHttpClient<IRestoService, RestoClientService>(client =>
    {
        client.BaseAddress = new Uri(builder.Configuration["BackendUrl"] ?? "https://localhost:5001");
    });

    builder.Services.AddHttpClient<IContactService, ContactService>(client =>
    {
        client.BaseAddress = new Uri(builder.Configuration["BackendUrl"] ?? "https://localhost:5001");
    });

    await builder.Build().RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "An exception occurred while creating the WASM host");
    throw;
}
