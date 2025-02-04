namespace AltGen.API.Generate.Providers.Gemini;

record Part();

record TextPart(string Text) : Part;

record InlineDataPart(InlineData InlineData) : Part;

record InlineData(string MimeType, string Data);