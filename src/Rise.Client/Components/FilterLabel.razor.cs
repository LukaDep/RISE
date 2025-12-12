using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using System.Web;

namespace Rise.Client.Components
{
    /// <summary>
    /// Filter label component that toggles URL query parameters.
    /// Provides a clickable filter chip that updates the URL filter state.
    /// </summary>
    public partial class FilterLabel : ComponentBase, IDisposable
    {
        /// <summary>The filter value this label represents.</summary>
        [Parameter, EditorRequired] public string Filter { get; set; } = default!;

        /// <summary>Navigation manager for URL handling.</summary>
        [Inject] public NavigationManager NavigationManager { get; set; } = default!;

        private bool IsActive = false;

        /// <summary>
        /// Toggles the filter in the URL query parameters.
        /// </summary>
        private void FilterProducts()
        {
            Dictionary<string, object?> parameters = new();

            var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
            var currentQueryParams = HttpUtility.ParseQueryString(uri.Query);
            foreach (string key in currentQueryParams)
            {
                if (key != "filter")
                {
                    parameters.Add(key, currentQueryParams[key]);
                }
            }
            parameters.Add("filter", Filter == currentQueryParams.Get("filter") ? "" : Filter); // toggle
            var newUri = NavigationManager.GetUriWithQueryParameters(parameters);
            NavigationManager.NavigateTo(newUri);
        }

        /// <summary>
        /// Initializes the component and subscribes to location changes.
        /// </summary>
        protected override void OnInitialized()
        {
            // set initial active state from current URI
            UpdateIsActive(NavigationManager.Uri);

            // subscribe to location changes
            NavigationManager.LocationChanged += OnLocationChanged;
        }

        /// <summary>
        /// Updates the active state based on the current URL filter parameter.
        /// </summary>
        /// <param name="location">The current location URI.</param>
        private void UpdateIsActive(string location)
        {
            var uri = NavigationManager.ToAbsoluteUri(location);
            var currentQueryParams = HttpUtility.ParseQueryString(uri.Query);
            IsActive = (currentQueryParams.Get("filter") == Filter);
        }

        /// <summary>
        /// Handles location changes to update the active state.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The location changed event arguments.</param>
        private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
        {
            // use e.Location (the new location) to compute the state
            UpdateIsActive(e.Location);

            // ensure UI updates on the renderer thread
            InvokeAsync(StateHasChanged);
        }

        public void Dispose()
        {
            NavigationManager.LocationChanged -= OnLocationChanged;
        }
    }
}