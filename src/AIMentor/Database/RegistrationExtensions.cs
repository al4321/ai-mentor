using Microsoft.EntityFrameworkCore;

namespace AIMentor.Database;

public static class RegistrationExtensions
{
    public static IHostApplicationBuilder AddAiMentorDatabase(this IHostApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString(AiMentorDbContext.ConnectionName);
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            connectionString = $"Data Source={AiMentorDbContext.ConnectionName}.db";
        }
        builder.Services.AddDbContext<AiMentorDbContext>(options =>
            options.UseSqlite(connectionString, _ => { }));

        return builder;
    }
}
