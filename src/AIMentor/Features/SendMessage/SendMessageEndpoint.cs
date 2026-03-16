using Microsoft.AspNetCore.Mvc;

namespace AIMentor.Features.SendMessage;

public class SendMessageEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost("sessions/{sessionId:int}/messages", Handler)
            .Produces<string>()
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest)
            .WithName("SendMessage")
            .WithSummary("Creates a new message in a session");
    }

    private static async Task<IResult> Handler(
        [FromRoute] int sessionId,
        [FromBody] CreateMessageDto? request,
        [FromServices] SendMessageHandler handler,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await handler.HandleAsync(sessionId, request?.Content, cancellationToken);
            return response == null ? Results.NotFound() : Results.Ok(response);
        }
        catch (Exception e)
        {
            return Results.BadRequest(e.Message);
        }
    }
}
