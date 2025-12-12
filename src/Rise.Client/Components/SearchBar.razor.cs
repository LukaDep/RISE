using Microsoft.AspNetCore.Components;

namespace Rise.Client.Components;

/// <summary>
/// Search bar component with toggle functionality.
/// Provides an expandable search input with clear functionality.
/// </summary>
public partial class SearchBar : ComponentBase
{
    private ElementReference inputElement;

    /// <summary>
    /// The current value of the search input.
    /// </summary>
    [Parameter]
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// Callback invoked when the search value changes.
    /// </summary>
    [Parameter]
    public EventCallback<string> ValueChanged { get; set; }

    /// <summary>
    /// Whether the search bar is open.
    /// </summary>
    [Parameter]
    public bool IsOpen { get; set; } = true;

    /// <summary>
    /// Callback invoked when the open state changes.
    /// </summary>
    [Parameter]
    public EventCallback<bool> IsOpenChanged { get; set; }

    /// <summary>
    /// Toggles the search bar state. Focuses input if empty, clears if has value.
    /// </summary>
    private async Task ToggleSearch()
    {
        if (string.IsNullOrWhiteSpace(Value))
        {
            await inputElement.FocusAsync();
        }
        else
        {
            await ValueChanged.InvokeAsync(string.Empty);
        }
    }

    /// <summary>
    /// Handles input change events and notifies parent of new value.
    /// </summary>
    /// <param name="e">The change event arguments.</param>
    private async Task OnInputChanged(ChangeEventArgs e)
    {
        var newValue = e.Value?.ToString() ?? string.Empty;
        await ValueChanged.InvokeAsync(newValue);
    }
}
