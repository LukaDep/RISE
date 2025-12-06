using Markdig;
using Microsoft.AspNetCore.Components;
using Rise.Shared.News;

namespace Rise.Client.News;

public partial class NewsArticle : ComponentBase
{
    [Inject] public required INewsService NewsService { get; set; }
    [Parameter] public Guid Id { get; set; }
    private NewsDto.Index? newsArticle;
    private string? errorMessage;

    protected override async Task OnInitializedAsync()
    {
        var result = await NewsService.GetByIdAsync(Id);

        if (result.IsSuccess)
        {
            newsArticle = result.Value.NewsArticle;
        }
        else
        {
            errorMessage = result.Errors.FirstOrDefault();
        }
    }

    private string ConvertMarkdownToHtml(string markdown)
    {
        var pipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .Build();

        return Markdown.ToHtml(markdown, pipeline);
    }
}
