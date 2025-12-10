using Microsoft.AspNetCore.Components;

namespace Rise.Client.Components;

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

    private async Task OnInputChanged(ChangeEventArgs e)
    {
        var newValue = e.Value?.ToString() ?? string.Empty;
        await ValueChanged.InvokeAsync(newValue);
    }
}
