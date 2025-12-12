using Microsoft.AspNetCore.Components;

namespace Rise.Client.Schedule;

/// <summary>
/// Reusable "Go to Today" button component.
/// </summary>
public partial class TodayButton : ComponentBase
{
    /// <summary>Whether to show the button.</summary>
    [Parameter] public bool ShowButton { get; set; }
    
    /// <summary>Button text label.</summary>
    [Parameter] public string ButtonText { get; set; } = "Today";
    
    /// <summary>Callback when button is clicked.</summary>
    [Parameter] public EventCallback OnClick { get; set; }
}
