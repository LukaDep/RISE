using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.JSInterop;
using Rise.Shared.Campus;

namespace Rise.Client.Campus;

/// <summary>
/// Code-behind for the CampusPlan page component.
/// Displays an interactive campus floor plan with building information.
/// </summary>
public partial class CampusPlan : ComponentBase
{
    /// <summary>Navigation manager for URL handling.</summary>
    [Inject] public required NavigationManager Navigation { get; set; }
    
    /// <summary>JavaScript runtime for interop calls.</summary>
    [Inject] public required IJSRuntime JS { get; set; }
    
    /// <summary>The campus ID to display.</summary>
    [Parameter] public required Guid campusId { get; set; }
    
    /// <summary>Return URL after viewing the campus plan.</summary>
    [SupplyParameterFromQuery(Name = "returnUrl")] public string returnUrlQuery { get; set; } = "campus";
    
    /// <summary>Service for campus data operations.</summary>
    [Inject] public required ICampusService CampusClientService { get; set; }
    
    /// <summary>Optional building code to highlight.</summary>
    [Parameter] public string? buildingCode { get; set; }
    
    private CampusDto.Index? campus;
    private BuildingDto.Index? building;
    private string returnUrl = "/campus";

    /// <summary>
    /// Initializes the component by loading campus and building data.
    /// </summary>
    protected override async Task OnInitializedAsync()
    {
        if (!string.IsNullOrWhiteSpace(returnUrlQuery))
        {
            returnUrl = returnUrlQuery;
        }

        if (string.IsNullOrEmpty(campusId.ToString()) && string.IsNullOrEmpty(buildingCode?.ToString()))
        {
            return;
        }
        else if (!string.IsNullOrEmpty(buildingCode))
        {
            var buildingResponse = await CampusClientService.GetBuildingByBuildingCodeAsync(buildingCode);
            building = buildingResponse.Value.Building;
            if (building != null)
            {
                campusId = building.CampusId;
            }
            else
            {
                return;
            }
        }

        var response = await CampusClientService.GetCampusByIdAsync(campusId);
        campus = response?.Value.Campus;

    }

    /// <summary>
    /// Handles post-render logic to initialize the Leaflet map and display buildings.
    /// </summary>
    /// <param name="firstRender">True if this is the first render.</param>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        // building id from de url
        var uri = new Uri(Navigation.Uri);
        var fragment = uri.Fragment; // "#building-GSCHB"
        if (fragment.StartsWith("#building-"))
        {
            buildingCode = fragment.Substring("#building-".Length);
        }

        if (campus != null)
        {

            await JS.InvokeVoidAsync("leafletMap.initTileMap", "map", campus.Latitude, campus.Longitude, 17);
            //await JS.InvokeVoidAsync("leafletMap.addLatLngMarker", campus.Latitude, campus.Longitude, campus.Name);

            if (!string.IsNullOrEmpty(buildingCode))
            {
                building = campus.Buildings?.FirstOrDefault(b => buildingCode.Equals(b.BuildingCode));
                if (building != null)
                {
                    await JS.InvokeVoidAsync("leafletMap.addMarkerWithGoogleLink", building.Latitude, building.Longitude, building.Name,
                    true);
                    await JS.InvokeVoidAsync("leafletMap.setView", building.Latitude, building.Longitude, 18);
                }
            }
            else
            {
                await JS.InvokeVoidAsync("leafletMap.addMarkerWithGoogleLink", campus.Latitude, campus.Longitude, campus.Name, true);

            }
        }
    }
}