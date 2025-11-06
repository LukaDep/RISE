namespace Rise.Domain.Menu;

public class Menu : Entity
{
    private Guid _restoId = Guid.Empty;
    public required Guid RestoId
    {
        get => _restoId;
        set => _restoId = Guard.Against.Default(value);
    }

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

    private List<MenuItem> _menuItems = new();
    public required List<MenuItem> MenuItems
    {
        get => _menuItems;
        set => _menuItems = value;
    }
}
