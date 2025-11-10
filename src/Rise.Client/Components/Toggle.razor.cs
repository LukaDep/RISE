using Microsoft.AspNetCore.Components;

namespace Rise.Client.Components;

public partial class Toggle
{
    private static int _idCounter = 0;

    /// <summary>
    /// Gets or sets the unique identifier for the toggle.
    /// </summary>
    [Parameter]
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets the current value of the toggle (true = on, false = false).
    /// </summary>
    [Parameter]
    public bool Value { get; set; }

    /// <summary>
    /// Gets or sets the callback that is invoked when the toggle value changes.
    /// </summary>
    [Parameter]
    public EventCallback<bool> ValueChanged { get; set; }

    /// <summary>
    /// Gets or sets whether the toggle is disabled.
    /// </summary>
    [Parameter]
    public bool Disabled { get; set; }

    /// <summary>
    /// Gets or sets an optional callback that is invoked when the toggle is clicked.
    /// </summary>
    [Parameter]
    public EventCallback<bool> OnChange { get; set; }

    protected override void OnInitialized()
    {
        // Generate a unique ID if none was provided
        Id ??= $"toggle-{++_idCounter}";
    }

    private async Task HandleToggleChange(ChangeEventArgs e)
    {
        if (Disabled)
            return;

        Value = e.Value is true;

        await ValueChanged.InvokeAsync(Value);
        await OnChange.InvokeAsync(Value);
    }
}
