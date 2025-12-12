using Microsoft.AspNetCore.Components;
using Rise.Shared.Campus;
using Rise.Shared.Schedule;

namespace Rise.Client.Schedule;

/// <summary>
/// Component that displays schedule item details in a modal.
/// Shows course info, time, location, and instructor.
/// </summary>
public partial class ScheduleItem : ComponentBase
{
    /// <summary>The schedule item to display.</summary>
    [Parameter] public ScheduleDto.Schedule? Schedule { get; set; }
    
    /// <summary>Callback when details panel is closed.</summary>
    [Parameter] public EventCallback OnClose { get; set; }

    /// <summary>Service for campus data.</summary>
    [Inject] public required ICampusService CampusClientService { get; set; }
    
    /// <summary>Navigation manager for routing.</summary>
    [Inject] public required NavigationManager Navigation { get; set; }

    /// <summary>
    /// Closes the details panel.
    /// </summary>
    private async Task CloseDetails()
    {
        await OnClose.InvokeAsync();
    }

    /// <summary>
    /// Navigates to the campus plan showing the room's building location.
    /// </summary>
    private async Task NavigateToRoom()
    {
        var buildingId = Schedule?.Room?.Substring(0, Schedule?.Room?.IndexOf('.') ?? 0) ?? "";
        var response = await CampusClientService.GetBuildingByBuildingCodeAsync(buildingId);
        var campusId = response?.Value?.Building.CampusId;
        var rel = Navigation.ToBaseRelativePath(Navigation.Uri);
        Navigation.NavigateTo($"/campus-plan/{campusId}?returnUrl={rel}#building-{buildingId}");
    }

    /// <summary>
    /// Truncates a title to a maximum length with ellipsis.
    /// </summary>
    /// <param name="title">The title to truncate.</param>
    /// <param name="maxLength">Maximum length (default 40).</param>
    /// <returns>Truncated title string.</returns>
    private static string TruncateTitle(string title, int maxLength = 40) =>
        ScheduleHelpers.TruncateTitle(title, maxLength);
}
