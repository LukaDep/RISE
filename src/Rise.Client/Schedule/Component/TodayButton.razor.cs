using Microsoft.AspNetCore.Components;

namespace Rise.Client.Schedule;

/// <summary>
/// Reusable "Go to Today" button component
/// </summary>
public partial class TodayButton : ComponentBase
{
    [Parameter] public bool ShowButton { get; set; }
    [Parameter] public string ButtonText { get; set; } = "Today";
    [Parameter] public EventCallback OnClick { get; set; }
}
