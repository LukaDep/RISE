namespace Rise.Shared.Widgets;

/// <summary>
/// Response wrappers for widget-related operations.
/// </summary>
public static partial class WidgetResponse
{
    /// <summary>
    /// Response containing a user's widget configuration.
    /// Used to retrieve all widgets for the current user.
    /// </summary>
    public class Index
    {
        public IEnumerable<UserWidgetDto.Index> UserWidgets { get; set; } = [];
    }
}