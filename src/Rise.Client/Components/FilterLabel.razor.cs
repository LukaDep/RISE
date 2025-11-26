using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using System.Web;

namespace Rise.Client.Components
{
    public partial class FilterLabel : ComponentBase, IDisposable
    {
        [Parameter, EditorRequired] public string Filter { get; set; } = default!;

        [Inject] public NavigationManager NavigationManager { get; set; } = default!;

        private bool IsActive = false;

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

        protected override void OnInitialized()
        {
            // set initial active state from current URI
            UpdateIsActive(NavigationManager.Uri);

            // subscribe to location changes
            NavigationManager.LocationChanged += OnLocationChanged;
        }

        private void UpdateIsActive(string location)
        {
            var uri = NavigationManager.ToAbsoluteUri(location);
            var currentQueryParams = HttpUtility.ParseQueryString(uri.Query);
            IsActive = (currentQueryParams.Get("filter") == Filter);
        }

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