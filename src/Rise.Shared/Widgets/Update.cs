namespace Rise.Shared.Widgets;

/// <summary>
/// Request wrappers for widget-related operations.
/// </summary>
public static partial class WidgetRequest
{
    /// <summary>
    /// Request to update a user's widget configuration.
    /// Contains the list of widgets with updated positions and sizes.
    /// </summary>
    public class Update
    {
        public IEnumerable<UserWidgetDto.Update>? UserWidgets { get; set; }
    }
}


