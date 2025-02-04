namespace AltGen.API.Generate.Providers.Gemini;

interface IGeminiService
{
  Task<string> GenerateContentAsync(string providerKey, string mimeType, MemoryStream image);
}
