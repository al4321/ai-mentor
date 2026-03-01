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
    public async Task<MessageResponseDto?> HandleAsync(int sessionId, string content, CancellationToken cancellationToken)
    {
        var session = await GetOrCreateSession(sessionId, cancellationToken);
        if (session == null)
        {
            return null;
        }

        await CreateMessage(content, session, MessageRoles.User, cancellationToken);
        var sessionMessages = await GetSessionMessages(session.Id, cancellationToken);
        var response = await RequestOpenAiResponse(sessionMessages, cancellationToken);
        await CreateMessage(response.GetOutputText(), session, MessageRoles.Assistant, cancellationToken);
        Console.WriteLine($"[ASSISTANT]: {response.GetOutputText()}");

        return new MessageResponseDto
        {
            SessionId = session.Id,
            Content = response.GetOutputText()
        };
    }

    private async Task CreateMessage(string content, SessionModel session, string role, CancellationToken cancellationToken)
    {
        var userMessage = new MessageModel
        {
            Content = content,
            Role = role,
            SessionId = session.Id
        };
        context.Messages.Add(userMessage);
        await context.SaveChangesAsync(cancellationToken);
    }

    [Experimental("OPENAI001")]
    private async Task<IReadOnlyCollection<ResponseItem>> GetSessionMessages(int sessionId, CancellationToken cancellationToken)
    {
        return await context.Messages
            .AsNoTracking()
            .Where(x => x.SessionId == sessionId)
            .OrderBy(x => x.CreatedAt)
            .Select(x => x.Role == MessageRoles.User
                ? ResponseItem.CreateUserMessageItem(x.Content)
                : ResponseItem.CreateAssistantMessageItem(x.Content, null))
            .ToListAsync(cancellationToken);
    }

    [Experimental("OPENAI001")]
    private async Task<ResponseResult> RequestOpenAiResponse(
        IReadOnlyCollection<ResponseItem> content,
        CancellationToken cancellationToken)
    {
        var client = new ResponsesClient(
            new ApiKeyCredential(options.Value.OpenAiKey),
            new OpenAIClientOptions
            {
                Endpoint = new Uri(options.Value.BaseUrl)
            });
        var createResponseOptions = new CreateResponseOptions
        {
            Model = options.Value.OpenAiModel
        };
        foreach (var item in content)
        {
            createResponseOptions.InputItems.Add(item);
        }

        var response = await client.CreateResponseAsync(createResponseOptions, cancellationToken);

        return response.Value;
    }

    private async Task<SessionModel?> GetOrCreateSession(int sessionId, CancellationToken cancellationToken)
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
            session = await context.Sessions
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == sessionId, cancellationToken: cancellationToken);
        }

        return session;
    }
}
