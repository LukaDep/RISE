namespace Rise.Shared.Menus;

public static partial class MenuResponse
{
    public class Index
    {
        public IEnumerable<MenuDto.Index> Menus { get; set; } = [];
    }
}
