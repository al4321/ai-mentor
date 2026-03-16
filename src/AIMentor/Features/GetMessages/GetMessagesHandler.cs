using AIMentor.Database;
using Microsoft.EntityFrameworkCore;

namespace AIMentor.Features.GetMessages;

public class GetMessagesHandler(AiMentorDbContext context)
{
    public Task<List<MessageDto>> HandleAsync(int sessionId, CancellationToken cancellationToken)
    {
        return context.Messages
            .AsNoTracking()
            .Where(m => m.SessionId == sessionId)
            .OrderBy(m => m.CreatedAt)
            .Select(m => new MessageDto
            {
                Role = m.Role,
                Content = m.Content,
                CreatedAt = m.CreatedAt
            })
            .ToListAsync(cancellationToken);
    }
}