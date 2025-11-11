using Microsoft.AspNetCore.Components;
using System.Web;

namespace Rise.Client.Components
{
    public partial class FilterLabel : ComponentBase
    {
        [Parameter, EditorRequired] public string Filter { get; set; }

        [Inject] public NavigationManager NavigationManager { get; set; } = default!;
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
            parameters.Add("filter", Filter == currentQueryParams.Get("filter") ? "" : Filter); //filtert op alles als je op dezelfde filter klikt
            var newUri = NavigationManager.GetUriWithQueryParameters(parameters);
            NavigationManager.NavigateTo(newUri);
        }
    }
}
