namespace AltGen.API.Generate.Providers;

static class LLMProvider
{
  public const string Gemini = "gemini";

  public static bool IsValid(string provider)
  {
    return provider.ToLowerInvariant() is Gemini;
  }
}