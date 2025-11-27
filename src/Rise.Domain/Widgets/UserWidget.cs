using Microsoft.AspNetCore.Identity;

namespace Rise.Domain.HomeWidgets;

public class UserWidget : Entity
{
    public Guid WidgetId { get; set; }
    public string? UserId { get; set; }

    // Navigation properties
    public Widget? Widget { get; set; }

    private int _x;
    public required int X
    {
        get => _x;
        set => _x = Guard.Against.Negative(value, nameof(X));
    }
    private int _y;
    public required int Y
    {
        get => _y;
        set => _y = Guard.Against.Negative(value, nameof(Y));
    }

    private int _width;
    public required int Width
    {
        get => _width;
        set => _width = Guard.Against.NegativeOrZero(value, nameof(Width));
    }

    private int _height;
    public required int Height
    {
        get => _height;
        set => _height = Guard.Against.NegativeOrZero(value, nameof(Height));
    }

    private int _minWidth;
    public required int MinWidth
    {
        get => _minWidth;
        set => _minWidth = Guard.Against.NegativeOrZero(value, nameof(MinWidth));
    }


}