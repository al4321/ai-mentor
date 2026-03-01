namespace AIMentor.Features.SendMessage;

public record MessageResponseDto
{
    public required int SessionId { get; init; }
    public required string Content { get; init; }
}

