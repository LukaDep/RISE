namespace Rise.Client.Components;

using Microsoft.AspNetCore.Components;

/// <summary>
/// Simple dropdown select component with key-value pairs.
/// Provides a mobile-friendly select input with custom styling.
/// </summary>
public partial class SimpleSelect : ComponentBase
{
    /// <summary>The items to display in the dropdown.</summary>
    [Parameter]
    public IEnumerable<KeyValuePair<string, string>> Items { get; set; } =
        Enumerable.Empty<KeyValuePair<string, string>>();
    
    /// <summary>The currently selected value.</summary>
    [Parameter] public string? SelectedValue { get; set; }
    
    /// <summary>Callback when selection changes.</summary>
    [Parameter] public EventCallback<string?> SelectedValueChanged { get; set; }
    
    /// <summary>Placeholder text when no selection.</summary>
    [Parameter] public string? Placeholder { get; set; }

    private List<KeyValuePair<string, string>> ItemsList = new();
    private bool IsOpen { get; set; }

    /// <summary>
    /// Updates the items list when parameters change.
    /// </summary>
    protected override void OnParametersSet()
    {
        ItemsList = (Items ?? Enumerable.Empty<KeyValuePair<string, string>>()).ToList();
    }

    /// <summary>
    /// Toggles the dropdown open/closed state.
    /// </summary>
    private void Toggle()
    {
        IsOpen = !IsOpen;
        Console.WriteLine(IsOpen);
    }

    /// <summary>
    /// Selects a value and closes the dropdown.
    /// </summary>
    /// <param name="value">The value to select.</param>
    private async Task Select(string? value)
    {
        SelectedValue = value;
        IsOpen = false;
        await SelectedValueChanged.InvokeAsync(SelectedValue);
    }

    /// <summary>
    /// Gets the display label for a selected value.
    /// </summary>
    /// <param name="value">The value to look up.</param>
    /// <returns>The label text or placeholder if not found.</returns>
    private string? GetLabel(string? value)
    {
        var item = ItemsList.FirstOrDefault(x => x.Key == value);
        if (!string.IsNullOrEmpty(item.Value))
            return item.Value;
        return Placeholder;
    }

    /// <summary>
    /// Handles native select change events.
    /// </summary>
    /// <param name="e">The change event arguments.</param>
    private async Task OnChange(ChangeEventArgs e)
    {
        var value = e.Value?.ToString();
        SelectedValue = value;
        await SelectedValueChanged.InvokeAsync(SelectedValue);
    }
}