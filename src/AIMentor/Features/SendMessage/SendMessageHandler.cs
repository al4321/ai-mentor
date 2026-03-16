using AIMentor.Database;
using AIMentor.Database.Models.Message;
using AIMentor.Database.Models.Session;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using OpenAI.Responses;
using System.Reflection;

namespace AIMentor.Features.SendMessage;

public class SendMessageHandler(ResponsesClient responsesClient, AiMentorDbContext context, IOptions<OpenAiOptions> options, IMemoryCache memoryCache)
{
    private const string SystemPromptResourceKey = "SystemPrompt.md";

    public async Task<MessageResponseDto?> HandleAsync(int sessionId, string? content, CancellationToken cancellationToken)
    {
        var session = await GetOrCreateSession(sessionId, cancellationToken);
        if (session == null)
        {
            return null;
        }

        if (content != null)
            await AppendMessageToSession(content, session, MessageRoles.User, cancellationToken);
        var sessionMessages = await GetSessionMessages(session.Id, cancellationToken);
        await PrependSystemMessage(sessionMessages);
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

    private Task<int> AppendMessageToSession(string content, SessionModel session, string role, CancellationToken cancellationToken)
    {
        var userMessage = new MessageModel
        {
            Content = content,
            Role = role,
            SessionId = session.Id
        };
        context.Messages.Add(userMessage);
        return context.SaveChangesAsync(cancellationToken);
    }

    private Task<List<MessageResponseItem>> GetSessionMessages(int sessionId, CancellationToken cancellationToken)
    {
        return context.Messages
            .AsNoTracking()
            .Where(x => x.SessionId == sessionId)
            .OrderBy(x => x.CreatedAt)
            .Select(x => x.Role == MessageRoles.User
                ? ResponseItem.CreateUserMessageItem(x.Content)
                : ResponseItem.CreateAssistantMessageItem(x.Content, null))
            .ToListAsync(cancellationToken);
    }

    private async Task PrependSystemMessage(List<MessageResponseItem> messages)
    {
        var systemPrompt = await memoryCache.GetOrCreateAsync(SystemPromptResourceKey, async _ =>
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = assembly.GetManifestResourceNames().FirstOrDefault(name => name.EndsWith(SystemPromptResourceKey));
            if (resourceName == null)
            {
                throw new InvalidOperationException($"{SystemPromptResourceKey} not found in embedded resources.");
            }
            await using var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null)
            {
                throw new InvalidOperationException($"{SystemPromptResourceKey} not found in embedded resources.");
            }
            using var reader = new StreamReader(stream);
            return await reader.ReadToEndAsync();
        });
        var systemItem = ResponseItem.CreateSystemMessageItem(systemPrompt);
        messages.Insert(0, systemItem);
    }

    private CreateResponseOptions PrepareOpenAiResponseOptions(IReadOnlyCollection<ResponseItem> messages)
    {
        var createResponseOptions = new CreateResponseOptions
        {
            Model = options.Value.OpenAiModel
        };
        foreach (var item in messages)
        {
            createResponseOptions.InputItems.Add(item);
        }

        return createResponseOptions;
    }
}
