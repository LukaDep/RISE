namespace Rise.Shared.Menu
{
    public static class MenuDto
    {
        public class Index
        {
            public required Guid Id { get; set; }
            public Guid RestoId { get; set; } = default!;
            public required DateTime Date { get; set; }
            public required List<MenuItemDto.Index> MenuItems { get; set; }
        }
    }


}