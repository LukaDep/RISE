using Rise.Shared.Common;

namespace Rise.Domain.Menu;

public class MenuItem : Entity
{
    private string _name = string.Empty;
    public required string Name
    {
        get => _name;
        set => _name = Guard.Against.NullOrWhiteSpace(value);
    }

    private string _description = string.Empty;
    public required string Description
    {
        get => _description;
        set => _description = Guard.Against.NullOrWhiteSpace(value);
    }

    private double _priceStudent;
    public double PriceStudent
    {
        get => _priceStudent;
        set => _priceStudent = value;
    }

    private double _priceExtern;
    public double PriceExtern
    {
        get => _priceExtern;
        set => _priceExtern = value;
    }

    private FoodType _type;
    public required FoodType Type
    {
        get => _type;
        set => _type = value;
    }

    private bool _isVegan;
    public bool IsVegan
    {
        get => _isVegan;
        set => _isVegan = value;
    }

    private bool _isVeggie;
    public bool IsVeggie
    {
        get => _isVeggie;
        set => _isVeggie = value;
    }
}


