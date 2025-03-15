namespace AltGen.Console.Generate;

sealed class AltGenService(HttpClient client) : IAltGenService
{
  static readonly JsonSerializerOptions JsonOptions = new()
  {
    PropertyNameCaseInsensitive = true
  };
  readonly HttpClient _client = client;

  public async Task<string> GenerateAltTextAsync(GenerateAltTextRequest req)
  {
    var byteContent = new ByteArrayContent(req.Image);
    byteContent.Headers.ContentType = new MediaTypeHeaderValue(req.ContentType);

    var request = new HttpRequestMessage(HttpMethod.Post, $"{Constants.AltGenApiUri}/generate")
    {
      Content = new MultipartFormDataContent
      {
        { new StringContent(req.Provider), "provider" },
        { new StringContent(req.ProviderKey), "providerKey" },
        { byteContent, "file", req.FileName }
      }
    };

    var response = await _client.SendAsync(request);
    var content = await response.Content.ReadAsStringAsync();

    if (response.IsSuccessStatusCode is false)
    {
      throw new AltTextException("Failed to generate alt text.");
    }

    var altTextResponse = JsonSerializer.Deserialize<AltTextResponse>(content, JsonOptions) ?? throw new AltTextException("Failed to deserialize response.");

    return altTextResponse.AltText;
  }
}

record AltTextResponse(string AltText);

class AltTextException(string message) : Exception(message)
{
}