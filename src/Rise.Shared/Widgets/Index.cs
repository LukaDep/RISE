namespace Rise.Shared.Widgets;

public static partial class WidgetResponse
{
    public class Index
    {
        public IEnumerable<UserWidgetDto.Index> UserWidgets { get; set; } = [];
    }

}