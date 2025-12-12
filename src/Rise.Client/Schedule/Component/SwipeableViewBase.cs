using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Rise.Client.Schedule
{
    /// <summary>
    /// Abstract base class for components that support swipe gesture navigation.
    /// Provides common functionality for handling left/right swipe gestures via JavaScript interop.
    /// 
    /// Derived classes must:
    /// 1. Override the <see cref="ContainerId"/> property to specify the HTML element ID to attach swipe listeners to.
    /// 2. Implement <see cref="OnSwipeNext"/> to handle left swipe (navigate forward).
    /// 3. Implement <see cref="OnSwipePrevious"/> to handle right swipe (navigate backward).
    /// 
    /// The base class handles:
    /// - JavaScript interop setup for touch/mouse swipe detection
    /// - CSS animation classes for visual swipe feedback
    /// - Proper disposal of .NET object references
    /// </summary>
    public abstract class SwipeableViewBase : ComponentBase, IAsyncDisposable
    {
        /// <summary>JavaScript runtime for interop calls.</summary>
        [Inject] public required IJSRuntime JSRuntime { get; set; }

        private DotNetObjectReference<SwipeableViewBase>? dotNetRef;
        
        /// <summary>
        /// CSS class applied during swipe animation. Empty when not animating.
        /// Values are "swipe-left" or "swipe-right" during animation.
        /// </summary>
        protected string SwipeClass { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the HTML element ID to attach swipe gesture handlers to.
        /// Must be overridden by derived classes.
        /// </summary>
        protected abstract string ContainerId { get; }

        /// <summary>
        /// Initializes swipe gesture handling after the first render.
        /// Creates a .NET object reference for JavaScript callbacks and invokes
        /// the initSwipe JavaScript function with the container element ID.
        /// </summary>
        /// <param name="firstRender">True if this is the component's first render.</param>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                dotNetRef = DotNetObjectReference.Create(this);
                await JSRuntime.InvokeVoidAsync("initSwipe", ContainerId, dotNetRef);
            }
        }

        /// <summary>
        /// Triggers a CSS swipe animation in the specified direction.
        /// Applies the animation class ("swipe-left" or "swipe-right"),
        /// waits for the CSS transition to complete (250ms), then removes
        /// the class. This provides visual feedback during navigation.
        /// </summary>
        /// <param name="direction">Animation direction: "left" for next, "right" for previous.</param>
        protected async Task AnimateSwipe(string direction)
        {
            SwipeClass = direction == "left" ? "swipe-left" : "swipe-right";
            StateHasChanged();

            await Task.Delay(250); // match your CSS transition time
            SwipeClass = string.Empty;
            StateHasChanged();
        }

        /// <summary>
        /// JavaScript-invokable method called when user swipes left.
        /// Delegates to the abstract <see cref="OnSwipeNext"/> method
        /// implemented by derived classes.
        /// </summary>
        [JSInvokable]
        public async Task SwipeNext()
        {
            await OnSwipeNext();
        }

        /// <summary>
        /// JavaScript-invokable method called when user swipes right.
        /// Delegates to the abstract <see cref="OnSwipePrevious"/> method
        /// implemented by derived classes.
        /// </summary>
        [JSInvokable]
        public async Task SwipePrevious()
        {
            await OnSwipePrevious();
        }

        /// <summary>
        /// Called when a left swipe gesture is detected (navigate forward).
        /// Must be implemented by derived classes to handle navigation to next item.
        /// </summary>
        protected abstract Task OnSwipeNext();
        
        /// <summary>
        /// Called when a right swipe gesture is detected (navigate backward).
        /// Must be implemented by derived classes to handle navigation to previous item.
        /// </summary>
        protected abstract Task OnSwipePrevious();

        /// <summary>
        /// Disposes the .NET object reference used for JavaScript callbacks.
        /// Called when the component is disposed to prevent memory leaks.
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            dotNetRef?.Dispose();
        }
    }
}
