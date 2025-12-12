using System.Collections.Generic;

namespace Rise.Shared.Resto;

/// <summary>
/// Response wrappers for restaurant-related operations.
/// </summary>
public static class RestoResponse
{
    /// <summary>
    /// Response containing a list of restaurants.
    /// Used for retrieving and displaying available restaurants.
    /// </summary>
    public class Index
    {
        public IEnumerable<RestoDto.Index> Restos { get; set; } = new List<RestoDto.Index>();
    }
}
