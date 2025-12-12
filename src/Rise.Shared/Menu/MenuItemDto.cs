using Rise.Shared.Common;

namespace Rise.Shared.Menu;

/// <summary>
/// Data transfer objects for menu items.
/// </summary>
public static class MenuItemDto
{
    /// <summary>
    /// Represents a menu item for display and retrieval.
    /// Contains item details including name, description, pricing, type, and dietary information.
    /// </summary>
    public class Index
    {
        public required Guid Id { get; set; }
        public required Guid MenuId { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public double PriceStudent { get; set; }
        public double PriceExtern { get; set; }
        public required FoodType Type { get; set; }
        public bool IsVegan { get; set; }
        public bool IsVeggie { get; set; }
    }
}
