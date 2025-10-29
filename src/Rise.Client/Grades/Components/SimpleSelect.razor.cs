namespace Rise.Client.Grades.Components;

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
    private int HighlightedIndex { get; set; } = -1;

    protected override void OnParametersSet()
    {
        ItemsList = Items.ToList();
        if (HighlightedIndex < 0 && ItemsList.Count > 0)
            HighlightedIndex = ItemsList.FindIndex(i => i.Key == SelectedValue);
    }

    private string? GetLabel(string? key) => ItemsList.FirstOrDefault(kv => kv.Key == key).Value;

    private void Toggle()
    {
        IsOpen = !IsOpen;
        if (IsOpen)
        {
            HighlightedIndex = ItemsList.FindIndex(i => i.Key == SelectedValue);
            if (HighlightedIndex < 0) HighlightedIndex = 0;
        }
    }

    private async Task Select(string key)
    {
        SelectedValue = key;
        IsOpen = false;
        await SelectedValueChanged.InvokeAsync(SelectedValue);
    }

    private void Highlight(int index) => HighlightedIndex = index;
}