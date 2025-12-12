namespace Rise.Shared.Menu;

/// <summary>
/// Response wrappers for menu-related operations.
/// </summary>
public static partial class MenuResponse
{
    /// <summary>
    /// Response containing a list of menus.
    /// Used for retrieving menus with their items for a specific period.
    /// </summary>
    public class Index
    {
        public IEnumerable<MenuDto.Index> Menus { get; set; } = [];
    }
}
