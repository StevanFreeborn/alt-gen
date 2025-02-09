namespace AltGen.API.Generate.Providers.Gemini;

record GeminiRequest(
  Content SystemInstruction,
  Content[] Contents
);