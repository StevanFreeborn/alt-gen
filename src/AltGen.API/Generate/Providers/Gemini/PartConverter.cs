namespace AltGen.API.Generate.Providers.Gemini;

class PartConverter : JsonConverter<Part>
{
  public override Part? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    using var doc = JsonDocument.ParseValue(ref reader);
    var root = doc.RootElement;

    if (root.TryGetProperty("text", out var text))
    {
      return JsonSerializer.Deserialize<TextPart>(root.GetRawText(), options);
    }

    if (root.TryGetProperty("inlineData", out var inlineData))
    {
      return JsonSerializer.Deserialize<InlineDataPart>(root.GetRawText(), options);
    }

    throw new JsonException("Invalid part type.");
  }

  public override void Write(Utf8JsonWriter writer, Part value, JsonSerializerOptions options)
  {
    JsonSerializer.Serialize(writer, value, value.GetType(), options);
  }
}