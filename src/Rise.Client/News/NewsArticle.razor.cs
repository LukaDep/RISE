using Markdig;
using Microsoft.AspNetCore.Components;
using Rise.Shared.News;

namespace Rise.Client.News;

/// <summary>
/// Code-behind for the NewsArticle page component.
/// Displays a single news article with markdown rendering.
/// </summary>
public partial class NewsArticle : ComponentBase
{
    /// <summary>Service for news data operations.</summary>
    [Inject] public required INewsService NewsService { get; set; }
    
    /// <summary>The ID of the news article to display.</summary>
    [Parameter] public Guid Id { get; set; }
    
    private NewsDto.Index? newsArticle;
    private string? errorMessage;

    /// <summary>
    /// Loads the news article by ID on initialization.
    /// </summary>
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

    /// <summary>
    /// Converts markdown text to HTML for rendering.
    /// </summary>
    /// <param name="markdown">The markdown text to convert.</param>
    /// <returns>HTML representation of the markdown.</returns>
    private string ConvertMarkdownToHtml(string markdown)
    {
        var pipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .Build();

        return Markdown.ToHtml(markdown, pipeline);
    }
}
