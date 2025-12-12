namespace Rise.Shared.Widgets;

/// <summary>
/// Data transfer objects for widget types.
/// </summary>
public static class WidgetDto
{
    /// <summary>
    /// Represents a widget type definition.
    /// Contains the widget identifier and its unique key for component rendering.
    /// </summary>
    public class Index
    {
        public required Guid Id { get; set; }
        public required string Key { get; set; }
    }
}