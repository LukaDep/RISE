using System.Collections.Generic;

namespace Rise.Shared.Resto;

public static class RestoResponse
{
    public class Index
    {
        public List<RestoDto.Index> Restos { get; set; } = new();
    }
}
