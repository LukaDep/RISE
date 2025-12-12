namespace Rise.Domain.HomeWidgets;

/// <summary>
/// Represents a widget type definition.
/// Contains the widget type name and associated user widget instances.
/// </summary>
public class Widget : Entity
{
    /// <summary>
    /// The unique type name of the widget (e.g., "schedule", "grades", "news").
    /// </summary>
    private string _typeName = string.Empty;
    public required string TypeName
    {
        get => _typeName;
        set => _typeName = Guard.Against.NullOrWhiteSpace(value);
    }

    /// <summary>
    /// Collection of user widget instances of this type.
    /// </summary>
    private List<UserWidget> _userWidgets = new();
    public List<UserWidget> UserWidgets
    {
        get => _userWidgets;
        set => _userWidgets = value ?? new List<UserWidget>();
    }

}