using AIMentor.Database;
using Microsoft.EntityFrameworkCore;

namespace AIMentor.Features.DeleteSession;

public class DeleteSessionHandler(AiMentorDbContext context)
{
    public async Task<bool> HandleAsync(int sessionId, CancellationToken cancellationToken)
    {
        var session = await context.Sessions
            .FirstOrDefaultAsync(s => s.Id == sessionId, cancellationToken);

        if (session == null)
            return false;

        await context.Messages
            .Where(m => m.SessionId == sessionId)
            .ExecuteDeleteAsync(cancellationToken);

        context.Sessions.Remove(session);
        await context.SaveChangesAsync(cancellationToken);

        return true;
    }
}