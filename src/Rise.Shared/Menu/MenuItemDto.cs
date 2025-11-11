using Rise.Shared.Common;

namespace Rise.Shared.Menu;

public static class MenuItemDto
{
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
