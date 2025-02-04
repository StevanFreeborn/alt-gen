var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();

builder.Services.AddHttpClient<IGeminiService, GeminiService>();
builder.Services.AddSingleton<IAltTextProviderFactory, AltTextProviderFactory>();
builder.Services.AddKeyedSingleton<IAltTextProvider, GeminiAltTextProvider>(LLMProvider.Gemini);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
  app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseStatusCodePages();

app.MapPost("/generate", GenerateEndpoint.HandleAsync).DisableAntiforgery();

app.Run();

[ExcludeFromCodeCoverage]
public partial class Program { }