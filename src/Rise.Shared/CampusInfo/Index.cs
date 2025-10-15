namespace Rise.Shared.CampusInfo;



public static partial class CampusInfoResponse
{
    public class Index
    {
        public IEnumerable<CampusInfoDto.Index> CampusInfo { get; set; } = [];
    }
}


