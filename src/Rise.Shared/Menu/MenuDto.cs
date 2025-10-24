namespace Rise.Shared.Menus
{
    public static class MenuDto
    {
        public class Index
        {
            public required string Id { get; set; }
            public required DateTime Date { get; set; }
            public required List<MenuItemDto.Index> MenuItems { get; set; }
        }
    }

    public static class MenuItemDto
    {
        public class Index
        {
            public required string Id { get; set; }
            public required string MenuId { get; set; }
            public required string Name { get; set; }
            public required string Description { get; set; }
            public double PriceStudent { get; set; }
            public double PriceExtern { get; set; }
            public required FoodType Type { get; set; }
            public bool IsVegan { get; set; }
            public bool IsVeggie { get; set; }
        }
    }

   public enum FoodType{
    WarmeMaaltijd,
    KoudeMaaltijd,
    Belegdbroodje,
    Koffiebar,
    Soep,
    Groente,
    Saus,
    Zetmeel,
    Dessert,
    Drank,
    Snack,
    Pasta
    }
}   