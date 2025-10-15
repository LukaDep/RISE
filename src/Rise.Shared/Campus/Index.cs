namespace Rise.Shared.Campus;

// <summary>
// Represents the response structure for campus-related operations.
// </summary>
public static partial class CampusResponse
{
    public class Index
    {
        public IEnumerable<CampusDto.Index> Campuses { get; set; } = [];
    }
    public class GetById<T>
    {
        public T Value { get; set; } = default!;
    }

}
