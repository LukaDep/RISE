using Rise.Shared.Common;

namespace Rise.Domain.Menu;

/// <summary>
/// Represents an individual item on a menu.
/// Contains item details including name, description, pricing, food type, and dietary information.
/// </summary>
public class MenuItem : Entity
{
    /// <summary>
    /// The name of the menu item.
    /// </summary>
    private string _name = string.Empty;
    public required string Name
    {
        get => _name;
        set => _name = Guard.Against.NullOrWhiteSpace(value);
    }

    /// <summary>
    /// A description of the menu item.
    /// </summary>
    private string _description = string.Empty;
    public required string Description
    {
        get => _description;
        set => _description = Guard.Against.NullOrWhiteSpace(value);
    }

    /// <summary>
    /// The price for students.
    /// </summary>
    private double _priceStudent;
    public double PriceStudent
    {
        get => _priceStudent;
        set => _priceStudent = value;
    }

    /// <summary>
    /// The price for external visitors (non-students).
    /// </summary>
    private double _priceExtern;
    public double PriceExtern
    {
        get => _priceExtern;
        set => _priceExtern = value;
    }

    /// <summary>
    /// The food type category of the menu item.
    /// </summary>
    private FoodType _type;
    public required FoodType Type
    {
        get => _type;
        set => _type = value;
    }

    /// <summary>
    /// Indicates whether the menu item is vegan.
    /// </summary>
    private bool _isVegan;
    public bool IsVegan
    {
        get => _isVegan;
        set => _isVegan = value;
    }

    /// <summary>
    /// Indicates whether the menu item is vegetarian.
    /// </summary>
    private bool _isVeggie;
    public bool IsVeggie
    {
        get => _isVeggie;
        set => _isVeggie = value;
    }
}


