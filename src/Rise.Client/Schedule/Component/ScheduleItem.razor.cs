using Microsoft.AspNetCore.Components;
using Rise.Shared.Campus;
using Rise.Shared.Schedule;

namespace Rise.Client.Schedule;

public partial class ScheduleItem : ComponentBase
{
    [Parameter] public ScheduleDto.Reservation? Reservation { get; set; }
    [Parameter] public EventCallback OnClose { get; set; }

    [Inject] public required ICampusService CampusClientService { get; set; }
    [Inject] public required NavigationManager Navigation { get; set; }

    private async Task CloseDetails()
    {
        await OnClose.InvokeAsync();
    }

    private async Task NavigateToRoom()
    {
        var buildingId = Reservation?.Room?.Substring(0, Reservation?.Room?.IndexOf('.') ?? 0) ?? "";
        var response = await CampusClientService.GetBuildingByIdAsync(buildingId);
        var campusId = response?.Value?.CampusId ?? "";
        Navigation.NavigateTo($"/campus-plan/{campusId}#building-{buildingId}");
    }

    private static string TruncateTitle(string title, int maxLength = 40)
    {
        if (string.IsNullOrEmpty(title))
            return title;

        if (title.Length <= maxLength)
            return title;

        return title.Substring(0, maxLength) + "...";
    }
}
