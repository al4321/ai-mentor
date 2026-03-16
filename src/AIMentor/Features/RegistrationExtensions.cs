using System.ClientModel;
using AIMentor.Features.GetMessages;
using AIMentor.Features.GetSessions;
using AIMentor.Features.SendMessage;
using Microsoft.Extensions.Options;
using OpenAI;
using OpenAI.Responses;

namespace AIMentor.Features;

public static class RegistrationExtensions
{
    public static IHostApplicationBuilder AddFeatures(this IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<SendMessageHandler>();
        builder.Services.AddScoped<GetSessionsHandler>();
        builder.Services.AddScoped<GetMessagesHandler>();
        builder.Services
            .AddOptions<OpenAiOptions>()
            .Bind(builder.Configuration.GetSection(OpenAiOptions.SectionPath))
            .Validate(o => !string.IsNullOrWhiteSpace(o.OpenAiKey), $"{nameof(OpenAiOptions.OpenAiKey)} is required")
            .ValidateOnStart();
        builder.Services.AddSingleton<ResponsesClient>(sp =>
        {
            var opts = sp.GetRequiredService<IOptions<OpenAiOptions>>().Value;

            return new ResponsesClient(
                new ApiKeyCredential(opts.OpenAiKey),
                new OpenAIClientOptions
                {
                    Endpoint = new Uri(opts.BaseUrl)
                });
        });

        return builder;
    }
}
