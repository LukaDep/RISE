namespace Rise.Domain.Menu;

/// <summary>
/// Represents a daily menu for a restaurant.
/// Contains the date, associated restaurant, and list of menu items.
/// </summary>
public class Menu : Entity
{
    /// <summary>
    /// The unique identifier of the restaurant this menu belongs to.
    /// </summary>
    private Guid _restoId = Guid.Empty;
    public required Guid RestoId
    {
        get => _restoId;
        set => _restoId = Guard.Against.Default(value);
    }

    /// <summary>
    /// The date this menu is valid for. Stored in UTC.
    /// </summary>
    private DateTime _date;
    public required DateTime Date
    {
        get => _date;
        set
        {
            Guard.Against.Default(value, nameof(Date));
            _date = value.Kind == DateTimeKind.Utc ? value : value.ToUniversalTime();
        }
    }

    /// <summary>
    /// Collection of menu items available on this menu.
    /// </summary>
    private List<MenuItem> _menuItems;
    public required List<MenuItem> MenuItems
    {
        get => _menuItems;
        set => _menuItems = value;
    }
}
