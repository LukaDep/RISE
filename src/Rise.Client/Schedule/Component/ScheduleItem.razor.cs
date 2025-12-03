using Microsoft.AspNetCore.Components;
using Rise.Shared.Campus;
using Rise.Shared.Schedule;

namespace Rise.Client.Schedule;

public partial class ScheduleItem : ComponentBase
{
    [Parameter] public ScheduleDto.Schedule? Schedule { get; set; }
    [Parameter] public EventCallback OnClose { get; set; }

    [Inject] public required ICampusService CampusClientService { get; set; }
    [Inject] public required NavigationManager Navigation { get; set; }

    private async Task CloseDetails()
    {
        await OnClose.InvokeAsync();
    }

    private async Task NavigateToRoom()
    {
        var buildingId = Schedule?.Room?.Substring(0, Schedule?.Room?.IndexOf('.') ?? 0) ?? "";
        var response = await CampusClientService.GetBuildingByBuildingCodeAsync(buildingId);
        var campusId = response?.Value?.Building.CampusId;
        var rel = Navigation.ToBaseRelativePath(Navigation.Uri);
        Navigation.NavigateTo($"/campus-plan/{campusId}?returnUrl={rel}#building-{buildingId}");
    }

    private static string TruncateTitle(string title, int maxLength = 40) =>
        ScheduleHelpers.TruncateTitle(title, maxLength);
}
