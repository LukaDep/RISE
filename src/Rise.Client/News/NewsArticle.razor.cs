using Microsoft.AspNetCore.Components;
using Rise.Shared.Common;
using Rise.Shared.News;

namespace Rise.Client.News;

public partial class NewsArticle : ComponentBase
{
  [Inject] public required INewsService NewsService { get; set; }
  [Parameter] public int Id { get; set; }

  private NewsDto.Index? newsItem;
  private bool isLoading = true;
  private string? errorMessage;

  private string GetThumbnailUrl()
  {
    // Return a placeholder image URL - you can customize this
    // to use actual image URLs from your news items when available
    return "https://www.bureaupartners.be/images/projecten-detail/hogent-sporthal/hogent-sporthal-01.jpg";
  }

  protected override async Task OnInitializedAsync()
  {
    try
    {
      var result = await NewsService.GetByIdAsync(Id);

      if (result.IsSuccess && result.Value?.NewsItem != null)
      {
        newsItem = result.Value.NewsItem;
      }
      else
      {
        errorMessage = result.Errors.FirstOrDefault() ?? "News item not found";
      }
    }
    catch (Exception ex)
    {
      errorMessage = $"Error loading news: {ex.Message}";
    }
    finally
    {
      isLoading = false;
    }
  }
}
