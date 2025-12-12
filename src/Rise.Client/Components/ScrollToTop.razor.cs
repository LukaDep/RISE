namespace Rise.Client.Components;

using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;

/// <summary>
/// Scroll-to-top button component.
/// Shows a floating button that scrolls the page to the top when clicked.
/// </summary>
public partial class ScrollToTop : ComponentBase, IAsyncDisposable
{
    private bool _initialized;
    
    /// <summary>
    /// Initializes the scroll-to-top button via JavaScript interop.
    /// </summary>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && !_initialized)
        {
            _initialized = true;
            try
            {
                // Call the global wrapper defined in wwwroot/scrollTop.js
                await JS.InvokeVoidAsync("initScrollTop", "scrollToTopBtn");
            }
            catch
            {
                // swallow JS errors — avoids breaking rendering if script not present
            }
        }
    }

    /// <summary>
    /// Disposes the scroll-to-top button JavaScript resources.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        if (_initialized)
        {
            try
            {
                await JS.InvokeVoidAsync("disposeScrollTop", "scrollToTopBtn");
            }
            catch
            {
                // ignore disposal errors
            }
            _initialized = false;
        }
    }
}