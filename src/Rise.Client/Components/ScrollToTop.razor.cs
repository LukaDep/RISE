namespace Rise.Client.Components;

using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;

public partial class ScrollToTop : ComponentBase, IAsyncDisposable
{
    private bool _initialized;
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