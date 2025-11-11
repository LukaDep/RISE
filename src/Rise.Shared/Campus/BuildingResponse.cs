namespace Rise.Shared.Campus;

public static partial class BuildingResponse
{
    public class Get
    {
        public required BuildingDto.Index Building { get; set; }
    }
}