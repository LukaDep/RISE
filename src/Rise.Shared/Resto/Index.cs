using System.Collections.Generic;

namespace Rise.Shared.Resto;

public static class RestoResponse
{
    public class Index
    {
        public IEnumerable<RestoDto.Index> Restos { get; set; } = new List<RestoDto.Index>();
    }
}
