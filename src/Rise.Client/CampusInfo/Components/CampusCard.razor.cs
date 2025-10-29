using Microsoft.AspNetCore.Components;


namespace Rise.Client.CampusInfo.Components
{
    public partial class CampusCard : ComponentBase
    {
        [Inject] private NavigationManager Navigation { get; set; }

        [Parameter] public string Name { get; set; }
        [Parameter] public string Location { get; set; }
        [Parameter] public List<string> Faculties { get; set; } = new();
        [Parameter] public string ContactPhone { get; set; }
        [Parameter] public string Website { get; set; }
        [Parameter] public string Description { get; set; }
        [Parameter] public string Id { get; set; }

        [Parameter] public string SearchTerm { get; set; }
        private bool IsOpen { get; set; } = false;
        private bool isVisible = true;

        private void ToggleOpen()
        {
            IsOpen = !IsOpen;
        }

        private void CheckIfVisible()
        {
            if (Name.ToLower().Contains(SearchTerm.ToLower()))
            {
                isVisible = true;
            }
            else
            {
                isVisible = false;
            }
        }

        private void GoToPlan()
        {
            // Implement navigation to the campus plan
            Navigation.NavigateTo($"/campus-plan/{Id}");
        }


        protected override void OnInitialized()
        {
            Navigation.LocationChanged += OnLocationChanged;
        }

        private void OnLocationChanged(object? sender, Microsoft.AspNetCore.Components.Routing.LocationChangedEventArgs e)
        {
            CheckIfVisible();
            StateHasChanged();

        }
    }
}



