using System.Globalization;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();

builder.Services.AddHttpClient<IGeminiService, GeminiService>();
builder.Services.AddSingleton<IAltTextProviderFactory, AltTextProviderFactory>();
builder.Services.AddKeyedSingleton<IAltTextProvider, GeminiAltTextProvider>(LLMProvider.Gemini);

builder.Services.AddRateLimiter(
  static o =>
  {
    o.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    o.OnRejected = static (context, rateLimitInfo) =>
    {
      if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
      {
        var retryDate = DateTime.UtcNow.Add(retryAfter);
        context.HttpContext.Response.Headers.Append(
          "x-retry-after",
          retryDate.ToString("o", CultureInfo.InvariantCulture)
        );
      }

      return ValueTask.CompletedTask;
    };

    o.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(
      static ctx => RateLimitPartition.GetFixedWindowLimiter(
        partitionKey: ctx.Request.Headers["X-Forwarded-For"].ToString()
          ?? ctx.Connection.RemoteIpAddress?.ToString()
          ?? "unknown",
        factory: static partition => new FixedWindowRateLimiterOptions()
        {
          PermitLimit = 10,
          QueueLimit = 0,
          Window = TimeSpan.FromHours(1)
        }
      )
    );
  }
);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
  app.MapOpenApi();
}

if (app.Environment.IsProduction())
{
  app.UseRateLimiter();
}

app.UseHttpsRedirection();

app.UseStatusCodePages();

app.MapGenerateEndpoint();

app.Run();

[ExcludeFromCodeCoverage]
public partial class Program { }