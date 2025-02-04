namespace AltGen.API.Generate.Providers.Gemini;

class GeminiService(HttpClient httpClient) : IGeminiService
{
  const string BaseUri = "https://generativelanguage.googleapis.com/v1beta/models";
  const string Method = "generateContent";
  const string ModelId = "gemini-1.5-flash";
  const string ApiKeyQueryKey = "key";

  // TODO: Use Lazy<T> to load the prompt resource
  string _prompt = "";
  string Prompt
  {
    get
    {
      if (string.IsNullOrWhiteSpace(_prompt))
      {
        var resource = Assembly.GetExecutingAssembly()
          .GetManifestResourceStream("AltGen.API.Resources.prompt.txt") ?? throw new AltGenException("Failed to load the prompt resource.");
        using var reader = new StreamReader(resource);
        _prompt = reader.ReadToEnd();
      }

      return _prompt;
    }
  }

  static readonly JsonSerializerOptions Options = new()
  {
    PropertyNameCaseInsensitive = true,
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    Converters = { new PartConverter() }
  };
  readonly HttpClient _httpClient = httpClient;

  public async Task<string> GenerateContentAsync(string providerKey, string mimeType, MemoryStream image)
  {
    var requestUri = GetRequestUri(providerKey);
    var imageBase64 = ConvertToBase64(image);

    var request = new GeminiRequest(
      new Content([new TextPart(Prompt)], GeminiRole.System),
      [new Content(
        [
          new TextPart(""),
          new InlineDataPart(new InlineData(mimeType, imageBase64))
        ],
        GeminiRole.User
      )]
    );

    var response = await _httpClient.PostAsJsonAsync(requestUri, request, Options);
    var content = await response.Content.ReadAsStringAsync();

    if (response.IsSuccessStatusCode is false)
    {
      throw new AltGenException("Failed to generate alt text.");
    }

    var geminiResponse = JsonSerializer.Deserialize<GeminiResponse>(content, Options) ?? throw new AltGenException("Failed to deserialize the Gemini response.");
    var firstCandidate = geminiResponse.Candidates.FirstOrDefault() ?? throw new AltGenException("No candidates found in the Gemini response.");
    var altText = firstCandidate.Content.Parts
      .OfType<TextPart>()
      .Aggregate(new StringBuilder(), static (sb, part) => sb.Append(part.Text))
      .ToString();

    return altText;
  }

  static string GetRequestUri(string providerKey)
  {
    return $"{BaseUri}/{ModelId}:{Method}/?{ApiKeyQueryKey}={providerKey}";
  }

  static string ConvertToBase64(MemoryStream image)
  {
    return Convert.ToBase64String(image.ToArray());
  }
}