namespace AIMentor.Features.GetMessages;

public record MessageDto
{
    public required string Role { get; init; }
    public required string Content { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
}