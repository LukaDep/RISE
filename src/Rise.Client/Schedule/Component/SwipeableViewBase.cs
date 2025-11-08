using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Rise.Client.Schedule
{
    public abstract class SwipeableViewBase : ComponentBase, IAsyncDisposable
    {
        [Inject] public required IJSRuntime JSRuntime { get; set; }

        private DotNetObjectReference<SwipeableViewBase>? dotNetRef;
        protected string SwipeClass { get; private set; } = string.Empty;

        protected abstract string ContainerId { get; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                dotNetRef = DotNetObjectReference.Create(this);
                await JSRuntime.InvokeVoidAsync("initSwipe", ContainerId, dotNetRef);
            }
        }

        protected async Task AnimateSwipe(string direction)
        {
            SwipeClass = direction == "left" ? "swipe-left" : "swipe-right";
            StateHasChanged();

            await Task.Delay(250); // match your CSS transition time
            SwipeClass = string.Empty;
            StateHasChanged();
        }

        [JSInvokable]
        public async Task SwipeNext()
        {
            await OnSwipeNext();
        }

        [JSInvokable]
        public async Task SwipePrevious()
        {
            await OnSwipePrevious();
        }

        protected abstract Task OnSwipeNext();
        protected abstract Task OnSwipePrevious();

        public async ValueTask DisposeAsync()
        {
            dotNetRef?.Dispose();
        }
    }
}
