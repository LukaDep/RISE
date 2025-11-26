namespace Rise.Client.Components;

using Microsoft.AspNetCore.Components;


public partial class SimpleSelect : ComponentBase
{
    [Parameter]
    public IEnumerable<KeyValuePair<string, string>> Items { get; set; } =
        Enumerable.Empty<KeyValuePair<string, string>>();
    [Parameter] public string? SelectedValue { get; set; }
    [Parameter] public EventCallback<string?> SelectedValueChanged { get; set; }
    [Parameter] public string? Placeholder { get; set; }

    private List<KeyValuePair<string, string>> ItemsList = new();
    private bool IsOpen { get; set; }

    protected override void OnParametersSet()
    {
        ItemsList = (Items ?? Enumerable.Empty<KeyValuePair<string, string>>()).ToList();
    }

    private void Toggle()
    {
        IsOpen = !IsOpen;
        Console.WriteLine(IsOpen);
    }

    private async Task Select(string? value)
    {
        SelectedValue = value;
        IsOpen = false;
        await SelectedValueChanged.InvokeAsync(SelectedValue);
    }

    private string? GetLabel(string? value)
    {
        var item = ItemsList.FirstOrDefault(x => x.Key == value);
        if (!string.IsNullOrEmpty(item.Value))
            return item.Value;
        return Placeholder;
    }

    private async Task OnChange(ChangeEventArgs e)
    {
        var value = e.Value?.ToString();
        SelectedValue = value;
        await SelectedValueChanged.InvokeAsync(SelectedValue);
    }
}