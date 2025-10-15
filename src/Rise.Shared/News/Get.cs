using System;

namespace Rise.Shared.News;

public static partial class NewsResponse
{
  public class Get
  {
    public required NewsDto.Index NewsItem { get; set; }
  }
}
