using System.Reflection;
using AIMentor.Database.Models.Message;
using AIMentor.Database.Models.Session;
using Microsoft.EntityFrameworkCore;

namespace AIMentor.Database;

public class AiMentorDbContext(DbContextOptions<AiMentorDbContext> options) : DbContext(options)
{
    public const string ConnectionName = "AiMentorDb";

    public DbSet<SessionModel> Sessions { get; init; }
    public DbSet<MessageModel> Messages { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
