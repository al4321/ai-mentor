using Microsoft.AspNetCore.Mvc;

namespace AIMentor.Features.DeleteSession;

public class DeleteSessionEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapDelete("sessions/{sessionId:int}", Handler)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest)
            .WithName("DeleteSession")
            .WithSummary("Deletes a session and all its messages");
    }

    private static async Task<IResult> Handler(
        [FromRoute] int sessionId,
        [FromServices] DeleteSessionHandler handler,
        CancellationToken cancellationToken)
    {
        try
        {
            var deleted = await handler.HandleAsync(sessionId, cancellationToken);
            return deleted ? Results.NoContent() : Results.NotFound();
        }
        catch (Exception e)
        {
            return Results.BadRequest(e.Message);
        }
    }
}