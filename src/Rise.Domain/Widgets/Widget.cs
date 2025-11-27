namespace Rise.Domain.HomeWidgets;

public class Widget : Entity
{

    private string _typeName = string.Empty;
    public required string TypeName
    {
        get => _typeName;
        set => _typeName = Guard.Against.NullOrWhiteSpace(value);
    }

    private List<UserWidget> _userWidgets = new();
    public List<UserWidget> UserWidgets
    {
        get => _userWidgets;
        set => _userWidgets = value ?? new List<UserWidget>();
    }

}