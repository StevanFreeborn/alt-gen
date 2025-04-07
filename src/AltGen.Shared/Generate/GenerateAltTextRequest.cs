namespace AltGen.Shared.Generate;

public record GenerateAltTextRequest(
  string Provider,
  string ProviderKey,
  string FileName,
  byte[] Image,
  string ContentType
);
