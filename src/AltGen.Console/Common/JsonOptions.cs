namespace AltGen.Console.Common;

static class JsonOptions
{
  public static JsonSerializerOptions Default { get; } = new()
  {
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    PropertyNameCaseInsensitive = true,
    WriteIndented = true
  };
}