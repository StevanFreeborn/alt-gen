namespace AltGen.API.Generate.Providers;

static class LLMProvider
{
  public const string Gemini = "Gemini";

  public static bool IsValid(string provider)
  {
    return provider is Gemini;
  }
}