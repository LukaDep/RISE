using Microsoft.AspNetCore.Components;
using Rise.Shared.Schedule;
using Rise.Shared.Common;
using Rise.Client.Schedule;
using Rise.Shared.Menu;
using Rise.Shared.Resto;
using Rise.Client.Menu;
using Rise.Shared.News;
using Rise.Shared.Grades;

namespace Rise.Client.Home;

public partial class Index : ComponentBase
{
    [Parameter]
    public string? mode { get; set; }
    [Inject] public IScheduleService ScheduleClientService { get; set; } = default!;
    [Inject] public IRestoService RestoClientService { get; set; } = default!;
    [Inject] public IMenuService MenuClientService { get; set; } = default!;
    [Inject] public INewsService NewsClientService { get; set; } = default!;
    [Inject] public IGradesService GradesClientService { get; set; } = default!;
    [Inject] public NavigationManager NavigationManager { get; set; } = default!;


    private List<ScheduleDto.Schedule>? UpcomingClasses { get; set; }
    private IEnumerable<RestoDto.Index>? Restos { get; set; }
    private IEnumerable<MenuDto.Index>? TodaysMenus { get; set; }
    private IEnumerable<NewsDto.Index>? News { get; set; }
    private GradesDto.Grade? LastGrade { get; set; }

    private bool IsEditMode => string.Equals(mode, "edit", StringComparison.OrdinalIgnoreCase);

    private void EnterEdit()
    {
        NavigationManager.NavigateTo("/home/edit");
    }

    private void SaveAndExit()
    {
        // placeholder for save logic
        NavigationManager.NavigateTo("/home");
    }

    private void AddItem()
    {
        Console.WriteLine("AddItem clicked");
        // placeholder for add item behaviour in edit mode
        // items.Add($"New Item {items.Count + 1}");
    }
    private Task RemoveItem(int index)
    {
        // if (index >= 0 && index < items.Count)
        //     items.RemoveAt(index);
        return Task.CompletedTask;
    }

    protected override async Task OnInitializedAsync()
    {
        var resultClasses = await ScheduleClientService.GetIndexAsync(new QueryRequest.SkipTake
        {
            Skip = 0,
            Take = 5,
            OrderBy = "StartDateTime"
        });
        UpcomingClasses = resultClasses
            .Value?
            .Schedules
            .Where(r => r.StartDateTime.Date == DateTime.Today.Date)
            .OrderBy(r => r.StartDateTime)
            .ToList();

        var resultRestos = await RestoClientService.GetIndexAsync(new QueryRequest.SkipTake
        {
            Skip = 0,
            Take = 10,
        });

        Restos = resultRestos.Value?.Restos;

        // get menus by date zou beter zijn
        var resultMenu = await MenuClientService.GetIndexAsync(new QueryRequest.SkipTake
        {
            Skip = 0,
            Take = 10,
        });

        TodaysMenus = resultMenu.Value?.Menus.Where(m => m.Date.Date == DateTime.Parse("2025-10-20").Date);
        var resultNews = await NewsClientService.GetIndexAsync(new QueryRequest.DateRange
        {
            Skip = 0,
            Take = 1,
            OrderBy = "PublishedAt desc"
        });
        News = resultNews.Value?.News;
        var resultGrades = await GradesClientService.GetIndexAsync(new QueryRequest.SkipTake
        {
            Skip = 0,
            Take = 1,
            OrderBy = "Date desc"
        });
        LastGrade = resultGrades.Value?.Grades.FirstOrDefault();

    }
}