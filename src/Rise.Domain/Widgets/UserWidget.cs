using Microsoft.AspNetCore.Identity;

namespace Rise.Domain.HomeWidgets;

/// <summary>
/// Represents a user's widget instance with position and size configuration.
/// Links a user to a widget type with custom layout properties.
/// </summary>
public class UserWidget : Entity
{
    /// <summary>
    /// The unique identifier of the widget type.
    /// </summary>
    public Guid WidgetId { get; set; }

    /// <summary>
    /// The unique identifier of the user who owns this widget instance.
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// Navigation property to the widget type definition.
    /// </summary>
    public Widget? Widget { get; set; }

    /// <summary>
    /// The horizontal position of the widget on the grid.
    /// </summary>
    private int _x;
    public required int X
    {
        get => _x;
        set => _x = Guard.Against.Negative(value, nameof(X));
    }
    /// <summary>
    /// The vertical position of the widget on the grid.
    /// </summary>
    private int _y;
    public required int Y
    {
        get => _y;
        set => _y = Guard.Against.Negative(value, nameof(Y));
    }

    /// <summary>
    /// The width of the widget in grid units.
    /// </summary>
    private int _width;
    public required int Width
    {
        get => _width;
        set => _width = Guard.Against.NegativeOrZero(value, nameof(Width));
    }

    /// <summary>
    /// The height of the widget in grid units.
    /// </summary>
    private int _height;
    public required int Height
    {
        get => _height;
        set => _height = Guard.Against.NegativeOrZero(value, nameof(Height));
    }

    /// <summary>
    /// The minimum width allowed for this widget.
    /// </summary>
    private int _minWidth;
    public required int MinWidth
    {
        get => _minWidth;
        set => _minWidth = Guard.Against.NegativeOrZero(value, nameof(MinWidth));
    }


}