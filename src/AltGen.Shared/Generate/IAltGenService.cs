namespace AltGen.Shared.Generate;

public interface IAltGenService
{
  public Task<string> GenerateAltTextAsync(GenerateAltTextRequest req);
}