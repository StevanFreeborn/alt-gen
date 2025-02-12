namespace AltGen.Console.Generate;

record GenerateAltTextRequest(
  string Provider,
  string ProviderKey,
  string FileName,
  byte[] Image,
  string ContentType
);
