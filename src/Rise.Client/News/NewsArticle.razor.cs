using Microsoft.AspNetCore.Components;
using Rise.Shared.News;

namespace Rise.Client.News;

public partial class NewsArticle : ComponentBase
{
    [Inject] public required INewsService NewsService { get; set; }
    [Parameter] public Guid Id { get; set; }
    private NewsDto.Index? newsArticle;
    private string? errorMessage;

    private string GetThumbnailUrl()
    {
        // Return a placeholder image URL for now
        return "https://www.bureaupartners.be/images/projecten-detail/hogent-sporthal/hogent-sporthal-01.jpg";
    }

    protected override async Task OnInitializedAsync()
    {
        var result = await NewsService.GetByIdAsync(Id.ToString());

        if (result.IsSuccess)
        {
            newsArticle = result.Value.NewsArticle;
        }
        else
        {
            errorMessage = result.Errors.FirstOrDefault();
        }
    }
}
