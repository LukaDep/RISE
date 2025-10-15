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
    return "https://images.unsplash.com/photo-1504711434969-e33886168f5c?w=1200&h=400&fit=crop";
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
