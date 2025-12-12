namespace Rise.Shared.Menu
{
    /// <summary>
    /// Data transfer objects for daily menus.
    /// </summary>
    public static class MenuDto
    {
        /// <summary>
        /// Represents a menu for display and retrieval.
        /// Contains the menu date, associated restaurant, and list of menu items.
        /// </summary>
        public class Index
        {
            public required Guid Id { get; set; }
            public Guid RestoId { get; set; } = default!;
            public required DateTime Date { get; set; }
            public required List<MenuItemDto.Index> MenuItems { get; set; }
        }
    }
}