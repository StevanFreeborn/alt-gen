namespace AltGen.API.Generate.Providers;

interface IAltTextProvider
{
  Task<string> GenerateAltTextAsync(string providerKey, string mimeType, MemoryStream image);
}