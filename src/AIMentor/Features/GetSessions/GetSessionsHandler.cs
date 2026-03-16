using AIMentor.Database;
using Microsoft.EntityFrameworkCore;

namespace AIMentor.Features.GetSessions;

public class GetSessionsHandler(AiMentorDbContext context)
{
    public Task<List<SessionDto>> HandleAsync(CancellationToken cancellationToken)
    {
        return context.Sessions
            .AsNoTracking()
            .OrderByDescending(s => s.CreatedAt)
            .Select(s => new SessionDto
            {
                Id = s.Id,
                Name = s.Name,
                CreatedAt = s.CreatedAt
            })
            .ToListAsync(cancellationToken);
    }
}