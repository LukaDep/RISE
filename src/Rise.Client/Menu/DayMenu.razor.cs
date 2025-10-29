using Microsoft.AspNetCore.Components;
using Rise.Shared.Menu;
using Rise.Client.Menu;

public class DayMenuBase : ComponentBase
{
    [Parameter] public DateTime Date { get; set; }
    [Parameter] public List<MenuItemDto.Index> MenuItems { get; set; } = new();
}
