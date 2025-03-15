interface IAltGenService
{
  Task<string> GenerateAltTextAsync(GenerateAltTextRequest req);
}