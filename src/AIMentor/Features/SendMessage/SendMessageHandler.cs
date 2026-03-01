using AIMentor.Database;
using AIMentor.Database.Models.Message;
using AIMentor.Database.Models.Session;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OpenAI.Responses;

namespace AIMentor.Features.SendMessage;

public class SendMessageHandler(ResponsesClient responsesClient, AiMentorDbContext context, IOptions<OpenAiOptions> options)
{
    public async Task<MessageResponseDto?> HandleAsync(int sessionId, string content, CancellationToken cancellationToken)
    {
        var session = await GetOrCreateSession(sessionId, cancellationToken);
        if (session == null)
        {
            return null;
        }

        await AppendMessageToSession(content, session, MessageRoles.User, cancellationToken);
        var sessionMessages = await GetSessionMessages(session.Id, cancellationToken);
        var createResponseOptions = PrepareOpenAiResponseOptions(sessionMessages);
        var response = await responsesClient.CreateResponseAsync(createResponseOptions, cancellationToken);
        var responseText = response.Value.GetOutputText();
        await AppendMessageToSession(responseText, session, MessageRoles.Assistant, cancellationToken);
        Console.WriteLine($"[ASSISTANT]: {responseText}");

        return new MessageResponseDto
        {
            SessionId = session.Id,
            Content = responseText
        };
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

    private async Task AppendMessageToSession(string content, SessionModel session, string role, CancellationToken cancellationToken)
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

    private CreateResponseOptions PrepareOpenAiResponseOptions(IReadOnlyCollection<ResponseItem> content)
    {
        var createResponseOptions = new CreateResponseOptions
        {
            Model = options.Value.OpenAiModel
        };
        foreach (var item in content)
        {
            createResponseOptions.InputItems.Add(item);
        }

        return createResponseOptions;
    }
}
