using Microsoft.AspNetCore.Components;

namespace Rise.Client.Schedule;

/// <summary>
/// Component that displays a red line indicating the current time on a schedule view
/// </summary>
public partial class CurrentTimeIndicator : ComponentBase
{
    [Parameter] public double Position { get; set; }
    [Parameter] public string LeftPosition { get; set; } = "64px";
    [Parameter] public string Width { get; set; } = "calc(100% - 64px)";
    [Parameter] public DateTime CurrentTime { get; set; }
    [Parameter] public string TimeLabelClass { get; set; } = "-left-16 -top-2.5";
}
