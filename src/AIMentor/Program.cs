using System.Reflection;
using System.Text.Json;
using AIMentor.Database;
using AIMentor.Features;

var builder = WebApplication.CreateSlimBuilder(args);

builder.AddAiMentorDatabase();
builder.AddFeatures();

builder.Services.AddMemoryCache();
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});
builder.Services
    .Scan(scan => scan
    .FromAssemblies(Assembly.GetExecutingAssembly())
    .AddClasses(c => c.AssignableTo<IEndpoint>())
    .AsImplementedInterfaces()
    .WithTransientLifetime());


var app = builder.Build();

foreach (var ep in app.Services.GetServices<IEndpoint>())
    ep.MapEndpoint(app);

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AiMentorDbContext>();
    await dbContext.Database.EnsureCreatedAsync();
}

app.Run();
