using Microsoft.AspNetCore.Mvc;

namespace AIMentor.Features.GetMessages;

public class GetMessagesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet("sessions/{sessionId:int}/messages", Handler)
            .Produces<List<MessageDto>>()
            .Produces(StatusCodes.Status400BadRequest)
            .WithName("GetMessages")
            .WithSummary("Returns all messages for a session");
    }

    private static async Task<IResult> Handler(
        [FromRoute] int sessionId,
        [FromServices] GetMessagesHandler handler,
        CancellationToken cancellationToken)
    {
        try
        {
            var messages = await handler.HandleAsync(sessionId, cancellationToken);
            return Results.Ok(messages);
        }
        catch (Exception e)
        {
            return Results.BadRequest(e.Message);
        }
    }
}