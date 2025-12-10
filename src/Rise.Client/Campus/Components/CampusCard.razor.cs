using Microsoft.AspNetCore.Components;
using Rise.Shared.Campus;

namespace Rise.Client.Campus.Components
{
    public partial class CampusCard : ComponentBase
    {
        [Inject] private NavigationManager Navigation { get; set; }

        [Parameter] public required CampusDto.Index Campus { get; set; }

        private bool isDescriptionExpanded = false;
        private string location => $"{Campus.Street} {Campus.HouseNumber}, {Campus.PostalCode} {Campus.City}";

        private void ToggleDescription()
        {
            isDescriptionExpanded = !isDescriptionExpanded;
        }

        private void GoToPlan()
        {
            var rel = Navigation.ToBaseRelativePath(Navigation.Uri);
            Navigation.NavigateTo($"/campus-plan/{Campus.Id}?returnUrl={rel}");
        }
    }
}


