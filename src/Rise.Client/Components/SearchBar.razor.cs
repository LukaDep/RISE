using Microsoft.AspNetCore.Components;

namespace Rise.Client.Components;

public enum SearchBarVariant
{
    /// <summary>
    /// Input slides out inline next to the search button
    /// </summary>
    Inline,

    /// <summary>
    /// Input appears below the search button
    /// </summary>
    Dropdown
}

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
    /// Whether the search bar is currently open/expanded.
    /// </summary>
    [Parameter]
    public bool IsOpen { get; set; }

    /// <summary>
    /// Callback invoked when the open state changes.
    /// </summary>
    [Parameter]
    public EventCallback<bool> IsOpenChanged { get; set; }

    /// <summary>
    /// The variant of the search bar: Inline (default) or Dropdown.
    /// </summary>
    [Parameter]
    public SearchBarVariant Variant { get; set; } = SearchBarVariant.Inline;

    private async Task ToggleSearch()
    {
        IsOpen = !IsOpen;
        await IsOpenChanged.InvokeAsync(IsOpen);

        if (IsOpen)
        {
            await Task.Delay(50); // Small delay to ensure the input is visible
            await inputElement.FocusAsync();
        }
    }

    private async Task OnInputChanged(ChangeEventArgs e)
    {
        var newValue = e.Value?.ToString() ?? string.Empty;
        await ValueChanged.InvokeAsync(newValue);
    }
}
