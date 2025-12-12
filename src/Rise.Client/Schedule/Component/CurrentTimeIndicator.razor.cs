using Microsoft.AspNetCore.Components;

namespace Rise.Client.Schedule;

/// <summary>
/// Component that displays a red line indicating the current time on a schedule view.
/// </summary>
public partial class CurrentTimeIndicator : ComponentBase
{
    /// <summary>Vertical position of the indicator in pixels.</summary>
    [Parameter] public double Position { get; set; }
    
    /// <summary>CSS left position value.</summary>
    [Parameter] public string LeftPosition { get; set; } = "64px";
    
    /// <summary>CSS width value.</summary>
    [Parameter] public string Width { get; set; } = "calc(100% - 64px)";
    
    /// <summary>Current time to display.</summary>
    [Parameter] public DateTime CurrentTime { get; set; }
    
    /// <summary>CSS class for time label positioning.</summary>
    [Parameter] public string TimeLabelClass { get; set; } = "-left-16 -top-2.5";
}
