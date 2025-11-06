namespace Rise.Shared.Campus;

public static partial class CampusResponse
{
    public class Get
    {
        public required CampusDto.Index Campus { get; set; }
    }
}