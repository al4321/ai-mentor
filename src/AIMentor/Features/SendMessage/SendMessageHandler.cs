using System.ClientModel;
using System.Diagnostics.CodeAnalysis;
using AIMentor.Database;
using AIMentor.Database.Models.Message;
using AIMentor.Database.Models.Session;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OpenAI;
using OpenAI.Responses;

namespace AIMentor.Features.SendMessage;

public class SendMessageHandler(AiMentorDbContext context, IOptions<OpenApiOptions> options)
{
    [Experimental("OPENAI001")]
    public async Task<MessageResponseDto?> Handle(int sessionId, string content, CancellationToken cancellationToken)
    {
        SessionModel? session;
        if (sessionId == 0)
        {
            session = new SessionModel
            {
                Name = $"Session {DateTimeOffset.UtcNow:yyyy-MM-dd HH:mm:ss}"
            };
            context.Sessions.Add(session);
            await context.SaveChangesAsync(cancellationToken);
        }
        else
        {
            session = await context.Sessions.FirstOrDefaultAsync(s => s.Id == sessionId, cancellationToken: cancellationToken);
        }
        if (session == null)
        {
            return null;
        }

        var message = new MessageModel
        {
            Content = content,
            Role = MessageRoles.User,
            SessionId = session.Id
        };
        context.Messages.Add(message);
        await context.SaveChangesAsync(cancellationToken);

        var client = new ResponsesClient(
            new ApiKeyCredential(options.Value.OpenAiKey),
            new OpenAIClientOptions
            {
                Endpoint = new Uri(options.Value.BaseUrl)
            });
        var createResponseOptions = new CreateResponseOptions
        {
            Model = options.Value.OpenAiModel,
            InputItems = { ResponseItem.CreateUserMessageItem("Say 'this is a test.'") }
        };

        ResponseResult response = await client.CreateResponseAsync(createResponseOptions, cancellationToken);

        Console.WriteLine($"[ASSISTANT]: {response.GetOutputText()}");


        // var client = new OpenAIClient(
        //     Environment.GetEnvironmentVariable("OPENAI_API_KEY")
        // );
        //
        // var response = await client.GetResponsesClient()...responses.CreateAsync(new ResponseCreateRequest
        // {
        //     Model = "gpt-5.2",
        //     Input = "Say 'this is a test.'"
        // });
        //
        // Console.WriteLine($"[ASSISTANT]: {response.OutputText()}");

        return new MessageResponseDto
        {
            SessionId = message.Id,
            Content = message.Content
        };
    }
}
