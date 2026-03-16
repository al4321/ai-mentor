using Microsoft.AspNetCore.Mvc;

namespace AIMentor.Features.GetSessions;

public class GetSessionsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet("sessions", Handler)
            .Produces<List<SessionDto>>()
            .Produces(StatusCodes.Status400BadRequest)
            .WithName("GetSessions")
            .WithSummary("Returns all sessions");
    }

    private static async Task<IResult> Handler(
        [FromServices] GetSessionsHandler handler,
        CancellationToken cancellationToken)
    {
        try
        {
            var sessions = await handler.HandleAsync(cancellationToken);
            return Results.Ok(sessions);
        }
        catch (Exception e)
        {
            return Results.BadRequest(e.Message);
        }
    }
}