namespace AIMentor.Features.SendMessage;

public record CreateMessageDto
{
    public required string Content { get; init; }
}


