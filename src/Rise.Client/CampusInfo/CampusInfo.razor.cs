using Microsoft.AspNetCore.Components;
using Rise.Shared.Common;
using Rise.Shared.CampusInfo;

namespace Rise.Client.CampusInfo;

public partial class CampusInfo
{
    private IEnumerable<CampusInfoDto.Index>? campusInfo;

    [Inject] public required ICampusInfoService CampusInfoService { get; set; }


    protected override async Task OnInitializedAsync()
    {
        var request = new QueryRequest.SkipTake
        {
            Skip = 0,
            Take = 50,
            OrderBy = "Id",
            //SearchTerm = SearchTerm
        };

        var result = await CampusInfoService.GetIndexAsync(request);
        campusInfo = result.Value.CampusInfo;
        
    }
}