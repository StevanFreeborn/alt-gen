using Microsoft.Extensions.Configuration;

namespace AltGen.API.Tests.Fixtures;

public class TestConfiguration
{
  static IConfiguration Config { get; } = new ConfigurationBuilder()
    .AddJsonFile("appsettings.Test.json")
    .AddEnvironmentVariables()
    .Build();

#pragma warning disable CA1822
  public string GeminiApiKey => GetGeminiApiKey();
#pragma warning restore CA1822

  static string GetGeminiApiKey()
  {
    var apiKey = Config.GetSection("Gemini").GetValue<string>("ApiKey");

    if (string.IsNullOrWhiteSpace(apiKey))
    {
      throw new InvalidOperationException("Gemini__ApiKey is required");
    }

    return apiKey;
  }
}