namespace AltGen.API.Generate.Providers.Gemini;

class GeminiAltTextProvider(IGeminiService geminiService) : IAltTextProvider
{
  readonly IGeminiService _geminiService = geminiService;

  public Task<string> GenerateAltTextAsync(string providerKey, string mimeType, MemoryStream image)
  {
    return _geminiService.GenerateContentAsync(providerKey, mimeType, image);
  }
}
