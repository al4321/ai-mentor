namespace AIMentor.Features;

public class OpenApiOptions
{
    public const string SectionPath = "Features:OpenApi";

    public required string OpenAiKey { get; init; }
    public required string BaseUrl { get; init; } = "https://api.openai.com/v1";
    public required string OpenAiModel { get; init; } = "gpt-5.2-2025-12-11";
}
