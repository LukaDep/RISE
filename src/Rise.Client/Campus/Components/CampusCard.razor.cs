using Microsoft.AspNetCore.Components;


namespace Rise.Client.Campus.Components
{
    public partial class CampusCard : ComponentBase
    {
        [Inject] private NavigationManager Navigation { get; set; }

        [Parameter] public string Name { get; set; }
        [Parameter] public string Location { get; set; }
        [Parameter] public IEnumerable<string> Facilities { get; set; } = Enumerable.Empty<string>();
        [Parameter] public string ContactPhone { get; set; }
        [Parameter] public string Website { get; set; }
        [Parameter] public string Description { get; set; }
        [Parameter] public Guid Id { get; set; }

        [Parameter] public string SearchTerm { get; set; }
        private bool IsOpen { get; set; } = false;

        private void ToggleOpen()
        {
            IsOpen = !IsOpen;
        }

        private void GoToPlan()
        {
            var rel = Navigation.ToBaseRelativePath(Navigation.Uri);
            Navigation.NavigateTo($"/campus-plan/{Id}?returnUrl={rel}");
        }
    }
}



