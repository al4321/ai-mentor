namespace AIMentor.Features.GetSessions;

public record SessionDto
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
}